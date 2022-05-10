using System.Collections.Generic;

namespace SwitchExecutive.Plugin.Internal.DriverOperations.Internal
{
   internal interface IVirtualDevices
   {
      IEnumerable<string> Names { get; }
      IEnumerable<string> Routes { get; }
      IEnumerable<DeviceInfo> DeviceInfo { get; }
      IEnumerable<ChannelInfo> ChannelInfo { get; }
      IEnumerable<RouteInfo> RouteInfo { get; }
      string SelectedName { get; set; }
      string SelectedRouteName { get; set; }
      string Comment { get; }
   }
}
