using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using SwitchExecutive.Plugin.Internal.DriverOperations;

namespace SwitchExecutive.Plugin.Internal
{
    /// <summary>
    /// Interaction logic for ChannelTable.xaml
    /// </summary>
    public partial class ChannelTable : UserControl
    {
        #region Fields

        public static readonly DependencyProperty InfoItemsSourceProperty = DependencyProperty.Register(
           "InfoItemsSource",
           typeof(IEnumerable<ChannelInfo>),
           typeof(ChannelTable),
           new PropertyMetadata(defaultValue: new List<ChannelInfo>()));

        #endregion

        #region Constructors

        public ChannelTable()
        {
            SetValue(InfoItemsSourceProperty, new List<ChannelInfo>());
            InitializeComponent();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The collection of <see cref="ChannelInfo"/> that populates the table.
        /// </summary>
        public IEnumerable<ChannelInfo> InfoItemsSource
        {
            get { return (IEnumerable<ChannelInfo>)GetValue(ChannelTable.InfoItemsSourceProperty); }
            set { SetValue(ChannelTable.InfoItemsSourceProperty, value); }
        }

        #endregion
    }
}
