using System;

namespace SwitchExecutive.Plugin.Internal.DriverOperations.Internal
{
    internal static class IVIConstants
    {
        public const uint IVI_ATTR_BASE = 1000000;
        public const uint IVI_CLASS_PUBLIC_ATTR_BASE = (IVI_ATTR_BASE + 250000); /* base for public attributes of class drivers */
        public const uint IVI_SPECIFIC_PUBLIC_ATTR_BASE = (IVI_ATTR_BASE + 150000);
        public const uint IVI_SPECIFIC_PRIVATE_ATTR_BASE = (IVI_ATTR_BASE + 200000);
        public const uint IVI_ENGINE_PUBLIC_ATTR_BASE = (IVI_ATTR_BASE + 50000);

        public const uint IVI_ATTR_SIMULATE /* ViBoolean, VI_FALSE */ = (IVI_ENGINE_PUBLIC_ATTR_BASE + 5);
        public const uint IVI_ATTR_INSTRUMENT_MODEL /* ViString, not user-writable*/ = (IVI_ENGINE_PUBLIC_ATTR_BASE + 512);

        public const uint IVIDCPWR_ATTR_VOLTAGE_LEVEL = (IVI_CLASS_PUBLIC_ATTR_BASE + 1); /* ViReal64,  multi-channel */
        public const uint IVIDCPWR_ATTR_CURRENT_LIMIT = (IVI_CLASS_PUBLIC_ATTR_BASE + 5); /* ViReal64,  multi-channel */
        public const uint IVIDCPWR_ATTR_OUTPUT_ENABLED = (IVI_CLASS_PUBLIC_ATTR_BASE + 6); /* ViBoolean, multi-channel */
        public const uint IVI_ATTR_SPECIFIC_DRIVER_REVISION = (IVI_ENGINE_PUBLIC_ATTR_BASE + 551); /* ViString */

        public const int IVIDCPWR_VAL_MEASURE_CURRENT = (0);
        public const int IVIDCPWR_VAL_MEASURE_VOLTAGE = (1);

        // ERROR CODES: Add any additional IVI error codes needed here to keep them all together.
        public const int IVI_ERROR_INVALID_SESSION_HANDLE = -1074130544;
        public const int VI_ERROR_RSRC_NFOUND = -1073807343;

        // From //InstrumentDriver/IviComponents/IviHeaders/export/16.0/16.0.0f0/includes/ivi.h.
        private const int IVI_MAX_MESSAGE_LEN = 255;
        public const int IVI_MAX_MESSAGE_BUF_SIZE = (IVI_MAX_MESSAGE_LEN + 1);

        // From C headers (visatype.h and ivi.h).
        private const int IVI_STATUS_CODE_BASE = 0x3FFA0000;
        private const int _VI_ERROR = (-2147483647 - 1); /* 0x80000000 */
        private const int IVI_ERROR_BASE = (_VI_ERROR + IVI_STATUS_CODE_BASE);
        public const int IVI_ERROR_INVALID_VALUE = (IVI_ERROR_BASE + 0x10);

        public const ushort VI_FALSE = 0;
        public const ushort VI_TRUE = 1;
    }
}
