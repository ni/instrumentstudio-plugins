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
      private readonly ISwitchExecutiveDriverOperations driverOperations;

      public ChannelTableViewModel(
         ISwitchExecutiveDriverOperations driverOperations)
      {
         this.driverOperations = driverOperations;

         this.driverOperations.PropertyChanged += DriverOperations_PropertyChanged;
      }

      public bool IsContentCollapsed { get; set; } = true;
      public double PreferredProportion { get; set; } = Constants.InstrumentPanels.TableContainerDefaultProportion;
      public IEnumerable<ChannelInfo> Info
      {
         get
         {
            var channels = this.driverOperations.ChannelInfo;
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
         if (e.PropertyName == nameof(this.driverOperations.DeviceInfo))
            this.NotifyPropertyChanged(nameof(this.Info));
         if (e.PropertyName == nameof(this.driverOperations.ConnectedRoutes))
            this.NotifyPropertyChanged(nameof(this.Info));
      }
   }

}
