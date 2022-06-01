using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SwitchExecutive.Plugin.Internal.Common;
using SwitchExecutive.Plugin.Internal.DriverOperations.Internal;

namespace SwitchExecutive.Plugin.Internal.DriverOperations
{
    public sealed class DriverOperationsConstants
    {
        public const string NoConnections = "No connections";
    }

    [JsonObject(MemberSerialization.OptIn)]
    public interface ISwitchExecutiveDriverOperations : IDisposable, INotifyPropertyChanged
    {
        string SelectedVirtualDevice { get; set; }
        string SelectedRoute { get; set; }

        IEnumerable<string> VirtualDeviceNames { get; }
        IEnumerable<string> RouteNames { get; }
        string Comment { get; }
        IEnumerable<DeviceInfo> DeviceInfo { get; }
        IEnumerable<ChannelInfo> ChannelInfo { get; }
        IEnumerable<RouteInfo> RouteInfo { get; }
        IEnumerable<string> ConnectedRoutes { get; }
        [JsonProperty]
        List<string> ConnectedRoutesCache { get; set; }

        string ExpandedRoutePath { get; }

        string ExpandedRoutePathForRoute(string route);
        bool CanConnect();
        bool CanDisconnect();
        bool CanDisconnectRoute(string route);
        bool CanDisconnectAll();
        void TryConnect(MulticonnectMode connectionMode);
        void TryDisconnect();
        void TryDisconnectRoute(string route);
        bool IsConnected();
        bool IsRouteConnected(string route);
        void TryDisconnectAll();
        void Shutdown();
        void Refresh();
        void RefreshOptions(bool auto);
        void ApplyLoadFromFile(MulticonnectMode connectionMode = MulticonnectMode.Multiconnect);
    }

    [JsonObject(MemberSerialization.OptIn)]
    public sealed class NISwitchExecutiveDriverOperations : BaseNotify, ISwitchExecutiveDriverOperations
    {
        private IEnumerable<string> _connectedRoutes = new List<string>();
        private List<string> _connectedRoutesCache = new List<string>();
        private string _expandedRoutePath = string.Empty;
        private string _selectedVirtualDevice = string.Empty;
        private string _selectedRoute = string.Empty;
        private bool _simulate;
        private ISwitchExecutive _switchExecutive;
        private IVirtualDevices _virtualDevices;
        private Timer _refreshTimer = null;

        public static bool IsDriverInstalled()
        {
            return VirtualDevicesFactory.IsDriverInstalled();
        }

        public NISwitchExecutiveDriverOperations(bool simulate = false)
        {
            SelectedVirtualDevice = string.Empty;
            _simulate = simulate;
        }

        public string SelectedVirtualDevice
        {
            get => _selectedVirtualDevice;

            set
            {
                _selectedVirtualDevice = value;
                VirtualDevices.SelectedName = value;
                SelectedRoute = string.Empty;
                NotifyPropertyChanged();
                Refresh();
            }
        }

        public void Refresh()
        {
            if (!string.IsNullOrEmpty(SelectedVirtualDevice))
            {
                NotifyPropertyChanged(nameof(RouteNames));
                NotifyPropertyChanged(nameof(Comment));
                NotifyPropertyChanged(nameof(DeviceInfo));
                NotifyPropertyChanged(nameof(ChannelInfo));
                NotifyPropertyChanged(nameof(DeviceInfo));
                NotifyPropertiesConnectedRouteChanged();
            }
        }

        public void RefreshOptions(bool auto)
        {
            if (auto)
            {
                if (_refreshTimer == null)
                {
                    Action refreshAction = async () =>
                    {
                        await Task.Run(() => Refresh());
                    };
                    const int kRefreshTimerIntervalInMilliseconds = 2500;
                    _refreshTimer =
                       new Timer(
                          o => refreshAction.Invoke(),
                          null,
                          kRefreshTimerIntervalInMilliseconds,
                          kRefreshTimerIntervalInMilliseconds);
                }
            }
            else
            {
                if (_refreshTimer != null)
                {
                    _refreshTimer.Dispose();
                    _refreshTimer = null;
                }
            }
        }

