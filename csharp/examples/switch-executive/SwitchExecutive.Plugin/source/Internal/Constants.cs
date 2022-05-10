using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using SwitchExecutive.Plugin.Internal.Controls;

using NationalInstruments.Controls;

namespace SwitchExecutive.Plugin.Internal
{
   public sealed class Constants
   {
      public sealed class AxisConfiguration
      {
         public const string DoubleAxisFormat = "0.000";
         public const string TimeAxisFormat = "HH:mm:ss.f";
      }

      public sealed class Channel
      {
         public const char SourceSeparator = '/';
         public static readonly Geometry ChannelPlotIconPathData = Geometry.Parse("M23,7.55273a10.65473,10.65473,0,0,1-1.978,1.65332A5.64934,5.64934,0,0,1,17.98438,10a5.64935,5.64935,0,0,1-3.0376-.794A12.02088,12.02088,0,0,1,12.708,7.294a13.72857,13.72857,0,0,0-2.62354-2.2129A7.66629,7.66629,0,0,0,6.01562,4,7.66628,7.66628,0,0,0,1.94678,5.08105,8.85614,8.85614,0,0,0,1,5.75073V8.44727A10.65473,10.65473,0,0,1,2.978,6.794,5.64934,5.64934,0,0,1,6.01562,6a5.64935,5.64935,0,0,1,3.0376.79395A12.02088,12.02088,0,0,1,11.292,8.70605a13.72857,13.72857,0,0,0,2.62354,2.2129A7.66629,7.66629,0,0,0,17.98438,12a7.66628,7.66628,0,0,0,4.06884-1.081A8.85614,8.85614,0,0,0,23,10.24927Z");

         public static readonly GeometryGroup PreviewChannelPlotIconPathData = new GeometryGroup
         {
            Children = new GeometryCollection
            {
               Geometry.Parse("M22.9,7.6L22.9,7.6C22.9,7.7,23,7.6,22.9,7.6L22.9,7.6z"),
               Geometry.Parse("M11,5.8c-0.3-0.2-0.5-0.4-0.8-0.6c-1-0.7-2.1-1-3.2-1.1v2c0.7,0.1,1.4,0.3,2,0.7c0.7,0.4,1.4,1.1,2,1.7V5.8z"),
               Geometry.Parse("M5,4.1c-1.1,0.1-2.1,0.4-3.1,1C1.6,5.3,1.3,5.5,1,5.8v2.7c0.6-0.6,1.3-1.2,2-1.7c0.6-0.4,1.3-0.6,2-0.7V4.1z"),
               Geometry.Parse("M17,10c-0.7-0.1-1.4-0.3-2-0.7c-0.7-0.4-1.4-1.1-2-1.7v2.7c0.3,0.2,0.5,0.4,0.8,0.6c1,0.6,2,0.9,3.2,1V10z"),
               Geometry.Parse("M21,9.3c-0.6,0.3-1.3,0.6-2,0.7v1.9c1-0.1,2-0.4,3-1c0.3-0.2,0.6-0.4,0.9-0.7V7.7C22.3,8.3,21.7,8.8,21,9.3z")
            }
         };
      }

      public sealed class Configuration
      {
         public const string NewFileConfiguration = "";
      }

      public sealed class Cursors
      {
         public const double ValueNotSet = double.NaN;
         public static readonly string AutomaticSource = "Automatic";
         public static readonly string SourceNotSet = "Unassigned";
      }

      public sealed class Graph
      {
         /// <summary>
         /// The number of usable divisions on either side of the axes.
         /// </summary>
         public const double DivisionsLimit = 10000;

         /// <summary>
         /// The number of mouse scroll wheel "ticks" that it should take to move a position handle one graph division.
         /// </summary>
         public const int ScrollWheelGranularity = 20;
      }

      public sealed class InstrumentCreation
      {
         public const string NoSelectedInstrument = "";
         public static readonly IEnumerable<string> NoSelectedDevices = Enumerable.Empty<string>();
      }

      public sealed class PanelPresentation
      {
         public static readonly IReadOnlyList<NationalInstruments.InstrumentFramework.Plugins.PanelPresentation> NoSupportedPresentations = new NationalInstruments.InstrumentFramework.Plugins.PanelPresentation[0];
      }

      public sealed class InstrumentPanels
      {
         public const string DisplayValueDelimiter = "  \u00B7  ";
         public const string NoBlockBannerColor = "#00FFFFFF";
         public const FrameworkElement NoCustomVisual = null;
         public const string NoLabel = "";
         public const FrameworkElement NoSectionHeaderControls = null;

         public const double DefaultMinimumHeightForLargePanel = 500;
         public const double DefaultMinimumHeightForSmallPanel = 235;

         // For the best behavior, default proportions should have the same ratio as minimum heights.
         // This makes it work if the user has never moved a grid splitter, but as soon as the user changes the proportions, you can still get the bottom of the makers Table cut off.
         public const double GraphContainerDefaultProportion = 600;
         public const double FFTGraphContainerDefaultProportion = 600;
         public const double TableContainerDefaultProportion = 150;

         public const double GraphContainerMinimumExpandedHeight = 300;
         public const double FFTGraphContainerMinimumExpandedHeight = 300;
         public const double TableContainerMinimumExpandedHeight = 75;

