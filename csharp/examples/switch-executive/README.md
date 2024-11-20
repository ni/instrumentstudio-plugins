# Switch Executive InstrumentStudio Plug-In

This plug-in allows users to use Switch Executive to connect/disconnect routes from within InstrumentStudio.

## Dependencies

This plug-in requires InstrumentStudio 2022 Q3 or later and NI Switch Executive.

Building this plug-in requires VisualStudio 2022. You cannot build this plug-in using `dotnet build`
because the .NET Core version of MSBuild does not support resolving COM references.

Building this plug-in requires the Microsoft .NET SDK. You can download the
latest from the link below.

| InstrumentStudio Version       | .NET SDK Version       | Link |
|--------------------------------|------------------------|------|
| 2022 Q3 - 2024 Q3              | 6.0                    | [https://dotnet.microsoft.com/en-us/download/dotnet/6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) |
| 2024 Q4 - 2025 Q1              | 8.0                    | [https://dotnet.microsoft.com/en-us/download/dotnet/8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) |

You also need to change the `TargetFramework` element in
SwitchExecutive.Plugin.csproj to match the required .NET SDK Version. For example,
to target InstrumentStudio 2024Q4 or greater, change the .csproj element like
this.
```xml
<TargetFramework>net8.0-windows</TargetFramework>
```

## Building

To build the plug-in, do one of the following:
- In VisualStudio, right click on the solution and select `Build`.
- From a command prompt:
  - cd to the directory containing `SwitchExecutive.Plugin.sln`.
  - Run `dotnet restore`.
  - Run `MSBuild.exe SwitchExecutive.Plugin.sln`. You may have to add `MSBuild.exe` to the `PATH`
  environment variable or provide the full path on the command line. `MSBuild.exe` should be located in the
  following directory: `C:\Program Files\Microsoft Visual Studio\<version>\<edition>\Msbuild\Current\Bin`

The default project settings assume that InstrumentStudio and NI Switch Executive are installed in the
default locations. If these applications are installed in custom locations or are not installed, you
can specify custom paths to the InstrumentStudio assemblies and NI Switch Executive DLL by setting
the `InstrumentStudioDirectory` and `NISwitchExecutiveDirectory` environment variables or editing
`SwitchExecutive.Plugin.csproj`.

## Installation

Copy the built assembly (`SwitchExecutive.Plugin.dll`) into the InstrumentStudio `Addons`
directory, which is `C:\Program Files\National Instruments\InstrumentStudio\Addons` by default.

You may optionally install the plug-in assembly into a subdirectory of the `Addons` directory
in order to isolate it from other plugins.

See [this forum post](https://forums.ni.com/t5/InstrumentStudio/SwitchExecutive-Hosted-Application/gpm-p/3998692?profile.language=en)
for more details about the Switch Executive plug-in.

## Testing

Integration tests are provided in the `SwitchExecutive.Plugin.Tests` directory. The tests
require NI Switch Executive but they do not require any hardware. One of the tests requires
a virtual device in MAX called `VirtualDevice1` that contains a route called `RouteGroup0`.