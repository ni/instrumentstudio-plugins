using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SwitchExecutive.Plugin.Internal.Common;
using SwitchExecutive.Plugin.Internal.DriverOperations;

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

        private readonly ISwitchExecutiveDriverOperations _driverOperations;
        private readonly ISave _saveOperation;
        private IStatus _status;
        private string _selectedConnectionMode = SupportedMulticonnectModes.First().Key;

        #endregion

        #region Constructors

        public ConfigurationViewModel(
           ISwitchExecutiveDriverOperations driverOperations,
           ISave saveOperation,
           IStatus status)
        {
            _driverOperations = driverOperations;
            _saveOperation = saveOperation;
            _status = status;

            ConnectedRouteTableViewModel = new ConnectedRouteTableViewModel(driverOperations, saveOperation, status);

            // these commands ask the driver to do work that could take time, so they execute in a separate thread
            ConnectRouteCommand = CreateConnectCommand();
            DisconnectRouteCommand = CreateDisconnectCommand();
            DisconnectAllRouteCommand = CreateDisconnectAllCommand();

            _driverOperations.PropertyChanged += DriverOperations_PropertyChanged;
        }

        #endregion

        #region Properties

        public HeaderMenuViewModel HeaderMenuViewModel { get; }
        public ConnectedRouteTableViewModel ConnectedRouteTableViewModel { get; }

        public IEnumerable<string> RouteList => _driverOperations.RouteNames;
        public bool IsRouteListSelectable => _driverOperations.SelectedVirtualDevice.Any();
        [JsonProperty]
        public string SelectedRoute
        {
            get => _driverOperations.SelectedRoute;
            set
            {
                if (value == null)
                {
                    return;
                }

                _driverOperations.SelectedRoute = value;
                NotifyPropertyChanged();

                Save();
            }
        }
        public string SelectedRouteComment => _driverOperations.Comment;
        public IEnumerable<string> ConnectionModes => SupportedMulticonnectModes.Keys;
        [JsonProperty]
        public string SelectedConnectionMode
        {
            get => _selectedConnectionMode;
            set
            {
                _selectedConnectionMode = value;
                Save();
            }
        }

        public ICommand ConnectRouteCommand { get; }
        public ICommand DisconnectRouteCommand { get; }
        public ICommand DisconnectAllRouteCommand { get; }

        #endregion

        private void OnConnect(object obj)
        {
            ClearErrorMessage();

            try
            {
                _driverOperations.TryConnect(SupportedMulticonnectModes[SelectedConnectionMode]);
            }
            catch (DriverException e)
            {
                SetErrorMessage(e.Message);
            }
        }

        private bool CanConnect(object obj)
        {
            bool canConnect = false;
            try
            {
                canConnect = _driverOperations.CanConnect();
            }
            catch (DriverException)
            {
                // this method is to prevent the user from clicking buttons that will fail
                // so swallow any other errors that come back.  if revelant the user will
                // get the error on a user interaction.
                ////SetErrorMessage(e.Message);
            }

            return canConnect;
        }

        private NationalInstruments.RelayCommand CreateConnectCommand()
        {
            Action connectAction = async () =>
            {
                await Task.Run(() => OnConnect(null));
                Save();
            };

            return
               new NationalInstruments.RelayCommand(
                  execute: o => connectAction(),
                  canExecute: CanConnect);
        }

        private void OnDisconnect(object obj)
        {
            ClearErrorMessage();

            try
            {
                if (_driverOperations.IsConnected())
                {
                    _driverOperations.TryDisconnect();
                }
            }
            catch (DriverException e)
            {
                SetErrorMessage(e.Message);
            }
        }

        private bool CanDisconnect(object obj)
        {
            bool canDisconnect = false;
            try
            {
                canDisconnect = _driverOperations.CanDisconnect();
            }
            catch (DriverException)
            {
                // this method is to prevent the user from clicking buttons that will fail
                // so swallow any other errors that come back.  if revelant the user will
                // get the error on a user interaction.
                ////SetErrorMessage(e.Message);
            }

            return canDisconnect;
        }

        private NationalInstruments.RelayCommand CreateDisconnectCommand()
        {
            Action disconnectAction = async () =>
            {
                await Task.Run(() => OnDisconnect(null));
                Save();
            };

            return
               new NationalInstruments.RelayCommand(
                  execute: o => disconnectAction(),
                  canExecute: CanDisconnect);
        }

        private void OnDisconnectAll(object obj)
        {
            ClearErrorMessage();

            try
            {
                _driverOperations.TryDisconnectAll();
            }
            catch (DriverException e)
            {
                SetErrorMessage(e.Message);
            }
        }

        private bool CanDisconnectAll(object obj)
        {
            bool canDisconnect = true;
            try
            {
                canDisconnect = _driverOperations.CanDisconnectAll();
            }
            catch (DriverException)
            {
                // this method is to prevent the user from clicking buttons that will fail
                // so swallow any other errors that come back.  if revelant the user will
                // get the error on a user interaction.
                ////SetErrorMessage(e.Message);
            }

            return canDisconnect;
        }

        private NationalInstruments.RelayCommand CreateDisconnectAllCommand()
        {
            Action disconnectAllAction = async () =>
            {
                await Task.Run(() => OnDisconnectAll(null));
                Save();
            };

            return
                new NationalInstruments.RelayCommand(
                  execute: o => disconnectAllAction(),
                  canExecute: CanDisconnectAll);
        }

        private void Save() => _saveOperation.Save();
        private void SetErrorMessage(string msg) => _status.Set(msg);
        private void ClearErrorMessage() => _status.Clear();
        private void DriverOperations_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(_driverOperations.RouteNames):
                    NotifyPropertyChanged(nameof(RouteList));
                    break;
                case nameof(_driverOperations.SelectedVirtualDevice):
                    NotifyPropertyChanged(nameof(IsRouteListSelectable));
                    break;
                case nameof(_driverOperations.Comment):
                    NotifyPropertyChanged(nameof(SelectedRouteComment));
                    break;
                case nameof(_driverOperations.SelectedRoute):
                    NotifyPropertyChanged(nameof(SelectedRoute));
                    break;
            }
        }
    }
}
