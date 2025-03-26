# Game of Life - InstrumentStudio Plugin Example

This InstrumentStudio plugin example illustrates how to host Conway's Game of Life simulation
LabVIEW code seamlessly within the InstrumentStudio environment.

- This example allows the user to specify the size of the game board and the update interval between
  generations.
- The simulation will run indefinitely as long as there are 'births' and 'deaths' occurring on the
  game board, unless the `Start/Stop` button is switched off.

## Features

- Contains a LabVIEW project featuring the Game of Life InstrumentStudio plugin example.
- Includes a `.gplugindata` file that informs InstrumentStudio about the properties of the
  InstrumentStudio plugin example and its implementation code.
- Contains an InstrumentStudio project and a soft front panel to host the Game of Life
  InstrumentStudio plugin example within InstrumentStudio.
- Initiates the game upon switching on `Start\Stop` button in `Game Panel` tab.
- Changes the InstrumentStudio soft front panel status to `running` when the game begins.
- Preserves the front panel control inputs in `Edit Time configuration` and retains their values
  when the soft front panel is reopened.
- Allows the user to stop the game using the `Stop all outputs` button which is present in the
  InstrumentStudio soft front panel header or the `Start\Stop` button present in the `Game Panel`
  tab of the soft front panel.
- Provides an option to view event logs and enable or disable event logging via a checkbox in the
  `Events` tab.
- Logs InstrumentStudio events and errors with timestamps in a CSV file, saved either in the root
  directory of the InstrumentStudio project under the `Event Logs` folder or in the user documents
  under the `InstrumentStudio Logs` folder if no InstrumentStudio project is found.

## User Guide

### Run the example plug-in

To run the example plug-in, follow these steps.

1. Open the LabVIEW project (Game of Life.lvproj) file which contains the example plug-in.
2. The example plug-in comes with a Packed Project Library (PPL) build specification.
3. To build the PPL, right-click on the PPL build specification and choose 'Build'.
4. Once the build is complete, copy or install the built example plug-in files into the
   InstrumentStudio Addons directory, which is `C:\Program Files\National
   Instruments\InstrumentStudio\Addons` by default. This action will require administrative
   permission.
   1. You may optionally install the plug-in files into a subdirectory of the Addons directory so
      its contents do not conflict with other plug-ins.
   2. In addition to the default Addons directory, you can also install the plug-in files to a
      user-defined directory configured in the InstrumentStudio preferences dialog for plug-ins.
        - This user-defined directory allows for the installation of plug-ins without requiring
          administrative permissions.
        - To configure this preference, InstrumentStudio must be launched with administrator
          privileges.
5. Now, open InstrumentStudio, create a new InstrumentStudio project, and save it.
6. From the project window, click `Manual Layout` to open the Edit Layout dialog.
   1. Navigate to `Add-Ons` -> `NI Example Plugins` -> `Conway's Game of Life`.
   2. Create a large panel.
7. Once the SFP is open, an instance of the plug-in VI will start running in the backend.
8. Click on the `Start` button to run the simulation to visualise Conway's Game of Life theory on
   the graph/game board.
   1. The simulation will run indefinitely as long as there are 'births' and 'deaths' occurring on
      the graph.
   2. Click on the `Stop` button to stop the simulation.
9. The plug-in VI instance will stop running on closure of the SFP.

### Stop the example plug-in

Close the soft front panel containing the example plug-in or the whole InstrumentStudio window to
stop the plug-in.

## Developer Guide

### Make changes to the example plug-in

- The implementation logic for the Game of Life InstrumentStudio plugin example is found in the
  top-level plug-in VI `Game of Life.vi`.
- Add your logic/edit the existing logic in the top-level VI to make changes to the plugin.

### How to add new UI elements

- Add controls and indicators to the front panel of the top-level plug-in VI.
- Capture events associated with the controls in the Event Handling Loop and pass the data to
  Message Handling Loop through the `Queue Driver.vi` for further operations.
- Add the necessary logic is respective states of Message handling loop to handle subsequent
  processing and actions.
