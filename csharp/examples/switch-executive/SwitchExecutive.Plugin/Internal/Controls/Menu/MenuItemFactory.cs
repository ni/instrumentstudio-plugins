using System.Windows.Controls;
using System.Windows.Input;

namespace SwitchExecutive.Plugin.Internal.Controls.Menu
{
    /// <summary>
    /// Creates Menu Items such as Separators, Basic Menu Headers and Parametrized Menu Items for Data driven menus
    /// </summary>
    public static class MenuItemFactory
    {
        private static ICommand AlwaysSelectableNoop { get; } = new NationalInstruments.RelayCommand(MenuItemFactory.Noop, MenuItemFactory.AlwaysSelectable);

        /// <summary>
        /// Creates a separator for the menu.
        /// Generally used along with MenuItemBuilder.
        /// </summary>
        /// <param name="weight"> Higher the weight lower it appears. Think of the menu like water and heavier items sink to the bottom. </param>
        /// <returns> A separator.</returns>
        public static IMenuItem CreateSeparator(double weight)
        {
            return MenuItemFactory.CreateMenuItem(null, null, null, weight, MenuType.Separator, null);
        }

        /// <summary>
        /// Creates an always clickable do nothing menu item for the menu.
        /// Generally used to create a header for a tiered menu item.
        /// </summary>
        /// <param name="menuText"> Text to be displayed on the menu item </param>
        /// <param name="weight"> Higher the weight lower it appears. Think of the menu like water and heavier items sink to the bottom. </param>
        /// <returns> A menu item. </returns>
        public static IMenuItem CreateMenuItem(string menuText, double weight)
        {
            return MenuItemFactory.CreateMenuItem(MenuItemFactory.AlwaysSelectableNoop, null, menuText, weight, MenuType.MenuItem, null);
        }

        /// <summary>
        /// Creates an always clickable menu item for the menu.
        /// Probably the most used create call.
        /// </summary>
        /// <param name="menuCommand"> The ICommand for the menu item to bind to. </param>
        /// <param name="menuText"> Text to be displayed on the menu item. </param>
        /// <param name="weight"> Higher the weight lower it appears. Think of the menu like water and heavier items sink to the bottom. </param>
        /// <param name="commandParameter"> Parameter that will be passed to the ICommand's Execute </param>
        /// <returns> A menu item. </returns>
        public static IMenuItem CreateMenuItem(ICommand menuCommand, string menuText, double weight, object commandParameter)
        {
            return MenuItemFactory.CreateMenuItem(menuCommand, null, menuText, weight, MenuType.MenuItem, commandParameter);
        }

        /// <summary>
        /// Creates an always clickable do nothing menu item with an icon.
        /// </summary>
        /// <param name="menuIcon"> The icon for the menu item. </param>
        /// <param name="menuText"> Text to be displayed on the menu item. </param>
        /// <param name="weight"> Higher the weight lower it appears. Think of the menu like water and heavier items sink to the bottom. </param>
        /// <returns> A menu item. </returns>
        public static IMenuItem CreateMenuItem(Image menuIcon, string menuText, double weight)
        {
            return MenuItemFactory.CreateMenuItem(MenuItemFactory.AlwaysSelectableNoop, menuIcon, menuText, weight, MenuType.MenuItem, null);
        }

        /// <summary>
        /// Creates a menu item with the parameters listed below
        /// </summary>
        /// <param name="menuCommand"> The ICommand for the menu item to bind to. </param>
        /// <param name="menuIcon"> The icon for the menu item. </param>
        /// <param name="menuText"> Text to be displayed on the menu item. </param>
        /// <param name="weight"> Higher the weight lower it appears. Think of the menu like water and heavier items sink to the bottom. </param>
        /// <param name="commandParameter"> Parameter that will be passed to the ICommand's Execute </param>
        /// <returns></returns>
        public static IMenuItem CreateMenuItem(ICommand menuCommand, Image menuIcon, string menuText, double weight, object commandParameter)
        {
            return MenuItemFactory.CreateMenuItem(menuCommand, menuIcon, menuText, weight, MenuType.MenuItem, commandParameter);
        }

        private static IMenuItem CreateMenuItem(ICommand menuCommand, Image menuIcon, string menuText, double weight, MenuType menuType, object commandParameter)
        {
            return new Internal.MenuItem(menuCommand, menuIcon, menuText, weight, menuType, commandParameter);
        }

        private static bool AlwaysSelectable(object dummyObject)
        {
            return true;
        }

        private static void Noop(object dummyObject)
        {
            // NOOP
        }
    }
}
