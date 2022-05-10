using System.Collections.Generic;

namespace SwitchExecutive.Plugin.Internal.Controls.Menu
{
   public interface IContextMenuDataProvider
   {
      /// <summary>
      /// As a menu data provider, this method will get called by the ContextMenuProvider to allow
      /// you to add IMenuItems. Your class is registered with the ContextMenuProvider by calling its
      /// AddContextMenuDataProvider method. The owner of the ContextMenuProvider defines what commandParameter
      /// will be passed to this method and you can optionally use when you create MenuItems.
      /// </summary>
      /// <param name="commandParameter">
      /// Specified by the owner of the ContextMenuProvider and you can optionally use to create your MenuItems.
      /// </param>
      /// <returns></returns>
      IEnumerable<IMenuItem> CollectContextMenuItems(object commandParameter);
   }
}
