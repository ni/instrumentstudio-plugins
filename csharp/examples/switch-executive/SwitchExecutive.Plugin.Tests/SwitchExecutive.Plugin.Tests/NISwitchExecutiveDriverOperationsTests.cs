using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SwitchExecutive.Plugin.Internal.DriverOperations;

namespace SwitchExecutive.Plugin.Tests
{
   [TestClass]
   public class NISwitchExecutiveDriverOperationsTests
   {
      [TestMethod]
      public void OnContruction_NoThrow()
      {
         ISwitchExecutiveDriverOperations driverOperations = new NISwitchExecutiveDriverOperations();
         Assert.IsNotNull(driverOperations);
      }

      [TestMethod]
      public void IsDriverInstalled_ReturnsTrue()
      {
         // The rest of these tests will fail if SwitchExecutive isn't installed. Needs default SwitchExecutiveExample in MAX.
         Assert.IsTrue(NISwitchExecutiveDriverOperations.IsDriverInstalled());
      }

      [TestMethod]
      public void DefaultSelectedVirtualDevice_IsEmpty()
      {
         ISwitchExecutiveDriverOperations driverOperations = new NISwitchExecutiveDriverOperations();
         Assert.IsTrue(driverOperations.SelectedVirtualDevice == "");
      }

      [TestMethod]
      public void SetVirtualDevice_ReturnsNoError()
      {
         ISwitchExecutiveDriverOperations driverOperations = new NISwitchExecutiveDriverOperations();
         string newVirtualDevice = "VirtualDevice1";
         driverOperations.SelectedVirtualDevice = newVirtualDevice;
         Assert.IsTrue(driverOperations.SelectedVirtualDevice == newVirtualDevice);
      }

      [TestMethod]
      public void SetRoute_ReturnsNoError()
      {
         ISwitchExecutiveDriverOperations driverOperations = new NISwitchExecutiveDriverOperations();
         string newRoute = "PowerUUT";
         driverOperations.SelectedRoute = newRoute;
         Assert.IsTrue(driverOperations.SelectedRoute == newRoute);
      }

      [TestMethod]
      public void VirtualDeviceNames_ReturnsSwitchExecutiveExample()
      {
         ISwitchExecutiveDriverOperations driverOperations = new NISwitchExecutiveDriverOperations();
         IEnumerable<string> virtualDevices = driverOperations.VirtualDeviceNames;
         Assert.IsTrue(virtualDevices.FirstOrDefault(device => device == "SwitchExecutiveExample").Any());
      }

      [TestMethod]
      public void VirtualDeviceRoutes_ReturnsPowerUUT()
      {
         ISwitchExecutiveDriverOperations driverOperations = new NISwitchExecutiveDriverOperations();
         string newVirtualDevice = "SwitchExecutiveExample";
         driverOperations.SelectedVirtualDevice = newVirtualDevice;

         IEnumerable<string> routes = driverOperations.RouteNames;
         Assert.IsTrue(routes.FirstOrDefault(route => route == "PowerUUT").Any());
      }

      [TestMethod]
      public void ConnectDisconnectRouteTest()
      {
         ISwitchExecutiveDriverOperations driverOperations = new NISwitchExecutiveDriverOperations();
         string newVirtualDevice = "SwitchExecutiveExample";
         driverOperations.SelectedVirtualDevice = newVirtualDevice;
         driverOperations.TryDisconnectAll();
         string newRoute = "PowerUUT";
         driverOperations.SelectedRoute = newRoute;

         Assert.IsTrue(driverOperations.CanConnect());
         Assert.IsFalse(driverOperations.IsConnected());
         driverOperations.TryConnect(MulticonnectMode.Multiconnect);
         Assert.IsTrue(driverOperations.IsConnected());
         Assert.IsTrue(driverOperations.ConnectedRoutes.Any());

         Assert.IsTrue(driverOperations.CanDisconnect());
         driverOperations.TryDisconnect();
         Assert.IsFalse(driverOperations.IsConnected());
         Assert.IsFalse(driverOperations.ConnectedRoutes.Any());
      }

      [TestMethod]
      public void ConnectDisconnectAllRouteTest()
      {
         ISwitchExecutiveDriverOperations driverOperations = new NISwitchExecutiveDriverOperations();
         string newVirtualDevice = "SwitchExecutiveExample";
         driverOperations.SelectedVirtualDevice = newVirtualDevice;
         driverOperations.TryDisconnectAll();
         string newRoute = "PowerUUT";
         driverOperations.SelectedRoute = newRoute;

         Assert.IsTrue(driverOperations.CanConnect());
         Assert.IsFalse(driverOperations.IsConnected());
         driverOperations.TryConnect(MulticonnectMode.Multiconnect);
         Assert.IsTrue(driverOperations.IsConnected());
         Assert.IsTrue(driverOperations.ConnectedRoutes.Any());

         Assert.IsTrue(driverOperations.CanDisconnect());
         driverOperations.TryDisconnectAll();
         Assert.IsFalse(driverOperations.IsConnected());
         Assert.IsFalse(driverOperations.ConnectedRoutes.Any());
      }

      [TestMethod]
      public void ConnectNoMulticonnectTwiceErrorsTest()
      {
         ISwitchExecutiveDriverOperations driverOperations = new NISwitchExecutiveDriverOperations();
         driverOperations.SelectedVirtualDevice = "SwitchExecutiveExample";
         driverOperations.TryDisconnectAll();
         driverOperations.SelectedRoute = "PowerUUT";
         driverOperations.TryConnect(MulticonnectMode.NoMulticonnect);

         Assert.ThrowsException<DriverException>(() => { driverOperations.TryConnect(MulticonnectMode.NoMulticonnect); });         
      }

      // note:  this tests requires going to MAX and creating a new VirtualDevice named VirtualDevice1 with a route named "RouteGroup0"
      [TestMethod]
      public void ConfiguredVirtualDevice_SwitchToAnotherVirtualDevice_CanConnect()
      {
         ISwitchExecutiveDriverOperations driverOperations = new NISwitchExecutiveDriverOperations();         
         driverOperations.SelectedVirtualDevice = "SwitchExecutiveExample";
         driverOperations.TryDisconnectAll();
         driverOperations.SelectedRoute = "PowerUUT";
         driverOperations.TryConnect(MulticonnectMode.Multiconnect);
         driverOperations.TryDisconnectAll();

         driverOperations.SelectedVirtualDevice = "VirtualDevice1";
         driverOperations.SelectedRoute = "RouteGroup0";
         driverOperations.TryConnect(MulticonnectMode.Multiconnect);
         Assert.IsTrue(driverOperations.IsConnected());         
      }
   }
}
