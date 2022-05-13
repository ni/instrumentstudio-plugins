using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwitchExecutive.Plugin.Internal.DriverOperations.Internal
{
    internal class FakeVirtualDevices : IVirtualDevices
    {
        public FakeVirtualDevices() { }

        public IEnumerable<string> Names => new List<string>();
        public IEnumerable<string> Routes => new List<string>();
        public IEnumerable<DeviceInfo> DeviceInfo => new List<DeviceInfo>();
        public IEnumerable<ChannelInfo> ChannelInfo => new List<ChannelInfo>();
        public IEnumerable<RouteInfo> RouteInfo => new List<RouteInfo>();

        public string SelectedName { get; set; }
        public string SelectedRouteName { get; set; }
        public string Comment { get; }
    }
}
