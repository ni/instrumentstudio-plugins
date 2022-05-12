using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;

namespace SwitchExecutive.Plugin.Internal.Controls.Menu
{
    public interface IMenuItem : IComparable<IMenuItem>, IEquatable<IMenuItem>
    {
        IEnumerable<IMenuItem> SubItems { get; }

        ICommand Command { get; }

        object CommandParameter { get; }

        MenuType Type { get; }

        Image Icon { get; }

        double Weight { get; }

        string Text { get; }

        void Add(IMenuItem menuItem);

        void Remove(IMenuItem menuItem);
    }
}
