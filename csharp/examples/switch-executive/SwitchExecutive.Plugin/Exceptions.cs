using System;
using System.Runtime.InteropServices;

namespace SwitchExecutive.Plugin
{
    public class DriverException : ExternalException
    {
        public DriverException()
        {
        }

        public DriverException(string message, int errorCode)
            : base(message, errorCode)
        {
        }

        public DriverException(string message)
            : base(message)
        {
        }

        public DriverException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
