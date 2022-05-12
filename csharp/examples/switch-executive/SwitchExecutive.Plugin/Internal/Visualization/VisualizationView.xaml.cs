using System.Windows.Controls;

namespace SwitchExecutive.Plugin.Internal
{
    internal partial class VisualizationView : UserControl
    {
        public VisualizationView(SwitchExecutiveControlViewModel instrumentViewModel, VisualizationViewModel visualizationViewModel)
        {
            InstrumentViewModel = instrumentViewModel;
            InitializeComponent();
            DataContext = visualizationViewModel;
        }

        public SwitchExecutiveControlViewModel InstrumentViewModel { get; private set; }
    }
}
