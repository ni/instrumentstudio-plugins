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
            this.Name = name;
            this.Topology = topology;
            this.Comment = comment;
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
           string hardwire,
           string comment)
        {
            this.Name = name;
            this.FormattedName = formattedName;
            this.Status = status ? "Enabled" : string.Empty;
            this.DeviceName = deviceName;
            this.ReservedForRouting = reservedForRouting;
            this.Hardwire = hardwire;
            this.Comment = comment;
        }

        public string Name { get; }
        public string FormattedName { get; }
        public string Status { get; }
        public string DeviceName { get; }
        public string ReservedForRouting { get; }
        public string Hardwire { get; }
        public string Comment { get; }
        public int index { set; get; } = ChannelInfo.NotConnected;
        public string DisplayColor { set; get; } = Constants.InstrumentPanels.NoBlockBannerColor;
        public bool Connected => this.index != ChannelInfo.NotConnected;
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
            this.Name = name;
            this.RouteGroup = routeGroup;
            this.Endpoint1 = endpoint1;
            this.Endpoint2 = endpoint2;
            this.Specification = specification;
            this.Comment = comment;
        }

        public string Name { get; }
        public string RouteGroup { get; }
        public string Endpoint1 { get; }
        public string Endpoint2 { get; }
        public string Specification { get; }
        public string Comment { get; }
        public string ConnectedGroup { set; get; }
        public string Connected => index == RouteInfo.NotConnected ? string.Empty : "Connected";
        public int index { set; get; } = RouteInfo.NotConnected;
        public string DisplayColor { set; get; } = Constants.InstrumentPanels.NoBlockBannerColor;
    }
}
