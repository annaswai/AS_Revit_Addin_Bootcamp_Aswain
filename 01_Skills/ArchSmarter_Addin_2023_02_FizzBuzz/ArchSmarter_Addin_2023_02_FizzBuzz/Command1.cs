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
            // int elevation = 0; (created in the loop instead)  // 2. Declare a starting elevation variable and set it to 0
            int floorHeight = 15;       // 3. Declare a floor height variable and set it to 15 

            //create a trasaction to lock the model
            Transaction t = new Transaction(doc);
            t.Start("Doing somthing in Revit");

            // 4. Loop through the number 1 to the number variable
            for (int elevation = 0; elevation <= numberVariable; elevation++) 
            // 5. Create a level for each number
            {
                // 6. After creating the level, increment the current elevation by the floor height value
                //create a floor level - show in Revit API (www.revitapidocs.com)
                Level newLevel = Level.Create(doc, elevation * floorHeight);

                // 9. If the number is divisible by both 3 and 5, create a sheet and name it "FIZZBUZZ_#"
                if (elevation % 3 == 0 && elevation % 5 == 0)
                {
                    newLevel.Name = "FIZZBUZZ_" + elevation.ToString();
                }
                
                // 7. If the number is divisible by 3, create a floor plan and name it "FIZZ_#"
                else if (elevation % 3 == 0)
                {
                    newLevel.Name = "FIZZ_" + elevation.ToString();
                }

                // 8. If the number is divisible by 5, create a ceiling plan and name it "BUZZ_#"
                else if (elevation % 5 == 0)
                {
                    newLevel.Name = "BUZZ_" + elevation.ToString();
                }

                else
                {
                    newLevel.Name = "Plain Old Level " + elevation.ToString();
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
