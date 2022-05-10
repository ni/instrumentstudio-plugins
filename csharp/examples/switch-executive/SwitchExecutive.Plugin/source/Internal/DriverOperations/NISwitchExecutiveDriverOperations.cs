using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

using SwitchExecutive.Plugin.Internal.DriverOperations.Internal;
using SwitchExecutive.Plugin.Internal.Common;

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
   public class NISwitchExecutiveDriverOperations : BaseNotify, ISwitchExecutiveDriverOperations
   {
      private IEnumerable<string> connectedRoutes = new List<string>();
      private List<string> connectedRoutesCache = new List<string>();
      private string expandedRoutePath = string.Empty;
      private string selectedVirtualDevice = string.Empty;
      private string selectedRoute = string.Empty;
      private bool simulate;
      private NISwitchExecutiveInterface switchExecutive;
      private IVirtualDevices virtualDevices;
      private Timer refreshTimer = null;

      public static bool IsDriverInstalled()
      {
         return VirtualDevicesFactory.IsDriverInstalled();
      }

      public NISwitchExecutiveDriverOperations(bool simulate = false)
      {
         this.SelectedVirtualDevice = string.Empty;
         this.simulate = simulate;
      }

      public string SelectedVirtualDevice
      {
         get => this.selectedVirtualDevice;

         set
         {
            this.selectedVirtualDevice = value;
            this.VirtualDevices.SelectedName = value;
            this.SelectedRoute = string.Empty;
            this.NotifyPropertyChanged();
            this.Refresh();
         }
      }

      public void Refresh()
      {
         if (this.SelectedVirtualDevice != string.Empty)
         {
            this.NotifyPropertyChanged(nameof(this.RouteNames));
            this.NotifyPropertyChanged(nameof(this.Comment));
            this.NotifyPropertyChanged(nameof(this.DeviceInfo));
            this.NotifyPropertyChanged(nameof(this.ChannelInfo));
            this.NotifyPropertyChanged(nameof(this.DeviceInfo));
            this.NotifyProperitiesConnectedRouteChanged();
         }
      }

      public void RefreshOptions(bool auto)
      {
         if (auto)
         {
            if (this.refreshTimer == null)
            {
               Action refreshAction = async () =>
               {
                  await Task.Run(() => this.Refresh());
               };
               const int kRefreshTimerIntervalInMilliseconds = 2500;
               this.refreshTimer = 
                  new Timer(
                     o => refreshAction.Invoke(), 
                     null, 
                     kRefreshTimerIntervalInMilliseconds, 
                     kRefreshTimerIntervalInMilliseconds);
            }
         }
         else
         {
            if (this.refreshTimer != null)
            {
               this.refreshTimer.Dispose();
               this.refreshTimer = null;
            }
         }
      }

      public string SelectedRoute
      {
         get => this.selectedRoute;

         set
         {
            this.selectedRoute = value;
            this.VirtualDevices.SelectedRouteName = value;
            this.NotifyPropertyChanged();
            this.NotifyPropertyChanged(nameof(this.ExpandedRoutePath));
         }
      }

      public IEnumerable<string> VirtualDeviceNames => this.VirtualDevices.Names;

      public IEnumerable<string> RouteNames => this.VirtualDevices.Routes;

      public string Comment => this.VirtualDevices.Comment;

      public IEnumerable<DeviceInfo> DeviceInfo => this.VirtualDevices.DeviceInfo;
      public IEnumerable<ChannelInfo> ChannelInfo
      {
         get
         {
            var channelInfo = this.VirtualDevices.ChannelInfo;
            var routeInfo = this.RouteInfo;

            foreach (var channel in channelInfo)
            {
               foreach (var route in routeInfo)
               {
                  if (route.Endpoint1.Any())
                  {
                     if (channel.Name == route.Endpoint1)
                     {
                        if (!channel.Connected)
                           channel.index = route.index;
                     }
                  }

                  if (route.Endpoint2.Any())
                  {
                     if (channel.Name == route.Endpoint2)
                     {
                        if (!channel.Connected)
                           channel.index = route.index;
                     }
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
            var routeInfo = this.VirtualDevices.RouteInfo;

            foreach (var route in routeInfo)
            {
               int i = 0;
               var connectedRoutes = this.ConnectedRoutes;
               foreach (var connectedRoute in connectedRoutes)
               {
                  IEnumerable<string> expandedRoutePathList = new List<string>();
                  var expandedRouteList = this.ExpandedRouteList(connectedRoute);
                  if (expandedRouteList.Any())
                     expandedRoutePathList = expandedRouteList.Split('&').Select(x => x.Trim());

                  foreach (var expandedRoute in expandedRoutePathList)
                  {
                     if (expandedRoute == route.Name)
                     {
                        route.index = i;

                        // if connection is a group then let the user know the group name
                        if (route.Name != connectedRoute)
                           route.ConnectedGroup = connectedRoute;
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
            if (this.SelectedVirtualDevice == string.Empty)
               this.connectedRoutes = new List<string>();
            else
            {
               var allConnections = this.SwitchExecutive.GetAllConnections();
               if (allConnections.Any())
                  this.connectedRoutes = allConnections.Split('&').Select(x => x.Trim());
               else
                  this.connectedRoutes = new List<string>();
            }

            return this.connectedRoutes;
         }
      }

      [JsonProperty]
      [JsonConverter(typeof(SingleOrArrayConverter<string>))]
      public List<string> ConnectedRoutesCache
      {
         get => this.ConnectedRoutes.ToList(); // used by save() to save routes into the file
         set => this.connectedRoutesCache = value; // used by applyLoadFromFile() to reapply routes
      }

      public void ApplyLoadFromFile(MulticonnectMode connectionMode = MulticonnectMode.Multiconnect)
      {
         if (!this.connectedRoutesCache.Any())
            return;

         // ensure a virtual device is selected to apply routes
         if (!this.SelectedVirtualDevice.Any())
            return;

         // assume if a route is selected then we should apply routes from the saved file
         if (!this.SelectedRoute.Any())
            return;

         foreach (var route in this.connectedRoutesCache)
         {
            if (!this.IsRouteConnected(route) && (route != DriverOperationsConstants.NoConnections))
               this.TryConnect(route, connectionMode);
         }

         this.NotifyProperitiesConnectedRouteChanged();
      }

      public string ExpandedRoutePath
      {
         get
         {
            if (this.CanDisconnect())
               this.expandedRoutePath = this.SwitchExecutive.ExpandRouteSpec(this.SelectedRoute, ExpandOptions.ExpandToPaths);
            else
               this.expandedRoutePath = string.Empty;

            return this.expandedRoutePath;
         }
      }

      public string ExpandedRoutePathForRoute(string route)
      {
         string path = string.Empty;
         if (this.CanDisconnectRoute(route))
            path = this.SwitchExecutive.ExpandRouteSpec(route, ExpandOptions.ExpandToPaths);
         else
            path = string.Empty;

         return path;
      }

      public bool CanConnect()
      {
         if (!this.SelectedVirtualDevice.Any())
            return false;

         if (!this.SelectedRoute.Any())
            return false;

         return true;
      }

      public bool CanDisconnect()
      {
         return this.CanDisconnectRoute(this.SelectedRoute);
      }

      public bool CanDisconnectAll()
      {
         if (!this.SelectedVirtualDevice.Any())
            return false;

         return this.connectedRoutes.Any();
      }

      public void TryConnect(MulticonnectMode connectionMode) => this.TryConnect(this.SelectedRoute, connectionMode);

      public void TryConnect(string route, MulticonnectMode connectionMode)
      {
         this.SwitchExecutive.Connect(route, connectionMode, true);
         this.NotifyProperitiesConnectedRouteChanged();
      }

      public void TryDisconnect() => this.TryDisconnectRoute(this.SelectedRoute);

      public void TryDisconnectRoute(string route)
      {
         this.SwitchExecutive.Disconnect(route);
         this.NotifyProperitiesConnectedRouteChanged();
      }

      public bool IsConnected() => this.SwitchExecutive.IsConnected(this.SelectedRoute);

      public void TryDisconnectAll()
      {
         this.SwitchExecutive.DisconnectAll();
         this.NotifyProperitiesConnectedRouteChanged();
      }

      public void Shutdown()
      {
         if (this.switchExecutive != null)
            this.switchExecutive.Dispose();
      }

      public void Dispose() => this.Shutdown();

      private string ExpandedRouteList(string route)
      {
         string expandedRoute = string.Empty;

         if (this.CanDisconnectRoute(route))
            expandedRoute = this.SwitchExecutive.ExpandRouteSpec(route, ExpandOptions.ExpandToRoutes);
         else
            expandedRoute = string.Empty;

         return expandedRoute;
      }

      public bool CanDisconnectRoute(string route)
      {
         if (!this.SelectedVirtualDevice.Any())
            return false;

         if (!this.SelectedRoute.Any())
            return false;

         if (route == DriverOperationsConstants.NoConnections)
            return false;

         return this.IsRouteConnected(route);
      }

      public bool IsRouteConnected(string route)
      {
         if (route == DriverOperationsConstants.NoConnections)
            return false;

         return this.SwitchExecutive.IsConnected(route);
      }

      private void NotifyProperitiesConnectedRouteChanged()
      {
         this.NotifyPropertyChanged(nameof(this.ConnectedRoutes));
         this.NotifyPropertyChanged(nameof(this.RouteInfo));
         this.NotifyPropertyChanged(nameof(this.ExpandedRoutePath));
      }

      private bool AreStringListsEqual(IEnumerable<string> A, IEnumerable<string> B)
      {
         bool equal = (A.Count() == B.Count() && (!A.Except(B).Any() || !B.Except(A).Any()));
         return equal;
      }

      private IVirtualDevices VirtualDevices
      {
         get
         {
            if (virtualDevices == null)
               this.virtualDevices = VirtualDevicesFactory.CreateVirtualDevice(this.simulate);

            return this.virtualDevices;
         }
      }

      private NISwitchExecutiveInterface SwitchExecutive
      {
         get
         {
            if (this.switchExecutive == null)
            {
               this.switchExecutive = 
                  NISwitchExecutiveFactory.CreateNISwitchExecutive(
                     this.SelectedVirtualDevice, 
                     this.simulate);

               return this.switchExecutive;
            }

            if (this.switchExecutive.Name == this.SelectedVirtualDevice)
            {
               // return existing session
               return this.switchExecutive;
            }

            // close existing session and create new one
            this.switchExecutive.Dispose();
            this.switchExecutive = 
               NISwitchExecutiveFactory.CreateNISwitchExecutive(
                  this.SelectedVirtualDevice, 
                  this.simulate);
            return this.switchExecutive;
         }
      }
   }
}