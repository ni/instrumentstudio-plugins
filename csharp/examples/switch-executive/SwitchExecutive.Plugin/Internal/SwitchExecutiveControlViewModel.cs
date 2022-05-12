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

        private ISwitchExecutiveDriverOperations _driverOperations;
        private readonly ISave _saveOperation;
        private readonly IStatus _status;
        private FrameworkElement _displayPanelVisual;

        #endregion

        #region Constructors

        public SwitchExecutiveControlViewModel(
           PanelPresentation requestedPresentation,
           bool isSwitchExecutiveInstalled,
           ISwitchExecutiveDriverOperations driverOperations,
           ISave saveOperation,
           IStatus status)
        {
            _driverOperations = driverOperations;
            _saveOperation = saveOperation;
            _status = status;

            // create view models
            if (requestedPresentation == PanelPresentation.ConfigurationWithVisualization)
                VisualizationViewModel = new VisualizationViewModel(driverOperations);
            HeaderViewModel = new HeaderViewModel(isSwitchExecutiveInstalled, driverOperations, saveOperation, status);
            ConfigurationViewModel = new ConfigurationViewModel(driverOperations, saveOperation, status);

            // we're ready to go, so let's hook up to recieve events from our model and other views models
            _driverOperations.PropertyChanged += DriverOperations_PropertyChanged;
        }

        #endregion

        #region Properties

        [JsonProperty]
        public SwitchExecutive.Plugin.Internal.Common.Version Version { get; set; } = new SwitchExecutive.Plugin.Internal.Common.Version();
        public Visibility DisplayPanelVisibility => (_displayPanelVisual == null) ? Visibility.Collapsed : Visibility.Visible;
        public bool IsReadyForUserInteraction => true;
        public bool IsInstrumentActive => HeaderViewModel.IsInstrumentActive;

        public SwitchExecutiveControlViewModel MainViewModel { get => this; }
        public VisualizationViewModel VisualizationViewModel { get; }
        public HeaderViewModel HeaderViewModel { get; }
        public ConfigurationViewModel ConfigurationViewModel { get; }
        public NISwitchExecutiveDriverOperations DriverOpertationsModel => (NISwitchExecutiveDriverOperations)_driverOperations;

        public FrameworkElement DisplayPanelVisual
        {
            get
            {
                if (_displayPanelVisual == null && VisualizationViewModel != null)
                {
                    _displayPanelVisual = (FrameworkElement)CreateVisualizationView();
                    NotifyPropertyChanged(nameof(DisplayPanelVisibility));
                }

                return _displayPanelVisual;
            }
        }

        #endregion

        #region Methods

        public void Shutdown() => _driverOperations.Shutdown();
        public void Dispose() => Shutdown();

        private FrameworkElement CreateVisualizationView() => new VisualizationView(this, VisualizationViewModel);
        private void Save() => _saveOperation.Save();

        public string Serialize()
        {
            // any property with [JsonProperty] is serialized on change to support save/load behavior
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };

            return
               JsonConvert.SerializeObject(
                  new
                  {
                      Version,
                      HeaderViewModel.HeaderMenuViewModel,
                      MainViewModel,
                      ConfigurationViewModel,
                      DriverOpertationsModel,
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
                       Version,
                       HeaderViewModel.HeaderMenuViewModel,
                       MainViewModel,
                       ConfigurationViewModel,
                       DriverOpertationsModel,
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
                if (HeaderViewModel.HeaderMenuViewModel.IncludeConnectedRoutesWithSave)
                {
                    _driverOperations.ApplyLoadFromFile(
                       ConfigurationViewModel.SupportedMulticonnectModes[ConfigurationViewModel.SelectedConnectionMode]);
                }
            }
            catch (DriverException e)
            {
                SetErrorMessage(e.Message);
            }
        }

        private void SetErrorMessage(string msg) => _status.Set(msg);
        private void ClearErrorMessage() => _status.Clear();

        /* hook up to notifications so that changes from models and other views can update this classes properities.
           This app uses a design where the model can notify one or more views about changes. This is because the 
           switch driver can make changes to the state that this app doesn't know about. */
        private void DriverOperations_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_driverOperations.SelectedVirtualDevice))
                NotifyPropertyChanged(nameof(IsInstrumentActive));
        }

        #endregion
    }
}
