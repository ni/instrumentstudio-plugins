using System.Collections.Generic;

namespace SwitchExecutive.Plugin.Internal.Controls.Menu
{
   public interface IContextMenuProvider
   {
      /// <summary>
      /// Adding IContextMenuDataProvider to the registration list
      /// </summary>
      /// <param name="contextMenuDataProvider">Data provider of the context menu items</param>
      void AddContextMenuDataProvider(IContextMenuDataProvider contextMenuDataProvider);

      /// <summary>
      /// Gets the context menu items from the data providers
      /// </summary>
      IEnumerable<IMenuItem> GetContextMenuItems();

      /// <summary>
      /// Removing IContextMenuDataProvider to the registration list
      /// </summary>
      /// <param name="contextMenuDataProvider">Data provider of the context menu items</param>
      void RemoveContextMenuDataProvider(IContextMenuDataProvider contextMenuDataProvider);
   }
}
