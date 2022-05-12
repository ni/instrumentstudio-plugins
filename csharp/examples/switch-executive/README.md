# Switch Executive InstrumentStudio Plugin

This plugin allows users to use Switch Executive to connect/disconnect routes from within InstrumentStudio.

## Build

To build this application, you must have `InstrumentStudio 2022 Q3` or later and `NI Switch Executive` 
installed. The default project settings assume that `InstrumentStudio` is installed in 
`C:\Program Files\National Instruments\InstrumentStudio`. If it is installed in a different directory, 
you will have to change the `InstrumentStudioDirectory` tag in `SwitchExecutive.Plugin.csproj`.

Building this plugin has been tested using VisualStudio 2022. It will not build using `dotnet build` 
because of the `COMReference` in the projects. There are two options for building:
- In VisualStudio, right click on the solution and select `Build`.
- From a command prompt, run `MSBuild.exe SwitchExecutive.Plugin.sln`. Make sure the command prompt 
directory is set to the same directory that contains the solution file. You also maybe have to add 
`MSBuild.exe` to your path variable or provide the full path to it in your command. `MSBuild.exe` will 
be located in the following directory: `C:\Program Files\Microsoft Visual Studio\<version>\<edition>\Msbuild\Current\Bin`

The output of the build is `SwitchExecutive.Plugin.dll`. You can copy the assembly to the InstrumentStudio 
`Addons` directory to have the plugin show up in InstrumentStudio. The `Addons` directory is typically at 
`C:\Program Files\National Instruments\InstrumentStudio\Addons`. 
See [this](https://forums.ni.com/t5/InstrumentStudio/SwitchExecutive-Hosted-Application/gpm-p/3998692?profile.language=en) 
forum post for more details about the Switch Executive plugin.

## Test

Integration tests are provided with a VisualStudio project.  The tests require that `NI Switch Executive` 
be installed but they do not require any hardware. One of the tests requires a virtual device in MAX called 
`VirtualDevice1` that contains a route called `RouteGroup0`.