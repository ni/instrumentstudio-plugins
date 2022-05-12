using NationalInstruments.InstrumentFramework.Plugins;
using System;
using System.Windows;
using System.Windows.Controls;
using SwitchExecutive.Plugin.Internal.DriverOperations;
using SwitchExecutive.Plugin.Internal.Common;

namespace SwitchExecutive.Plugin.Internal
{
    public partial class SwitchExecutiveControl : UserControl
    {
        public SwitchExecutiveControl(PluginSession pluginSession)
        {
            InitializeComponent();

            /* used to save/load our view models and models.  for save we serialize
               and return the string to InstrumentStudio via PluginSession.  This 
               class also handles not saving during load.  */
            var saveDelegator = new SaveDelegator(pluginSession);

            /* crete the main view model which creates all the child view models and models.  by
               doing this creation here we imply that the view is created first. Also we:

               1. check the registry to see if SwitchExecutive is installed. 
               2. create a DriverOperations class that is basically our model and conneciton to the driver.
               3. create a status option that is shared to all view models.  this allows any code to report errors. */
            var mainViewModel =
               new SwitchExecutiveControlViewModel(
                     pluginSession.RequestedPresentation,
                     NISwitchExecutiveDriverOperations.IsDriverInstalled(),
                     (ISwitchExecutiveDriverOperations)new NISwitchExecutiveDriverOperations(),
                     (ISave)saveDelegator,
                     (IStatus)new Status());

            /* attach the serialize and deserialize commands after.  This allows us to create any objects we need
               prior to creating the ViewModels ... just for a cleaner construction. */
            saveDelegator.Attach(
               serialize: new Func<string>(() => mainViewModel.Serialize()),
               deserialize: o => mainViewModel.Deserialize(o));


            DataContext = mainViewModel;

            // update our state based on the state saved in the .sfp file
            saveDelegator.Deserialize(pluginSession.EditTimeConfiguration);
            // restore connections from the saved file
            mainViewModel.ApplyLoadFromFile();
        }
    }
}
