#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
#endregion

namespace CurveLengthSample
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            // Access current selection

            Reference r;
               
            try
            {
                r = uidoc.Selection.PickObject(ObjectType.Element, new FloorFilter(),  "Pick a floor face");
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return Result.Cancelled;
            }

            var floor =
                doc.get_Element(r.ElementId) as Floor;

            var floorGeometry =
                floor.get_Geometry(new Options());

            foreach (Solid solid in floorGeometry)
            {
                var edges = solid.Edges;


                StringBuilder sb = 
                    new StringBuilder();

                var idx = 1;

                foreach (Edge edge in edges)
                {
                    var curve =
                        edge.AsCurve();

                    var length = curve.Length;
                    var approximateLength =
                        curve.ApproximateLength;

                    sb.AppendLine(string.Format("[{0}] Length: {1}; ApproximateLength: {2}",
                                                idx++,
                                                length,
                                                approximateLength));
                }

                TaskDialog.Show("Curves", sb.ToString());
            }

            
            
            return Result.Succeeded;
        }
    }
    class FloorFilter: ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            return elem is Floor;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            throw new NotImplementedException();
        }
    }
}
