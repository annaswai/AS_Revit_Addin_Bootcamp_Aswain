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

namespace _2023_CSV_Bonus_Lesson
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
            // 1. declare variables
            string levelPath = "C:\\GitHub_Repositories\\ArchSmarter\\ArchSmarter_Revit_Addin_Bootcamp\\01_Skills\\Referance Files\\Session_03_Challenge.xlsx";

            // 2. Create a list of string arrays for CSV data
            List<string[]> levelData = new List<string[]>();

            // 3. read test file datas
            string[] levelArray = System.IO.File.ReadAllLines(levelPath);

            // 4. Loop through data file and put into list
            foreach (string levelString in levelArray)
            {
                string[] rowArray = levelString.Split(',');
                levelData.Add(rowArray);
            }

            // 5. Remove Header Row
            levelData.RemoveAt(0);

            // 6. a transaction
            Transaction t = new Transaction(doc);
            t.Start("Create Levels");

            // 7. loop through level data
            int counter = 0;
            foreach (string[] current )

            return Result.Succeeded;
        }

        public static String GetMethod()
        {
            var method = MethodBase.GetCurrentMethod().DeclaringType?.FullName;
            return method;
        }
    }
}
