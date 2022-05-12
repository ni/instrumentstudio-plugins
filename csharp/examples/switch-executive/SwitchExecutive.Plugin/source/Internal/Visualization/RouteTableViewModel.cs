using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using SwitchExecutive.Plugin.Internal.DriverOperations;
using SwitchExecutive.Plugin.Internal.Controls;
using SwitchExecutive.Plugin.Internal.Common;

namespace SwitchExecutive.Plugin.Internal
{
    internal sealed class RouteTableViewModel : BaseNotify
    {
        private readonly ISwitchExecutiveDriverOperations driverOperations;

        public RouteTableViewModel(
           ISwitchExecutiveDriverOperations driverOperations)
        {
            this.driverOperations = driverOperations;

            this.driverOperations.PropertyChanged += DriverOperations_PropertyChanged;
        }

        public bool IsContentCollapsed { get; set; } = false;
        public double PreferredProportion { get; set; } = Constants.InstrumentPanels.TableContainerDefaultProportion;
        public IEnumerable<RouteInfo> Info
        {
            get
            {
                var routes = this.driverOperations.RouteInfo;
                foreach (var route in routes)
                {
                    if (route.index != RouteInfo.NotConnected)
                    {
                        route.DisplayColor = PlotColors.GetPlotColorStringForIndex(route.index);
                    }
                }
                return routes;
            }
        }

        public void DriverOperations_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(this.driverOperations.RouteInfo))
                this.NotifyPropertyChanged(nameof(this.Info));
            if (e.PropertyName == nameof(this.driverOperations.ConnectedRoutes))
                this.NotifyPropertyChanged(nameof(this.Info));
        }
    }
}
