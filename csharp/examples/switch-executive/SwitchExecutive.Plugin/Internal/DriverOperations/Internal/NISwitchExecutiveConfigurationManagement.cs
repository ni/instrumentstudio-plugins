using System;
using System.Collections.Generic;
using System.Linq;

namespace SwitchExecutive.Plugin.Internal.DriverOperations.Internal
{
    internal interface NISwitchExecutiveConfigurationManagementInterface
    {
        IEnumerable<string> VirtualDeviceNames { get; }
        IEnumerable<string> Routes(string virtualDeviceName);
        string Comment(string virtualDeviceName, string route);
        IEnumerable<DeviceInfo> DeviceInfo(string virtualDeviceName);
        IEnumerable<ChannelInfo> ChannelInfo(string virtualDeviceName);
        IEnumerable<RouteInfo> RouteInfo(string virtualDeviceName);
    }

    internal class NISwitchExecutiveConfigurationManagement : NISwitchExecutiveConfigurationManagementInterface
    {
        public IEnumerable<string> VirtualDeviceNames
        {
            get
            {
                niseCfg.NiseVirtualDevices switchConfigurationManager = new niseCfg.NiseVirtualDevices();
                foreach (niseCfg.NiseVirtualDevice device in switchConfigurationManager)
                {
                    yield return device.Name;
                }
            }
        }

        public IEnumerable<string> Routes(string virtualDeviceName)
        {
            niseCfg.NiseVirtualDevices switchConfigurationManager = new niseCfg.NiseVirtualDevices();
            List<string> allRoutes = new List<string>();

            niseCfg.NiseVirtualDevice virtualDevice = null;
            foreach (niseCfg.NiseVirtualDevice device in switchConfigurationManager)
            {
                if (device.Name == virtualDeviceName)
                {
                    virtualDevice = device;
                    break;
                }
            }

            if (virtualDevice == null)
                return allRoutes;

            // return all route groups first then routes but alphabetize each one
            List<string> allRoutesGroups = new List<string>();
            foreach (niseCfg.RouteGroup routeGroup in virtualDevice.RouteGroups)
            {
                allRoutesGroups.Add(routeGroup.Name);
            }
            allRoutesGroups.OrderBy(q => q);

            foreach (niseCfg.Route route in virtualDevice.Routes)
            {
                allRoutes.Add(route.Name);
            }
            allRoutes.OrderBy(q => q);

            allRoutesGroups.AddRange(allRoutes);
            allRoutes = allRoutesGroups;

            return allRoutes;
        }

        public string Comment(string virtualDeviceName, string route)
        {
            niseCfg.NiseVirtualDevices switchConfigurationManager = new niseCfg.NiseVirtualDevices();

            niseCfg.NiseVirtualDevice virtualDevice = null;
            foreach (niseCfg.NiseVirtualDevice device in switchConfigurationManager)
            {
                if (device.Name == virtualDeviceName)
                {
                    virtualDevice = device;
                    break;
                }
            }

            if (virtualDevice == null)
                return string.Empty;

            foreach (niseCfg.RouteGroup routeGroup in virtualDevice.RouteGroups)
            {
                if (routeGroup.Name == route)
                    return routeGroup.Comment;
            }

            foreach (niseCfg.Route aRoute in virtualDevice.Routes)
            {
                if (aRoute.Name == route)
                    return aRoute.Comment;
            }

            return string.Empty;
        }

        public IEnumerable<DeviceInfo> DeviceInfo(string virtualDeviceName)
        {
            niseCfg.NiseVirtualDevices switchConfigurationManager = new niseCfg.NiseVirtualDevices();
            List<DeviceInfo> info = new List<DeviceInfo>();

            niseCfg.NiseVirtualDevice virtualDevice = null;
            foreach (niseCfg.NiseVirtualDevice device in switchConfigurationManager)
            {
                if (device.Name == virtualDeviceName)
                {
                    virtualDevice = device;
                    break;
                }
            }

            if (virtualDevice == null)
                return info;

            foreach (niseCfg.IviDevice2 device in virtualDevice.IviDevices)
            {
                info.Add(new DeviceInfo(device.Name, device.TopologyName, device.Comment));
            }

            return info;
        }

        public IEnumerable<ChannelInfo> ChannelInfo(string virtualDeviceName)
        {
            niseCfg.NiseVirtualDevices switchConfigurationManager = new niseCfg.NiseVirtualDevices();
            List<ChannelInfo> info = new List<ChannelInfo>();

            niseCfg.NiseVirtualDevice virtualDevice = null;
            foreach (niseCfg.NiseVirtualDevice device in switchConfigurationManager)
            {
                if (device.Name == virtualDeviceName)
                {
                    virtualDevice = device;
                    break;
                }
            }

            if (virtualDevice == null)
                return info;

            foreach (niseCfg.NiseChannel channel in virtualDevice.Channels)
            {
                info.Add(
                   new ChannelInfo(
                      channel.Name,
                      channel.FormattedName[niseCfg.NiseChannelNameStyle.kFullName],
                      channel.Enabled,
                      channel.IviDevice.Name,
                      channel.ReservedForRouting.ToString(),
                      channel.Hardwire?.Name,
                      channel.Comment));
            }

            return info;
        }

        public IEnumerable<RouteInfo> RouteInfo(string virtualDeviceName)
        {
            niseCfg.NiseVirtualDevices switchConfigurationManager = new niseCfg.NiseVirtualDevices();
            List<RouteInfo> info = new List<RouteInfo>();

            niseCfg.NiseVirtualDevice virtualDevice = null;
            foreach (niseCfg.NiseVirtualDevice device in switchConfigurationManager)
            {
                if (device.Name == virtualDeviceName)
                {
                    virtualDevice = device;
                    break;
                }
            }

            if (virtualDevice == null)
                return info;

            foreach (niseCfg.NiseRoute route in virtualDevice.Routes)
            {
                var routeGroups = new List<string>();
                foreach (niseCfg.NiseRouteGroup routeGroup in route.ParentRouteGroups)
                {
                    routeGroups.Add(routeGroup.Name);
                }

                info.Add(
                   new RouteInfo(
                      route.Name,
                      string.Join(",", routeGroups.ToArray()),
                      route.Endpoint1?.Name,
                      route.Endpoint2?.Name,
                      route.Specification,
                      route.Comment));
            }

            return info;
        }
    }
}
