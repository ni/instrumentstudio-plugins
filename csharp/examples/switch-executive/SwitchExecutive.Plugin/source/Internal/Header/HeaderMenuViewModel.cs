using System;
using System.Windows.Input;
using System.ComponentModel;
using System.Collections.Generic;
using SwitchExecutive.Plugin.Internal.DriverOperations;
using SwitchExecutive.Plugin.Internal.Controls.Menu;
using SwitchExecutive.Plugin.Internal.Common;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace SwitchExecutive.Plugin.Internal
{
   [JsonObject(MemberSerialization.OptIn)]
   internal sealed class HeaderMenuViewModel : BaseNotify, IDynamicMenuDataProvider
   {
      private readonly ISwitchExecutiveDriverOperations driverOperations;
      private readonly ISave saveOperation;
      private IStatus status;
      private static ICommand disabledCommand = new NationalInstruments.RelayCommand((o) => System.Linq.Expressions.Expression.Empty(), (o) => false);
      private bool autoRefreshEnabled = false;
      private bool includedConntectedRoutesWithSave = true;

      #region Constructors

      public HeaderMenuViewModel(
         ISwitchExecutiveDriverOperations driverOperations,
         ISave saveOperation,
         IStatus status)
      {
         this.driverOperations = driverOperations;
         this.saveOperation = saveOperation;
         this.status = status;

         this.MenuProvider = new MenuProvider(this);
         this.MenuProvider.AddMenuDataProvider(this);

         this.driverOperations.PropertyChanged += DriverOperations_PropertyChanged;
         this.driverOperations.RefreshOptions(auto: true);
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
            var virtualDevices = this.driverOperations.VirtualDeviceNames;
            var currentlySelectedVirtualDevice = this.SelectedVirtualDevice;
            foreach (var virtualDevice in virtualDevices)
            {
               IMenuItem deviceMenuItem = 
                  MenuItemFactory.CreateMenuItem(
                     menuCommand: 
                        new NationalInstruments.RelayCommand(
                           executeParam => this.SelectedVirtualDevice = virtualDevice,
                           canExecuteParam => virtualDevice != currentlySelectedVirtualDevice),
                     menuText: virtualDevice, 
                     weight: i,
                     commandParameter: null);
               builder.AddMenu(deviceMenuItem);
               i++;
            }
         }

         builder.AddMenu(MenuItemFactory.CreateSeparator(currentWeight++));
         
         //Menu: Refresh
         builder.AddMenu(
            MenuItemFactory.CreateMenuItem(
                     menuCommand:
                        new NationalInstruments.RelayCommand(
                           executeParam => this.driverOperations.Refresh(),
                           canExecuteParam => !this.IsAnyDeviceOffline),
                     menuText: "Refresh",
                     weight: currentWeight++,
                     commandParameter: null));

         return builder.MenuItems;
      }
      public bool IsAnyDeviceOffline => this.driverOperations.SelectedVirtualDevice == string.Empty;
      [JsonProperty]
      public bool AutoRefreshEnabled
      {
         get => this.autoRefreshEnabled;
         set
         {
            this.autoRefreshEnabled = value;
            this.driverOperations.RefreshOptions(auto: value);
            this.Save();

            this.NotifyPropertyChanged();
         }
      }
      [JsonProperty]
      public bool IncludeConnectedRoutesWithSave
      {
         get => this.includedConntectedRoutesWithSave;
         set
         {
            this.includedConntectedRoutesWithSave = value;
            this.Save();

            this.NotifyPropertyChanged();
         }
      }

      [JsonProperty]
      public string SelectedVirtualDevice
      {
         get => this.driverOperations.SelectedVirtualDevice;
         set
         {
            if (value == null) { return; }
            this.ClearErrorMessage();

            this.driverOperations.SelectedVirtualDevice = value;
            this.NotifyPropertyChanged();

            // the .net framework handles the policy on when to call 'canExecutes' on ICommands.
            // for whatever reason it doesn't work well for this app (likely because state 
            // changes are happening in the driver hidden from policy.  This call hints to
            // the framework to requery.  This makes the buttons on the app to be disabled/enabled
            // properly.
            CommandManager.InvalidateRequerySuggested();

            this.Save();
         }
      }

      #endregion

      private void DriverOperations_PropertyChanged(object sender, PropertyChangedEventArgs e)
      {
         if (e.PropertyName == nameof(this.driverOperations.SelectedVirtualDevice))
            this.NotifyPropertyChanged(nameof(this.IsAnyDeviceOffline));
      }

      private void SetErrorMessage(string msg) => this.status.Set(msg);
      private void ClearErrorMessage() => this.status.Clear();
      private void Save() => this.saveOperation.Save();
   }
}
