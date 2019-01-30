using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE;
using DS;

// שם: דניאל רוטנמר
// ת.ז: 316240837
// שותף: רפאל לרמן
// פלאפון: 0583266859
// מרצה: יאיר גולדשטיין
// קורס: מיני פרוייקט במערכות חלונות

namespace DAL
{
    public class Dal_imp : Idal
    {

        #region Singleton

        public static Dal_imp Instance { get; } = new Dal_imp(); // auto property, i have the latest version of C# 2017, it forces me to use the new syntax.

        static Dal_imp() { }

        #endregion

        #region Idal Implementation        

        /// <summary>
        /// Adds a new test to the data source (If satisfied all conditions)
        /// </summary>
        /// <param name="test"></param>
        public void AddTest(Test test)
        {
            // GETTING THE TRAINEE
            var trainee = DataSource.Trainees.FirstOrDefault(t => t.Id == test.TraineeId);
            if (trainee == null)
                throw new Exception("The specified trainee not found in the system.");

            if ((Math.Abs((test.DateOfTest - GetLastTestDateForSpecificTrainee(trainee)).Days) < Configuration.MinimumIntervalOfDaysBetween2Tests) || (trainee.NumberOfDrivingLessons < Configuration.MinimumNumberOfLessons))
            {
                string exceptionMessage = "";
                if (Math.Abs((test.DateOfTest - GetLastTestDateForSpecificTrainee(trainee)).Days) < Configuration.MinimumIntervalOfDaysBetween2Tests)
                    exceptionMessage += "The minimum interval of time between 2 tests is 7 days.";
                if (trainee.NumberOfDrivingLessons < Configuration.MinimumNumberOfLessons)
                    exceptionMessage += "A trainee must must have record of at least 20 driving lessons to get a test.";
                throw new Exception(exceptionMessage);
            }

            var futureTests = from t in DataSource.Tests where (t.TraineeId == test.TraineeId) && (Math.Abs((test.DateOfTest - t.DateOfTest).Days) < Configuration.MinimumIntervalOfDaysBetween2Tests) select t;
            if (futureTests.Count() > 0)
                throw new Exception("This trainee have a test fixed for " + futureTests.ElementAt(0).DateOfTest.ToLongDateString() + ", The interval ofdays between 2 tests is at least " + Configuration.MinimumIntervalOfDaysBetween2Tests.ToString() + " Days.");

            var futureTestsWithTheSameCarType = from t in futureTests where t.CarType == test.CarType select t;
            if (futureTestsWithTheSameCarType.Count() > 0)
                throw new Exception("This trainee have a test with the same car type fixed for " + futureTestsWithTheSameCarType.ElementAt(0).DateOfTest.ToLongDateString() + ".");

            // GETTING THE TESTER
            var tester = DataSource.Testers.FirstOrDefault(t => (t.Id == test.TesterId && t.Active));
            if (tester == null)
                throw new Exception("The specified tester not found in the system.");
            if (!tester.Active)
                throw new Exception("The specified tester is no longer active in the system.");
            if (tester.MaximumPossibleWeeklyTests < GetNumberOfWeeklyTestsForSpecificTester(tester))
                throw new Exception("The specified tester reached the number of tests for this week.");

            if (!TesterAvailableAt(test.DateOfTest, tester))
            {
                string message = "The specified tester is not available at: " + test.DateOfTest.ToLongDateString() + ".";
                DateTime availableDate = test.DateOfTest;
                bool available = false;
                while (!available)
                {
                    if (availableDate.Hour < 15)
                    {
                        if (TesterAvailableAt(availableDate, tester))
                        {
                            futureTests = from t in DataSource.Tests where (t.TraineeId == test.TraineeId) && (Math.Abs((availableDate - t.DateOfTest).Days) < Configuration.MinimumIntervalOfDaysBetween2Tests) select t;
                            if (futureTests.Count() == 0)
                            {
                                message += "\nAlternative time for test is: " + availableDate.ToLongDateString() + ", " + availableDate.Hour.ToString() + ":00.";
                                available = true;
                            }
                        }
                        availableDate = availableDate.AddHours(1);
                    }
                    else
                    {
                        if ((int)availableDate.DayOfWeek == 4)
                            availableDate = availableDate.AddDays(3);
                        availableDate = availableDate.AddHours(-6);
                    }
                }
                throw new Exception(message);
            }
            if (!TraineeAvailableForTestAt(test.DateOfTest, trainee))
                throw new Exception("The specified trainee is not available at: " + test.DateOfTest.DayOfWeek.ToString() + ", " + test.DateOfTest.Hour.ToString() + ".");
            if (tester.CarType != trainee.CarType)
                throw new Exception("The tester car type is not the same as the trainee.");
            if (trainee.CarType != test.CarType)
                throw new Exception("The trainee car type is not the same as the test car type.");

            if (SuccesfullyPassedTestWithSpecifiedCarTypeAndGearBoxType(trainee))
                throw new Exception("The specified trainee already successfully passed a test with this car type and gear box type.");

            test.TestId = Configuration.GetTestId();
            DataSource.Tests.Add(test);
            
            tester.NotAvailableAtTimes.Add(test.DateOfTest);
            UpdateTester(tester);
        }

