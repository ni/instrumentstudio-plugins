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
        private static string Disconnected = "Disconnected";

        private readonly ISwitchExecutiveDriverOperations driverOperations;
        private IStatus status;
        private FrameworkElement headerMenuVisual;

        #region Constructors

        public HeaderViewModel(
           bool isSwitchExecutiveInstalled,
           ISwitchExecutiveDriverOperations driverOperations,
           ISave saveOperation,
           IStatus status)
        {
            this.IsSwitchExecutiveInstalled = isSwitchExecutiveInstalled;
            this.driverOperations = driverOperations;
            this.status = status;

            this.HeaderMenuViewModel = new HeaderMenuViewModel(driverOperations, saveOperation, status);

            this.driverOperations.PropertyChanged += DriverOperations_PropertyChanged;
            this.status.PropertyChanged += Status_PropertyChanged;
        }

        #endregion

        #region Properties

        public string HeaderPanelTitle => "SWITCH EXECUTIVE APP";
        public bool IsInstrumentActive => this.Status != HeaderViewModel.Disconnected;
        public HeaderMenuViewModel HeaderMenuViewModel { get; }
        public FrameworkElement HeaderMenuVisual
        {
            get
            {
                if (this.headerMenuVisual == null)
                {
                    this.headerMenuVisual = (FrameworkElement)this.CreateHeaderMenuView();
                    this.NotifyPropertyChanged();
                }

                return this.headerMenuVisual;
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
                if (!this.IsSwitchExecutiveInstalled)
                    this.SetErrorMessage("Error: Switch Executive is not installed.");

                if (this.status.IsFatal)
                    return this.status.GetMessage();

                bool connected = this.driverOperations.SelectedVirtualDevice != string.Empty;
                if (connected)
                    return this.driverOperations.SelectedVirtualDevice;
                else
                    return HeaderViewModel.Disconnected;
            }
        }
        private bool IsSwitchExecutiveInstalled { get; }

        #endregion

        private FrameworkElement CreateHeaderMenuView() => new HeaderMenuView(this.HeaderMenuViewModel);
        private void SetErrorMessage(string msg) => this.status.Set(msg);
        private void ClearErrorMessage() => this.status.Clear();
        private void DriverOperations_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(this.driverOperations.SelectedVirtualDevice))
            {
                this.NotifyPropertyChanged(nameof(this.Status));
                this.NotifyPropertyChanged(nameof(this.IsInstrumentActive));
            }
        }
        private void Status_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(this.status.Message))
                this.NotifyPropertyChanged(nameof(this.Status));
        }
    }
}
