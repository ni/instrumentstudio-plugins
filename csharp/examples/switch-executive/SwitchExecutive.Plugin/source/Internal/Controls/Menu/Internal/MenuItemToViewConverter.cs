using System;
using System.Windows;
using System.Windows.Controls;

namespace SwitchExecutive.Plugin.Internal.Controls.Menu.Internal
{
    /*public static class WeightExtension
    {
       public static readonly DependencyProperty WeightProperty =
           DependencyProperty.RegisterAttached(
              "Weight",
              typeof(double),
              typeof(WeightExtension),
              new PropertyMetadata(double.PositiveInfinity));

       public static double GetWeight(FrameworkElement frameworkElement)
       {
          return (double)frameworkElement.GetValue(WeightProperty);
       }

       public static void SetWeight(FrameworkElement frameworkElement, double value)
       {
          frameworkElement.SetValue(WeightProperty, value);
       }
    } */

    internal static class MenuItemToViewConverter
    {
        /// <summary>
        /// Creates the view for the given IMenuItem type object.
        /// </summary>
        /// <param name="menuItem"> IMenuItem type object for which the view needs to be created. </param>
        /// <returns> Either a Separator or a MenuItem depending on the MenuItem.Type. </returns>
        public static FrameworkElement CreateViewForMenuItem(IMenuItem menuItem)
        {
            switch (menuItem.Type)
            {
                case MenuType.MenuItem: return CreateMenuItemView(menuItem);
                case MenuType.Separator: return CreateViewForSeparator(menuItem);
                default: throw new NotSupportedException("Unknown MenuType");
            }
        }

        private static FrameworkElement CreateMenuItemView(IMenuItem menuItem)
        {
            var menuView = new System.Windows.Controls.MenuItem()
            {
                Command = menuItem.Command,
                CommandParameter = menuItem.CommandParameter,
                Icon = menuItem.Icon,

                // Note: Assigning Text to Header is not advised since WPF by default uses “Label” which does not display underscores correctly.
                // See : http://stackoverflow.com/questions/9684619/label-doesnt-display-character
                Header = new TextBlock() { Text = menuItem.Text },
            };

            WeightExtension.SetWeight(menuView, menuItem.Weight);
            return menuView;
        }

        private static FrameworkElement CreateViewForSeparator(IMenuItem menuItem)
        {
            var separator = new Separator();
            WeightExtension.SetWeight(separator, menuItem.Weight);
            return separator;
        }
    }
}
