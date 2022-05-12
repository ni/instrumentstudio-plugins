using System.Windows.Controls;

namespace SwitchExecutive.Plugin.Internal
{
    /// <summary>
    /// Interaction logic for HeaderMenuView.xaml
    /// </summary>
    internal partial class HeaderMenuView : UserControl
    {
        public HeaderMenuView(HeaderMenuViewModel viewModel)
        {
            this.InitializeComponent();
            this.DataContext = viewModel;
        }
    }
}
