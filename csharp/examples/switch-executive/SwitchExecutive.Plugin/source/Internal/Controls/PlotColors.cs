using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace SwitchExecutive.Plugin.Internal.Controls
{
   public static class PlotColors
   {
      private static readonly ResourceDictionary InstrumentPanelsResources = Controls.InstrumentPanelResources.Instance;
      private static IList<Color> plotColorList;

      public static IList<Color> PlotColorList
      {
         get
         {
            if (plotColorList == null)
            {
               plotColorList = new List<Color>()
               {
                  (Color)InstrumentPanelsResources["Plot1Color"],
                  (Color)InstrumentPanelsResources["Plot2Color"],
                  (Color)InstrumentPanelsResources["Plot3Color"],
                  (Color)InstrumentPanelsResources["Plot4Color"],
                  (Color)InstrumentPanelsResources["Plot5Color"],
                  (Color)InstrumentPanelsResources["Plot6Color"],
                  (Color)InstrumentPanelsResources["Plot7Color"],
                  (Color)InstrumentPanelsResources["Plot8Color"],
                  (Color)InstrumentPanelsResources["Plot9Color"],
                  (Color)InstrumentPanelsResources["Plot10Color"],
                  (Color)InstrumentPanelsResources["Plot11Color"],
                  (Color)InstrumentPanelsResources["Plot12Color"],
                  (Color)InstrumentPanelsResources["Plot13Color"],
                  (Color)InstrumentPanelsResources["Plot14Color"],
                  (Color)InstrumentPanelsResources["Plot15Color"],
                  (Color)InstrumentPanelsResources["Plot16Color"],
               };
            }

            return plotColorList;
         }
      }

      public static string GetPlotColorStringForIndex(int index)
      {
         int count = PlotColors.PlotColorList.Count;
         index = index % count;

         var color = PlotColors.PlotColorList[index];
         return new ColorConverter().ConvertToString(color);
      }
   }
}
