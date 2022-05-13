using System;
using System.Collections.Generic;
using System.Linq;

namespace SwitchExecutive.Plugin.Internal.Controls.Menu
{
    public sealed class BuilderScope : IDisposable
    {
        private readonly Action _disposeAction;

        public BuilderScope(Action disposeAction)
        {
            _disposeAction = disposeAction;
        }

        public void Dispose()
        {
            _disposeAction();
        }
    }

    public sealed class MenuBuilder : IMenuBuilder
    {
        private readonly Stack<IMenuItem> _rootMenuItems = new Stack<IMenuItem>();
        private readonly IList<IMenuItem> _menuItems = new List<IMenuItem>();

        public IEnumerable<IMenuItem> MenuItems => _menuItems;

        private IMenuItem Parent
        {
            get
            {
                if (!_rootMenuItems.Any())
                {
                    return null;
                }

                return _rootMenuItems.Peek();
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
            var parent = Parent;
            if (parent == null)
            {
                _menuItems.Add(menuItem);
                return;
            }

            parent.Add(menuItem);
        }

        public IDisposable AddMenuGroup(IMenuItem menuItem)
        {
            return PushMenu(menuItem);
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
            _rootMenuItems.Push(menuItem);
            return new BuilderScope(PopMenu);
        }

        private void PopMenu()
        {
            var rootMenuItem = _rootMenuItems.Pop();
            AddMenu(rootMenuItem);
        }
    }
}
