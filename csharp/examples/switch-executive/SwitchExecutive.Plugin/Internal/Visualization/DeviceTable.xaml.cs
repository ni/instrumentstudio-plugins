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
    /// Interaction logic for DeviceTable.xaml
    /// </summary>
    public partial class DeviceTable : UserControl
    {
        #region Fields

        public static readonly DependencyProperty DeviceInfoItemsSourceProperty = DependencyProperty.Register(
           "DeviceInfoItemsSource",
           typeof(IEnumerable<DeviceInfo>),
           typeof(DeviceTable),
           new PropertyMetadata(defaultValue: new List<DeviceInfo>()));

        #endregion

        #region Constructors

        public DeviceTable()
        {
            SetValue(DeviceInfoItemsSourceProperty, new List<DeviceInfo>());
            InitializeComponent();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The collection of <see cref="DeviceInfo"/> that populates the table.
        /// </summary>
        public IEnumerable<DeviceInfo> DeviceInfoItemsSource
        {
            get { return (IEnumerable<DeviceInfo>)GetValue(DeviceTable.DeviceInfoItemsSourceProperty); }
            set { SetValue(DeviceTable.DeviceInfoItemsSourceProperty, value); }
        }

        #endregion
    }
}
