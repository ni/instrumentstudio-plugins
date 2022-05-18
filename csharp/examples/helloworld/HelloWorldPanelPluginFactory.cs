using NationalInstruments.InstrumentFramework.Plugins;

namespace NationalInstruments.InstrumentStudio.HelloWorldPlugin
{
    [ExportPanelPlugin(DisplayName, UniqueName, GroupName, PanelType, SupportedPresentations)]
    public class ExamplePanelPluginFactory : IPanelPluginFactory
    {
        public const string DisplayName = "Hello World";
        public const string UniqueName = "NI | CSharpPlugin | IS_2022 | Hello World";
        public const string GroupName = "NI InstrumentStudio";
        public const string PanelType = "Example";
        public const PanelPresentation SupportedPresentations = PanelPresentation.ConfigurationWithVisualization | PanelPresentation.ConfigurationOnly;

        public PanelPlugin CreatePlugin(PluginSession pluginSession)
            => new HelloWorldPanelPlugin(pluginSession);
    }
}
