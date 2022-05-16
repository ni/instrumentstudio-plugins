using NationalInstruments.InstrumentFramework.Plugins;

namespace SwitchExecutive.Plugin
{
    // This export attribute is what InstrumentStudio uses to discover your plugin.
    [ExportPanelPlugin("SwitchExecutive", "NI | CSharpPlugin | IS_2022 | SwitchExecutive", "NI InstrumentStudio", "SwitchExecutive", PanelPresentation.ConfigurationWithVisualization | PanelPresentation.ConfigurationOnly)]
    public class SwitchExecutiveFactory : IPanelPluginFactory
    {
        /// <summary>
        /// This method is called by InstrumentStudio when your plugin is placed within a document.
        /// </summary>
        /// <param name="pluginSession">The interface to the InstrumentStudio plugin framework. This is used to get and receive data from InstrumentStudio.</param>
        /// <returns>The PanelPlugin to be hosted.</returns>
        public PanelPlugin CreatePlugin(PluginSession pluginSession)
        {
            return new SwitchExecutivePlugin(pluginSession);
        }
    }
}