        // Returns all the active testers in the system.
        public List<Tester> GetTesters()
        {
            var testersList = DataSource.Testers.Clone().ToList();
            return (from tester in testersList where tester.Active select tester).ToList(); // copy is returned
        }

        // Returns all the retired testers in the system.
        public List<Tester> GetRetiredTesters()
        {
            var testersList = DataSource.Testers.Clone().ToList();
            return (from tester in testersList where !tester.Active select tester).ToList(); // copy is returned
        }

        // Returns all the removed tests in the system.
        public List<Test> GetRemovedTests()
        {
            var testsList = DataSource.Tests.Clone().ToList();
            return (from test in testsList where test.Removed select test).ToList(); // copy is returned
        }

        // Returns all the active tests in the system.
        public List<Test> GetTests()
        {
            var testsList = DataSource.Tests.Clone().ToList();// po baaya
            return (from test in testsList where !test.Removed select test).ToList(); // copy is returned
        }

        // Returns all the active trainees in the system.
        public List<Trainee> GetTrainees()
        {
            var traineesList = DataSource.Trainees.Clone().ToList();
            return (from trainee in traineesList where !trainee.Removed select trainee).ToList(); // copy is returned
        }

        // Returns all the removed trainees in the system.
        public List<Trainee> GetRemovedTrainees()
        {
            var traineesList = DataSource.Trainees.Clone().ToList();
            return (from trainee in traineesList where trainee.Removed select trainee).ToList(); // copy is returned
        }

        /// <summary>
        /// Adds a new tester to the data source (If satisfied all conditions)
        /// </summary>
        /// <param name="tester"></param>
        public void AddTester(Tester tester)
        {
            tester.Id = Configuration.GetTesterId();
            if (tester.Age > Configuration.MinimumTesterAge)
                DataSource.Testers.Add(tester);
            else
                throw new Exception("Can't add a tester whose age < 40.");
        }

        /// <summary>
        /// Adds a new trainee to the data source (If satisfied all conditions)
        /// </summary>
        /// <param name="trainee"></param>
        public void AddTrainee(Trainee trainee)
        {
            trainee.Id = Configuration.GetTraineeId();
            if (trainee.Age > Configuration.MinimumTraineeAge)
                DataSource.Trainees.Add(trainee);
            else
                throw new Exception("Can't add a trainee whose age < 18.");
        }

        /// <summary>
        /// Retires a tester but not removes him
        /// </summary>
        /// <param name="tester"></param>
        public void RemoveTester(Tester tester)
        {
            var found = from t in GetTesters() where t.Id == tester.Id select t;
            if (found.Count() > 0 && TesterCanRetire(tester))
            {
                tester.Active = false;
                tester.DateOfResignation = DateTime.Now;
                UpdateTester(tester);
            }
            else
            {
                if (!TesterCanRetire(tester))
                    throw new Exception("The specified tester have some future tests to perform.");
                else
                    throw new Exception("The specified tester doesn't exist in the system.");
            }
        }

        /// <summary>
        /// Removes a test but not removes him
        /// </summary>
        /// <param name="tester"></param>
        public void RemoveTest(Test test)
        {
            var found = from t in GetTests() where t.TestId == test.TestId select t;
            if (found.Count() > 0)
            {
                if (test.Finished == false)
                    throw new Exception("The specified test can't be removed because it is not finished yet.");
                test.Removed = true;
                UpdateTest(test);
            }
            else
                throw new Exception("The specified test doesn't exist in the system.");
        }

        /// <summary>
        /// Removes a trainee from the data source
        /// </summary>
        /// <param name="trainee"></param>
        public void RemoveTrainee(Trainee trainee)
        {
            var found = from t in DataSource.Trainees where t.Id == trainee.Id select t;
            if (found.Count() == 0)
                throw new Exception("The specified trainee doesn't exist in the system.");
            trainee.Removed = true;
            UpdateTrainee(trainee);
        }

