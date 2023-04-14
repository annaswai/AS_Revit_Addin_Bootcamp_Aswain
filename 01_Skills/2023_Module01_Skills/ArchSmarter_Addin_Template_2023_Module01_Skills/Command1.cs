#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
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
            dou

            return Result.Succeeded;
        }

        public static String GetMethod()
        {
            var method = MethodBase.GetCurrentMethod().DeclaringType?.FullName;
            return method;
        }
    }
}
