using System.Collections.Generic;
using System.Linq;

namespace SwitchExecutive.Plugin.Internal.DriverOperations.Internal
{
    internal class VirtualDevices : IVirtualDevices
    {
        private readonly NISwitchExecutiveConfigurationManagementInterface _switchExecutiveConfigurationManagement;
        private string _selectedName = string.Empty;
        private string _selectedRouteName = string.Empty;

        public VirtualDevices(NISwitchExecutiveConfigurationManagementInterface switchExecutiveConfigurationManagement)
        {
            _switchExecutiveConfigurationManagement = switchExecutiveConfigurationManagement;
        }

        public IEnumerable<string> Names => _switchExecutiveConfigurationManagement.VirtualDeviceNames;
        public IEnumerable<string> Routes => _switchExecutiveConfigurationManagement.Routes(SelectedName);
        public IEnumerable<DeviceInfo> DeviceInfo => _switchExecutiveConfigurationManagement.DeviceInfo(SelectedName);
        public IEnumerable<ChannelInfo> ChannelInfo => _switchExecutiveConfigurationManagement.ChannelInfo(SelectedName);
        public IEnumerable<RouteInfo> RouteInfo => _switchExecutiveConfigurationManagement.RouteInfo(SelectedName);

        public string Comment =>
           SelectedRouteName.Any()
              ? _switchExecutiveConfigurationManagement.Comment(SelectedName, SelectedRouteName)
              : string.Empty;

        public string SelectedName
        {
            get => _selectedName;

            set
            {
                if (value == null) { return; }
                _selectedName = value;
                SelectedRouteName = string.Empty;
            }
        }

        public string SelectedRouteName
        {
            get => _selectedRouteName;

            set
            {
                if (value == null) { return; }
                _selectedRouteName = value;
            }
        }
    }
}