        /// <summary>
        /// Updates a test in the data source (Usually when finished) through the data access layer
        /// this function enables to update a test that will be executed in the future, Ex: update date etc...
        /// </summary>
        /// <param name="test"></param>
        public void UpdateTest(Test test)
        {
            var found = DataSource.Tests.FirstOrDefault(t => t.TestId == test.TestId);
            if (found == null)
                throw new Exception("The specified test doesn't exist in the system.");
            if (found.Finished)
                throw new Exception("Can't update a test that was finished in the past.");
            if (test.Finished)
            {
                if (test.CriteriaOfTheTest.Count == 0)
                    throw new Exception("The criteria list of this test is empty, Can't update!");
                int notPassed = (from criteria in test.CriteriaOfTheTest where !criteria.SuccessfullyPassed select criteria).Count();
                if (notPassed > (test.CriteriaOfTheTest.Count / 2) && test.Passed)
                    throw new Exception("Not possible to pass the test while most of the criteria not passed!");
            }
            else
            {
                if (test.Passed || test.CriteriaOfTheTest.Count > 0)
                    throw new Exception("The specified test is not finished and has a grade.");
            }
            DataSource.Tests[DataSource.Tests.FindIndex(t => t.TestId == test.TestId)] = test;
        }

        /// <summary>
        /// Updates a tester in the data source
        /// </summary>
        /// <param name="tester"></param>
        public void UpdateTester(Tester tester)
        {
            var found = from t in DataSource.Testers where t.Id == tester.Id select t;
            if (found.Count() == 0)
                throw new Exception("The specified tester doesn't exist in the system.");
            DataSource.Testers[DataSource.Testers.FindIndex(t => t.Id == tester.Id)] = tester;
        }

        /// <summary>
        /// Updates a trainee in the data source
        /// </summary>
        /// <param name="trainee"></param>
        public void UpdateTrainee(Trainee trainee)
        {
            var found = from t in DataSource.Trainees where t.Id == trainee.Id select t;
            if (found.Count() == 0)
                throw new Exception("The specified trainee doesn't exist in the system.");
            DataSource.Trainees[DataSource.Trainees.FindIndex(t => t.Id == trainee.Id)] = trainee;
        }

        #endregion

        #region Methods

        public DateTime GetLastTestDateForSpecificTrainee(Trainee trainee)
        {
            var tests = from t in DataSource.Tests where (t.TraineeId == trainee.Id && t.DateOfTest < DateTime.Now) orderby t.DateOfTest descending select t;
            if (tests.Count() > 0)
                return tests.ElementAt(0).DateOfTest;
            else
                return DateTime.MinValue;
        }

        public bool SuccesfullyPassedTestWithSpecifiedCarTypeAndGearBoxType(Trainee trainee)
        {
            var passed = from test in DataSource.Tests where (test.TraineeId == trainee.Id && test.Passed && test.CarType == trainee.CarType && test.GearboxType == trainee.GearboxType) select test;
            return (passed.Count() > 0) ? true : false;
        }

        public IEnumerable<Tester> GetAllAvailableTestersForSpecificDateTime(DateTime dateTime)
        {
            var availableTesters = from tester in DataSource.Testers where TesterAvailableAt(dateTime, tester) select tester;
            return availableTesters;
        }

        public bool TesterAvailableAt(DateTime dateTime, Tester tester)
        {
            int dayOfWeek = (int)dateTime.DayOfWeek;
            int hour = dateTime.Hour;
            if (hour >= 9 && hour <= 14 && (dayOfWeek) >= 0 && (dayOfWeek) <= 4)
            {
                if (tester.WorkSchedule[hour - 9, dayOfWeek] == false)
                    return false;
            }
            else
                return false;
            if (tester.NotAvailableAtTimes != null && tester.NotAvailableAtTimes.Count > 0)
            {
                var dt = from date in tester.NotAvailableAtTimes where date == dateTime select date;
                return (dt.Count() > 0) ? false : true;
            }
            return true;
        }

        public int GetNumberOfTestsDoneBySpecificTrainee(Trainee trainee)
        {
            var tests = from test in DataSource.Tests where (test.TraineeId == trainee.Id && test.DateOfTest < DateTime.Now) select test;
            return tests.Count();
        }

        public int GetNumberOfWeeklyTestsForSpecificTester(Tester tester)
        {
            var tests = from test in DataSource.Tests where (int)(test.DateOfTest.DayOfWeek) <= 4 && (test.TesterId == tester.Id) select test;
            return tests.Count();
        }

        public bool TraineeAvailableForTestAt(DateTime dateTime, Trainee trainee)
        {
            var available = from test in DataSource.Tests where (test.TraineeId == trainee.Id && test.DateOfTest == dateTime) select test;
            return (available.Count() > 0) ? false : true;
        }

        public IEnumerable<Test> GetListOfFutureTestsForSpecificTrainee(Trainee trainee)
        {
            var tests = from t in DataSource.Tests where (t.TraineeId == trainee.Id && t.DateOfTest > DateTime.Now) select t;
            return tests;
        }

        public bool TesterCanRetire(Tester tester)
        {
            var ans = from t in DataSource.Tests where (t.TesterId == tester.Id && t.DateOfTest > DateTime.Now) select t;
            return (ans.Count() == 0);
        }

        #endregion
    }
}
