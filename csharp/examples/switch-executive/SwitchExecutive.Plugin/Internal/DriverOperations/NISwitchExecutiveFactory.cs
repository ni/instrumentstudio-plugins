using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwitchExecutive.Plugin.Internal.DriverOperations.Internal;

namespace SwitchExecutive.Plugin.Internal.DriverOperations
{
    internal class NISwitchExecutiveFactory
    {
        public static NISwitchExecutiveInterface CreateNISwitchExecutive(string resourceName, bool simulate = false)
        {
            if (simulate)
                return new FakeNISwitchExecutive(resourceName);

            bool switchExecutiveInstalled = NISwitchExecutiveConfigurationUtilities.CheckIfSwitchExecutiveInstalled();
            if (!switchExecutiveInstalled)
                return new FakeNISwitchExecutive(resourceName);

            return NISwitchExecutive.TryCreateOwnedSession(resourceName);
        }

        public static bool IsDriverInstalled()
        {
            return NISwitchExecutiveConfigurationUtilities.CheckIfSwitchExecutiveInstalled();
        }
    }
}
