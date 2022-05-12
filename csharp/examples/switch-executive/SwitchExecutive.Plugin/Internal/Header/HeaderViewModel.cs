using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows;

using SwitchExecutive.Plugin.Internal.DriverOperations;
using SwitchExecutive.Plugin.Internal.Common;


namespace SwitchExecutive.Plugin.Internal
{
    internal sealed class HeaderViewModel : BaseNotify
    {
        private static string _disconnected = "Disconnected";

        private readonly ISwitchExecutiveDriverOperations _driverOperations;
        private IStatus _status;
        private FrameworkElement _headerMenuVisual;

        #region Constructors

        public HeaderViewModel(
           bool isSwitchExecutiveInstalled,
           ISwitchExecutiveDriverOperations driverOperations,
           ISave saveOperation,
           IStatus status)
        {
            IsSwitchExecutiveInstalled = isSwitchExecutiveInstalled;
            _driverOperations = driverOperations;
            _status = status;

            HeaderMenuViewModel = new HeaderMenuViewModel(driverOperations, saveOperation, status);

            _driverOperations.PropertyChanged += DriverOperations_PropertyChanged;
            _status.PropertyChanged += Status_PropertyChanged;
        }

        #endregion

        #region Properties

        public string HeaderPanelTitle => "SWITCH EXECUTIVE APP";
        public bool IsInstrumentActive => Status != HeaderViewModel._disconnected;
        public HeaderMenuViewModel HeaderMenuViewModel { get; }
        public FrameworkElement HeaderMenuVisual
        {
            get
            {
                if (_headerMenuVisual == null)
                {
                    _headerMenuVisual = (FrameworkElement)CreateHeaderMenuView();
                    NotifyPropertyChanged();
                }

                return _headerMenuVisual;
            }
        }
        /* very basic error handling.
        * 1. clear (any action clears existing error)
        * 2. try/catch, on catch set a status string with reason
        * 3. show status string at top of the UI. (tooltip for full description)*/
        public string Status
        {
            get
            {
                if (!IsSwitchExecutiveInstalled)
                    SetErrorMessage("Error: Switch Executive is not installed.");

                if (_status.IsFatal)
                    return _status.GetMessage();

                bool connected = _driverOperations.SelectedVirtualDevice != string.Empty;
                if (connected)
                    return _driverOperations.SelectedVirtualDevice;
                else
                    return HeaderViewModel._disconnected;
            }
        }
        private bool IsSwitchExecutiveInstalled { get; }

        #endregion

        private FrameworkElement CreateHeaderMenuView() => new HeaderMenuView(HeaderMenuViewModel);
        private void SetErrorMessage(string msg) => _status.Set(msg);
        private void ClearErrorMessage() => _status.Clear();
        private void DriverOperations_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_driverOperations.SelectedVirtualDevice))
            {
                NotifyPropertyChanged(nameof(Status));
                NotifyPropertyChanged(nameof(IsInstrumentActive));
            }
        }
        private void Status_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_status.Message))
                NotifyPropertyChanged(nameof(Status));
        }
    }
}
