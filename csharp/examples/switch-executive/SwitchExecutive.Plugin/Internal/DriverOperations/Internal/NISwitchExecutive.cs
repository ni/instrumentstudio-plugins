using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;


namespace SwitchExecutive.Plugin.Internal.DriverOperations.Internal
{
    internal class NISwitchExecutiveHandle : SafeHandle
    {
        internal NISwitchExecutiveHandle()
           : base(IntPtr.Zero, true)
        {
        }

        internal NISwitchExecutiveHandle(bool ownsHandle)
           : base(IntPtr.Zero, ownsHandle)
        {
        }

        public static NISwitchExecutiveHandle CreateFromCHandle(IntPtr handle, bool ownsHandle)
        {
            NISwitchExecutiveHandle switchHandle = new NISwitchExecutiveHandle(ownsHandle);
            switchHandle.SetHandle(handle);
            return switchHandle;
        }

        public IntPtr Handle => base.handle;

        public override bool IsInvalid
        {
            [SecurityCritical]
            get
            {
                return handle == IntPtr.Zero;
            }
        }

        private const string NativeDllName64 = "nise.dll";

        [SuppressUnmanagedCodeSecurity]
        [DllImport(NativeDllName64, EntryPoint = "niSE_CloseSession", CallingConvention = CallingConvention.StdCall)]
        private static extern int Close(IntPtr instrumentHandle);

        [SecurityCritical]
        protected override bool ReleaseHandle()
        {
            return !NISwitchExecutive.HasErrorClearIfDoes(new NISwitchExecutiveHandle(), Close(base.handle));
        }

        public void SetDCPowerHandle(IntPtr handle)
        {
            base.SetHandle(handle);
        }
    }

    internal class NISwitchExecutive : NISwitchExecutiveInterface
    {
        private readonly NISwitchExecutiveHandle _sessionHandle;
        private readonly string _resourceName;
        private const string NativeDllName64 = "nise.dll";

