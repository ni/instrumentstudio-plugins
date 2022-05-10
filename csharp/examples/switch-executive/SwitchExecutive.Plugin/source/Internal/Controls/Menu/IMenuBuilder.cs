using System;
using System.Collections.Generic;

namespace SwitchExecutive.Plugin.Internal.Controls.Menu
{
   public interface IMenuBuilder
   {
      IEnumerable<IMenuItem> MenuItems { get; }

      void AddMenu(IMenuItem menuItem);

      IDisposable AddMenuGroup(IMenuItem menuItem);
   }
}
