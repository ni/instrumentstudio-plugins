using System.Diagnostics.CodeAnalysis;
using System.Windows.Controls;

namespace SwitchExecutive.Plugin.Internal
{
    /// <summary>
    /// Interaction logic for HeaderView.xaml
    /// </summary>
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Instantiated by XAML")]
    internal partial class HeaderView : UserControl
    {
        public HeaderView()
        {
            InitializeComponent();
        }
    }
}
