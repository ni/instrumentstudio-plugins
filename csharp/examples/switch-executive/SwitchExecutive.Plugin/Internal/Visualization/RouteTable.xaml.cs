using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using SwitchExecutive.Plugin.Internal.DriverOperations;

namespace SwitchExecutive.Plugin.Internal
{
    /// <summary>
    /// Interaction logic for RouteTable.xaml
    /// </summary>
    public partial class RouteTable : UserControl
    {
        #region Fields

        public static readonly DependencyProperty InfoItemsSourceProperty = DependencyProperty.Register(
           "InfoItemsSource",
           typeof(IEnumerable<RouteInfo>),
           typeof(RouteTable),
           new PropertyMetadata(defaultValue: new List<RouteInfo>()));

        #endregion

        #region Constructors

        public RouteTable()
        {
            SetValue(InfoItemsSourceProperty, new List<RouteInfo>());
            InitializeComponent();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The collection of <see cref="RouteInfo"/> that populates the table.
        /// </summary>
        public IEnumerable<RouteInfo> InfoItemsSource
        {
            get { return (IEnumerable<RouteInfo>)GetValue(RouteTable.InfoItemsSourceProperty); }
            set { SetValue(RouteTable.InfoItemsSourceProperty, value); }
        }

        #endregion
    }
}
