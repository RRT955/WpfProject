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
    public class Test : ICloneable
    {

        #region Constructor

        public Test() { }

        public Test(CarType carType, GearboxType gearbox, int traineeId, int testerId, DateTime dateOfTest, bool removed = false, int id = -1, bool passed = false, bool finished = false)
        {
            CarType = carType;
            TraineeId = traineeId;
            TesterId = testerId;
            DateOfTest = dateOfTest;
            GearboxType = gearbox;
            Removed = removed;
            TestId = id;
            CriteriaOfTheTest = new List<CriteriaOfTheTest>();
            Passed = passed;
            Finished = finished;
        }

        public Test(Test test)
        {
            CarType = test.CarType;
            TraineeId = test.TraineeId;
            TesterId = test.TesterId;
            DateOfTest = test.DateOfTest;
            GearboxType = test.GearboxType;
            Removed = test.Removed;
            TestDepartureAddress = test.TestDepartureAddress;
            CriteriaOfTheTest = test.CriteriaOfTheTest;
            Passed = test.Passed;
            Finished = test.Finished;
            TestId = test.TestId;
        }

        #endregion

        #region Properties

        // Custom fields
        public CarType CarType { get; set; }
        public GearboxType GearboxType { get; set; }
        public bool Finished { get; set; }
        public bool Removed { get; set; }

        // Required fields
        public int TestId { get; set; }
        public int TraineeId { get; set; }
        public int TesterId { get; set; }
        public DateTime DateOfTest { get; set; }
        public Address TestDepartureAddress { get; set; }
        public List<CriteriaOfTheTest> CriteriaOfTheTest { get; set; }
        public bool Passed { get; set; }

        #endregion

        #region Overloads & Implementations

        private string GetTestCode()
        {
            string testId = "";
            for (int i = TestId.ToString().Length; i < 8; i++)
                testId += "0";
            testId += TestId.ToString();
            return testId;
        }

        public override string ToString()
        {
            return "Test number: " + GetTestCode() + "\nTester Id: " + TesterId + "\nTrainee Id: " + TraineeId + "\nTest Date: " + DateOfTest.ToLongDateString() + ", " + DateOfTest.Hour.ToString() + ":00";
        }

        public object Clone()
        {
            Test test = new Test(CarType, GearboxType, TraineeId, TesterId, DateOfTest, Removed, TestId, Passed, Finished);
            test.CriteriaOfTheTest = CriteriaOfTheTest;
            test.TestDepartureAddress = TestDepartureAddress;
            return test;
        }

        #endregion

    }
}
