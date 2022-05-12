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
    internal sealed class DeviceTableViewModel : BaseNotify
    {
        private readonly ISwitchExecutiveDriverOperations driverOperations;

        public DeviceTableViewModel(
           ISwitchExecutiveDriverOperations driverOperations)
        {
            this.driverOperations = driverOperations;

            this.driverOperations.PropertyChanged += DriverOperations_PropertyChanged;
        }

        public bool IsContentCollapsed { get; set; } = true;
        public double PreferredProportion { get; set; } = Constants.InstrumentPanels.TableContainerDefaultProportion;
        public IEnumerable<DeviceInfo> Info => this.driverOperations.DeviceInfo;

        public void DriverOperations_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(this.driverOperations.DeviceInfo))
                this.NotifyPropertyChanged(nameof(this.Info));
        }
    }
}
