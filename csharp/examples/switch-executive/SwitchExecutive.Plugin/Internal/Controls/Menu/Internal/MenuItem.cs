using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;


namespace SwitchExecutive.Plugin.Internal.Controls.Menu.Internal
{
    internal sealed class MenuItem : IMenuItem
    {
        private readonly ISet<IMenuItem> _subItems;

        internal MenuItem(ICommand command, Image icon, string text, double weight, MenuType type, object commandParameter)
        {
            Command = command ?? DisabledCommand;
            CommandParameter = commandParameter;
            _subItems = new SortedSet<IMenuItem>();
            Text = text;
            Icon = icon;
            Weight = weight;
            Type = type;
        }

        public ICommand Command { get; }

        public object CommandParameter { get; }

        public IEnumerable<IMenuItem> SubItems => _subItems;

        public Image Icon { get; }

        public MenuType Type { get; }

        public double Weight { get; }

        public string Text { get; }

        private static ICommand DisabledCommand { get; } = new NationalInstruments.RelayCommand(null, (o) => false);

        public void Add(IMenuItem subMenuItem)
        {
            if (subMenuItem != this && subMenuItem != null)
            {
                _subItems.Add(subMenuItem);
            }
        }

        public void Remove(IMenuItem subMenuItem)
        {
            _subItems.Remove(subMenuItem);
        }

        public int CompareTo(IMenuItem other)
        {
            if (Weight >= other.Weight)
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

            return Text?.Equals(menuItem.Text) == true && Weight == menuItem.Weight;
        }

        public override int GetHashCode()
        {
            return Text.GetHashCode() ^ Weight.GetHashCode();
        }
    }
}
