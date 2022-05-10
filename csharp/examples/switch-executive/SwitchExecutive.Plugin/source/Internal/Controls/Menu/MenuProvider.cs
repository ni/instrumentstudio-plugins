using System.Collections.Generic;
using System.Linq;
using System.Windows;
using MenuItem = System.Windows.Controls.MenuItem;

namespace SwitchExecutive.Plugin.Internal.Controls.Menu
{
   /// <summary>
   /// Collects the dynamic menu items to be displayed from the IMenuDataProvider and creates a view for them.
   /// </summary>
   public sealed class MenuProvider : IMenuProvider
   {
      private readonly ISet<IDynamicMenuDataProvider> dynamicMenuDataProviders;
      private IList<FrameworkElement> dynamicMenuItemsRoot;
      private object commandParameter;

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
         this.dynamicMenuItemsRoot = new List<FrameworkElement>();
         this.dynamicMenuDataProviders = new HashSet<IDynamicMenuDataProvider>();
         this.commandParameter = commandParameter;
      }

      /// <summary>
      /// Returns uSFP and instrument specific dynamic menu items.
      /// </summary>
      /// <returns> Returns the list of framework elements for the dynamically </returns>
      public IEnumerable<FrameworkElement> GetDynamicMenuItems()
      {
         var dynamicMenuItems = new List<IMenuItem>();

         foreach (var dynamicMenuDataProvider in this.dynamicMenuDataProviders)
         {
            dynamicMenuItems.AddRange(dynamicMenuDataProvider.CollectDynamicMenuItems(this.commandParameter));
         }

         if (!dynamicMenuItems.Any())
         {
            return Enumerable.Empty<FrameworkElement>();
         }

         this.dynamicMenuItemsRoot = new List<FrameworkElement>();
         foreach (var dynamicMenuItem in dynamicMenuItems)
         {
            // Add the entire tree for each root menuItem
            this.AddMenuItemToMenuTree(dynamicMenuItem, null);
         }

         return this.dynamicMenuItemsRoot;
      }

      /// <summary>
      /// Adds to the menu providers set. This set is used to collect the menu items from all the dynamic menu item sources.
      /// Also see MenuProvider.GetDynamicMenuItems()
      /// </summary>
      /// <param name="dynamicMenuDataProvider"> Source of the menu items to be added </param>
      public void AddMenuDataProvider(IDynamicMenuDataProvider dynamicMenuDataProvider)
      {
         // Note we need not perform .contains check since this is a hashSet.
         this.dynamicMenuDataProviders.Add(dynamicMenuDataProvider);
      }

      /// <summary>
      /// Removes from the menu providers set. This set is used to collect the menu items from all the dynamic menu item sources.
      /// Also see MenuProvider.GetDynamicMenuItems()
      /// </summary>
      /// <param name="dynamicMenuDataProvider"> Source of the menu items to be removed </param>
      public void RemoveMenuDataProvider(IDynamicMenuDataProvider dynamicMenuDataProvider)
      {
         // Note we need not perform .contains check since this is a hashSet.
         this.dynamicMenuDataProviders.Remove(dynamicMenuDataProvider);
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
            this.dynamicMenuItemsRoot.Add(menuView);
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
            this.AddMenuItemToMenuTree(subMenuItem, menuView);
         }
      }
   }
}
