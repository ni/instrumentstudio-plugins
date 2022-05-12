using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using SwitchExecutive.Plugin.Internal.DriverOperations;
using SwitchExecutive.Plugin.Internal.Controls;
using SwitchExecutive.Plugin.Internal.Common;

namespace SwitchExecutive.Plugin.Internal
{
    class ConnectedRoute
    {
        public const string NoConnections = "No connections";

        private readonly ISwitchExecutiveDriverOperations driverOperations;
        private readonly ISave saveOperation;
        private readonly IStatus status;

        public ConnectedRoute(
           string name,
           string displayColor,
           ISwitchExecutiveDriverOperations driverOperations,
           ISave saveOperation,
           IStatus status)
        {
            this.Name = name;
            this.DisplayColor = displayColor;
            this.driverOperations = driverOperations;
            this.saveOperation = saveOperation;
            this.status = status;

            Action disconnectAction = async () =>
            {
                await Task.Run(() => this.OnDisconnect(null));
                this.Save();
            };
            this.DisconnectRouteCommand =
               new NationalInstruments.RelayCommand(
                  execute: o => disconnectAction(),
                  canExecute: this.CanDisconnect);
        }

        public bool ShowViewOptions => this.Name != ConnectedRoute.NoConnections;
        public string Name { get; }
        public string DisplayColor { get; } = Constants.InstrumentPanels.NoBlockBannerColor;
        public string ExpandedRoutePath
        {
            get
            {
                string path = string.Empty;
                var routeList = this.driverOperations.ExpandedRoutePathForRoute(this.Name);
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
                if (this.Name != ConnectedRoute.NoConnections)
                {
                    if (this.driverOperations.IsRouteConnected(this.Name))
                        this.driverOperations.TryDisconnectRoute(this.Name);
                }
            }
            catch (DriverException e)
            {
                this.SetErrorMessage(e.Message);
            }
        }
        private bool CanDisconnect(object obj)
        {
            bool canDisconnect = false;
            try
            {
                if (this.Name != ConnectedRoute.NoConnections)
                    canDisconnect = this.driverOperations.CanDisconnectRoute(this.Name);
            }
            catch (DriverException)
            {
                // this method is to prevent the user from clicking buttons that will fail
                // so swallow any other errors that come back.  if revelant the user will
                // get the error on a user interaction.
                //this.SetErrorMessage(e.Message);
            }

            return canDisconnect;
        }

        private void SetErrorMessage(string msg) => this.status.Set(msg);
        private void ClearErrorMessage() => this.status.Clear();
        private void Save() => this.saveOperation.Save();
    }

    internal sealed class ConnectedRouteTableViewModel : BaseNotify
    {
        private readonly ISwitchExecutiveDriverOperations driverOperations;
        private readonly ISave saveOperation;
        private IStatus status;
        private IEnumerable<string> connectedRoutesCache = new List<string>();

        public ConnectedRouteTableViewModel(
           ISwitchExecutiveDriverOperations driverOperations,
           ISave saveOperation,
           IStatus status)
        {
            this.driverOperations = driverOperations;
            this.saveOperation = saveOperation;
            this.status = status;

            this.driverOperations.PropertyChanged += DriverOperations_PropertyChanged;
        }

        public IEnumerable<ConnectedRoute> Info
        {
            get
            {
                var connectedRoutesList = new List<ConnectedRoute>();
                int i = 0;
                foreach (string route in this.ConnectedRoutesCache)
                {
                    connectedRoutesList.Add(
                       new ConnectedRoute(
                          route,
                          PlotColors.GetPlotColorStringForIndex(i),
                          this.driverOperations,
                          this.saveOperation,
                          this.status));
                    i++;
                }

                if (!connectedRoutesList.Any())
                {
                    connectedRoutesList.Add(
                       new ConnectedRoute(
                          ConnectedRoute.NoConnections,
                          Constants.InstrumentPanels.NoBlockBannerColor,
                          this.driverOperations,
                          this.saveOperation,
                          this.status));
                }

                this.NotifyPropertyChanged(nameof(ConnectedRoutesStyle));
                return connectedRoutesList;
            }
        }
        public FontStyle ConnectedRoutesStyle => this.AnyConnectedRoutes ? FontStyles.Normal : FontStyles.Italic;
        private bool AnyConnectedRoutes => this.ConnectedRoutesCache.Any();

        private void DriverOperations_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(this.driverOperations.ConnectedRoutes))
            {
                // the table has popups, so redrawing unnecessarily causes the popups to dismiss.  Let's 
                // do a check to ensure something has changed before redrawing.
                var newConnectedRoutes = this.driverOperations.ConnectedRoutes;
                if (!this.AreStringListsEqual(this.ConnectedRoutesCache, newConnectedRoutes))
                {
                    this.ConnectedRoutesCache = newConnectedRoutes;
                    this.NotifyPropertyChanged(nameof(this.Info));
                }
            }
        }

        private IEnumerable<string> ConnectedRoutesCache
        {
            get => this.connectedRoutesCache;
            set => this.connectedRoutesCache = value;
        }

        private bool AreStringListsEqual(IEnumerable<string> A, IEnumerable<string> B)
        {
            bool equal = (A.Count() == B.Count() && (!A.Except(B).Any() || !B.Except(A).Any()));
            return equal;
        }
    }
}
