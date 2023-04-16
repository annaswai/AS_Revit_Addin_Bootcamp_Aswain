#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

#endregion

namespace ArchSmarter_Addin_Template_2023_Module01_Skills
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

            // DataType VariableName = Value; <- Always end the line with a semicolon!

            //Create String Variables
            string text1 = "This is my text";
            string text2 = "This is my next text";

            //combine strings
            string text3 = text1 + text2;
            string text4 = text1 + " " + text2 + "abcd";

            //Create Number Variables
            int number1 = 10;
            double number2 = 20.5;

            //do some math
            double number3 = number1 + number2;
            double number4 = number3 - number2;
            double number5 = number4 / 100;
            double number6 = number5 + number4;
            double number7 = (number6 + number5) / number4;

            //convert meters to feet (revit API is always in imperal feet)
            double meters = 4;
            double metersToFeet = meters * 3.28084;

            //convert mm to feet
            double mm = 3500;
            double mmToFeet = mm / 304.8;
            double mmToFeet2 = (mm / 1000) * 3.28084;

            //find the reminder when dividing (ie. the modulo or mod)
            double remainder1 = 100 % 10; // equals 0 (100 divided by 10 = 10)
            double remainder2 = 100 % 9; // equals 1 (100 divided by 9 = 11 with a remainder of 1) 

            // increment or remove a number by 1
            number6++; // adds 1
            number6--; // removes 1
            number6 += 10; // adds 10 to "number6"'s value

            // use condintional logic to compare things
            // compare using boolean operators
            // == equals
            // != equal
            // > greater than
            // < less than
            // >= and <=  (greater than or equal to)

            // Check a value and preform a single action if true
            if (number6 > 10)
            {
                //do something if true
            }
        
            
            // Check a value and preform anction if it is true and another if it is false
            if (number5 == 100)
            {
                //do something if true
            }
            else
            {
                // do something else if false
            }


            // Check multiple values and preform a actions if true and false (it will only exicute one action in order)
            if (number6 > 10)
            {
                //do something if true
            }
            else if (number6 == 8)
            {
                // (will not happen if the first if is true) do something else if true
            }
            
            else
            {
                // do a third thing if false
            }

            // compond conditional statments 
            // look for 2 things (or more) using &&
            if (number6 > 10 && number5 == 100)
            {
                //do something true
            }

            //look for either thing using || (or)
            if (number5 == 100 || number6 == 100)
            {
                // do somthing if either is true
            }

            //create a list
            List<string> list1 = new List<string>();

            // add items to a list
            list1.Add(text1);
            list1.Add(text2);
            list1.Add("This is some text");

            // Create list and add items to it - method 2
            // (you need to use the {} when making the list, not () even though thouse are auto added)
            List<int> list2 = new List<int> { 1, 2, 3, 4, 5 };

            // loop through a list using a ForEach Loop
            int letterCounter = 0;
            foreach (string currentString  in list1)
            {
                //Do something with currentString
                //letterCounter = letterCounter + currentString.Length;
                letterCounter += currentString.Length;

            }

            // loop through a range of numbers
            int numberCount = 0;
            int counter = 100;
            for (int i = 0; i <= counter; i++)
            {
                numberCount += i;
                //do something
            }

            TaskDialog.Show( "Number Counter", "The number count is " + numberCount.ToString() );


            //create a trasaction to lock the model
            Transaction t = new Transaction(doc);
            t.Start("Doing somthing in Revit");

            //create a floor level - show in Revit API (www.revitapidocs.com)
            // elevation value is in decimal feet regardless of the model's units
            double elevation = 100;
            Level newLevel = Level.Create(doc, elevation);
            newLevel.Name = "My New Level";

            // create a floor plan view - show in Revit API
            // but first need to get a floor plan View Family Type
            // by creating a filtered element collector
            FilteredElementCollector collector1 = new FilteredElementCollector(doc);
            collector1.OfClass(typeof(ViewFamilyType));
            
            
            // Get floor plan view family type
            ViewFamilyType floorPlanVTF = null;
            foreach (ViewFamilyType curVFT in collector1)
            {
                if (curVFT.ViewFamily == ViewFamily.FloorPlan)
                {
                    floorPlanVTF = curVFT;
                    break;
                }
            }

            // create a view by specifying the document, view family type, and level
            ViewPlan newPlan = ViewPlan.Create(doc, floorPlanVTF.Id, newLevel.Id);
            newPlan.Name = "My New Floor Plan";

            // Get ceiling plan view family type
            ViewFamilyType ceilingPlanVTF = null;
            foreach (ViewFamilyType curVFT in collector1)
            {
                if (curVFT.ViewFamily == ViewFamily.CeilingPlan)
                {
                    ceilingPlanVTF = curVFT;
                    break;
                }
            }

            //Create a ceiling plan using the ceiling plan view family type
            ViewPlan newCeilingPlan = ViewPlan.Create(doc, ceilingPlanVTF.Id, newLevel.Id);
            newCeilingPlan.Name = "My New Ceiling Plan";

            // Create a sheet
            // but first need to get the title block
            // by creating a filtered element collector
            FilteredElementCollector collector2 = new FilteredElementCollector(doc);
            collector2.OfCategory(BuiltInCategory.OST_TitleBlocks); //grabs all elements in the model that have a TB

            // create a Sheet
            ViewSheet newSheet = ViewSheet.Create(doc, collector2.FirstElementId());
                newSheet.Name = "My New Sheet";
                newSheet.SheetNumber = "A-101";

            // add a view to a sheet using a Viewport - Show in API
            // first create a point
            XYZ insPoint = new XYZ(1, 0.5, 0);

            Viewport newViewport = Viewport.Create(doc, newSheet.Id, newPlan.Id, insPoint);
        


            t.Commit();
            t.Dispose();

            return Result.Succeeded;
        }

        public static String GetMethod()
        {
            var method = MethodBase.GetCurrentMethod().DeclaringType?.FullName;
            return method;
        }
    }
}
