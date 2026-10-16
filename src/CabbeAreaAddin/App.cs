using System;
using System.Reflection;
using Autodesk.Revit.UI;

namespace CabbeAreaAddin
{
    public class App : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication application)
        {
            string tabName = "Cabbe";
            try
            {
                application.CreateRibbonTab(tabName);
            }
            catch
            {
                // Fliken kan redan finnas (t.ex. från CabbeBipAddin)
            }

            RibbonPanel? panel = null;
            var panels = application.GetRibbonPanels(tabName);
            foreach (var p in panels)
            {
                if (p.Name == "Areor")
                {
                    panel = p;
                    break;
                }
            }

            if (panel == null)
            {
                panel = application.CreateRibbonPanel(tabName, "Areor");
            }

            string thisAssemblyPath = Assembly.GetExecutingAssembly().Location;
            PushButtonData buttonData = new PushButtonData(
                "cmdCabbeAreakontroll",
                "Areakontroll",
                thisAssemblyPath,
                "CabbeAreaAddin.Command"
            )
            {
                ToolTip = "Kvalitetsgranska areamodellering (BTA, BRA m.fl.) enligt svensk standard SS 21054."
            };

            try
            {
                using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("CabbeAreaAddin.icon.png");
                if (stream != null)
                {
                    var decoder = new System.Windows.Media.Imaging.PngBitmapDecoder(
                        stream,
                        System.Windows.Media.Imaging.BitmapCreateOptions.PreservePixelFormat,
                        System.Windows.Media.Imaging.BitmapCacheOption.Default);
                    if (decoder.Frames.Count > 0)
                    {
                        buttonData.LargeImage = decoder.Frames[0];
                    }
                }
            }
            catch { }

            panel.AddItem(buttonData);
            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}
