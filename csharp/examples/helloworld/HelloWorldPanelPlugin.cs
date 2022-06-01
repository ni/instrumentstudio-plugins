using System.Windows;
using System.Windows.Controls;
using NationalInstruments.InstrumentFramework.Plugins;

namespace NationalInstruments.InstrumentStudio.HelloWorldPlugin
{
    internal class HelloWorldPanelPlugin : PanelPlugin
    {
        public HelloWorldPanelPlugin(PluginSession pluginSession)
            : base(pluginSession)
        {
        }

        public override FrameworkElement PanelContent => new TextBlock { Text = "Hello InstrumentStudio World!" };
    }
}
