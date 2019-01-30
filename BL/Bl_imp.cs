using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using BE;
using DAL;

// שם: דניאל רוטנמר
// ת.ז: 316240837
// שותף: רפאל לרמן
// פלאפון: 0583266859
// מרצה: יאיר גולדשטיין
// קורס: מיני פרוייקט במערכות חלונות

namespace BL
{
    public class Bl_imp : Ibl // implements the IBL interface
    {

        #region Singleton

        public static Bl_imp Instance { get; } = new Bl_imp(); // auto property, i have the latest version of C# 2017, it doesn't let me use the old syntax, it gives a warning.

        static Bl_imp()
        {
            dal = FactoryDal.GetDalXML(); // use ((Dal_imp)dal). to get access to all the functions that are uniquely declared in Dal_imp in addition to the interface implementation
        }

        #endregion

        #region Private Varialables

        private static Idal dal;

        #endregion

        #region IBL Implementation

        /// <summary>
        /// Adds a new test (If satisfied all conditions) to the data source through the data access layer
        /// </summary>
        /// <param name="test"></param>
        public void AddTest(Test test)
        {
            Trainee trainee = null;
            Tester tester = null;

            // GETTING THE TRAINEE
            trainee = dal.GetTrainees().FirstOrDefault(t => t.Id == test.TraineeId);
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

            var futureTests = from t in dal.GetTests() where (t.TraineeId == test.TraineeId) && (Math.Abs((test.DateOfTest - t.DateOfTest).Days) < Configuration.MinimumIntervalOfDaysBetween2Tests) select t;
            if (futureTests.Count() > 0)
                throw new Exception("This trainee have a test fixed for " + futureTests.ElementAt(0).DateOfTest.ToLongDateString() + ", The interval of days between 2 tests is at least " + Configuration.MinimumIntervalOfDaysBetween2Tests.ToString() + " Days.");

            var futureTestsWithTheSameCarType = from t in dal.GetTests() where (t.TraineeId == test.TraineeId && test.DateOfTest > DateTime.Now) where t.CarType == test.CarType select t;
            if (futureTestsWithTheSameCarType.Count() > 0)
                throw new Exception("This trainee have a test with the same car type fixed for " + futureTestsWithTheSameCarType.ElementAt(0).DateOfTest.ToLongDateString() + ".");

            // GETTING THE TESTER
            tester = dal.GetTesters().FirstOrDefault(t => (t.Id == test.TesterId && t.Active));
            if (tester == null)
                throw new Exception("The specified tester not found in the system.");
            if (!tester.Active)
                throw new Exception("The specified tester is no longer active in the system.");
            if (tester.MaximumPossibleWeeklyTests < GetNumberOfWeeklyTestsForSpecificTester(tester))
                throw new Exception("The specified tester reached the number of tests for this week.");
                        
            // thread
            //BackgroundWorker bw = new BackgroundWorker();
            //bw.DoWork += (boj, ea) => RequestDistance(tester, test.TestDepartureAddress);
            //bw.RunWorkerAsync();

            if (RequestDistanceBool(tester, test.TestDepartureAddress))
            {
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
                                futureTests = from t in dal.GetTests() where (t.TraineeId == test.TraineeId) && (Math.Abs((availableDate - t.DateOfTest).Days) < Configuration.MinimumIntervalOfDaysBetween2Tests) select t;
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
                            {
                                availableDate = availableDate.AddDays(3);
                                availableDate = availableDate.AddHours(-6);
                            }
                            else
                                availableDate = availableDate.AddHours(18);
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
                    throw new Exception("The specified trainee already successfully passed a test with this type of car and gear box type.");

                dal.AddTest(test);
            }            
        }

