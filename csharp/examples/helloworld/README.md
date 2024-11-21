# Hello World C# InstrumentStudio Plug-In

This is a minimal example of a C# plug-in for InstrumentStudio.

## Dependencies

This plug-in requires InstrumentStudio 2022 Q3 or later.

Building this plug-in requires the Microsoft .NET SDK. You can download the
latest from the link below.

| InstrumentStudio Version       | .NET SDK Version       | Link |
|--------------------------------|------------------------|------|
| 2022 Q3 - 2024 Q3              | 6.0                    | [https://dotnet.microsoft.com/en-us/download/dotnet/6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) |
| 2024 Q4 - 2025 Q1              | 8.0                    | [https://dotnet.microsoft.com/en-us/download/dotnet/8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) |

You also need to change the `TargetFramework` element in
HelloWorld.NetCore.csproj to match the required .NET SDK Version. For example,
to target InstrumentStudio 2024Q4 or greater, change the .csproj element like
this.
```xml
<TargetFramework>net8.0-windows</TargetFramework>
```

## Building

To build the plug-in, run this command from the 'csharp/examples/helloworld'
directory:

```
dotnet build
```

The default project settings assume that InstrumentStudio is installed in the
default location. If InstrumentStudio is installed in a custom location or is
not installed, you can specify a custom path to the InstrumentStudio assemblies
by setting the `InstrumentStudioDirectory` environment variable or editing the
corresponding variable in `HelloWorld.NetCore.csproj`.

Building InstrumentStudio C# plug-ins requires at least the following assemblies
from the InstrumentStudio installation directory:

- `NationalInstruments.Core.dll`
- `NationalInstruments.InstrumentFramework.Plugins.dll`

## Installation of the Plug-In

Copy the built assembly (`NationalInstruments.HelloWorldPlugin.dll`) into the
InstrumentStudio `Addons` directory, which is `C:\Program Files\National
Instruments\InstrumentStudio\Addons` by default.

You may optionally install the plug-in assembly into a subdirectory of the
`Addons` directory so its contents do not conflict with other plug-ins.

If the plug-in is installed correctly, it will appear in the Edit Layout dialog
in InstrumentStudio:

![Hello World In Edit Layout Dialog](images/HelloWorldInEditLayout.png)

The panel looks like this in InstrumentStudio once it is created:

![Hello World Panel](images/HelloWorldLargePanel.png)