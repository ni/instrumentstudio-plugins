using System;
using Microsoft.Win32;

namespace SwitchExecutive.Plugin.Internal.DriverOperations.Internal
{
    internal class NISwitchExecutiveConfigurationUtilities
    {
        static public bool CheckIfSwitchExecutiveInstalled()
        {
            bool isInstalled = false;
            const string SwitchExecutiveCurrentVersionRegistryKey = @"SOFTWARE\National Instruments\Switch Executive\CurrentVersion";
            const string VersionKey = "Version";

            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(SwitchExecutiveCurrentVersionRegistryKey))
                {
                    var versionString = key?.GetValue(VersionKey)?.ToString() ?? string.Empty;
                    isInstalled = versionString != string.Empty;
                    return isInstalled;
                }
            }
            catch (Exception)
            {
                return isInstalled;
            }
        }
    }
}
