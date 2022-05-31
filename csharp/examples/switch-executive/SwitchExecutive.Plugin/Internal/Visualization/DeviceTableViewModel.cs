using System.Collections.Generic;
using System.ComponentModel;
using SwitchExecutive.Plugin.Internal.Common;
using SwitchExecutive.Plugin.Internal.DriverOperations;

namespace SwitchExecutive.Plugin.Internal
{
    internal sealed class DeviceTableViewModel : BaseNotify
    {
        private readonly ISwitchExecutiveDriverOperations _driverOperations;

        public DeviceTableViewModel(
           ISwitchExecutiveDriverOperations driverOperations)
        {
            _driverOperations = driverOperations;

            _driverOperations.PropertyChanged += DriverOperations_PropertyChanged;
        }

        public bool IsContentCollapsed { get; set; } = true;
        public double PreferredProportion { get; set; } = Constants.InstrumentPanels.TableContainerDefaultProportion;
        public IEnumerable<DeviceInfo> Info => _driverOperations.DeviceInfo;

        public void DriverOperations_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_driverOperations.DeviceInfo))
                NotifyPropertyChanged(nameof(Info));
        }
    }
}
