using System.Collections.Generic;
using System.Linq;
using SwitchExecutive.Plugin.Internal.DriverOperations;
using Xunit;

namespace SwitchExecutive.Plugin.Tests
{
    public class NISwitchExecutiveDriverOperationsTests
    {
        [Fact]
        public void OnConstruction_NoThrow()
        {
            var driverOperations = new NISwitchExecutiveDriverOperations();
            Assert.NotNull(driverOperations);
        }

        [Fact]
        public void IsDriverInstalled_ReturnsTrue()
        {
            // The rest of these tests will fail if SwitchExecutive isn't installed. Needs default SwitchExecutiveExample in MAX.
            Assert.True(NISwitchExecutiveDriverOperations.IsDriverInstalled());
        }

        [Fact]
        public void DefaultSelectedVirtualDevice_IsEmpty()
        {
            var driverOperations = new NISwitchExecutiveDriverOperations();
            Assert.True(string.IsNullOrEmpty(driverOperations.SelectedVirtualDevice));
        }

        [Fact]
        public void SetVirtualDevice_ReturnsNoError()
        {
            var driverOperations = new NISwitchExecutiveDriverOperations();
            string newVirtualDevice = "VirtualDevice1";
            driverOperations.SelectedVirtualDevice = newVirtualDevice;
            Assert.True(driverOperations.SelectedVirtualDevice == newVirtualDevice);
        }

        [Fact]
        public void SetRoute_ReturnsNoError()
        {
            var driverOperations = new NISwitchExecutiveDriverOperations();
            string newRoute = "PowerUUT";
            driverOperations.SelectedRoute = newRoute;
            Assert.True(driverOperations.SelectedRoute == newRoute);
        }

        [Fact]
        public void VirtualDeviceNames_ReturnsSwitchExecutiveExample()
        {
            var driverOperations = new NISwitchExecutiveDriverOperations();
            IEnumerable<string> virtualDevices = driverOperations.VirtualDeviceNames;
            Assert.True(virtualDevices.FirstOrDefault(device => device == "SwitchExecutiveExample").Any());
        }

        [Fact]
        public void VirtualDeviceRoutes_ReturnsPowerUUT()
        {
            var driverOperations = new NISwitchExecutiveDriverOperations();
            string newVirtualDevice = "SwitchExecutiveExample";
            driverOperations.SelectedVirtualDevice = newVirtualDevice;

            IEnumerable<string> routes = driverOperations.RouteNames;
            Assert.True(routes.FirstOrDefault(route => route == "PowerUUT").Any());
        }

        [Fact]
        public void ConnectDisconnectRouteTest()
        {
            var driverOperations = new NISwitchExecutiveDriverOperations();
            string newVirtualDevice = "SwitchExecutiveExample";
            driverOperations.SelectedVirtualDevice = newVirtualDevice;
            driverOperations.TryDisconnectAll();
            string newRoute = "PowerUUT";
            driverOperations.SelectedRoute = newRoute;

            Assert.True(driverOperations.CanConnect());
            Assert.False(driverOperations.IsConnected());
            driverOperations.TryConnect(MulticonnectMode.Multiconnect);
            Assert.True(driverOperations.IsConnected());
            Assert.True(driverOperations.ConnectedRoutes.Any());

            Assert.True(driverOperations.CanDisconnect());
            driverOperations.TryDisconnect();
            Assert.False(driverOperations.IsConnected());
            Assert.False(driverOperations.ConnectedRoutes.Any());
        }

        [Fact]
        public void ConnectDisconnectAllRouteTest()
        {
            var driverOperations = new NISwitchExecutiveDriverOperations();
            string newVirtualDevice = "SwitchExecutiveExample";
            driverOperations.SelectedVirtualDevice = newVirtualDevice;
            driverOperations.TryDisconnectAll();
            string newRoute = "PowerUUT";
            driverOperations.SelectedRoute = newRoute;

            Assert.True(driverOperations.CanConnect());
            Assert.False(driverOperations.IsConnected());
            driverOperations.TryConnect(MulticonnectMode.Multiconnect);
            Assert.True(driverOperations.IsConnected());
            Assert.True(driverOperations.ConnectedRoutes.Any());

            Assert.True(driverOperations.CanDisconnect());
            driverOperations.TryDisconnectAll();
            Assert.False(driverOperations.IsConnected());
            Assert.False(driverOperations.ConnectedRoutes.Any());
        }

        [Fact]
        public void ConnectNoMulticonnectTwiceErrorsTest()
        {
            var driverOperations = new NISwitchExecutiveDriverOperations();
            driverOperations.SelectedVirtualDevice = "SwitchExecutiveExample";
            driverOperations.TryDisconnectAll();
            driverOperations.SelectedRoute = "PowerUUT";
            driverOperations.TryConnect(MulticonnectMode.NoMulticonnect);

            Assert.Throws<DriverException>(() => { driverOperations.TryConnect(MulticonnectMode.NoMulticonnect); });
        }

        // note:  this tests requires going to MAX and creating a new VirtualDevice named VirtualDevice1 with a route named "RouteGroup0"
        [Fact]
        public void ConfiguredVirtualDevice_SwitchToAnotherVirtualDevice_CanConnect()
        {
            var driverOperations = new NISwitchExecutiveDriverOperations();
            driverOperations.SelectedVirtualDevice = "SwitchExecutiveExample";
            driverOperations.TryDisconnectAll();
            driverOperations.SelectedRoute = "PowerUUT";
            driverOperations.TryConnect(MulticonnectMode.Multiconnect);
            driverOperations.TryDisconnectAll();

            driverOperations.SelectedVirtualDevice = "VirtualDevice1";
            driverOperations.SelectedRoute = "RouteGroup0";
            driverOperations.TryConnect(MulticonnectMode.Multiconnect);
            Assert.True(driverOperations.IsConnected());
        }
    }
}
