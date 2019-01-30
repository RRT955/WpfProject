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
    public class Tester : ICloneable
    {

        #region Constructor

        public Tester(string firstName, string lastName, DateTime dateOfBirth, Gender gender, string phoneNumber, Address address,
            int yearsOfExperience, int maximumPossibleWeeklyTests, CarType carType, bool[,] schedule, int maxDistanceOfTestLocation, bool active,
            DateTime dateOfResignation, int id = -1)
        {
            FirstName = firstName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
            Gender = gender;
            PhoneNumber = phoneNumber;
            Address = address;
            YearsOfExperience = yearsOfExperience;
            MaximumPossibleWeeklyTests = maximumPossibleWeeklyTests;
            CarType = carType;
            WorkSchedule = schedule;
            MaxDistanceOfTestLocation = maxDistanceOfTestLocation;
            NotAvailableAtTimes = new List<DateTime>();
            DateOfResignation = dateOfResignation;
            Id = id;
            Active = active; // THESE FIELDS NEED TO BE PASSED BY THE CONSTRUCTOR, IF I SET IT HERE TO TRUE, 
                             // EVERY TIME I WILL SEND A TESTER TO A FUNCTION THIS PROPERTY WILL BE TRUE EVEN IF I CHANGED IT TO FALSE BEFORE.
        }

        #endregion

        #region Properties

        // Custom fields
        public int Age { get { return (DateTime.Now.Year - DateOfBirth.Year); } }

        // Required fields
        public DateTime DateOfResignation { get; set; } // תאריך ההתפטרות, אם הייתה כזו
        public bool Active { get; set; }
        public int Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public string PhoneNumber { get; set; }
        public Address Address { get; set; }
        public int YearsOfExperience { get; set; }
        public int MaximumPossibleWeeklyTests { get; set; }
        public CarType CarType { get; set; }
        public bool[,] WorkSchedule { get; set; } // constant work schedule for all weeks
        public int MaxDistanceOfTestLocation { get; set; }
        public List<DateTime> NotAvailableAtTimes { get; set; } // containes all the times (in rounded hours) denoting when the tester is boosy with tests

        #endregion

        #region Overloads & Implementations

        public object Clone()
        {
            Tester tester = new Tester(FirstName, LastName, DateOfBirth, Gender, PhoneNumber, Address, YearsOfExperience, MaximumPossibleWeeklyTests, CarType, WorkSchedule, MaxDistanceOfTestLocation, Active, DateOfResignation);
            tester.Id = Id;
            tester.NotAvailableAtTimes = NotAvailableAtTimes;
            return tester;
        }

        public override string ToString()
        {
            return "Tester name: " + FirstName + " " + LastName + "\nPhone number: " + PhoneNumber + "\nYears Of Experience: " + YearsOfExperience.ToString();
        }

        #endregion

    }
}
