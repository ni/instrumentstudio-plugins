using NationalInstruments.InstrumentFramework.Plugins;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using SwitchExecutive.Plugin.Internal.DriverOperations;
using SwitchExecutive.Plugin.Internal.Common;

/* this code is just a quick and dirty simple switch executive app for the purpose of exploring creating c# plugins 
   for InstrumentStudio.  The code isn't tested and doesn't handle exceptions well. Use at your own risk. */
namespace SwitchExecutive.Plugin.Internal
{
    [JsonObject(MemberSerialization.OptIn)]
    class SwitchExecutiveControlViewModel : BaseNotify, IDisposable
    {
        #region Fields

        private ISwitchExecutiveDriverOperations driverOperations;
        private readonly ISave saveOperation;
        private readonly IStatus status;
        private FrameworkElement displayPanelVisual;

        #endregion

        #region Constructors

        public SwitchExecutiveControlViewModel(
           PanelPresentation requestedPresentation,
           bool isSwitchExecutiveInstalled,
           ISwitchExecutiveDriverOperations driverOperations,
           ISave saveOperation,
           IStatus status)
        {
            this.driverOperations = driverOperations;
            this.saveOperation = saveOperation;
            this.status = status;

            // create view models
            if (requestedPresentation == PanelPresentation.ConfigurationWithVisualization)
                this.VisualizationViewModel = new VisualizationViewModel(driverOperations);
            this.HeaderViewModel = new HeaderViewModel(isSwitchExecutiveInstalled, driverOperations, saveOperation, status);
            this.ConfigurationViewModel = new ConfigurationViewModel(driverOperations, saveOperation, status);

            // we're ready to go, so let's hook up to recieve events from our model and other views models
            this.driverOperations.PropertyChanged += DriverOperations_PropertyChanged;
        }

        #endregion

        #region Properties

        [JsonProperty]
        public SwitchExecutive.Plugin.Internal.Common.Version Version { get; set; } = new SwitchExecutive.Plugin.Internal.Common.Version();
        public Visibility DisplayPanelVisibility => (this.displayPanelVisual == null) ? Visibility.Collapsed : Visibility.Visible;
        public bool IsReadyForUserInteraction => true;
        public bool IsInstrumentActive => this.HeaderViewModel.IsInstrumentActive;

        public SwitchExecutiveControlViewModel MainViewModel { get => this; }
        public VisualizationViewModel VisualizationViewModel { get; }
        public HeaderViewModel HeaderViewModel { get; }
        public ConfigurationViewModel ConfigurationViewModel { get; }
        public NISwitchExecutiveDriverOperations DriverOpertationsModel => (NISwitchExecutiveDriverOperations)this.driverOperations;

        public FrameworkElement DisplayPanelVisual
        {
            get
            {
                if (this.displayPanelVisual == null && this.VisualizationViewModel != null)
                {
                    this.displayPanelVisual = (FrameworkElement)this.CreateVisualizationView();
                    this.NotifyPropertyChanged(nameof(this.DisplayPanelVisibility));
                }

                return this.displayPanelVisual;
            }
        }

        #endregion

        #region Methods

        public void Shutdown() => this.driverOperations.Shutdown();
        public void Dispose() => this.Shutdown();

        private FrameworkElement CreateVisualizationView() => new VisualizationView(this, this.VisualizationViewModel);
        private void Save() => this.saveOperation.Save();

        public string Serialize()
        {
            // any property with [JsonProperty] is serialized on change to support save/load behavior
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };

            return
               JsonConvert.SerializeObject(
                  new
                  {
                      this.Version,
                      this.HeaderViewModel.HeaderMenuViewModel,
                      this.MainViewModel,
                      this.ConfigurationViewModel,
                      this.DriverOpertationsModel,
                  },
                  Formatting.Indented,
                  settings);
        }

        public void Deserialize(string json)
        {
            // for a new file (never saved) the json will be empty.
            if (json == null)
                return;

            try
            {
                var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };

                JObject.Parse(json);
                JsonConvert.PopulateObject(
                   json,
                   new
                   {
                       this.Version,
                       this.HeaderViewModel.HeaderMenuViewModel,
                       this.MainViewModel,
                       this.ConfigurationViewModel,
                       this.DriverOpertationsModel,
                   },
                   settings);
            }
            catch
            {
                // not json so do nothing.
            }
        }

        public void ApplyLoadFromFile()
        {
            try
            {
                if (this.HeaderViewModel.HeaderMenuViewModel.IncludeConnectedRoutesWithSave)
                {
                    this.driverOperations.ApplyLoadFromFile(
                       ConfigurationViewModel.SupportedMulticonnectModes[this.ConfigurationViewModel.SelectedConnectionMode]);
                }
            }
            catch (DriverException e)
            {
                this.SetErrorMessage(e.Message);
            }
        }

        private void SetErrorMessage(string msg) => this.status.Set(msg);
        private void ClearErrorMessage() => this.status.Clear();

        /* hook up to notifications so that changes from models and other views can update this classes properities.
           This app uses a design where the model can notify one or more views about changes. This is because the 
           switch driver can make changes to the state that this app doesn't know about. */
        private void DriverOperations_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(this.driverOperations.SelectedVirtualDevice))
                this.NotifyPropertyChanged(nameof(this.IsInstrumentActive));
        }

        #endregion
    }
}
