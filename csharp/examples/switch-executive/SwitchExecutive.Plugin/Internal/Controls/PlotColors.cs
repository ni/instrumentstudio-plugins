using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace SwitchExecutive.Plugin.Internal.Controls
{
    public static class PlotColors
    {
        private static readonly ResourceDictionary _instrumentPanelsResources = Controls.InstrumentPanelResources.Instance;
        private static IList<Color> _plotColorList;

        public static IList<Color> PlotColorList
        {
            get
            {
                if (_plotColorList == null)
                {
                    _plotColorList = new List<Color>()
               {
                  (Color)_instrumentPanelsResources["Plot1Color"],
                  (Color)_instrumentPanelsResources["Plot2Color"],
                  (Color)_instrumentPanelsResources["Plot3Color"],
                  (Color)_instrumentPanelsResources["Plot4Color"],
                  (Color)_instrumentPanelsResources["Plot5Color"],
                  (Color)_instrumentPanelsResources["Plot6Color"],
                  (Color)_instrumentPanelsResources["Plot7Color"],
                  (Color)_instrumentPanelsResources["Plot8Color"],
                  (Color)_instrumentPanelsResources["Plot9Color"],
                  (Color)_instrumentPanelsResources["Plot10Color"],
                  (Color)_instrumentPanelsResources["Plot11Color"],
                  (Color)_instrumentPanelsResources["Plot12Color"],
                  (Color)_instrumentPanelsResources["Plot13Color"],
                  (Color)_instrumentPanelsResources["Plot14Color"],
                  (Color)_instrumentPanelsResources["Plot15Color"],
                  (Color)_instrumentPanelsResources["Plot16Color"],
               };
                }

                return _plotColorList;
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
