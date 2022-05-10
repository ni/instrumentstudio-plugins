using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;


namespace SwitchExecutive.Plugin.Internal.Controls.Menu.Internal
{
   internal sealed class MenuItem : IMenuItem
   {
      private readonly ISet<IMenuItem> subItems;

      internal MenuItem(ICommand command, Image icon, string text, double weight, MenuType type, object commandParameter)
      {
         this.Command = command ?? DisabledCommand;
         this.CommandParameter = commandParameter;
         this.subItems = new SortedSet<IMenuItem>();
         this.Text = text;
         this.Icon = icon;
         this.Weight = weight;
         this.Type = type;
      }

      public ICommand Command { get; }

      public object CommandParameter { get; }

      public IEnumerable<IMenuItem> SubItems => this.subItems;

      public Image Icon { get; }

      public MenuType Type { get; }

      public double Weight { get; }

      public string Text { get; }

      private static ICommand DisabledCommand { get; } = new NationalInstruments.RelayCommand(null, (o) => false);

      public void Add(IMenuItem subMenuItem)
      {
         if (subMenuItem != this && subMenuItem != null)
         {
            this.subItems.Add(subMenuItem);
         }
      }

      public void Remove(IMenuItem subMenuItem)
      {
         this.subItems.Remove(subMenuItem);
      }

      public int CompareTo(IMenuItem other)
      {
         if (this.Weight >= other.Weight)
         {
            return 1;
         }

         return -1;
      }

      public bool Equals(IMenuItem menuItem)
      {
         if (menuItem == null)
         {
            return false;
         }

         return this.Text?.Equals(menuItem.Text) == true && this.Weight == menuItem.Weight;
      }

      public override int GetHashCode()
      {
         return this.Text.GetHashCode() ^ this.Weight.GetHashCode();
      }
   }
}
