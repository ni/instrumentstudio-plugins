# Game of Life - InstrumentStudio Plugin Example

This InstrumentStudio plugin example illustrates how to host Game of Life LabVIEW code seamlessly
into InstrumentStudio environment.

This example demonstrates the usage of XY Data in a graph. It allows the user to specify the size of
the game board and the update interval between generations. It will run indefinitely as long as
there are 'births' and 'deaths' happening on the game board unless the `Stop all outputs` or
`Start/Stop` button is switched off.

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

1. Open the LabVIEW project (Game of Life.lvproj) file for the example.
2. Build the packed project library under the build specification.
3. Once the build is over, copy the folder containing the builds and place it under the Addons
   folder of InstrumentStudio at `C:\Program Files\National Instruments\InstrumentStudio\Addons`.
4. Now, open InstrumentStudio, create a new InstrumentStudio project, and save it.
5. Within the project, create a manual layout by following `Add-Ons -> NI Example Plugins -> Game of
   Life` in Edit Layout.
6. Once the soft front panel is open, the Game of Life InstrumentStudio plugin will start running.

### Stop the example plug-in

Close the soft front panel containing the example plug-in or the whole InstrumentStudio window to
stop the plug-in.

## Developer Guide

### Make changes to the example plug-in

Logic for the Game of Life InstrumentStudio plugin example is found in the top-level plug-in VI
`Game of Life.vi`.

### How to add new UI elements

Add controls and indicators to the front panel of the top-level plug-in VI. Capture events
associated with the controls in the Event Handling Loop and pass the data to Message Handling Loop
through the Queue Driver.vi for further operations.
