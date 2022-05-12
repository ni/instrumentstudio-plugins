using NationalInstruments.InstrumentFramework.Plugins;

namespace SwitchExecutive.Plugin
{
    // This export attribute is what InstrumentStudio uses to discover your plugin. The first argument is the name
    // of your plugin that will show up in InstrumentStudio for your users to select. The second argument is a
    // unique identifier that InstrumentStudio uses internally. Use guidgen to generate a guid. The third argument 
    // is which panel presentations this plugin supports.
    [PanelPlugin("SwitchExecutive", "need guid", "Group Name", "Panel Type", PanelPresentation.ConfigurationWithVisualization | PanelPresentation.ConfigurationOnly)]
    public class SwtichExecutiveFactory : IPanelPluginFactory
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