        public string SelectedRoute
        {
            get => _selectedRoute;

            set
            {
                _selectedRoute = value;
                VirtualDevices.SelectedRouteName = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(ExpandedRoutePath));
            }
        }

        public IEnumerable<string> VirtualDeviceNames => VirtualDevices.Names;

        public IEnumerable<string> RouteNames => VirtualDevices.Routes;

        public string Comment => VirtualDevices.Comment;

        public IEnumerable<DeviceInfo> DeviceInfo => VirtualDevices.DeviceInfo;
        public IEnumerable<ChannelInfo> ChannelInfo
        {
            get
            {
                var channelInfo = VirtualDevices.ChannelInfo;
                var routeInfo = RouteInfo;

                foreach (var channel in channelInfo)
                {
                    foreach (var route in routeInfo)
                    {
                        if (route.Endpoint1.Any()
                            && channel.Name == route.Endpoint1
                            && !channel.Connected)
                        {
                            channel.Index = route.Index;
                        }

                        if (route.Endpoint2.Any()
                            && channel.Name == route.Endpoint2
                            && !channel.Connected)
                        {
                            channel.Index = route.Index;
                        }
                    }
                }

                return channelInfo;
            }
        }

        public IEnumerable<RouteInfo> RouteInfo
        {
            get
            {
                var routeInfo = VirtualDevices.RouteInfo;

                foreach (var route in routeInfo)
                {
                    int i = 0;
                    var connectedRoutes = ConnectedRoutes;
                    foreach (var connectedRoute in connectedRoutes)
                    {
                        IEnumerable<string> expandedRoutePathList = new List<string>();
                        var expandedRouteList = ExpandedRouteList(connectedRoute);
                        if (expandedRouteList.Any())
                        {
                            expandedRoutePathList = expandedRouteList.Split('&').Select(x => x.Trim());
                        }

                        foreach (var expandedRoute in expandedRoutePathList)
                        {
                            if (expandedRoute == route.Name)
                            {
                                route.Index = i;

                                // if connection is a group then let the user know the group name
                                if (route.Name != connectedRoute)
                                {
                                    route.ConnectedGroup = connectedRoute;
                                }
                            }
                        }

                        i++;
                    }
                }

                return routeInfo;
            }
        }

        public IEnumerable<string> ConnectedRoutes
        {
            get
            {
                if (string.IsNullOrEmpty(SelectedVirtualDevice))
                {
                    _connectedRoutes = new List<string>();
                }
                else
                {
                    var allConnections = SwitchExecutive.GetAllConnections();
                    if (allConnections.Any())
                    {
                        _connectedRoutes = allConnections.Split('&').Select(x => x.Trim());
                    }
                    else
                    {
                        _connectedRoutes = new List<string>();
                    }
                }

                return _connectedRoutes;
            }
        }

        [JsonProperty]
        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> ConnectedRoutesCache
        {
            get => ConnectedRoutes.ToList(); // used by save() to save routes into the file
            set => _connectedRoutesCache = value; // used by applyLoadFromFile() to reapply routes
        }

        public void ApplyLoadFromFile(MulticonnectMode connectionMode = MulticonnectMode.Multiconnect)
        {
            if (!_connectedRoutesCache.Any())
            {
                return;
            }

            // ensure a virtual device is selected to apply routes
            if (!SelectedVirtualDevice.Any())
            {
                return;
            }

            // assume if a route is selected then we should apply routes from the saved file
            if (!SelectedRoute.Any())
            {
                return;
            }

            foreach (var route in _connectedRoutesCache)
            {
                if (!IsRouteConnected(route) && (route != DriverOperationsConstants.NoConnections))
                {
                    TryConnect(route, connectionMode);
                }
            }

            NotifyPropertiesConnectedRouteChanged();
        }

        public string ExpandedRoutePath
        {
            get
            {
                if (CanDisconnect())
                {
                    _expandedRoutePath = SwitchExecutive.ExpandRouteSpec(SelectedRoute, ExpandOptions.ExpandToPaths);
                }
                else
                {
                    _expandedRoutePath = string.Empty;
                }

                return _expandedRoutePath;
            }
        }

