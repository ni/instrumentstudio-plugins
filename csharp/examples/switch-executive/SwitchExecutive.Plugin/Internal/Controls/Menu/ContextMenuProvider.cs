using System.Collections.Generic;
using System.Linq;
using NationalInstruments.Core;

namespace SwitchExecutive.Plugin.Internal.Controls.Menu
{
    public class ContextMenuProvider : IContextMenuProvider
    {
        private readonly ISet<IContextMenuDataProvider> contextMenuDataProviders;
        private readonly object commandParameter;

        public ContextMenuProvider(object commandParameter)
        {
            this.contextMenuDataProviders = new HashSet<IContextMenuDataProvider>();
            this.commandParameter = commandParameter;
        }

        /// <summary>
        /// Utility method to convert an enumerable of IMenuItems to an enumerable of ICommandEx.
        /// </summary>
        /// <param name="menuItems">A list of IMenuItems to convert.</param>
        public static IEnumerable<ICommandEx> ConvertMenuItemsToCommands(IEnumerable<IMenuItem> menuItems)
        {
            var commands = new List<ICommandEx>();
            ICommandEx previousCommand = null;
            foreach (var menuItem in menuItems)
            {
                if (menuItem.Type == MenuType.Separator)
                {
                    commands.Add(ConvertSeparatorMenuItemToCommandEx(menuItem, previousCommand));
                    continue;
                }

                previousCommand = ConvertMenuItemToCommandEx(menuItem);
                commands.Add(previousCommand);
            }

            return commands;
        }

        /// <inheritdoc/>
        public void AddContextMenuDataProvider(IContextMenuDataProvider contextMenuDataProvider)
        {
            if (contextMenuDataProvider != null)
            {
                this.contextMenuDataProviders.Add(contextMenuDataProvider);
            }
        }

        /// <inheritdoc/>
        public IEnumerable<IMenuItem> GetContextMenuItems()
        {
            var contextMenuItems = new List<IMenuItem>();
            foreach (var contextMenuDataProvider in this.contextMenuDataProviders)
            {
                contextMenuItems.AddRange(contextMenuDataProvider.CollectContextMenuItems(this.commandParameter));
            }

            if (!contextMenuItems.Any())
            {
                return Enumerable.Empty<IMenuItem>();
            }

            return contextMenuItems;
        }

        /// <inheritdoc/>
        public void RemoveContextMenuDataProvider(IContextMenuDataProvider contextMenuDataProvider)
        {
            if (contextMenuDataProvider != null)
            {
                this.contextMenuDataProviders.Remove(contextMenuDataProvider);
            }
        }

        private static ICommandEx ConvertSeparatorMenuItemToCommandEx(IMenuItem separator, ICommandEx previousCommand)
        {
            var separatorCommand = CommandHelpers.CreateAdjacentSeparator(previousCommand);
            separatorCommand.Weight = separator.Weight;

            return separatorCommand;
        }

        private static ICommandEx ConvertMenuItemToCommandEx(IMenuItem contextMenuItem)
        {
            if (!contextMenuItem.SubItems.Any())
            {
                return new RelayCommandEx(executeParam => contextMenuItem.Command.Execute(contextMenuItem.CommandParameter), canExecuteParam => contextMenuItem.Command.CanExecute(contextMenuItem.CommandParameter))
                {
                    LabelTitle = contextMenuItem.Text,
                    Weight = contextMenuItem.Weight,
                    SmallImageSource = contextMenuItem.Icon?.Source
                };
            }

            var subMenuCommands = new List<ICommandEx>();
            var groupCommand = new RelayCommandEx(executeParam => contextMenuItem.Command.Execute(contextMenuItem.CommandParameter), canExecuteParam => contextMenuItem.Command.CanExecute(contextMenuItem.CommandParameter))
            {
                LabelTitle = contextMenuItem.Text,
                Weight = contextMenuItem.Weight,
                SmallImageSource = contextMenuItem.Icon?.Source
            };

            subMenuCommands.AddRange(ConvertMenuItemsToCommands(contextMenuItem.SubItems));
            return AddGroupCommands(groupCommand, subMenuCommands, groupCommand.Weight);
        }

        private static ShellCommandInstance AddGroupCommands(ICommandEx parentCommand, List<ICommandEx> subMenuCommands, double weight)
        {
            var subMenuShellCommandInstanceList = new List<ShellCommandInstance>();
            foreach (var subMenuCommand in subMenuCommands)
            {
                var subMenuShellCommandInstance = subMenuCommand as ShellCommandInstance;
                subMenuShellCommandInstanceList.Add(subMenuShellCommandInstance ?? new ShellCommandInstance(subMenuCommand));
            }

            var itemsEnumerableCommandParameter = new ItemsEnumerableCommandParameter(parentCommand)
            {
                Items = subMenuShellCommandInstanceList
            };

            return new ShellCommandInstance(parentCommand)
            {
                CommandParameter = itemsEnumerableCommandParameter,
                Weight = weight
            };
        }
    }
}
