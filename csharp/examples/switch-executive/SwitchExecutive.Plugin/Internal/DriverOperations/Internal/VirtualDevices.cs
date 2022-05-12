using System.Collections.Generic;
using System.Linq;

namespace SwitchExecutive.Plugin.Internal.DriverOperations.Internal
{
    internal class VirtualDevices : IVirtualDevices
    {
        private readonly NISwitchExecutiveConfigurationManagementInterface switchExecutiveConfigurationManagement;
        private string selectedName = string.Empty;
        private string selectedRouteName = string.Empty;

        public VirtualDevices(NISwitchExecutiveConfigurationManagementInterface switchExecutiveConfigurationManagement)
        {
            this.switchExecutiveConfigurationManagement = switchExecutiveConfigurationManagement;
        }

        public IEnumerable<string> Names => this.switchExecutiveConfigurationManagement.VirtualDeviceNames;
        public IEnumerable<string> Routes => this.switchExecutiveConfigurationManagement.Routes(this.SelectedName);
        public IEnumerable<DeviceInfo> DeviceInfo => this.switchExecutiveConfigurationManagement.DeviceInfo(this.SelectedName);
        public IEnumerable<ChannelInfo> ChannelInfo => this.switchExecutiveConfigurationManagement.ChannelInfo(this.SelectedName);
        public IEnumerable<RouteInfo> RouteInfo => this.switchExecutiveConfigurationManagement.RouteInfo(this.SelectedName);

        public string Comment =>
           this.SelectedRouteName.Any()
              ? this.switchExecutiveConfigurationManagement.Comment(this.SelectedName, this.SelectedRouteName)
              : string.Empty;

        public string SelectedName
        {
            get => this.selectedName;

            set
            {
                if (value == null) { return; }
                this.selectedName = value;
                this.SelectedRouteName = string.Empty;
            }
        }

        public string SelectedRouteName
        {
            get => this.selectedRouteName;

            set
            {
                if (value == null) { return; }
                this.selectedRouteName = value;
            }
        }
    }
}
