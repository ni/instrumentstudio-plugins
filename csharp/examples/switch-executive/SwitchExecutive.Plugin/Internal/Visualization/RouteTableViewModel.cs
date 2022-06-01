using System.Collections.Generic;
using System.ComponentModel;
using SwitchExecutive.Plugin.Internal.Common;
using SwitchExecutive.Plugin.Internal.Controls;
using SwitchExecutive.Plugin.Internal.DriverOperations;

namespace SwitchExecutive.Plugin.Internal
{
    internal sealed class RouteTableViewModel : BaseNotify
    {
        private readonly ISwitchExecutiveDriverOperations _driverOperations;

        public RouteTableViewModel(
           ISwitchExecutiveDriverOperations driverOperations)
        {
            _driverOperations = driverOperations;

            _driverOperations.PropertyChanged += DriverOperations_PropertyChanged;
        }

        public bool IsContentCollapsed { get; set; } = false;
        public double PreferredProportion { get; set; } = Constants.InstrumentPanels.TableContainerDefaultProportion;
        public IEnumerable<RouteInfo> Info
        {
            get
            {
                var routes = _driverOperations.RouteInfo;
                foreach (var route in routes)
                {
                    if (route.Index != RouteInfo.NotConnected)
                    {
                        route.DisplayColor = PlotColors.GetPlotColorStringForIndex(route.Index);
                    }
                }
                return routes;
            }
        }

        public void DriverOperations_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_driverOperations.RouteInfo)
                || e.PropertyName == nameof(_driverOperations.ConnectedRoutes))
            {
                NotifyPropertyChanged(nameof(Info));
            }
        }
    }
}
