using NationalInstruments.InstrumentFramework.Plugins;

namespace NationalInstruments.InstrumentStudio.HelloWorldPlugin
{
    [PanelPlugin(DisplayName, UniqueName, GroupName, PanelType, SupportedPresentations)]
    public class ExamplePanelPluginFactory : IPanelPluginFactory
    {
        public const string DisplayName = "InstrumentStudio .NET Example Plugin";
        public const string UniqueName = "NI | CSharpPlugin | IS_2022 | Example Plugin";
        public const string GroupName = "NI InstrumentStudio";
        public const string PanelType = "Example";
        public const PanelPresentation SupportedPresentations = PanelPresentation.ConfigurationWithVisualization | PanelPresentation.ConfigurationOnly;

        public PanelPlugin CreatePlugin(PluginSession pluginSession)
            => new HelloWorldPanelPlugin(pluginSession);
    }
}
