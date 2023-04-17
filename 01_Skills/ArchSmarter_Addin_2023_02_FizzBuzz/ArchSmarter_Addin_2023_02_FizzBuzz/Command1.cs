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

namespace ArchSmarter_Addin_2023_02_FizzBuzz
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

            int numberVariable = 250;   // 1. Declare a number variable and set it to 250
            // int levelNumber = 0; (created in the loop instead)  // 2. Declare a starting elevation (levelNumber) variable and set it to 0
            int floorHeight = 15;       // 3. Declare a floor height variable and set it to 15 

            //create a trasaction to lock the model
            Transaction t = new Transaction(doc);
            t.Start("Doing somthing in Revit");

            // Creating a filtered element colector to grab the TB in order to make new sheets
            FilteredElementCollector collectorSheets = new FilteredElementCollector(doc);
            collectorSheets.OfCategory(BuiltInCategory.OST_TitleBlocks);

            // Creating a filtered element colector to grab the View Family Type in order to make new Floor Plans and RCPs
            FilteredElementCollector collectorFloorPlan = new FilteredElementCollector(doc);
            collectorFloorPlan.OfClass(typeof(ViewFamilyType));


            // 4. Loop through the number 1 to the number variable
            for (int levelNumber = 0; levelNumber <= numberVariable; levelNumber++) 

            // 5. Create a level for each number
            {
                // 6. After creating the level, increment the current elevation by the floor height value
                //create a floor level - show in Revit API (www.revitapidocs.com)
                Level newLevel = Level.Create(doc, levelNumber * floorHeight);
                newLevel.Name = "Level_" + levelNumber.ToString();

                // 9. If the number is divisible by both 3 and 5... "
                if (levelNumber % 3 == 0 && levelNumber % 5 == 0)
                {

                    // 9.1 "...create a sheet and name it "FIZZBUZZ_#"


                    //Creating Sheet
                    ViewSheet newSheet = ViewSheet.Create(doc, collectorSheets.FirstElementId());
                    newSheet.Name = "FIZZBUZZ_" + levelNumber.ToString();
                    newSheet.SheetNumber = "A-" + levelNumber.ToString();
                }
                
                // 7. If the number is divisible by 3..."
                else if (levelNumber % 3 == 0)
                {
                    // "...create a floor plan and name it "FIZZ_#"

                   

                    // Get floor plan view family type
                    ViewFamilyType floorPlanVTF = null;
                    foreach (ViewFamilyType curVFT in collectorFloorPlan)
                    {
                        if (curVFT.ViewFamily == ViewFamily.FloorPlan)
                        {
                            floorPlanVTF = curVFT;
                            break;
                        }
                    }
                    // create a view by specifying the document, view family type, and level
                    ViewPlan newPlan = ViewPlan.Create(doc, floorPlanVTF.Id, newLevel.Id);
                    newPlan.Name = "FIZZ_" + levelNumber.ToString();

                }

                // 8. "If the number is divisible by 5..." 
                else if (levelNumber % 5 == 0)
                {
                    // "...create a ceiling plan and name it "BUZZ_#"

               
                    // Get ceiling plan view family type
                    ViewFamilyType ceilingPlanVTF = null;
                    foreach (ViewFamilyType curVFT in collectorFloorPlan)
                    {
                        if (curVFT.ViewFamily == ViewFamily.CeilingPlan)
                        {
                            ceilingPlanVTF = curVFT;
                            break;
                        }
                    }

                    //Create a ceiling plan using the ceiling plan view family type
                    ViewPlan newCeilingPlan = ViewPlan.Create(doc, ceilingPlanVTF.Id, newLevel.Id);
                    newCeilingPlan.Name = "BUZZ_" + levelNumber.ToString();

                }


            }

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
