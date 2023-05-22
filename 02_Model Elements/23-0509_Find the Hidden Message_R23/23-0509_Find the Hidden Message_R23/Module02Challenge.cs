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
using System.Security.Cryptography;
using System.Windows.Media;

#endregion

namespace _23_0509_Find_the_Hidden_Message_R23
{
    [Transaction(TransactionMode.Manual)]
    public class Module02Challenge : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // this is a variable for the Revit application
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;

            // this is a variable for the current Revit model
            Document doc = uiapp.ActiveUIDocument.Document;

            // Your code goes here

            // 1. pick elements and filter them in to a list
            TaskDialog.Show("Select Lines", "Select some lines to convert into Revit elements.");
            IList<Element> pickList = uidoc.Selection.PickElementsByRectangle("Select elements");

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
            TaskDialog.Show("Selection", $"You selected {modelCurves.Count} Curves");

            // 3.1 Get active View level 
            Parameter levelParam = doc.ActiveView.LookupParameter("Associated Level");

            // 3. Get Level and Various types
            Level currentLevel = GetLevelByName(doc, levelParam.AsString());

            // 4. Get types
            WallType wt1 = GetWallTypeByName(doc, "Storefront");
            WallType wt2 = GetWallTypeByName(doc, "Generic - 8\"");

            MEPSystemType ductSystemType = GetMEPSystemTypeByName(doc, "Supply Air");
            DuctType ductType = GetDuctTypeByName(doc, "Default");

            MEPSystemType pipeSystemType = GetMEPSystemTypeByName(doc, "Domestic Hot Water");
            PipeType pipeType = GetPipeTypeByName(doc, "Default");

            List<ElementId> linesToHide = new List<ElementId>();

            // 5. loop through the curve elements and get data
            using (Transaction t = new Transaction(doc))
            {
                t.Start("Create Revit Elements");
                foreach (CurveElement currentCurve in modelCurves)
                {
                    // 6. Get GraphicStyle and Curve for each CurveElement
                    Curve elementCurve = currentCurve.GeometryCurve;
                    GraphicsStyle currentStyle = currentCurve.LineStyle as GraphicsStyle;

                    // Check for Circles
                    if (elementCurve is Arc)
                    {
                        linesToHide.Add(currentCurve.Id);
                        continue;
                    }

                   
                    // 7. Get start and end points
                    XYZ startPoint = elementCurve.GetEndPoint(0);
                    XYZ endPoint = elementCurve.GetEndPoint(1);

                    // 9. Use Switch statment to create walls, ducts, and pipes
                    switch (currentStyle.Name)
                    {
                        case "A-GLAZ":
                            Wall currentWall = Wall.Create(doc, elementCurve, wt1.Id, currentLevel.Id, 20, 0, false, false);
                            break;

                        case "A-WALL":
                            Wall currentWall2 = Wall.Create(doc, elementCurve, wt2.Id, currentLevel.Id, 20, 0, false, false);
                            break;

                        case "M-DUCT":
                            Duct currentDuct = Duct.Create(doc, ductSystemType.Id, ductType.Id, currentLevel.Id, startPoint, endPoint);
                            break;

                        case "P-PIPE":
                            Pipe currentPipe = Pipe.Create(doc, pipeSystemType.Id, pipeType.Id, currentLevel.Id, startPoint, endPoint);
                            break;

                        default:
                            linesToHide.Add(currentCurve.Id);
                            break;
                    }

                }

                doc.ActiveView.HideElements(linesToHide);

                t.Commit();

            }
                

            
            // 6. get system types
            FilteredElementCollector systemCollector = new FilteredElementCollector(doc);
            systemCollector.OfClass(typeof(MEPSystemType));

           
            

            return Result.Succeeded;
        }

        private PipeType GetPipeTypeByName(Document doc, string typeName)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(PipeType));

            foreach (PipeType curType in collector)
            {
                if (curType.Name == typeName)
                    return curType;

            }
            return null;
        }

        private DuctType GetDuctTypeByName(Document doc, string typeName)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(DuctType));

            foreach (DuctType curType in collector)
            {
                if (curType.Name == typeName)
                    return curType;

            }
            return null;
        }

        private MEPSystemType GetMEPSystemTypeByName(Document doc, string typeName)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(MEPSystemType));

            foreach (MEPSystemType curType in collector)
            {
                if (curType.Name == typeName)
                    return curType;

            }
            return null;
        }

        private WallType GetWallTypeByName(Document doc, string typeName)
        {
            {
                FilteredElementCollector collector = new FilteredElementCollector(doc);
                collector.OfClass(typeof(WallType));

                foreach (WallType curType in collector)
                {
                    if (curType.Name == typeName)
                        return curType;

                }
                return null;
            }
        }

        private Level GetLevelByName(Document doc, string levelName)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc); 
            collector.OfClass(typeof(Level));

            foreach(Level curLevel in collector)
            {
                if(curLevel.Name == levelName)
                    return curLevel;

            }
            return null;
        }

        //Get Curve Data fuction
        /*    internal CurveElement GetCurveData(Document doc, CurveElement currentCurve)
            {
                Curve curve = currentCurve.GeometryCurve;
                XYZ startPoint = curve.GetEndPoint(0);
                XYZ endPoint = curve.GetEndPoint(1);

                GraphicsStyle curStyle = currentCurve.LineStyle as GraphicsStyle;

                Debug.Print(curStyle.Name);
                return null;
            }
*/

        public static String GetMethod()
        {
            var method = MethodBase.GetCurrentMethod().DeclaringType?.FullName;
            return method;
        }
    }
}
