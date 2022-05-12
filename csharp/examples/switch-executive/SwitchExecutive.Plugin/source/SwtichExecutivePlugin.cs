using NationalInstruments.InstrumentFramework.Plugins;
using System.Windows;
using SwitchExecutive.Plugin.Internal;

namespace SwitchExecutive.Plugin
{
    /// <summary>
    /// The switch executive plugin to InstrumentStudio.
    /// </summary>
    public class SwitchExecutivePlugin : PanelPlugin
    {
        private readonly SwitchExecutiveControl _switchExecutiveControl;

        public SwitchExecutivePlugin(PluginSession pluginSession)
            : base(pluginSession)
        {
            _switchExecutiveControl = new SwitchExecutiveControl(pluginSession);
        }

        public override FrameworkElement PanelContent => _switchExecutiveControl;
    }
}
