#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
//Add stucture
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

#endregion

namespace Mod_03_Families
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
            //2. create instance of class - v1
            Building theater = new Building("Grand Opera House", "5 Main Street", 4, 35000);
            Building hotel = new Building("Fancy Hotel", "10 Main Street", 10, 100000);
            Building office = new Building("Big Fancy Office Building", "15 Main Street", 15, 1500000);

            
            // 3. Create last of buildings
            List<Building> buildingList = new List<Building>();
            buildingList.Add(theater);
            buildingList.Add(hotel);
            buildingList.Add(office);
            buildingList.Add(new Building("hospital", "20 Main Street", 20, 350000));

            // 6. Create instance of class and use method
            Neighborhood downtown = new Neighborhood("Downtown", "Middletown", "CT", buildingList);
            TaskDialog.Show("Test", $"There are {downtown.GetBuildingCount()} buildings in the {downtown.Name} neighborhood");

            // 7. Working with rooms
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfCategory(BuiltInCategory.OST_Rooms);

            // 8. Insert Family
            FamilySymbol curFS = GetFamilySymbolByName(doc, "Desk", "60\" x 30\"");

            using (Transaction t = new Transaction(doc))
            {
                t.Start("Insert Family into room");
                
                // 9.Activate Family Symbol
                curFS.Activate();
                
                // 7.1 Working with rooms (getting the location of the room)
                foreach (SpatialElement room in collector)
                {
                    LocationPoint loc = room.Location as LocationPoint;
                    XYZ roomPoint = loc.Point as XYZ;

                    FamilyInstance curFT = doc.Create.NewFamilyInstance(roomPoint, curFS, StructuralType.NonStructural);
                }
                t.Commit(); 

            }



            


            return Result.Succeeded;
        }
        internal FamilySymbol GetFamilySymbolByName(Document doc, string famName, string fsName)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(FamilySymbol));

            foreach (FamilySymbol fs in collector)
            {
                if (fs.Name == fsName && fs.FamilyName == famName)
                    return fs;
            }

            return null;
        }


        public static String GetMethod()
        {
            var method = MethodBase.GetCurrentMethod().DeclaringType?.FullName;
            return method;
        }
    }

    public class Building
    {
        public string Name { get; set; }

        public string Address { get; set; }

        public int NumFloors { get; set; }

        public double Area { get; set; }

        //3. Add constuctor class
        public Building(string _name, string _address, int _numFloors, double _area)
        {
            Name = _name;
            Address = _address;
            NumFloors = _numFloors;
            Area = _area;
        }

    }

    // 4. define dynamic class #2
    public class Neighborhood
    {
        public string Name { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public List<Building> BuildingList { get; set; }

        public Neighborhood (string _name, string _city, string _state, List<Building> _buildings)
        {
            Name = _name;
            City = _city;
            State = _state;
            BuildingList = _buildings;
        }

        // 5. Add method to class
        public int GetBuildingCount()
        {
            return BuildingList.Count;
        }
    }
    

    

}
