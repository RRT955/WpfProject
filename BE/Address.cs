using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// שם: דניאל רוטנמר
// ת.ז: 316240837
// שותף: רפאל לרמן
// פלאפון: 0583266859
// מרצה: יאיר גולדשטיין
// קורס: מיני פרוייקט במערכות חלונות

namespace BE
{
    public class Address
    {
        public string Street
        {
            get; set;
        }

        public string City
        {
            get; set;
        }

        public string Country
        {
            get; set;
        }

        public int BuildingNumber
        {
            get; set;
        }

        public Address(string _country, string _city, string _street, int _buildingNumber)
        {
            Country = _country;
            Street = _street;
            City = _city;
            BuildingNumber = _buildingNumber;
        }

        public Address() { Country = ""; City = ""; Street = ""; BuildingNumber = 0; } // to enable serialization

        // IN ORDER TO BIND THE ADDRESS OBJECT TO THE DATAGRID WE NEED TO COVER ITS PROPERTIES.
        // THIS PROPERTY WILL BE USED TO BIND THE ADDRESS DATA MEMBERS TO THE DATAGRID, FUNCTIONS LIKE 'ToString()' AREN'T BINDABLE.
        public string AddressAsString
        {
            get
            { 
                if (Country == null)
                    return "";
                return Country + ", " + City + ", " + Street + ", " + BuildingNumber;
            }
        }
    }
}
