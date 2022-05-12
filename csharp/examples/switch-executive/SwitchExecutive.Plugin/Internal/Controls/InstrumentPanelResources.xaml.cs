using System.Windows;

namespace SwitchExecutive.Plugin.Internal.Controls
{
    public partial class InstrumentPanelResources : ResourceDictionary
    {
        /// <summary>
        /// Constructs a new instance of <see cref="InstrumentPanelResources"/>.
        /// This public constructor should only be called when this resource dictionary is instantiated from other xaml files.
        /// If you need this resource dictionary from C# code, use the single Instance.
        /// </summary>
        public InstrumentPanelResources()
        {
            InitializeComponent();
        }

        public static InstrumentPanelResources Instance { get; } = new InstrumentPanelResources();
    }
}
