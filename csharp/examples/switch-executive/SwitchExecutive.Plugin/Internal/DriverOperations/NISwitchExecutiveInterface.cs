using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwitchExecutive.Plugin.Internal.DriverOperations
{
    public enum MulticonnectMode
    {
        DefaultMode = -1,
        NoMulticonnect = 0,
        Multiconnect = 1
    }

    public enum OperationOrder
    {
        None = 0,
        BreakBeforeMake = 1,
        BreakAfterMake = 2
    }

    public enum ExpandOptions
    {
        ExpandToRoutes = 0,
        ExpandToPaths = 1
    }

    public interface NISwitchExecutiveInterface
    {
        void Dispose();
        string Name { get; }
        void Connect(string connectSpec, MulticonnectMode multiconnectMode, bool waitForDebounce);
        void Disconnect(string spec);
        void DisconnectAll();
        void ConnectAndDisconnect(string connectSpec, string disconnectSpec, MulticonnectMode multiconnectMode, OperationOrder operationOrder, bool waitForDebounce);
        bool IsConnected(string spec);
        string ExpandRouteSpec(string spec, ExpandOptions expandOptions);
        string GetAllConnections();
    }

    public class DeviceInfo
    {
        public DeviceInfo(string name, string topology, string comment)
        {
            Name = name;
            Topology = topology;
            Comment = comment;
        }

        public string Name { get; }
        public string Topology { get; }
        public string Comment { get; }
    }

    public class ChannelInfo
    {
        public const int NotConnected = -1;

        public ChannelInfo(
           string name,
           string formattedName,
           bool status,
           string deviceName,
           string reservedForRouting,
           string hardWire,
           string comment)
        {
            Name = name;
            FormattedName = formattedName;
            Status = status ? "Enabled" : string.Empty;
            DeviceName = deviceName;
            ReservedForRouting = reservedForRouting;
            HardWire = hardWire;
            Comment = comment;
        }

        public string Name { get; }
        public string FormattedName { get; }
        public string Status { get; }
        public string DeviceName { get; }
        public string ReservedForRouting { get; }
        public string HardWire { get; }
        public string Comment { get; }
        public int Index { get; set; } = ChannelInfo.NotConnected;
        public string DisplayColor { get; set; } = Constants.InstrumentPanels.NoBlockBannerColor;
        public bool Connected => Index != ChannelInfo.NotConnected;
    }

    public class RouteInfo
    {
        public const int NotConnected = -1;

        public RouteInfo(
           string name,
           string routeGroup,
           string endpoint1,
           string endpoint2,
           string specification,
           string comment)
        {
            Name = name;
            RouteGroup = routeGroup;
            Endpoint1 = endpoint1;
            Endpoint2 = endpoint2;
            Specification = specification;
            Comment = comment;
        }

        public string Name { get; }
        public string RouteGroup { get; }
        public string Endpoint1 { get; }
        public string Endpoint2 { get; }
        public string Specification { get; }
        public string Comment { get; }
        public string ConnectedGroup { get; set; }
        public string Connected => Index == RouteInfo.NotConnected ? string.Empty : "Connected";
        public int Index { get; set; } = RouteInfo.NotConnected;
        public string DisplayColor { get; set; } = Constants.InstrumentPanels.NoBlockBannerColor;
    }
}