        public string ExpandedRoutePathForRoute(string route)
        {
            string path = string.Empty;
            if (CanDisconnectRoute(route))
            {
                path = SwitchExecutive.ExpandRouteSpec(route, ExpandOptions.ExpandToPaths);
            }
            else
            {
                path = string.Empty;
            }

            return path;
        }

        public bool CanConnect()
        {
            if (!SelectedVirtualDevice.Any())
            {
                return false;
            }

            if (!SelectedRoute.Any())
            {
                return false;
            }

            return true;
        }

        public bool CanDisconnect()
        {
            return CanDisconnectRoute(SelectedRoute);
        }

        public bool CanDisconnectAll()
        {
            if (!SelectedVirtualDevice.Any())
            {
                return false;
            }

            return _connectedRoutes.Any();
        }

        public void TryConnect(MulticonnectMode connectionMode) => TryConnect(SelectedRoute, connectionMode);

        public void TryConnect(string route, MulticonnectMode connectionMode)
        {
            SwitchExecutive.Connect(route, connectionMode, true);
            NotifyPropertiesConnectedRouteChanged();
        }

        public void TryDisconnect() => TryDisconnectRoute(SelectedRoute);

        public void TryDisconnectRoute(string route)
        {
            SwitchExecutive.Disconnect(route);
            NotifyPropertiesConnectedRouteChanged();
        }

        public bool IsConnected() => SwitchExecutive.IsConnected(SelectedRoute);

        public void TryDisconnectAll()
        {
            SwitchExecutive.DisconnectAll();
            NotifyPropertiesConnectedRouteChanged();
        }

        public void Shutdown()
        {
            _switchExecutive?.Dispose();
            _refreshTimer?.Dispose();
        }

        public void Dispose() => Shutdown();

        private string ExpandedRouteList(string route)
        {
            string expandedRoute = string.Empty;

            if (CanDisconnectRoute(route))
            {
                expandedRoute = SwitchExecutive.ExpandRouteSpec(route, ExpandOptions.ExpandToRoutes);
            }
            else
            {
                expandedRoute = string.Empty;
            }

            return expandedRoute;
        }

        public bool CanDisconnectRoute(string route)
        {
            if (!SelectedVirtualDevice.Any())
            {
                return false;
            }

            if (!SelectedRoute.Any())
            {
                return false;
            }

            if (route == DriverOperationsConstants.NoConnections)
            {
                return false;
            }

            return IsRouteConnected(route);
        }

        public bool IsRouteConnected(string route)
        {
            if (route == DriverOperationsConstants.NoConnections)
            {
                return false;
            }

            return SwitchExecutive.IsConnected(route);
        }

        private void NotifyPropertiesConnectedRouteChanged()
        {
            NotifyPropertyChanged(nameof(ConnectedRoutes));
            NotifyPropertyChanged(nameof(RouteInfo));
            NotifyPropertyChanged(nameof(ExpandedRoutePath));
        }

        private IVirtualDevices VirtualDevices
        {
            get
            {
                if (_virtualDevices == null)
                {
                    _virtualDevices = VirtualDevicesFactory.CreateVirtualDevice(_simulate);
                }

                return _virtualDevices;
            }
        }

        private ISwitchExecutive SwitchExecutive
        {
            get
            {
                if (_switchExecutive == null)
                {
                    _switchExecutive =
                       NISwitchExecutiveFactory.CreateNISwitchExecutive(
                          SelectedVirtualDevice,
                          _simulate);

                    return _switchExecutive;
                }

                if (_switchExecutive.Name == SelectedVirtualDevice)
                {
                    // return existing session
                    return _switchExecutive;
                }

                // close existing session and create new one
                _switchExecutive.Dispose();
                _switchExecutive =
                   NISwitchExecutiveFactory.CreateNISwitchExecutive(
                      SelectedVirtualDevice,
                      _simulate);
                return _switchExecutive;
            }
        }
    }
}
