using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using SwitchExecutive.Plugin.Internal.DriverOperations;
using NationalInstruments.Controls;

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
            this.SetValue(InfoItemsSourceProperty, new List<RouteInfo>());
            this.InitializeComponent();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The collection of <see cref="RouteInfo"/> that populates the table.
        /// </summary>
        public IEnumerable<RouteInfo> InfoItemsSource
        {
            get { return (IEnumerable<RouteInfo>)this.GetValue(RouteTable.InfoItemsSourceProperty); }
            set { this.SetValue(RouteTable.InfoItemsSourceProperty, value); }
        }

        #endregion
    }
}
