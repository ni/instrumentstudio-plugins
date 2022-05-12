using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using SwitchExecutive.Plugin.Internal.Controls.Menu;

namespace SwitchExecutive.Plugin.Internal.Controls
{
    public static class WeightExtension
    {
        public static readonly DependencyProperty WeightProperty =
            DependencyProperty.RegisterAttached(
               "Weight",
               typeof(double),
               typeof(WeightExtension),
               new PropertyMetadata(double.PositiveInfinity));

        public static double GetWeight(FrameworkElement frameworkElement)
        {
            return (double)frameworkElement.GetValue(WeightProperty);
        }

        public static void SetWeight(FrameworkElement frameworkElement, double value)
        {
            frameworkElement.SetValue(WeightProperty, value);
        }
    }

    /// <summary>
    /// Interaction logic for MenuButton.xaml
    /// </summary>
    [ContentProperty("ButtonContent")]
    public partial class MenuButton : UserControl
    {
        #region Fields

        public static readonly DependencyProperty MenuItemsProperty = DependencyProperty.Register(
           "MenuItems",
           typeof(ObservableCollection<FrameworkElement>),
           typeof(MenuButton),
           new PropertyMetadata(new ObservableCollection<FrameworkElement>()));

        public static readonly DependencyProperty ButtonContentProperty = DependencyProperty.Register(
           "ButtonContent",
           typeof(object),
           typeof(MenuButton),
           new UIPropertyMetadata(null));

        public static readonly DependencyProperty MenuProviderProperty = DependencyProperty.Register(
           "MenuProvider",
           typeof(IMenuProvider),
           typeof(MenuButton),
           new UIPropertyMetadata(null));

        private bool shouldOpenMenu = true;
        private bool didCacheStaticMenuItems = false;
        private IEnumerable<FrameworkElement> staticMenuItems;

        #endregion

        #region Constructor

        public MenuButton()
        {
            this.SetValue(MenuItemsProperty, new ObservableCollection<FrameworkElement>());
            this.InitializeComponent();
        }

        #endregion

        #region Properties

        public ObservableCollection<FrameworkElement> MenuItems
        {
            get { return (ObservableCollection<FrameworkElement>)this.GetValue(MenuButton.MenuItemsProperty); }
            set { this.SetValue(MenuButton.MenuItemsProperty, value); }
        }

        public object ButtonContent
        {
            get { return this.GetValue(MenuButton.ButtonContentProperty); }
            set { this.SetValue(MenuButton.ButtonContentProperty, value); }
        }

        public MenuProvider MenuProvider
        {
            get { return (MenuProvider)this.GetValue(MenuButton.MenuProviderProperty); }
            set { this.SetValue(MenuButton.MenuProviderProperty, value); }
        }

        #endregion

        #region Methods

        private void OnContextMenuClosed(object sender, RoutedEventArgs e)
        {
            var menu = (ContextMenu)sender;
            var menuToggleButton = (ToggleButton)menu.PlacementTarget;

            if (!this.IsMouseOver || this.IsKeyboardFocusWithin)
            {
                menuToggleButton.IsChecked = false;
            }

            // If the user is clicking the button when the drop-down menu closes, don't re-open it in OnMenuToggleButtonClick
            if (this.IsMouseOver && menuToggleButton.IsMouseCaptured)
            {
                this.shouldOpenMenu = false;
            }
        }

        private void OnMenuToggleButtonClick(object sender, RoutedEventArgs e)
        {
            var button = (ToggleButton)sender;

            // If the drop-down menu was just closed, uncheck the button and don't reopen the drop-down.
            if (!this.shouldOpenMenu)
            {
                button.IsChecked = false;
                this.shouldOpenMenu = true;
            }

            if (button.IsChecked == true)
            {
                button.ContextMenu.PlacementTarget = button;
                button.ContextMenu.Placement = PlacementMode.Bottom;
                button.ContextMenu.HorizontalOffset = -1;
                button.ContextMenu.IsOpen = true;
            }
        }

        private void OnContextMenuOpened(object sender, RoutedEventArgs e)
        {
            if (this.MenuProvider == null)
            {
                return;
            }

            if (!this.didCacheStaticMenuItems)
            {
                this.staticMenuItems = this.MenuItems;
                this.didCacheStaticMenuItems = true;
            }

            var dynamicMenuItems = this.MenuProvider.GetDynamicMenuItems();

            // We need to merge the dynamic and static menu items and sort them based on their weights.
            // Note 1: In the rare case when the static and dynamic menu items all have the same weight the below union decides their order.
            // Order in this case is, static followed by dynamic menu items.
            // Note 2: Only the first level of menu items are sorted.
            // Dynamically added menu items are sorted completely, since they are sorted during construction.
            // Note 3: Using OrderBy since List.Sort() is not a stable sort
            // see http://stackoverflow.com/questions/15883112/what-should-icomparer-return-to-indicate-keep-the-existing-sort-order
            // TODO: need to find a way to sort the itemsCollection of each subMenu (for completely sorting even the static elements)
            var compositeMenuItems = this.staticMenuItems
               .Union(dynamicMenuItems)
               .OrderBy(WeightExtension.GetWeight);
            this.MenuItems = new ObservableCollection<FrameworkElement>(compositeMenuItems);
        }

        #endregion
    }
}
