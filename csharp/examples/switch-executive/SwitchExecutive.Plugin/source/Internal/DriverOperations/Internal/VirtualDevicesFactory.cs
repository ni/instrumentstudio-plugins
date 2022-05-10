namespace SwitchExecutive.Plugin.Internal.DriverOperations.Internal
{
   internal class VirtualDevicesFactory
   {
      static internal IVirtualDevices CreateVirtualDevice(bool simulate = false)
      {
         if (simulate)
            return new FakeVirtualDevices();

         bool switchExecutiveInstalled = NISwitchExecutiveConfigurationUtilities.CheckIfSwitchExecutiveInstalled();
         if (!switchExecutiveInstalled)
            return new FakeVirtualDevices();

         return new VirtualDevices(new NISwitchExecutiveConfigurationManagement());
      }

      static public bool IsDriverInstalled()
      {
         return NISwitchExecutiveConfigurationUtilities.CheckIfSwitchExecutiveInstalled();
      }
   }
}