        [DllImport(NativeDllName64, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern int niSE_GetError(NISwitchExecutiveHandle vi, out int errorCode, StringBuilder errorDescription, out int errorDescriptionBufferSize);

        [DllImport(NativeDllName64, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern int niSE_OpenSession(string resourceName, string options, out NISwitchExecutiveHandle instrumentHandle);

        private static bool HasError(int status)
        {
            return status < 0;
        }

        internal static bool HasErrorClearIfDoes(NISwitchExecutiveHandle handle, int status)
        {
            bool hasError = HasError(status);
            if (hasError)
            {
                ClearError(handle);
            }
            return hasError;
        }

        private int TestForError(int status)
        {
            if (HasError(status))
            {
                ThrowError(_sessionHandle, status);
            }

            return status;
        }

        private static string GetError(NISwitchExecutiveHandle handle, out int errorCode)
        {
            int size = 0;
            niSE_GetError(handle, out errorCode, null, out size);
            var msg = new StringBuilder();
            if (size > 0)
            {
                msg.Capacity = size;
                niSE_GetError(handle, out errorCode, msg, out size);
            }
            return msg.ToString();
        }

        internal static void ClearError(NISwitchExecutiveHandle handle)
        {
            int notUsed;
            GetError(handle, out notUsed);
        }

        private static void ThrowError(NISwitchExecutiveHandle handle, int code, string errorMessagePrefix = null)
        {
            int codeFromGetError;
            string errorString = GetError(handle, out codeFromGetError);
            throw new DriverException(errorString, code);
        }

        public static NISwitchExecutive TryCreateOwnedSession(string resourceName)
        {
            NISwitchExecutiveHandle sessionHandle;
            int result;
            try
            {
                result = niSE_OpenSession(resourceName, string.Empty, out sessionHandle);
            }
            catch (DllNotFoundException)
            {
                // Driver not installed, convert to DriverException.
                throw new DriverException("The NI-DCPower driver is not installed.");
            }

            if (HasError(result))
            {
                if (result == IVIConstants.VI_ERROR_RSRC_NFOUND)
                {
                    ClearError(new NISwitchExecutiveHandle());
                    // Return null to support the partial session use case if there is no such device on this system.
                    return null;
                }

                ThrowError(new NISwitchExecutiveHandle(), result, "Connecting to NI Switch Executive hardware failed with the following error:\n\n");
            }

            return new NISwitchExecutive(sessionHandle, resourceName, string.Empty);
        }

        private NISwitchExecutive(NISwitchExecutiveHandle sessionHandle, string resourceName, string options)
        {
            _sessionHandle = sessionHandle;
            _resourceName = resourceName;
        }

        public void Dispose()
        {
            _sessionHandle.Dispose();
        }

        [DllImport(NativeDllName64, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern int niSE_Connect(NISwitchExecutiveHandle vi, string connectSpec, long multiconnectMode, ushort waitForDebounce);
        public void Connect(string connectSpec, MulticonnectMode multiconnectMode, bool waitForDebounce)
        {
            ushort localWaitForDebounce = waitForDebounce ? IVIConstants.VI_TRUE : IVIConstants.VI_FALSE;

            TestForError(
               niSE_Connect(
                  _sessionHandle,
                  connectSpec,
                  (int)multiconnectMode,
                  localWaitForDebounce));
        }

        [DllImport(NativeDllName64, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern int niSE_Disconnect(NISwitchExecutiveHandle vi, string spec);
        public void Disconnect(string spec)
        {
            TestForError(
               niSE_Disconnect(
                  _sessionHandle,
                  spec));
        }

        [DllImport(NativeDllName64, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern int niSE_DisconnectAll(NISwitchExecutiveHandle vi);
        public void DisconnectAll()
        {
            TestForError(
               niSE_DisconnectAll(
                  _sessionHandle));
        }

        [DllImport(NativeDllName64, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern int niSE_ConnectAndDisconnect(NISwitchExecutiveHandle vi, string connectSpec, string disconnectSpec, long multiconnectMode, long operationOrder, ushort waitForDebounce);
        public void ConnectAndDisconnect(string connectSpec, string disconnectSpec, MulticonnectMode multiconnectMode, OperationOrder operationOrder, bool waitForDebounce)
        {
            ushort localWaitForDebounce = waitForDebounce ? IVIConstants.VI_TRUE : IVIConstants.VI_FALSE;

            TestForError(
               niSE_ConnectAndDisconnect(
                  _sessionHandle,
                  connectSpec,
                  disconnectSpec,
                  (int)multiconnectMode,
                  (int)operationOrder,
                  localWaitForDebounce));
        }

        [DllImport(NativeDllName64, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern int niSE_IsConnected(NISwitchExecutiveHandle vi, string spec, out ushort isConnected);
        public bool IsConnected(string spec)
        {
            ushort localIsConnected = IVIConstants.VI_FALSE;

            TestForError(
               niSE_IsConnected(
                  _sessionHandle,
                  spec,
                  out localIsConnected));

            return localIsConnected == IVIConstants.VI_TRUE;
        }

        [DllImport(NativeDllName64, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern int niSE_ExpandRouteSpec(NISwitchExecutiveHandle vi, string spec, ExpandOptions expandOptions, StringBuilder expandedRouteSpec, out int expandedRouteSpecSize);
        public string ExpandRouteSpec(string spec, ExpandOptions expandOptions)
        {
            int expandedRouteSpecSize = 0;

            TestForError(
               niSE_ExpandRouteSpec(
                  _sessionHandle,
                  spec,
                  expandOptions,
                  null,
                  out expandedRouteSpecSize));
            var expandedRouteSpec = new StringBuilder();
            if (expandedRouteSpecSize > 0)
            {
                expandedRouteSpec.Capacity = expandedRouteSpecSize;
                TestForError(
                   niSE_ExpandRouteSpec(
                      _sessionHandle,
                      spec,
                      expandOptions,
                      expandedRouteSpec,
                      out expandedRouteSpecSize));
            }

            return expandedRouteSpec.ToString();
        }

        [DllImport(NativeDllName64, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern int niSE_GetAllConnections(NISwitchExecutiveHandle vi, StringBuilder routeSpec, out int routeSpecSize);
        public string GetAllConnections()
        {
            int routeSpecSize = 0;

            TestForError(
               niSE_GetAllConnections(
                  _sessionHandle,
                  null,
                  out routeSpecSize));
            var routeSpec = new StringBuilder();
            if (routeSpecSize > 0)
            {
                routeSpec.Capacity = routeSpecSize;
                TestForError(
                   niSE_GetAllConnections(
                      _sessionHandle,
                      routeSpec,
                      out routeSpecSize));
            }

            return routeSpec.ToString();
        }

        public string Name => _resourceName;
    }
}
