using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SwitchExecutive.Plugin.Internal.DriverOperations;
using SwitchExecutive.Plugin.Internal.Controls;
using SwitchExecutive.Plugin.Internal.Common;

namespace SwitchExecutive.Plugin.Internal
{
    internal sealed class ChannelTableViewModel : BaseNotify
    {
        private readonly ISwitchExecutiveDriverOperations _driverOperations;

        public ChannelTableViewModel(
           ISwitchExecutiveDriverOperations driverOperations)
        {
            _driverOperations = driverOperations;

            _driverOperations.PropertyChanged += DriverOperations_PropertyChanged;
        }

        public bool IsContentCollapsed { get; set; } = true;
        public double PreferredProportion { get; set; } = Constants.InstrumentPanels.TableContainerDefaultProportion;
        public IEnumerable<ChannelInfo> Info
        {
            get
            {
                var channels = _driverOperations.ChannelInfo;
                foreach (var channel in channels)
                {
                    if (channel.index != ChannelInfo.NotConnected)
                        channel.DisplayColor = PlotColors.GetPlotColorStringForIndex(channel.index);
                }
                return channels;
            }
        }

        public void DriverOperations_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_driverOperations.DeviceInfo))
                NotifyPropertyChanged(nameof(Info));
            if (e.PropertyName == nameof(_driverOperations.ConnectedRoutes))
                NotifyPropertyChanged(nameof(Info));
        }
    }

}