        /*private void RequestDistance(Tester tester, Address testDepartureAddress)
        {
            string origin = tester.Address.Street + " " + tester.Address.BuildingNumber.ToString() + " " + tester.Address.City + " " + tester.Address.Country;
            string destination = testDepartureAddress.Street + " " + testDepartureAddress.BuildingNumber.ToString() + " " + testDepartureAddress.City + " " + testDepartureAddress.Country;
            string key = @"UU3jYL5u5Ltj0phASFKGnGF8AEMnVb1X";

            string url = @"https://www.mapquestapi.com/directions/v2/route?key=" + key + @"&from=" + origin + @"&to=" + destination + @"&outFormat=xml&ambiguities=ignore&routeType=fastest&doReverseGeocode=false&enhancedNarrative=false&avoidTimedConditions=false";

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            WebResponse resp = req.GetResponse();
            Stream s = resp.GetResponseStream();
            StreamReader sr = new StreamReader(s);
            string rawResponse = sr.ReadToEnd();
            resp.Close();

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(rawResponse);

            if (xml.GetElementsByTagName("statusCode")[0].ChildNodes[0].InnerText == "0")
            {
                XmlNodeList dist = xml.GetElementsByTagName("distance");
                double distInMiles = Convert.ToDouble(dist[0].ChildNodes[0].InnerText);
                double distInKm = distInMiles * 1.609344;
                if (distInKm > tester.MaxDistanceOfTestLocation)
                    throw new Exception("The specified tester can't perform test in the specified location, It's too far.");

            }
            else if (xml.GetElementsByTagName("statusCode")[0].ChildNodes[0].InnerText == "402")
            {
                throw new Exception("An error occured, on of the addresses isn't found, try again.");
            }
            else
            {
                throw new Exception("We havn't got an answer, maybe the net is busy.");
            }
        }*/

        private bool RequestDistanceBool(Tester tester, Address testDepartureAddress)
        {
            string origin = tester.Address.Street + " " + tester.Address.BuildingNumber.ToString() + " " + tester.Address.City + " " + tester.Address.Country;
            string destination = testDepartureAddress.Street + " " + testDepartureAddress.BuildingNumber.ToString() + " " + testDepartureAddress.City + " " + testDepartureAddress.Country;
            string key = @"UU3jYL5u5Ltj0phASFKGnGF8AEMnVb1X";

            string url = @"https://www.mapquestapi.com/directions/v2/route?key=" + key + @"&from=" + origin + @"&to=" + destination + @"&outFormat=xml&ambiguities=ignore&routeType=fastest&doReverseGeocode=false&enhancedNarrative=false&avoidTimedConditions=false";

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            WebResponse resp = req.GetResponse();
            Stream s = resp.GetResponseStream();
            StreamReader sr = new StreamReader(s);
            string rawResponse = sr.ReadToEnd();
            resp.Close();

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(rawResponse);

            if (xml.GetElementsByTagName("statusCode")[0].ChildNodes[0].InnerText == "0")
            {
                XmlNodeList dist = xml.GetElementsByTagName("distance");
                double distInMiles = Convert.ToDouble(dist[0].ChildNodes[0].InnerText);
                double distInKm = distInMiles * 1.609344;
                if (distInKm > tester.MaxDistanceOfTestLocation)
                    throw new Exception("The specified tester can't perform test in the specified location, It's too far.");

            }
            else if (xml.GetElementsByTagName("statusCode")[0].ChildNodes[0].InnerText == "402")
            {
                throw new Exception("An error occured, on of the addresses isn't found, try again.");
            }
            else
            {
                throw new Exception("We havn't got an answer, maybe the net is busy.");
            }
            return true;
        }

        /// <summary>
        /// Adds the tester (If satisfied all conditions) to the data source through the data access layer
        /// </summary>
        /// <param name="tester"></param>
        public void AddTester(Tester tester)
        {
            if (tester.NotAvailableAtTimes == null)
                tester.NotAvailableAtTimes = new List<DateTime>();
            if (tester.Age > Configuration.MinimumTesterAge)
                dal.AddTester(tester);
            else
                throw new Exception("Can't add a tester whose age < 40.");
        }

        /// <summary>
        /// Adds a trainee (If satisfied all conditions) to the data source through the data access layer
        /// </summary>
        /// <param name="trainee"></param>
        public void AddTrainee(Trainee trainee)
        {
            if (trainee.Age > Configuration.MinimumTraineeAge)
                dal.AddTrainee(trainee);
            else
                throw new Exception("Can't add a trainee whose age < 18.");
        }

        /// <summary>
        /// Retires the tester in the system through the data access layer
        /// </summary>
        /// <param name="tester"></param>
        public void RemoveTester(Tester tester)
        {
            var found = from t in dal.GetTesters() where t.Id == tester.Id select t;
            if (found.Count() > 0 && TesterCanRetire(tester))
                dal.RemoveTester(tester);
            else
            {
                if (!TesterCanRetire(tester))
                    throw new Exception("The specified tester have some future tests to perform.");
                else
                    throw new Exception("The specified tester doesn't exist in the system.");
            }
        }

        /// <summary>
        /// Removes a trainee from the data source through the data access layer
        /// </summary>
        /// <param name="trainee"></param>
        public void RemoveTrainee(Trainee trainee)
        {
            var found = from t in dal.GetTrainees() where t.Id == trainee.Id select t;
            if (found.Count() > 0)
                dal.RemoveTrainee(trainee);
            else
                throw new Exception("The specified trainee doesn't exist in the system.");
        }

