using System;
using System.Collections.Generic;
using NationalInstruments.Core;

namespace SwitchExecutive.Plugin.Internal.Controls.Menu
{
    public class ContextMenuService
    {
        public ContextMenuService(Func<CreateContextMenuRoutedEventArgs, IEnumerable<IMenuItem>> getContextMenu)
        {
            this.GetContextMenu = getContextMenu;
        }

        public Func<CreateContextMenuRoutedEventArgs, IEnumerable<IMenuItem>> GetContextMenu { get; }
    }
}
