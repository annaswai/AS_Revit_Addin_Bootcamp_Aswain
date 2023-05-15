#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

#endregion

namespace _23_0405_Creating_Model_Elements_Skills_2023
{
    [Transaction(TransactionMode.Manual)]
    public class Command1 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // this is a variable for the Revit application
            UIApplication uiapp = commandData.Application;

            // this is a variable for the current Revit model
            Document doc = uiapp.ActiveUIDocument.Document;

            // Your code goes here

            // 1. pick elements and filter them in to a list
            UIDocument uidoc = uiapp.ActiveUIDocument;
            IList<Element> pickList = uidoc.Selection.PickElementsByRectangle("Select elements");

            TaskDialog.Show ("Test", "I selected " + pickList.Count.ToString() + " elements");

            // 2.filter selected elements for curves
            List<CurveElement> allCurves = new List<CurveElement>();
            foreach (Element elem in pickList)
            {
                if (elem is CurveElement)
                {
                    allCurves.Add(elem as CurveElement);
                }
            }

            // 2b. Filter selected elements for model curves
            List<CurveElement> modelCurves = new List<CurveElement>();
            foreach (Element elem in pickList) 
            {
                if (elem is CurveElement)
                {
                    //CurveElement curveElem = elem as CurveElement;
                    CurveElement curveElem = (CurveElement)elem;

                    if (curveElem.CurveElementType == CurveElementType.ModelCurve)
                    {
                        modelCurves.Add(curveElem);
                    }
                }
            }

            // 3. curve data
            foreach (CurveElement currentCurve in modelCurves)
            {
                Curve curve = currentCurve.GeometryCurve;
                XYZ startPoint = curve.GetEndPoint(0);
                XYZ endPoint = curve.GetEndPoint(1);

                GraphicsStyle curStyle = currentCurve.LineStyle as GraphicsStyle;

                Debug.Print(curStyle.Name);
            }

            // 5. Create transaction with using statment (makes temperatory code)
            using (Transaction t = new Transaction(doc))
            {
                // 4. create wall
                t.Start("create Revit elements");
                Level newLevel = Level.Create(doc, 20);
                Curve curCurve1 = modelCurves[0].GeometryCurve;

                Wall.Create(doc, curCurve1, newLevel.Id, false);

                FilteredElementCollector wallTypes = new FilteredElementCollector(doc);
                wallTypes.OfClass(typeof(WallType));

                Curve curCurve2 = modelCurves[1].GeometryCurve;
                Wall.Create(doc, curCurve2, wallTypes.FirstElementId(), newLevel.Id, 20, 0, false, false);
            }

           

            return Result.Succeeded;
        }

        public static String GetMethod()
        {
            var method = MethodBase.GetCurrentMethod().DeclaringType?.FullName;
            return method;
        }
    }
}