        /// <summary>
        /// Removes a test from the data source through the data access layer
        /// </summary>
        /// <param name="trainee"></param>
        public void RemoveTest(Test test)
        {
            var found = from t in dal.GetTests() where t.TestId == test.TestId select t;
            if (found.Count() > 0)
            {
                if (test.Finished == false)
                    throw new Exception("The specified test can't be removed because it is not finished yet.");
                dal.RemoveTest(test);
            }                
            else
                throw new Exception("The specified test doesn't exist in the system.");
        }

        /// <summary>
        /// Updates a test in the data source (Usually when finished) through the data access layer
        /// this function enables to update a test that will be executed in the future, Ex: update date etc...
        /// </summary>
        /// <param name="test"></param>
        public void UpdateTest(Test test)
        {
            var found = dal.GetTests().FirstOrDefault(t => t.TestId == test.TestId);
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
                if (test.Passed && test.CriteriaOfTheTest.Count > 0)
                    throw new Exception("The specified test is not finished and has a grade.");
            }
            dal.UpdateTest(test);
        }

        /// <summary>
        /// Updates the tester in  the data source through the data access layer
        /// </summary>
        /// <param name="tester"></param>
        public void UpdateTester(Tester tester)
        {
            var found = from t in dal.GetTesters() where t.Id == tester.Id select t;
            if (found.Count() == 0)
                throw new Exception("The specified tester doesn't exist in the system.");
            dal.UpdateTester(tester);
        }

        /// <summary>
        /// Updates a trainee in the data source through the data access layer
        /// </summary>
        /// <param name="trainee"></param>
        public void UpdateTrainee(Trainee trainee)
        {
            var found = from t in dal.GetTrainees() where t.Id == trainee.Id select t;
            if (found.Count() == 0)
                throw new Exception("The specified trainee doesn't exist in the system.");
            dal.UpdateTrainee(trainee);
        }

        // Get Data Methods
        public List<Tester> GetTesters() { return dal.GetTesters(); }
        public List<Tester> GetRetiredTesters() { return dal.GetRetiredTesters(); }
        public List<Trainee> GetTrainees() { return dal.GetTrainees(); }
        public List<Trainee> GetRemovedTrainees() { return dal.GetRemovedTrainees(); }
        public List<Test> GetTests() { return dal.GetTests(); }
        public List<Test> GetRemovedTests() { return dal.GetRemovedTests(); }

        #endregion

        #region Methods



        public DateTime GetLastTestDateForSpecificTrainee(Trainee trainee)
        {
            DateTime dt = DateTime.MinValue;            
            foreach (var test in dal.GetTests())
                if (test.TraineeId == trainee.Id)
                    if (dt < test.DateOfTest)
                        dt = test.DateOfTest;
            return dt;
        }

        public bool SuccesfullyPassedTestWithSpecifiedCarTypeAndGearBoxType(Trainee trainee)
        {
            var passed = from test in dal.GetTests() where (test.TraineeId == trainee.Id && test.Passed && test.CarType == trainee.CarType && test.GearboxType == trainee.GearboxType) select test;
            return (passed.Count() > 0) ? true : false;
        }

        public IEnumerable<Tester> GetAllAvailableTestersForSpecificDateTime(DateTime dateTime)
        {
            var availableTesters = from tester in dal.GetTesters() where TesterAvailableAt(dateTime, tester) select tester;
            return availableTesters;
        }

        public int GetNumberOfTestsDoneBySpecificTrainee(Trainee trainee)
        {
            var tests = from test in dal.GetTests() where (test.TraineeId == trainee.Id && test.DateOfTest < DateTime.Now) select test;
            return tests.Count();
        }

        public bool TesterCanRetire(Tester tester)
        {
            var ans = from t in dal.GetTests() where (t.TesterId == tester.Id && t.DateOfTest > DateTime.Now) select t;
            return (ans.Count() == 0);
        }

        public int GetNumberOfWeeklyTestsForSpecificTester(Tester tester)
        {
            var tests = from test in dal.GetTests() where (int)(test.DateOfTest.DayOfWeek) <= 4 && (test.TesterId == tester.Id) select test;
            return tests.Count();
        }

