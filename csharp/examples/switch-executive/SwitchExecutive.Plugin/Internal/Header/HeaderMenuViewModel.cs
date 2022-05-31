using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using SwitchExecutive.Plugin.Internal.Common;
using SwitchExecutive.Plugin.Internal.Controls.Menu;
using SwitchExecutive.Plugin.Internal.DriverOperations;

namespace SwitchExecutive.Plugin.Internal
{
    [JsonObject(MemberSerialization.OptIn)]
    internal sealed class HeaderMenuViewModel : BaseNotify, IDynamicMenuDataProvider
    {
        private readonly ISwitchExecutiveDriverOperations _driverOperations;
        private readonly ISave _saveOperation;
        private IStatus _status;
        private bool _autoRefreshEnabled = false;
        private bool _includedConnectedRoutesWithSave = true;

        #region Constructors

        public HeaderMenuViewModel(
           ISwitchExecutiveDriverOperations driverOperations,
           ISave saveOperation,
           IStatus status)
        {
            _driverOperations = driverOperations;
            _saveOperation = saveOperation;
            _status = status;

            MenuProvider = new MenuProvider(this);
            MenuProvider.AddMenuDataProvider(this);

            _driverOperations.PropertyChanged += DriverOperations_PropertyChanged;
            _driverOperations.RefreshOptions(auto: true);
        }

        #endregion

        #region Properties
        public ImageSource MissingDeviceIcon => new BitmapImage(new Uri("/SwitchExecutive.Plugin;component/resources/missingdevice_16x16.png", UriKind.Relative));
        public IMenuProvider MenuProvider { get; }
        public IEnumerable<IMenuItem> CollectDynamicMenuItems(object commandParameter)
        {
            var builder = new MenuBuilder();

            // Menu:  Virtual devices
            int currentWeight = 0;
            IMenuItem deviceMenuGroup =
               MenuItemFactory.CreateMenuItem(
                  menuText: "Virtual Devices",
                  weight: currentWeight++);

            using (builder.AddMenuGroup(deviceMenuGroup))
            {
                int i = 0;
                var virtualDevices = _driverOperations.VirtualDeviceNames;
                var currentlySelectedVirtualDevice = SelectedVirtualDevice;
                foreach (var virtualDevice in virtualDevices)
                {
                    IMenuItem deviceMenuItem =
                       MenuItemFactory.CreateMenuItem(
                          menuCommand:
                             new NationalInstruments.RelayCommand(
                                executeParam => SelectedVirtualDevice = virtualDevice,
                                canExecuteParam => virtualDevice != currentlySelectedVirtualDevice),
                          menuText: virtualDevice,
                          weight: i,
                          commandParameter: null);
                    builder.AddMenu(deviceMenuItem);
                    i++;
                }
            }

            builder.AddMenu(MenuItemFactory.CreateSeparator(currentWeight++));

            // Menu: Refresh
            builder.AddMenu(
               MenuItemFactory.CreateMenuItem(
                        menuCommand:
                           new NationalInstruments.RelayCommand(
                              executeParam => _driverOperations.Refresh(),
                              canExecuteParam => !IsAnyDeviceOffline),
                        menuText: "Refresh",
                        weight: currentWeight++,
                        commandParameter: null));

            return builder.MenuItems;
        }
        public bool IsAnyDeviceOffline => string.IsNullOrEmpty(_driverOperations.SelectedVirtualDevice);
        [JsonProperty]
        public bool AutoRefreshEnabled
        {
            get => _autoRefreshEnabled;
            set
            {
                _autoRefreshEnabled = value;
                _driverOperations.RefreshOptions(auto: value);
                Save();

                NotifyPropertyChanged();
            }
        }
        [JsonProperty]
        public bool IncludeConnectedRoutesWithSave
        {
            get => _includedConnectedRoutesWithSave;
            set
            {
                _includedConnectedRoutesWithSave = value;
                Save();

                NotifyPropertyChanged();
            }
        }

        [JsonProperty]
        public string SelectedVirtualDevice
        {
            get => _driverOperations.SelectedVirtualDevice;
            set
            {
                if (value == null)
                {
                    return;
                }
                ClearErrorMessage();

                _driverOperations.SelectedVirtualDevice = value;
                NotifyPropertyChanged();

                // the .net framework handles the policy on when to call 'canExecutes' on ICommands.
                // for whatever reason it doesn't work well for this app (likely because state
                // changes are happening in the driver hidden from policy.  This call hints to
                // the framework to requery.  This makes the buttons on the app to be disabled/enabled
                // properly.
                CommandManager.InvalidateRequerySuggested();

                Save();
            }
        }

        #endregion

        private void DriverOperations_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_driverOperations.SelectedVirtualDevice))
            {
                NotifyPropertyChanged(nameof(IsAnyDeviceOffline));
            }
        }

        private void SetErrorMessage(string msg) => _status.Set(msg);
        private void ClearErrorMessage() => _status.Clear();
        private void Save() => _saveOperation.Save();
    }
}
