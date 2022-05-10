namespace SwitchExecutive.Plugin.Internal.DriverOperations.Internal
{
   internal class FakeNISwitchExecutive : NISwitchExecutiveInterface
   {
      private string resourceName;

      public FakeNISwitchExecutive(string resourceName) { this.resourceName = resourceName; }

      public void Dispose() { }
      public string Name { get { return this.resourceName; } }
      public void Connect(string connectSpec, MulticonnectMode multiconnectMode, bool waitForDebounce) { }
      public void Disconnect(string spec) { }
      public void DisconnectAll() { }
      public void ConnectAndDisconnect(string connectSpec, string disconnectSpec, MulticonnectMode multiconnectMode, OperationOrder operationOrder, bool waitForDebounce) { }
      public bool IsConnected(string spec) { return false; }
      public string ExpandRouteSpec(string spec, ExpandOptions expandOptions) { return string.Empty; }
      public string GetAllConnections() { return string.Empty; }
   }
}