        public bool TraineeAvailableForTestAt(DateTime dateTime, Trainee trainee)
        {
            var available = from test in dal.GetTests() where (test.TraineeId == trainee.Id && test.DateOfTest == dateTime) select test;
            return (available.Count() > 0) ? false : true;
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

        public IEnumerable<Tester> AllTestersSatisfiesCondition(Func<Tester, bool> conditon)
        {
            var testersMeetCondition = from tester in dal.GetTesters() where (conditon(tester)) select tester;
            return testersMeetCondition;
        }

        public IEnumerable<Trainee> AllTraineesWhoMeetCondition(Func<Trainee, bool> conditon)
        {
            var traineesMeetCondition = from trainee in dal.GetTrainees() where (conditon(trainee)) select trainee;
            return traineesMeetCondition;
        }

        public IEnumerable<Test> AllTestsWhoMeetCondition(Func<Test, bool> conditon)
        {
            var testsMeetCondition = from test in dal.GetTests() where (conditon(test)) select test;
            return testsMeetCondition;
        }

        public bool AllowedToGetDrivingLicence(Trainee trainee)
        {
            var allowed = from test in dal.GetTests() where (test.TraineeId == trainee.Id && test.Passed) select test;
            return (allowed.Count() > 0) ? true : false;
        }

        public IEnumerable<Test> GetListOfTestsAccordingDateTime()
        {
            var furtureTests = from test in dal.GetTests() where (test.DateOfTest > DateTime.Now) select test;
            return furtureTests;
        }

        public IEnumerable<Tester> AllTestersAtDistanceOf(int distance) // for this level, in level 3 we will use google maps for this.
        {
            var listOfTestersAtSpecifiedDistance = from tester in dal.GetTesters()
                                                   where (Math.Abs(tester.MaxDistanceOfTestLocation - distance) > 0)
                                                   select tester;
            return listOfTestersAtSpecifiedDistance;
        }

        public IEnumerable<Test> GetListOfFutureTestsForSpecificTrainee(Trainee trainee)
        {
            var tests = from t in dal.GetTests() where (t.TraineeId == trainee.Id && t.DateOfTest > DateTime.Now) select t;
            return tests;
        }

        #endregion

        #region Grouping Methods

        public List<List<Tester>> GetGroupedListOfTestersAccordingSpecialization(bool sorted)
        {
            var groupedList = dal.GetTesters().GroupBy(tester => tester.CarType).Select(group => group.ToList()).ToList();
            if (sorted)
            {
                for (int i = 0; i < groupedList.Count; i++)
                {
                    for (int j = 0; j < groupedList[i].Count; j++)
                        groupedList[i] = (groupedList[i].OrderBy(tester => tester.FirstName).ToList());
                }
            }
            return groupedList;
        }

        public List<List<Trainee>> GetGroupedListOfTraineesAccordingSchool(bool sorted)
        {
            var groupedList = dal.GetTrainees().GroupBy(trainee => trainee.DrivingSchoolName).Select(group => group.ToList()).ToList();
            if (sorted)
            {
                for (int i = 0; i < groupedList.Count; i++)
                {
                    for (int j = 0; j < groupedList[i].Count; j++)
                        groupedList[i] = (groupedList[i].OrderBy(trainee => trainee.FirstName).ToList());
                }
            }
            return groupedList;
        }

        public List<List<Trainee>> GetGroupedListOfTraineesAccordingTeacher(bool sorted)
        {
            var groupedList = dal.GetTrainees().GroupBy(trainee => trainee.TeacherName).Select(group => group.ToList()).ToList();
            if (sorted)
            {
                for (int i = 0; i < groupedList.Count; i++)
                {
                    for (int j = 0; j < groupedList[i].Count; j++)
                        groupedList[i] = (groupedList[i].OrderBy(trainee => trainee.FirstName).ToList());
                }
            }
            return groupedList;
        }

        // RETURNS A GROUPED LIST OF TRAINEES ACCORDING THE NUMBER OF TESTS, IF 'sorted' == true THE LIST IS SORTED ACCORDING THE ID
        // ELSE - THE LIST IS NOT SORTED.
        public IEnumerable<IGrouping<int, Trainee>> GetGroupedListOfTraineesAccordingNumberOfTests(bool sorted)
        {
            var groupedList = (sorted) ?
                from trainee in dal.GetTrainees()
                orderby trainee.Id
                group trainee
                by GetNumberOfTestsDoneBySpecificTrainee(trainee) into grp
                select grp
                :
                from trainee in dal.GetTrainees()
                group trainee
                by GetNumberOfTestsDoneBySpecificTrainee(trainee) into grp
                select grp;
            return groupedList;
        }

        #endregion

    }
}