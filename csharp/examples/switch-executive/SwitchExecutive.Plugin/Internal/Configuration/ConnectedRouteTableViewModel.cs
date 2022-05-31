using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using SwitchExecutive.Plugin.Internal.Common;
using SwitchExecutive.Plugin.Internal.Controls;
using SwitchExecutive.Plugin.Internal.DriverOperations;

namespace SwitchExecutive.Plugin.Internal
{
    internal class ConnectedRoute
    {
        public const string NoConnections = "No connections";

        private readonly ISwitchExecutiveDriverOperations _driverOperations;
        private readonly ISave _saveOperation;
        private readonly IStatus _status;

        public ConnectedRoute(
           string name,
           string displayColor,
           ISwitchExecutiveDriverOperations driverOperations,
           ISave saveOperation,
           IStatus status)
        {
            Name = name;
            DisplayColor = displayColor;
            _driverOperations = driverOperations;
            _saveOperation = saveOperation;
            _status = status;

            Action disconnectAction = async () =>
            {
                await Task.Run(() => OnDisconnect(null));
                Save();
            };
            DisconnectRouteCommand =
               new NationalInstruments.RelayCommand(
                  execute: o => disconnectAction(),
                  canExecute: CanDisconnect);
        }

        public bool ShowViewOptions => Name != ConnectedRoute.NoConnections;
        public string Name { get; }
        public string DisplayColor { get; } = Constants.InstrumentPanels.NoBlockBannerColor;
        public string ExpandedRoutePath
        {
            get
            {
                string path = string.Empty;
                var routeList = _driverOperations.ExpandedRoutePathForRoute(Name);
                if (routeList.Any())
                {
                    path =
                       string.Join(
                          Environment.NewLine,
                          routeList.Split('&').Select(x => x.Trim()));
                }

                return path;
            }
        }

        public ICommand DisconnectRouteCommand { get; }

        private void OnDisconnect(object obj)
        {
            try
            {
                if (Name != ConnectedRoute.NoConnections)
                {
                    if (_driverOperations.IsRouteConnected(Name))
                    {
                        _driverOperations.TryDisconnectRoute(Name);
                    }
                }
            }
            catch (DriverException e)
            {
                SetErrorMessage(e.Message);
            }
        }
        private bool CanDisconnect(object obj)
        {
            bool canDisconnect = false;
            try
            {
                if (Name != ConnectedRoute.NoConnections)
                {
                    canDisconnect = _driverOperations.CanDisconnectRoute(Name);
                }
            }
            catch (DriverException)
            {
                // this method is to prevent the user from clicking buttons that will fail
                // so swallow any other errors that come back.  if revelant the user will
                // get the error on a user interaction.
                ////SetErrorMessage(e.Message);
            }

            return canDisconnect;
        }

        private void SetErrorMessage(string msg) => _status.Set(msg);
        private void ClearErrorMessage() => _status.Clear();
        private void Save() => _saveOperation.Save();
    }

    internal sealed class ConnectedRouteTableViewModel : BaseNotify
    {
        private readonly ISwitchExecutiveDriverOperations _driverOperations;
        private readonly ISave _saveOperation;
        private IStatus _status;
        private IEnumerable<string> _connectedRoutesCache = new List<string>();

        public ConnectedRouteTableViewModel(
           ISwitchExecutiveDriverOperations driverOperations,
           ISave saveOperation,
           IStatus status)
        {
            _driverOperations = driverOperations;
            _saveOperation = saveOperation;
            _status = status;

            _driverOperations.PropertyChanged += DriverOperations_PropertyChanged;
        }

        public IEnumerable<ConnectedRoute> Info
        {
            get
            {
                var connectedRoutesList = new List<ConnectedRoute>();
                int i = 0;
                foreach (string route in ConnectedRoutesCache)
                {
                    connectedRoutesList.Add(
                       new ConnectedRoute(
                          route,
                          PlotColors.GetPlotColorStringForIndex(i),
                          _driverOperations,
                          _saveOperation,
                          _status));
                    i++;
                }

                if (!connectedRoutesList.Any())
                {
                    connectedRoutesList.Add(
                       new ConnectedRoute(
                          ConnectedRoute.NoConnections,
                          Constants.InstrumentPanels.NoBlockBannerColor,
                          _driverOperations,
                          _saveOperation,
                          _status));
                }

                NotifyPropertyChanged(nameof(ConnectedRoutesStyle));
                return connectedRoutesList;
            }
        }
        public FontStyle ConnectedRoutesStyle => AnyConnectedRoutes ? FontStyles.Normal : FontStyles.Italic;
        private bool AnyConnectedRoutes => ConnectedRoutesCache.Any();

        private void DriverOperations_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_driverOperations.ConnectedRoutes))
            {
                // the table has popups, so redrawing unnecessarily causes the popups to dismiss.  Let's
                // do a check to ensure something has changed before redrawing.
                var newConnectedRoutes = _driverOperations.ConnectedRoutes;
                if (!AreStringListsEqual(ConnectedRoutesCache, newConnectedRoutes))
                {
                    ConnectedRoutesCache = newConnectedRoutes;
                    NotifyPropertyChanged(nameof(Info));
                }
            }
        }

        private IEnumerable<string> ConnectedRoutesCache
        {
            get => _connectedRoutesCache;
            set => _connectedRoutesCache = value;
        }

        private bool AreStringListsEqual(IEnumerable<string> a, IEnumerable<string> b)
        {
            bool equal = (a.Count() == b.Count() && (!a.Except(b).Any() || !b.Except(a).Any()));
            return equal;
        }
    }
}
