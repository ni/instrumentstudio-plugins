using NationalInstruments.InstrumentFramework.Plugins;
using SwitchExecutive.Plugin.Internal;
using System;

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
      /// This method is called by InstrumentStudio when your plugin is placed within a document. It should return the visual (FrameworkElement) that
      /// you want displayed.
      /// </summary>
      /// <param name="editTimeConfiguration">The previously saved edit-time configuration (empty string if this is the first instantiation)</param>
      /// <param name="runTimeConfiguration">The previously saved run-time configuration (empty string if this is the first instantiation)</param>
      /// <param name="updateConfigurationDelegate">Use this delegate whenever you want to notify InstrumentStudio that your plugin's configuration has changed.
      /// Strings that are passed will be persisted with the .sfp. Upon reopening the .sfp, InstrumentStudio will pass these strings as the
      /// "editTimeConfiguration" and "runTimeConfiguration" inputs so that you can restore the state of your UI.</param>
      /// <param name="requestedPresentation">Render your UI accordingly, based on whether your plugin is placed in a small panel (ConfigurationOnly) or
      /// large panel (ConfigurationWithVisualization).</param>
      /// <returns>The IInstrumentStudioPlugin to be hosted.</returns>
      public IPanelPlugin CreatePlugin(string editTimeConfiguration, string runTimeConfiguration, UpdateConfigurationDelegate updateConfigurationDelegate, PanelPresentation requestedPresentation)
      {
         return new SwitchExecutiveControl(editTimeConfiguration, runTimeConfiguration, updateConfigurationDelegate, requestedPresentation);
      }
   }
}
