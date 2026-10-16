using System;
using System.Windows.Interop;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace CabbeAreaAddin
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            if (uidoc == null || uidoc.Document == null)
            {
                TaskDialog.Show("Cabbe Areakontroll", "Öppna en Revit-modell innan du kör areakontrollen.");
                return Result.Cancelled;
            }

            try
            {
                var window = new AreaCheckerWindow(uidoc);
                new WindowInteropHelper(window).Owner = uiapp.MainWindowHandle;
                window.ShowDialog();
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                TaskDialog.Show("Fel vid start", ex.ToString());
                return Result.Failed;
            }
        }
    }
}
