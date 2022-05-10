# niinstrumentstudio-switchexecutive-hosted-application

This plugin allows users to use SwitchExecutive to make connect/disconnect routes from within InstrumentStudio 2019.

# Getting Started

The application is built using VisualStudio 2017 for 64 bit user mode.  The project is included with the source code and can be used to build the assembly.  The output assembly is "SwitchExecutive.Plugin.dll".  That assembly can be copied to c:\users\public\documents\National Instruments\InstrumentStudio <year>\Addons\.  After that hosted applications needs to be enabled from the preferences dialog of InstrumentStudio.  More details can be found at [SwitchExecutive App](https://forums.ni.com/t5/InstrumentStudio/SwitchExecutive-Hosted-Application/gpm-p/3998692?profile.language=en)

# Testing

Integration tests are provided with a VisualStudio project.  The tests require that SwitchExecutive be installed on the system but the tests do not require any hardware.
