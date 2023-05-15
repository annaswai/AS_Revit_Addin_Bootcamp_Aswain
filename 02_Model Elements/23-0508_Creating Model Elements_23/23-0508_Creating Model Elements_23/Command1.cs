#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

#endregion

namespace _23_0508_Creating_Model_Elements_23
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

            TaskDialog.Show("Test", "I selected " + pickList.Count.ToString() + " elements");

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
                    CurveElement curveElem = elem as CurveElement;
                    //CurveElement curveElem = (CurveElement)elem;

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
                t.Start("Create Revit elements");

                // 4. create wall
                Level newLevel = Level.Create(doc, 20);
                Curve curCurve1 = modelCurves[0].GeometryCurve;

                Wall.Create(doc, curCurve1, newLevel.Id, false);

                FilteredElementCollector wallTypes = new FilteredElementCollector(doc);
                wallTypes.OfClass(typeof(WallType));

                Curve curCurve2 = modelCurves[1].GeometryCurve;

                //Call wall type method
                WallType myWallType = GetWallTypeByName(doc, "Exterior - Brick on CMU");

                Wall.Create(doc, curCurve2, myWallType.Id, newLevel.Id, 20, 0, false, false);

                // 6. get system types
                FilteredElementCollector systemCollector = new FilteredElementCollector(doc);
                systemCollector.OfClass(typeof(MEPSystemType));

                // 7. get duct system type
                MEPSystemType ductSystemType = null;
                foreach (MEPSystemType curType in systemCollector)
                {
                    if (curType.Name == "Supply Air")
                    {
                        ductSystemType = curType;
                        break;
                    }
                }

                



                // 8. get the duct type
                FilteredElementCollector collector1 = new FilteredElementCollector(doc);
                collector1.OfClass(typeof(DuctType));

                // 9.Create Duct
                Curve curCurve3 = modelCurves[2].GeometryCurve;
                Duct newDuct = Duct.Create(doc, ductSystemType.Id, collector1.FirstElementId(), newLevel.Id, curCurve3.GetEndPoint(0), curCurve3.GetEndPoint(1));

                // 10. Get pipe System Type
                MEPSystemType pipeSystemType = null;
                foreach (MEPSystemType curType in systemCollector)
                {
                    if (curType.Name == "Domestic Hot Water")
                    {
                        pipeSystemType = curType;
                        break;
                    }
                }
                // 11. Get pipe type
                FilteredElementCollector collector2 = new FilteredElementCollector(doc);
                collector2.OfClass(typeof(PipeType));

                // 12. Create Pipe
                Curve curCurve4 = modelCurves[3].GeometryCurve;
                Pipe newPipe = Pipe.Create(doc, pipeSystemType.Id, collector2.FirstElementId(),
                    newLevel.Id, curCurve4.GetEndPoint(0), curCurve4.GetEndPoint(1));

                // 13. use our new methods
                string testString = MyFirstMethod();
                MySecondMethod();
                string testString2 = MyThirdMethod("Hello World");

                // 15. Switch statment
                int numberValue = 5;
                string numAsString = " ";

                switch (numberValue)
                {
                    case 1:
                        numAsString = "One";
                        break;

                    case 2:
                        numAsString = "Two";
                        break;

                    case 3:
                        numAsString = "Three";
                        break;

                    case 4:
                        numAsString = "Four";
                        break;

                    case 5:
                        numAsString = "Five";
                        break;

                    default:
                        numAsString = "Zero";
                        break;

                }
                // 16. advanced switch statement
                Curve curve5 = modelCurves[1].GeometryCurve;
                GraphicsStyle curve5GS = modelCurves[1].LineStyle as GraphicsStyle;

                WallType wallType1 = GetWallTypeByName(doc, "Storefront");
                WallType wallType2 = GetWallTypeByName(doc, "Exterior - Brick on CMU");

                switch (curve5GS.Name)
                {
                    case "<Thin Lines>":
                        Wall.Create(doc, curve5, wallType1.Id, newLevel.Id, 20, 0, false, false);
                        break;

                    case "<Wide Lines>":
                        Wall.Create(doc, curve5, wallType1.Id, newLevel.Id, 20, 0, false, false);
                        break;

                    default:
                        Wall.Create(doc, curve5, newLevel.Id, false);
                        break;
                }

                t.Commit(); 
            }



            return Result.Succeeded;
        }

        internal string MyFirstMethod()
        {
            return "This is my First Method";
        }

        internal void MySecondMethod()
        {
            Debug.Print("This is my second method!");
        }

        internal string MyThirdMethod(string input)
        {
            return "this is mt third method!" + input;
        }

        internal WallType GetWallTypeByName(Document doc, string typeName)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(WallType));

            foreach (WallType curType in collector)
            {
                if (curType.Name == typeName)
                {
                    return curType;
                }
            }

            return null;
        }

        public static String GetMethod()
        {
            var method = MethodBase.GetCurrentMethod().DeclaringType?.FullName;
            return method;
        }
    }
}
