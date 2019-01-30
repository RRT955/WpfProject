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
    public class Trainee : ICloneable
    {

        #region Constructor

        public Trainee()
        {

        }

        public Trainee(string firstName, string lastName, DateTime dateOfBirth, Gender gender, string phoneNumber,
            Address address, CarType carType, GearboxType gearboxType, string drivingSchoolName, int numOfDrivingLessons,
            string teacherName, bool removed = false, int id = -1)
        {
            Removed = removed;
            LastName = lastName;
            FirstName = firstName;
            DateOfBirth = dateOfBirth;
            Gender = gender;
            PhoneNumber = phoneNumber;
            Address = address;
            CarType = carType;
            GearboxType = gearboxType;
            DrivingSchoolName = drivingSchoolName;
            NumberOfDrivingLessons = numOfDrivingLessons;
            TeacherName = teacherName;
            Id = id; 
        }

        #endregion

        #region Properties

        // Custom fields
        public int Age { get { return (DateTime.Now.Year - DateOfBirth.Year); } }
        public bool Removed { get; set; }

        // Required fields
        public int Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public string PhoneNumber { get; set; }
        public Address Address { get; set; }
        public CarType CarType { get; set; }
        public GearboxType GearboxType { get; set; }
        public string DrivingSchoolName { get; set; }
        public int NumberOfDrivingLessons { get; set; }
        public string TeacherName { get; set; }

        #endregion

        #region Overloads & Implementations

        public object Clone()
        {
            Trainee trainee = new Trainee(FirstName, LastName, DateOfBirth, Gender, PhoneNumber, Address, CarType, GearboxType, DrivingSchoolName, NumberOfDrivingLessons, TeacherName, Removed);
            trainee.Id = Id;
            return trainee;
        }

        public override string ToString()
        {
            return "Trainee name: " + FirstName + " " + LastName + "\nPhone number: " + PhoneNumber + "\nTeacher Name: " + TeacherName;
        }

        #endregion
    }
}
