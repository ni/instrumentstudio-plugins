using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SwitchExecutive.Plugin.Internal.DriverOperations;

namespace SwitchExecutive.Plugin.Internal
{
   /// <summary>
   /// Interaction logic for ConnectedRouteTable.xaml
   /// </summary>
   public partial class ConnectedRouteTable : UserControl
   {
      public ConnectedRouteTable()
      {
         this.InitializeComponent();
      }

      public static Visibility VisibilityForBooleanValue(bool? value)
      {
         return value == true ? Visibility.Visible : Visibility.Collapsed;
      }

      private void PopupClosed(object sender, EventArgs e)
      {
         Application.Current.MainWindow.PreviewKeyDown -= this.OnPreviewKeyDown;
      }

      private void PopupOpened(object sender, EventArgs e)
      {
         Application.Current.MainWindow.PreviewKeyDown += this.OnPreviewKeyDown;
      }

      private void OnPreviewKeyDown(object sender, KeyEventArgs e)
      {
         if (e.Key == Key.Escape)
         {
            //this.Popup.IsOpen = false;
         }
      }

   }
}
