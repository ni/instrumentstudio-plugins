using System;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;


using SwitchExecutive.Plugin.Internal.DriverOperations;
using SwitchExecutive.Plugin.Internal.Common;


namespace SwitchExecutive.Plugin.Internal
{
   [JsonObject(MemberSerialization.OptIn)]
   internal sealed class ConfigurationViewModel : BaseNotify
   {
      #region Fields

      public static readonly Dictionary<string, MulticonnectMode> SupportedMulticonnectModes = new Dictionary<string, MulticonnectMode>
      {
         { "Multiconnect Routes", MulticonnectMode.Multiconnect },
         { "No Multiconnect", MulticonnectMode.NoMulticonnect },
         { "Use Default Setting for Routes", MulticonnectMode.DefaultMode },
      };

      private readonly ISwitchExecutiveDriverOperations driverOperations;
      private readonly ISave saveOperation;
      private IStatus status;
      private string selectedConnectionMode = SupportedMulticonnectModes.First().Key;

      #endregion

      #region Constructors

      public ConfigurationViewModel(
         ISwitchExecutiveDriverOperations driverOperations,
         ISave saveOperation,
         IStatus status)
      {
         this.driverOperations = driverOperations;
         this.saveOperation = saveOperation;
         this.status = status;

         this.ConnectedRouteTableViewModel = new ConnectedRouteTableViewModel(driverOperations, saveOperation, status);

         // these commands ask the driver to do work that could take time, so they execute in a separate thread
         this.ConnectRouteCommand = this.CreateConnectCommand();
         this.DisconnectRouteCommand = this.CreateDisconnectCommand();
         this.DisconnectAllRouteCommand = this.CreateDisconnectAllCommand();

         this.driverOperations.PropertyChanged += DriverOperations_PropertyChanged;
      }

      #endregion

      #region Properties
      
      public HeaderMenuViewModel HeaderMenuViewModel { get; }
      public ConnectedRouteTableViewModel ConnectedRouteTableViewModel { get; }

      public IEnumerable<string> RouteList => this.driverOperations.RouteNames;
      public bool IsRouteListSelectable => this.driverOperations.SelectedVirtualDevice.Any();
      [JsonProperty]
      public string SelectedRoute
      {
         get => this.driverOperations.SelectedRoute;
         set
         {
            if (value == null) { return; }

            this.driverOperations.SelectedRoute = value;
            this.NotifyPropertyChanged();

            this.Save();
         }
      }
      public string SelectedRouteComment => this.driverOperations.Comment;
      public IEnumerable<string> ConnectionModes => SupportedMulticonnectModes.Keys;
      [JsonProperty]
      public string SelectedConnectionMode
      {
         get => this.selectedConnectionMode;
         set
         {
            this.selectedConnectionMode = value;
            this.Save();
         }
      }

      public ICommand ConnectRouteCommand { get; }
      public ICommand DisconnectRouteCommand { get; }
      public ICommand DisconnectAllRouteCommand { get; }

      #endregion

      private void OnConnect(object obj)
      {
         this.ClearErrorMessage();

         try
         {
            this.driverOperations.TryConnect(SupportedMulticonnectModes[this.SelectedConnectionMode]);
         }
         catch (DriverException e)
         {
            this.SetErrorMessage(e.Message);
         }
      }

      private bool CanConnect(object obj)
      {
         bool canConnect = false;
         try
         {
            canConnect = this.driverOperations.CanConnect();
         }
         catch (DriverException e)
         {
            // this method is to prevent the user from clicking buttons that will fail
            // so swallow any other errors that come back.  if revelant the user will
            // get the error on a user interaction.
            //this.SetErrorMessage(e.Message);
         }

         return canConnect;
      }

      private NationalInstruments.RelayCommand CreateConnectCommand()
      {
         Action connectAction = async () =>
         {
            await Task.Run(() => this.OnConnect(null));
            this.Save();
         };

         return
            new NationalInstruments.RelayCommand(
               execute: o => connectAction(),
               canExecute: this.CanConnect);
      }

      private void OnDisconnect(object obj)
      {
         this.ClearErrorMessage();

         try
         {
            if (this.driverOperations.IsConnected())
               this.driverOperations.TryDisconnect();
         }
         catch (DriverException e)
         {
            this.SetErrorMessage(e.Message);
         }
      }

      private bool CanDisconnect(object obj)
      {
         bool canDisconnect = false;
         try
         {
            canDisconnect = this.driverOperations.CanDisconnect();
         }
         catch (DriverException e)
         {
            // this method is to prevent the user from clicking buttons that will fail
            // so swallow any other errors that come back.  if revelant the user will
            // get the error on a user interaction.
            //this.SetErrorMessage(e.Message);
         }

         return canDisconnect;
      }

      private NationalInstruments.RelayCommand CreateDisconnectCommand()
      {
         Action disconnectAction = async () =>
         {
            await Task.Run(() => this.OnDisconnect(null));
            this.Save();
         };

         return
            new NationalInstruments.RelayCommand(
               execute: o => disconnectAction(),
               canExecute: this.CanDisconnect);
      }

      private void OnDisconnectAll(object obj)
      {
         this.ClearErrorMessage();

         try
         {
            this.driverOperations.TryDisconnectAll();
         }
         catch (DriverException e)
         {
            this.SetErrorMessage(e.Message);
         }
      }

      private bool CanDisconnectAll(object obj)
      {
         bool canDisconnect = true;
         try
         {
            canDisconnect = this.driverOperations.CanDisconnectAll();
         }
         catch (DriverException e)
         {
            // this method is to prevent the user from clicking buttons that will fail
            // so swallow any other errors that come back.  if revelant the user will
            // get the error on a user interaction.
            //this.SetErrorMessage(e.Message);
         }

         return canDisconnect;
      }

      private NationalInstruments.RelayCommand CreateDisconnectAllCommand()
      {
         Action disconnectAllAction = async () =>
         {
            await Task.Run(() => this.OnDisconnectAll(null));
            this.Save();
         };

         return
             new NationalInstruments.RelayCommand(
               execute: o => disconnectAllAction(),
               canExecute: this.CanDisconnectAll);
      }

      private void Save() => this.saveOperation.Save();
      private void SetErrorMessage(string msg) => this.status.Set(msg);
      private void ClearErrorMessage() => this.status.Clear();
      private void DriverOperations_PropertyChanged(object sender, PropertyChangedEventArgs e)
      {
         if (e.PropertyName == nameof(this.driverOperations.RouteNames))
            this.NotifyPropertyChanged(nameof(this.RouteList));
         if (e.PropertyName == nameof(this.driverOperations.SelectedVirtualDevice))
            this.NotifyPropertyChanged(nameof(this.IsRouteListSelectable));
         if (e.PropertyName == nameof(this.driverOperations.Comment))
            this.NotifyPropertyChanged(nameof(this.SelectedRouteComment));
         if (e.PropertyName == nameof(this.driverOperations.SelectedRoute))
            this.NotifyPropertyChanged(nameof(this.SelectedRoute));
      }
   }
}
