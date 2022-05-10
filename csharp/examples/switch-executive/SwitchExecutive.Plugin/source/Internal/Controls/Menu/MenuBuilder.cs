using System;
using System.Collections.Generic;
using System.Linq;

namespace SwitchExecutive.Plugin.Internal.Controls.Menu
{
   public sealed class BuilderScope : IDisposable
   {
      private readonly Action disposeAction;

      public BuilderScope(Action disposeAction)
      {
         this.disposeAction = disposeAction;
      }

      public void Dispose()
      {
         this.disposeAction();
      }
   }

   public sealed class MenuBuilder : IMenuBuilder
   {
      private readonly Stack<IMenuItem> rootMenuItems = new Stack<IMenuItem>();
      private readonly IList<IMenuItem> menuItems = new List<IMenuItem>();

      public IEnumerable<IMenuItem> MenuItems => this.menuItems;

      private IMenuItem Parent
      {
         get
         {
            if (!this.rootMenuItems.Any())
            {
               return null;
            }

            return this.rootMenuItems.Peek();
         }
      }

      public static IEnumerable<IMenuItem> MergeMenuLists(IEnumerable<IMenuItem> existingContextMenuItems, IEnumerable<IMenuItem> newContextMenuItems)
      {
         var itemsToAdd = new List<IMenuItem>();

         foreach (var newMenuItem in newContextMenuItems)
         {
            var matchedMenuItem = existingContextMenuItems.FirstOrDefault(menuItem => menuItem.Equals(newMenuItem));
            if (matchedMenuItem == null)
            {
               itemsToAdd.Add(newMenuItem);
            }
            else
            {
               MergeInPlace(matchedMenuItem, newMenuItem);
            }
         }

         return existingContextMenuItems.Concat(itemsToAdd);
      }

      public void AddMenu(IMenuItem menuItem)
      {
         var parent = this.Parent;
         if (parent == null)
         {
            this.menuItems.Add(menuItem);
            return;
         }

         parent.Add(menuItem);
      }

      public IDisposable AddMenuGroup(IMenuItem menuItem)
      {
         return this.PushMenu(menuItem);
      }

      private static void MergeInPlace(IMenuItem existingMenuItem, IMenuItem newMenuItem)
      {
         foreach (var newSubMenuItem in newMenuItem.SubItems)
         {
            var matchedSubMenuItem = existingMenuItem.SubItems?.FirstOrDefault(item => newSubMenuItem.Equals(item));
            if (matchedSubMenuItem != null)
            {
               MergeInPlace(matchedSubMenuItem, newSubMenuItem);
            }
            else
            {
               existingMenuItem.Add(newSubMenuItem);
            }
         }
      }

      private IDisposable PushMenu(IMenuItem menuItem)
      {
         this.rootMenuItems.Push(menuItem);
         return new BuilderScope(this.PopMenu);
      }

      private void PopMenu()
      {
         var rootMenuItem = this.rootMenuItems.Pop();
         this.AddMenu(rootMenuItem);
      }
   }
}