         public static readonly double DisplayContainerSplitterHeight = (double)InstrumentPanelResources.Instance["DisplayContainerSplitterHeight"];
         public static readonly double DisplayContainerCollapsedHeight = (double)InstrumentPanelResources.Instance["DisplaySectionHeaderHeight"];
         public static readonly double DocumentBorderThickness = ((Thickness)InstrumentPanelResources.Instance["DocumentBorderThickness"]).Top;
      }

      public sealed class InstrumentPanelsTierTwo
      {
         public const double DefaultHeight = -2;
         public const double DefaultWidth = -2;
         public const double AutoHeight = double.NaN;
         public const double AutoWidth = double.NaN;
         public const string NoTitle = "";
         public const FrameworkElement NoTitleBarControls = null;

         public static readonly string TierTwoWindowTitleForChannels = "TierTwoChannelConfiguration";
      }

      public sealed class NumericAttributeFormattingConstants
      {
         public const int DoNotConsiderNumberOfDigitsBeforeDecimal = -1;
         public const double IntervalPercentage = 0.05;
         public const int DefaultNumberOfSignificantDigits = 5;
      }

      public sealed class NumericTextBoxInteractionModesConstants
      {
         public const NumericTextBoxInteractionModes AllInput = NumericTextBoxInteractionModes.ArrowKeys
                                                              | NumericTextBoxInteractionModes.ButtonClicks
                                                              | NumericTextBoxInteractionModes.ScrollWheel
                                                              | NumericTextBoxInteractionModes.TextInput;

         public const NumericTextBoxInteractionModes AllExceptButtonClicks = NumericTextBoxInteractionModes.ArrowKeys
                                                                           | NumericTextBoxInteractionModes.ScrollWheel
                                                                           | NumericTextBoxInteractionModes.TextInput;
      }

      public sealed class TriggerSourceAndEventDestinationTerminal
      {
         public const string None = "None";
         public const string External = "External";
         public const string PXITrig0 = "PXI_Trig 0";
         public const string PXITrig1 = "PXI_Trig 1";
         public const string PXITrig2 = "PXI_Trig 2";
         public const string PXITrig3 = "PXI_Trig 3";
         public const string PXITrig4 = "PXI_Trig 4";
         public const string PXITrig5 = "PXI_Trig 5";
         public const string PXITrig6 = "PXI_Trig 6";
         public const string PXITrig7 = "PXI_Trig 7";
         public const string RTSI0 = "RTSI 0";
         public const string RTSI1 = "RTSI 1";
         public const string RTSI2 = "RTSI 2";
         public const string RTSI3 = "RTSI 3";
         public const string RTSI4 = "RTSI 4";
         public const string RTSI5 = "RTSI 5";
         public const string RTSI6 = "RTSI 6";
         public const string RTSI7 = "RTSI 7";
         public const string PFI0 = "PFI 0";
         public const string PFI1 = "PFI 1";
         public const string PFI2 = "PFI 2";
         public const string PFI3 = "PFI 3";
         public const string PFI4 = "PFI 4";
         public const string PFI5 = "PFI 5";
         public const string PXIStar = "PXI Star";
         public const string AUX0PFI0 = "AUX 0/PFI 0";
         public const string AUX0PFI1 = "AUX 0/PFI 1";
         public const string AUX0PFI2 = "AUX 0/PFI 2";
         public const string AUX0PFI3 = "AUX 0/PFI 3";
         public const string AUX0PFI4 = "AUX 0/PFI 4";
         public const string AUX0PFI5 = "AUX 0/PFI 5";
         public const string AUX0PFI6 = "AUX 0/PFI 6";
         public const string AUX0PFI7 = "AUX 0/PFI 7";
      }

      public sealed class FFT
      {
         public static readonly int VerticalDivisions = 8;
         public static readonly int HorizontalDivisions = 10;
      }

      public sealed class Markers
      {
         public static readonly string UnassignedSource = "Unassigned";

         public static readonly string UnassignedKey = "Unassigned";
      }

      public sealed class MenuItemWeights
      {
         #region Menu Weights for Instrument Settings, used in SFP-specific HeaderMenuView.xaml

         public const double MenuWeightDataCapture = 100.0;

         public const double MenuWeightExportConfiguration = 200.0;

         public const double MenuWeightSeparatorAfterExportOptions = 999.9;

         public const double MenuWeightBeginDeviceList = 1000.0;

         public const double MenuWeightAddDevice = 1800.0;

         public const double MenuWeightSeparatorAfterHardware = 1999.9;

         public const double MenuWeightLaunchInNewTab = 2000.0;

         public const double MenuWeightDeleteContainer = 2100.0;

         public const double MenuWeightConfigureDebug = 1900.0;

         public const double MenuWeightSeparatorAfterLayout = 2999.9;

         #endregion

         #region Menu Weights for the Context Menu

         public const double MenuWeightData = .320;

         public const double MenuWeightSubMenuItems = 0.12;

         public const double MenuWeightExport = .310;

         public const double MenuWeightApiGuide = .330;

         public const double MenuWeightStopAllOutputs = .300;

         public const double MenuWeightExportToTestStand = .13;

         public const double MenuWeightInstrumentSpecificCommands = .200;

         public const double MenuWeightSpecificCommands = 0.100;

         public const double MenuWeightSeparatorAfterStopAllOutputs = .301;

         public const double MenuWeightSeparatorAfterInstrumentSpecificCommands = .299;

         public const double MenuWeightSeparatorAfterSpecificCommands = .199;

         public const double MenuWeightSeparatorAfterMoveHere = 0.11;

         public const double MenuWeightMoveHereCommand = 0.1;

         #endregion
      }
   }
}
