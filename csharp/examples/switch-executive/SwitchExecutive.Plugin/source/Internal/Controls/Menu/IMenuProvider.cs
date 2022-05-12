using System.Collections.Generic;
using System.Windows;

namespace SwitchExecutive.Plugin.Internal.Controls.Menu
{
    public interface IMenuProvider
    {
        IEnumerable<FrameworkElement> GetDynamicMenuItems();

        void AddMenuDataProvider(IDynamicMenuDataProvider dynamicMenuDataProvider);

        void RemoveMenuDataProvider(IDynamicMenuDataProvider dynamicMenuDataProvider);
    }
}
