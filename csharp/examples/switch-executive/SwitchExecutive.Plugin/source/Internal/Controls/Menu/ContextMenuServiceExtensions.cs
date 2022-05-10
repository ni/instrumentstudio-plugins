using System.Windows;

namespace SwitchExecutive.Plugin.Internal.Controls.Menu
{
   public static class ContextMenuServiceExtensions
   {
      public static readonly DependencyProperty ContextMenuServiceProperty = DependencyProperty.RegisterAttached(
         "ContextMenuService",
         typeof(ContextMenuService),
         typeof(ContextMenuServiceExtensions),
         new FrameworkPropertyMetadata(null));

      public static void SetContextMenuService(FrameworkElement element, ContextMenuService service)
      {
         element?.SetValue(ContextMenuServiceProperty, service);
      }

      public static ContextMenuService GetContextMenuService(FrameworkElement element)
      {
         return (ContextMenuService)element?.GetValue(ContextMenuServiceProperty);
      }
   }
}
