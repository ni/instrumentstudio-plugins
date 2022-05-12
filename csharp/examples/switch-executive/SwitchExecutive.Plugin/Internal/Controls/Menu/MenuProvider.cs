using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SwitchExecutive.Plugin.Internal.Controls.Menu
{
    /// <summary>
    /// Collects the dynamic menu items to be displayed from the IMenuDataProvider and creates a view for them.
    /// </summary>
    public sealed class MenuProvider : IMenuProvider
    {
        private readonly ISet<IDynamicMenuDataProvider> _dynamicMenuDataProviders;
        private IList<FrameworkElement> _dynamicMenuItemsRoot;
        private object _commandParameter;

        /// <summary>
        /// Collects the dynamic menu items to be displayed from the all the IDynamicMenuDataProvider(s) and creates a view for them.
        /// <param name="commandParameter">
        /// As creator/owner of the MenuProvider, you dictate what will get passed to IDynamicMenuDataProviders. It is up to those
        /// providers to use when it creates its MenuItems, or choose to provide its own CommandParameter that will then be
        /// passed to the MenuItem's ICommand.Execute.
        /// </param>
        /// </summary>
        public MenuProvider(object commandParameter)
        {
            _dynamicMenuItemsRoot = new List<FrameworkElement>();
            _dynamicMenuDataProviders = new HashSet<IDynamicMenuDataProvider>();
            _commandParameter = commandParameter;
        }

        /// <summary>
        /// Returns uSFP and instrument specific dynamic menu items.
        /// </summary>
        /// <returns> Returns the list of framework elements for the dynamically </returns>
        public IEnumerable<FrameworkElement> GetDynamicMenuItems()
        {
            var dynamicMenuItems = new List<IMenuItem>();

            foreach (var dynamicMenuDataProvider in _dynamicMenuDataProviders)
            {
                dynamicMenuItems.AddRange(dynamicMenuDataProvider.CollectDynamicMenuItems(_commandParameter));
            }

            if (!dynamicMenuItems.Any())
            {
                return Enumerable.Empty<FrameworkElement>();
            }

            _dynamicMenuItemsRoot = new List<FrameworkElement>();
            foreach (var dynamicMenuItem in dynamicMenuItems)
            {
                // Add the entire tree for each root menuItem
                AddMenuItemToMenuTree(dynamicMenuItem, null);
            }

            return _dynamicMenuItemsRoot;
        }

        /// <summary>
        /// Adds to the menu providers set. This set is used to collect the menu items from all the dynamic menu item sources.
        /// Also see MenuProvider.GetDynamicMenuItems()
        /// </summary>
        /// <param name="dynamicMenuDataProvider"> Source of the menu items to be added </param>
        public void AddMenuDataProvider(IDynamicMenuDataProvider dynamicMenuDataProvider)
        {
            // Note we need not perform .contains check since this is a hashSet.
            _dynamicMenuDataProviders.Add(dynamicMenuDataProvider);
        }

        /// <summary>
        /// Removes from the menu providers set. This set is used to collect the menu items from all the dynamic menu item sources.
        /// Also see MenuProvider.GetDynamicMenuItems()
        /// </summary>
        /// <param name="dynamicMenuDataProvider"> Source of the menu items to be removed </param>
        public void RemoveMenuDataProvider(IDynamicMenuDataProvider dynamicMenuDataProvider)
        {
            // Note we need not perform .contains check since this is a hashSet.
            _dynamicMenuDataProviders.Remove(dynamicMenuDataProvider);
        }

        /// <summary>
        /// Recursively called method which creates and populates the menuTree (viewTree) for a given menuItem
        /// </summary>
        /// <param name="menuItem"> MenuItem to add</param>
        /// <param name="parent"> Parent of the MenuItem to add</param>
        private void AddMenuItemToMenuTree(IMenuItem menuItem, UIElement parent)
        {
            var menuView = Internal.MenuItemToViewConverter.CreateViewForMenuItem(menuItem);

            if (parent == null)
            {
                _dynamicMenuItemsRoot.Add(menuView);
            }
            else
            {
                var menuItemParent = parent as MenuItem;
                menuItemParent?.Items.Add(menuView);
            }

            // If there are no SubMenus or the MenuType is not MenuItem do not add sub menus.
            if (!menuItem.SubItems.Any() || menuItem.Type != MenuType.MenuItem)
            {
                return;
            }

            foreach (var subMenuItem in menuItem.SubItems)
            {
                AddMenuItemToMenuTree(subMenuItem, menuView);
            }
        }
    }
}
