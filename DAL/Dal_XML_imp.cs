using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using BE;

// שם: דניאל רוטנמר
// ת.ז: 316240837
// שותף: רפאל לרמן
// פלאפון: 0583266859
// מרצה: יאיר גולדשטיין
// קורס: מיני פרוייקט במערכות חלונות

namespace DAL
{
    public class Dal_XML_imp : Idal
    {

        #region Singleton
      
        public static Dal_XML_imp Instance { get; } = new Dal_XML_imp(); 

        static Dal_XML_imp() { }

        #endregion

        #region Variables

        string testsPath = @"Tests.xml", testersPath = @"Testers.xml", traineesPath = @"Trainees.xml", configPath = @"config.xml";
         
        #endregion

        #region Idal Implementation

        public void AddTester(Tester tester)
        {
            XDocument config = XDocument.Load(configPath);
            var testerId = from t in config.Elements("Configuration").Elements("Identification").Elements("TesterId") select t;            
            int id = Convert.ToInt32(testerId.ElementAt(0).Value);
            testerId.ElementAt(0).Value = (id + 1).ToString();            

            var testerAge = from t in config.Elements("Configuration").Elements("General").Elements("MinimumTesterAge") select t;
            int age = Convert.ToInt32(testerAge.ElementAt(0).Value);
            config.Save(configPath);
            if (tester.Age < age)
                throw new Exception("Can't add a tester whose age < 40.");

            if (!File.Exists(testersPath))
            {
                XElement testersRoot = new XElement("Testers", "");
                testersRoot.Save(testersPath);
            }
            tester.Id = id;
            XDocument testersDocument = XDocument.Load(testersPath); 
            XElement newTester = new XElement("Tester");
            newTester.Add(new XElement("Id", tester.Id));
            newTester.Add(new XElement("FirstName", tester.FirstName));
            newTester.Add(new XElement("LastName", tester.LastName));
            newTester.Add(new XElement("Gender", tester.Gender.ToString()));
            newTester.Add(new XElement("Age", (DateTime.Now.Year - tester.DateOfBirth.Year)));
            newTester.Add(new XElement("DateOfBirth", tester.DateOfBirth.ToString()));
            newTester.Add(new XElement("PhoneNumber", tester.PhoneNumber));
            newTester.Add(new XElement("Address", new XElement("Country", tester.Address.Country), new XElement("City", tester.Address.City), new XElement("Street", tester.Address.Street), new XElement("BuildingNumber", tester.Address.BuildingNumber)));
            newTester.Add(new XElement("YearsOfExperience", tester.YearsOfExperience));
            newTester.Add(new XElement("MaximumPossibleWeeklyTests", tester.MaximumPossibleWeeklyTests));
            newTester.Add(new XElement("CarType", tester.CarType.ToString()));
            newTester.Add(new XElement("Active", tester.Active.ToString()));
            newTester.Add(new XElement("DateOfResignation", tester.DateOfResignation.ToString()));
            newTester.Add(new XElement("MaximumDistanceForTest", tester.MaxDistanceOfTestLocation.ToString()));
            XElement occupiedHours = new XElement("NotAvailableAtTimes", "");
            for (int i = 0; i < tester.NotAvailableAtTimes.Count; i++)
                occupiedHours.Add("Date", tester.NotAvailableAtTimes[i].ToString());
            newTester.Add(occupiedHours);

            XElement hour9 = new XElement("h9");
            for (int i = 0; i < 5; i++)
                hour9.Add(new XElement("d" + (i + 1).ToString(), tester.WorkSchedule[0, i].ToString()));

            XElement hour10 = new XElement("h10");
            for (int i = 0; i < 5; i++)
                hour10.Add(new XElement("d" + (i + 1).ToString(), tester.WorkSchedule[1, i].ToString()));

            XElement hour11 = new XElement("h11");
            for (int i = 0; i < 5; i++)
                hour11.Add(new XElement("d" + (i + 1).ToString(), tester.WorkSchedule[2, i].ToString()));

            XElement hour12 = new XElement("h12");
            for (int i = 0; i < 5; i++)
                hour12.Add(new XElement("d" + (i + 1).ToString(), tester.WorkSchedule[3, i].ToString()));

            XElement hour13 = new XElement("h13");
            for (int i = 0; i < 5; i++)
                hour13.Add(new XElement("d" + (i + 1).ToString(), tester.WorkSchedule[4, i].ToString()));

            XElement hour14 = new XElement("h14");
            for (int i = 0; i < 5; i++)
                hour14.Add(new XElement("d" + (i + 1).ToString(), tester.WorkSchedule[5, i].ToString()));

            newTester.Add(new XElement("WeeklySchedule", hour9, hour10, hour11, hour12, hour13, hour14));
            testersDocument.Element("Testers").Add(newTester);
            testersDocument.Save(testersPath);
        }

        public void UpdateTester(Tester tester)
        {            
            XDocument testersDocument = XDocument.Load(testersPath);
            var testerElement = (from t in testersDocument.Elements("Testers").Elements("Tester") where t.Element("Id").Value == tester.Id.ToString() select t).ElementAtOrDefault(0);
            if (testerElement == null)
                throw new Exception("The specified tester doesn't exist in the system.");
            testerElement.Element("FirstName").Value = tester.FirstName;
            testerElement.Element("LastName").Value = tester.LastName;
            testerElement.Element("Gender").Value = tester.Gender.ToString();
            testerElement.Element("PhoneNumber").Value = tester.PhoneNumber;
            testerElement.Element("Address").Element("Country").Value = tester.Address.Country;
            testerElement.Element("Address").Element("City").Value = tester.Address.City;
            testerElement.Element("Address").Element("Street").Value = tester.Address.Street;
            testerElement.Element("Address").Element("BuildingNumber").Value = tester.Address.BuildingNumber.ToString();
            testerElement.Element("YearsOfExperience").Value = tester.YearsOfExperience.ToString();
            testerElement.Element("MaximumPossibleWeeklyTests").Value = tester.MaximumPossibleWeeklyTests.ToString();
            testerElement.Element("CarType").Value = tester.CarType.ToString();
            testerElement.Element("Active").Value = tester.Active.ToString();
            testerElement.Element("DateOfResignation").Value = tester.DateOfResignation.ToString();
            testerElement.Element("MaximumDistanceForTest").Value = tester.MaxDistanceOfTestLocation.ToString();
            List<XElement> occupiedHours = new List<XElement>();
            for (int i = 0; i < tester.NotAvailableAtTimes.Count; i++)
                occupiedHours.Add(new XElement("Date", tester.NotAvailableAtTimes[i].ToString()));
            testerElement.Element("NotAvailableAtTimes").ReplaceNodes(occupiedHours);

            XElement hour9 = new XElement("h9");
            for (int i = 0; i < 5; i++)
                hour9.Add(new XElement("d" + (i + 1).ToString(), tester.WorkSchedule[0, i].ToString()));

            XElement hour10 = new XElement("h10");
            for (int i = 0; i < 5; i++)
                hour10.Add(new XElement("d" + (i + 1).ToString(), tester.WorkSchedule[1, i].ToString()));

            XElement hour11 = new XElement("h11");
            for (int i = 0; i < 5; i++)
                hour11.Add(new XElement("d" + (i + 1).ToString(), tester.WorkSchedule[2, i].ToString()));

            XElement hour12 = new XElement("h12");
            for (int i = 0; i < 5; i++)
                hour12.Add(new XElement("d" + (i + 1).ToString(), tester.WorkSchedule[3, i].ToString()));

            XElement hour13 = new XElement("h13");
            for (int i = 0; i < 5; i++)
                hour13.Add(new XElement("d" + (i + 1).ToString(), tester.WorkSchedule[4, i].ToString()));

            XElement hour14 = new XElement("h14");
            for (int i = 0; i < 5; i++)
                hour14.Add(new XElement("d" + (i + 1).ToString(), tester.WorkSchedule[5, i].ToString()));

            testerElement.Element("WeeklySchedule").ReplaceNodes(hour9, hour10, hour11, hour12, hour13, hour14);
            testersDocument.Save(testersPath);
        }

        public void RemoveTester(Tester tester)
        {
            XDocument testersDocument = XDocument.Load(testersPath);
            var testerElement = (from t in testersDocument.Elements("Testers").Elements("Tester") where t.Element("Id").Value == tester.Id.ToString() select t).ElementAtOrDefault(0);
            if (testerElement == null)
                throw new Exception("The specified tester doesn't exist in the system.");
            if (TesterCanRetire(tester))
            {
                testerElement.Element("Active").Value = false.ToString();
                testerElement.Element("DateOfResignation").Value = DateTime.Now.ToString();
            }
            else
                throw new Exception("The specified tester have some future tests to perform.");
            testersDocument.Save(testersPath);
        }

        public void AddTest(Test test)
        {
            List<Test> list;
            if (!File.Exists(testersPath))
                throw new Exception("No tester Available.");
            if (!File.Exists(traineesPath))
                throw new Exception("No trainee Available.");
            list = File.Exists(testsPath) ? GetAllTests().ToList() : new List<Test>();     

            XDocument config = XDocument.Load(configPath);
            var testId = from t in config.Element("Configuration").Elements("Identification").Elements("TestId") select t;
            int id = Convert.ToInt32(testId.ElementAt(0).Value);
            testId.ElementAt(0).Value = (id + 1).ToString();
            config.Save(configPath);

            // GETTING THE TRAINEE
            XDocument traineesDocument = XDocument.Load(traineesPath);
            XElement trainee = (from t in traineesDocument.Elements("ArrayOfTrainee").Elements("Trainee") where t.Element("Id").Value == test.TraineeId.ToString() select t).ElementAtOrDefault(0);
            if (trainee == null)
                throw new Exception("The specified trainee not found in the system.");

            // GETTING THE TESTER
            XDocument testersDocument = XDocument.Load(testersPath);
            XElement tester = (from t in testersDocument.Elements("Testers").Elements("Tester") where t.Element("Id").Value == test.TesterId.ToString() select t).ElementAtOrDefault(0);
            if (tester == null)
                throw new Exception("The specified trainee not found in the system.");

            if (Math.Abs((test.DateOfTest - GetLastTestDateForSpecificTrainee(trainee)).Days) < Convert.ToInt32(config.Element("Configuration").Element("General").Element("MinimumIntervalOfDaysBetween2Tests").Value) || Convert.ToInt32(trainee.Element("NumberOfDrivingLessons").Value) < Convert.ToInt32(config.Element("Configuration").Element("General").Element("MinimumNumberOfLessons").Value))
            {
                string exceptionMessage = "";
                if (Math.Abs((test.DateOfTest - GetLastTestDateForSpecificTrainee(trainee)).Days) < Convert.ToInt32(config.Element("Configuration").Element("General").Element("MinimumIntervalOfDaysBetween2Tests").Value))
                    exceptionMessage += "The minimum interval of time between 2 tests is 7 days.";
                if (Convert.ToInt32(trainee.Element("NumberOfDrivingLessons").Value) < Convert.ToInt32(config.Element("Configuration").Element("General").Element("MinimumNumberOfLessons").Value))
                    exceptionMessage += "A trainee must must have record of at least 20 driving lessons to get a test.";
                throw new Exception(exceptionMessage);
            }

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
                            if (File.Exists(testsPath))
                            {
                                XDocument testsDocument = XDocument.Load(testsPath);
                                var futureTests = from t in testsDocument.Elements("ArrayOfTest").Elements("Test") where (t.Element("TraineeId").Value == test.TraineeId.ToString()) && (Math.Abs((availableDate - Convert.ToDateTime(t.Element("DateOfTest").Value)).Days) < Convert.ToInt32(config.Element("Configuration").Element("General").Element("MinimumIntervalOfDaysBetween2Tests").Value)) select t;
                                if (futureTests.Count() == 0)
                                {
                                    message += "\nAlternative time for test is: " + availableDate.ToLongDateString() + ", " + availableDate.Hour.ToString() + ":00.";
                                    available = true;
                                }
                            }
                            else
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

            if (File.Exists(testsPath))
            {
                XDocument testsDocument = XDocument.Load(testsPath);
                var futureTests = from t in testsDocument.Elements("ArrayOfTest").Elements("Test") where (t.Element("TraineeId").Value == test.TraineeId.ToString() && Math.Abs((test.DateOfTest - Convert.ToDateTime(t.Element("DateOfTest").Value)).Days) < Convert.ToInt32(config.Element("Configuration").Element("General").Element("MinimumIntervalOfDaysBetween2Tests").Value)) select t;
                if (futureTests.Count() > 0)
                    throw new Exception("This trainee have a test fixed for " + futureTests.ElementAt(0).Element("DateOfTest").Value + ", The interval ofdays between 2 tests is at least " + Convert.ToInt32(config.Element("Configuration").Element("General").Element("MinimumIntervalOfDaysBetween2Tests").Value) + " Days.");

                var futureTestsWithTheSameCarType = from t in testsDocument.Elements("Tests").Elements() where (CarType)Enum.Parse(typeof(CarType), t.Element("CarType").Value) == test.CarType select t;
                if (futureTestsWithTheSameCarType.Count() > 0)
                    throw new Exception("This trainee have a test with the same car type fixed for " + futureTestsWithTheSameCarType.ElementAt(0).Element("DateOfTest").Value + ".");

                if (!TraineeAvailableForTestAt(test.DateOfTest, trainee))
                    throw new Exception("The specified trainee is not available at: " + test.DateOfTest.DayOfWeek.ToString() + ", " + test.DateOfTest.Hour.ToString() + ".");

                if (SuccesfullyPassedTestWithSpecifiedCarTypeAndGearBoxType(trainee))
                    throw new Exception("The specified trainee already successfully passed a test with this car type and gear box type.");
            }                        
                        
            if (!Convert.ToBoolean(tester.Element("Active").Value))
                throw new Exception("The specified tester is no longer active in the system.");
            if (Convert.ToInt32(tester.Element("MaximumPossibleWeeklyTests").Value) < GetNumberOfWeeklyTestsForSpecificTester(tester))
                throw new Exception("The specified tester reached the number of tests for this week.");
                                    
            if (tester.Element("CarType").Value != trainee.Element("CarType").Value)
                throw new Exception("The tester car type is not the same as the trainee.");
            if (trainee.Element("CarType").Value != test.CarType.ToString())
                throw new Exception("The trainee car type is not the same as the test car type.");
            
            test.TestId = id;

            list.Add(test);
            FileMode fm = FileMode.Truncate;
            if (!File.Exists(testsPath))
                fm = FileMode.Create;
            XmlSerializer x = new XmlSerializer(list.GetType());
            FileStream fs = new FileStream(testsPath, fm);
            x.Serialize(fs, list);
            fs.Close();

            /*XElement newTest = new XElement("Test");
            newTest.Add(new XElement("TestId", test.TestId.ToString()));
            newTest.Add(new XElement("TesterId", test.TesterId.ToString()));
            newTest.Add(new XElement("TraineeId", test.TraineeId.ToString()));
            newTest.Add(new XElement("CarType", test.CarType.ToString()));
            newTest.Add(new XElement("GearBoxType", test.GearboxType.ToString()));
            newTest.Add(new XElement("DateOfTest", test.DateOfTest.ToString()));
            newTest.Add(new XElement("Finished", false.ToString()));
            newTest.Add(new XElement("Removed", false.ToString()));
            newTest.Add(new XElement("CriteriaOfTheTest", ""));
            newTest.Add(new XElement("TestDepartureAddress", ""));
            newTest.Add(new XElement("Passed", ""));

            testsDocument.Element("Tests").Add(newTest);
            testsDocument.Save(testsPath);*/

            tester.Element("NotAvailableAtTimes").Add(new XElement("Date", test.DateOfTest.ToString()));
            testersDocument.Save(testersPath);
        }

        public void UpdateTest(Test test)
        {
            List<Test> list;
            if (File.Exists(testsPath))
                list = GetAllTests().ToList();
            else
                throw new Exception("No test to update.");
            if (!list.Exists(t => t.TestId == test.TestId))
                throw new Exception("The specified test doesn't exists in the system.");
                        
            if (list.Find(t => t.TestId == test.TestId).Finished)
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

            list[list.FindIndex(t => t.TestId == test.TestId)] = test;
            FileMode fm = FileMode.Truncate;
            XmlSerializer x = new XmlSerializer(list.GetType());
            FileStream fs = new FileStream(testsPath, fm);
            x.Serialize(fs, list);
            fs.Close();
           /*
            XDocument testsDocument = XDocument.Load(testsPath);
            var testElement = (from t in testsDocument.Elements("ArrayOfTest").Elements("Test") where t.Element("TestId").Value == test.TestId.ToString() select t).ElementAtOrDefault(0);
            if (testElement == null)
                throw new Exception("The specified test doesn't exist in the system.");
            if (Convert.ToBoolean(testElement.Element("Finished").Value) == true)
                throw new Exception("Can't update a test that was finished in the past.");
            if (test.Finished)
            {
                if (test.CriteriaOfTheTest.Count == 0)
                    throw new Exception("The criteria list of this test is empty, Can't update!");
                int notPassed = (from c in test.CriteriaOfTheTest where !c.SuccessfullyPassed select c).Count();
                if (notPassed > (test.CriteriaOfTheTest.Count / 2) && test.Passed)
                    throw new Exception("Not possible to pass the test while most of the criteria not passed!");
            }
            else
            {
                if (test.Passed || test.CriteriaOfTheTest.Count > 0)
                    throw new Exception("The specified test is not finished and has a grade.");
            }
            testElement.Element("Finished").Value = true.ToString();
            testElement.Element("Removed").Value = test.Removed.ToString();
             
            List<XElement> criteria = new List<XElement>();
            for (int i = 0; i < test.CriteriaOfTheTest.Count; i++)
                criteria.Add(new XElement("Criteria", new XElement("Name", test.CriteriaOfTheTest[i].CriterionDescription), new XElement("Remeraks", test.CriteriaOfTheTest[i].Remarks), new XElement("Passed", test.CriteriaOfTheTest[i].SuccessfullyPassed.ToString())));
            testElement.Element("CriteriaOfTheTest").ReplaceNodes(criteria);
            testElement.Element("TestDepartureAddress").ReplaceNodes(new XElement("Country", test.TestDepartureAddress.Country), new XElement("City", test.TestDepartureAddress.City), new XElement("Street", test.TestDepartureAddress.Street), new XElement("BuildingNumber", test.TestDepartureAddress.BuildingNumber.ToString()));
            testElement.Element("Passed").Value = test.Passed.ToString();
            */
            //testsDocument.Save(testsPath);
        }

        public void RemoveTest(Test test)
        {
            XDocument testsDocument = XDocument.Load(testsPath);
            var testElement = (from t in testsDocument.Elements("ArrayOfTest").Elements("Test") where t.Element("TestId").Value == test.TestId.ToString() select t).ElementAtOrDefault(0);
            if (testElement == null)
                throw new Exception("The specified test doesn't exist in the system.");
            if (Convert.ToBoolean(testElement.Element("Finished").Value) == false)
                throw new Exception("The specified test can't be removed because it is not finished yet.");
            testElement.Element("Removed").Value = true.ToString();
            testsDocument.Save(testsPath);
        }

        public void AddTrainee(Trainee trainee)
        {
            XDocument config = XDocument.Load(configPath);
            var traineeId = from t in config.Element("Configuration").Elements("Identification").Elements("TraineeId") select t;
            int id = Convert.ToInt32(traineeId.ElementAt(0).Value);
            traineeId.ElementAt(0).Value = (id + 1).ToString();
            config.Save(configPath);
            if (trainee.Age < Configuration.MinimumTraineeAge)
                throw new Exception("Can't add a trainee whose age < 18.");

            trainee.Id = id;
            List<Trainee> list;
            if (File.Exists(traineesPath))
                list = GetAllTrainees().ToList();
            else
                list = new List<Trainee>();
            list.Add(trainee);
            FileMode fm = FileMode.Truncate;
            if (!File.Exists(traineesPath))
                fm = FileMode.Create;
            XmlSerializer x = new XmlSerializer(list.GetType());
            FileStream fs = new FileStream(traineesPath, fm);
            x.Serialize(fs, list);
            fs.Close();
        }

        public void UpdateTrainee(Trainee trainee)
        {
            List<Trainee> list;
            if (File.Exists(traineesPath))
                list = GetAllTrainees().ToList();
            else
                throw new Exception("No trainee to update.");
            if (!list.Exists(t => t.Id == trainee.Id))
                throw new Exception("The specified trainee doesn't exists in the system.");
            list[list.FindIndex(t => t.Id == trainee.Id)] = trainee;
            FileMode fm = FileMode.Truncate;
            XmlSerializer x = new XmlSerializer(list.GetType());
            FileStream fs = new FileStream(traineesPath, fm);
            x.Serialize(fs, list);
            fs.Close();
        }

        public void RemoveTrainee(Trainee trainee)
        {
            if (!File.Exists(traineesPath))
                throw new Exception("No trainee to remove.");
            List<Trainee> traineesList = GetAllTrainees();
            List<Test> testsList = GetAllTests();
            if (!traineesList.Exists(t => t.Id == trainee.Id))
                throw new Exception("The specified trainee doesn't exists in the system.");
            if (testsList.Exists(t => t.TraineeId == trainee.Id && t.DateOfTest > DateTime.Now))
                throw new Exception("The specified trainee has tests to complete in th future, can't remove him.");
            traineesList[traineesList.FindIndex(t => t.Id == trainee.Id)].Removed = true;
            FileMode fm = FileMode.Truncate;
            XmlSerializer x = new XmlSerializer(traineesList.GetType());
            FileStream fs = new FileStream(traineesPath, fm);
            x.Serialize(fs, traineesList);
            fs.Close();
        }

        // Get Data Methods
        public List<Tester> GetTesters()
        {
            List<Tester> testersList = new List<Tester>();
            if (GetAllTesters() == null)
                return testersList;
            testersList = GetAllTesters();
            testersList.RemoveAll(t => t.Active == false);
            return testersList;
        }

        public List<Test> GetTests()
        {
            List<Test> testsList = new List<Test>();
            if (GetAllTests() == null)
                return testsList;
            testsList = GetAllTests();
            testsList.RemoveAll(t => t.Removed == true);
            return testsList;
        }

        public List<Trainee> GetTrainees()
        {           
            List<Trainee> traineesList = new List<Trainee>();
            if (GetAllTrainees() != null)
                traineesList = GetAllTrainees();
            traineesList.RemoveAll(t => t.Removed == true);
            return traineesList;
        }

        public List<Tester> GetRetiredTesters()
        {
            List<Tester> testersList = new List<Tester>();
            if (GetAllTesters() == null)
                return testersList;
            testersList = GetAllTesters();
            testersList.RemoveAll(t => t.Active == true);
            return testersList;
        }

        public List<Trainee> GetRemovedTrainees()
        {
            List<Trainee> traineesList = new List<Trainee>();
            if (GetAllTrainees() == null)
                return traineesList;
            traineesList = GetAllTrainees();
            traineesList.RemoveAll(t => t.Removed == false);
            return traineesList;
        }
        
        public List<Test> GetRemovedTests()
        {
            List<Test> testsList = new List<Test>();
            if (GetAllTests() == null)
                return testsList;
            testsList = GetAllTests();
            testsList.RemoveAll(t => t.Removed == false);
            return testsList;
        }

        #endregion

        #region Methods

        private List<Test> GetAllTests()
        {
            /*if (!File.Exists(testsPath))
                return null;
            XDocument testsDocument = XDocument.Load(testsPath);
            var testsList = from test in testsDocument.Elements("Tests").Elements()
                            select new Test((CarType)Enum.Parse(typeof(CarType), test.Element("CarType").Value), (GearboxType)Enum.Parse(typeof(GearboxType), test.Element("GearBoxType").Value), Convert.ToInt32(test.Element("TraineeId").Value),
                            Convert.ToInt32(test.Element("TesterId").Value), Convert.ToDateTime(test.Element("DateOfTest").Value), Convert.ToBoolean(test.Element("Removed").Value), Convert.ToInt32(test.Element("TestId").Value), Convert.ToBoolean(test.Element("Passed").Value), Convert.ToBoolean(test.Element("Finished").Value));
            return testsList.ToList();*/
            if (!File.Exists(testsPath))
                return null;
            List<Test> list;
            XmlSerializer x = new XmlSerializer(typeof(List<Test>));
            FileStream fs = new FileStream(testsPath, FileMode.Open);
            list = (List<Test>)x.Deserialize(fs);
            fs.Close();
            return list;
        }

        private List<Tester> GetAllTesters() // both active and not active
        {
            XDocument testersDocument = XDocument.Load(testersPath);
            List<Tester> testersList = (from tester in testersDocument.Elements("Testers").Elements("Tester")                             
                              select new Tester(tester.Element("FirstName").Value, tester.Element("LastName").Value, Convert.ToDateTime(tester.Element("DateOfBirth").Value), (Gender)Enum.Parse(typeof(Gender), tester.Element("Gender").Value), tester.Element("PhoneNumber").Value,
                              new Address(tester.Element("Address").Element("Country").Value, tester.Element("Address").Element("City").Value, tester.Element("Address").Element("Street").Value, Convert.ToInt32(tester.Element("Address").Element("BuildingNumber").Value)),
                              Convert.ToInt32(tester.Element("YearsOfExperience").Value), Convert.ToInt32(tester.Element("MaximumPossibleWeeklyTests").Value), (CarType)Enum.Parse(typeof(CarType), tester.Element("CarType").Value),
                              new bool[6, 5]
                              {
                                  { Convert.ToBoolean(tester.Element("WeeklySchedule").Element("h9").Element("d1").Value), Convert.ToBoolean(tester.Element("WeeklySchedule").Element("h9").Element("d2").Value), Convert.ToBoolean(tester.Element("WeeklySchedule").Element("h9").Element("d3").Value), Convert.ToBoolean(tester.Element("WeeklySchedule").Element("h9").Element("d4").Value), Convert.ToBoolean(tester.Element("WeeklySchedule").Element("h9").Element("d5").Value),  },
                                  { Convert.ToBoolean(tester.Element("WeeklySchedule").Element("h10").Element("d1").Value), Convert.ToBoolean(tester.Element("WeeklySchedule").Element("h10").Element("d2").Value), Convert.ToBoolean(tester.Element("WeeklySchedule").Element("h10").Element("d3").Value), Convert.ToBoolean(tester.Element("WeeklySchedule").Element("h10").Element("d4").Value), Convert.ToBoolean(tester.Element("WeeklySchedule").Element("h10").Element("d5").Value),  },
                                  { Convert.ToBoolean(tester.Element("WeeklySchedule").Element("h11").Element("d1").Value), Convert.ToBoolean(tester.Element("WeeklySchedule").Element("h11").Element("d2").Value), Convert.ToBoolean(tester.Element("WeeklySchedule").Element("h11").Element("d3").Value), Convert.ToBoolean(tester.Element("WeeklySchedule").Element("h11").Element("d4").Value), Convert.ToBoolean(tester.Element("WeeklySchedule").Element("h11").Element("d5").Value),  },
                                  { Convert.ToBoolean(tester.Element("WeeklySchedule").Element("h12").Element("d1").Value), Convert.ToBoolean(tester.Element("WeeklySchedule").Element("h12").Element("d2").Value), Convert.ToBoolean(tester.Element("WeeklySchedule").Element("h12").Element("d3").Value), Convert.ToBoolean(tester.Element("WeeklySchedule").Element("h12").Element("d4").Value), Convert.ToBoolean(tester.Element("WeeklySchedule").Element("h12").Element("d5").Value),  },
                                  { Convert.ToBoolean(tester.Element("WeeklySchedule").Element("h13").Element("d1").Value), Convert.ToBoolean(tester.Element("WeeklySchedule").Element("h13").Element("d2").Value), Convert.ToBoolean(tester.Element("WeeklySchedule").Element("h13").Element("d3").Value), Convert.ToBoolean(tester.Element("WeeklySchedule").Element("h13").Element("d4").Value), Convert.ToBoolean(tester.Element("WeeklySchedule").Element("h13").Element("d5").Value),  },
                                  { Convert.ToBoolean(tester.Element("WeeklySchedule").Element("h14").Element("d1").Value), Convert.ToBoolean(tester.Element("WeeklySchedule").Element("h14").Element("d2").Value), Convert.ToBoolean(tester.Element("WeeklySchedule").Element("h14").Element("d3").Value), Convert.ToBoolean(tester.Element("WeeklySchedule").Element("h14").Element("d4").Value), Convert.ToBoolean(tester.Element("WeeklySchedule").Element("h14").Element("d5").Value),  },
                              },
                              Convert.ToInt32(tester.Element("MaximumDistanceForTest").Value), Convert.ToBoolean(tester.Element("Active").Value), Convert.ToDateTime(tester.Element("DateOfResignation").Value), Convert.ToInt32(tester.Element("Id").Value))).ToList();
            for (int i = 0; i < testersList.Count(); i++)
            {
                var testerElement = (from t in testersDocument.Elements("Testers").Elements("Tester") where t.Element("Id").Value == testersList[i].Id.ToString() select t).FirstOrDefault();
                testersList[i].NotAvailableAtTimes = (from t in testerElement.Element("NotAvailableAtTimes").Elements("Date") select Convert.ToDateTime(t.Value)).ToList();
            }
            return testersList.ToList();
        }

        private List<Trainee> GetAllTrainees() 
        {
            if (!File.Exists(traineesPath))
                return null;
            List<Trainee> list;
            XmlSerializer x = new XmlSerializer(typeof(List<Trainee>));
            FileStream fs = new FileStream(traineesPath, FileMode.Open);
            list = (List<Trainee>)x.Deserialize(fs);
            fs.Close();
            return list;
        }
         
        public bool TesterAvailableAt(DateTime dateTime, XElement tester)
        {
            int dayOfWeek = (int)dateTime.DayOfWeek;
            int hour = dateTime.Hour;
            if (hour >= 9 && hour <= 14 && (dayOfWeek) >= 0 && (dayOfWeek) <= 4)
            {
                if (Convert.ToBoolean(tester.Element("WeeklySchedule").Element("h" + (hour).ToString()).Element("d" + (dayOfWeek + 1).ToString()).Value) == false)
                    return false;
            }
            else
                return false;
            if (!tester.Element("NotAvailableAtTimes").IsEmpty)
            {
                var dt = from date in tester.Element("NotAvailableAtTimes").Elements("Date") where Convert.ToDateTime(date.Value) == dateTime select date;
                return (dt.Count() > 0) ? false : true;
            }
            return true;
        }

        public DateTime GetLastTestDateForSpecificTrainee(XElement trainee)
        {
            DateTime dt = DateTime.MinValue;
            if (!File.Exists(testsPath))
                return dt;
            XDocument testsRoot = XDocument.Load(testsPath);
            var tests = (from test in testsRoot.Elements("ArrayOfTest").Elements("Test") where test.Element("TraineeId").Value == trainee.Element("Id").Value && Convert.ToDateTime(test.Element("DateOfTest").Value) < DateTime.Now orderby Convert.ToDateTime(test.Element("DateOfTest").Value) descending select test).ElementAtOrDefault(0);
            if (tests == null)
                return DateTime.MinValue;
            return Convert.ToDateTime(tests.Element("DateOfTest").Value);
        }

        public bool TesterCanRetire(Tester tester)
        {
            if (!File.Exists(testsPath))
                return true;
            List<Test> tests = GetAllTests();
            if (!tests.Exists(t => t.TesterId == tester.Id && t.DateOfTest > DateTime.Now))
                return true;
            return false;
        }

        public int GetNumberOfWeeklyTestsForSpecificTester(XElement tester)
        {
            if (!File.Exists(testsPath))
                return 0;
            XDocument testsRoot = XDocument.Load(testsPath);
            var tests = from test in testsRoot.Elements("ArrayOfTest").Elements("Test") where ((int)(Convert.ToDateTime(test.Element("DateOfTest").Value).DayOfWeek) <= 4 && (test.Element("TesterId").Value == tester.Element("Id").Value)) select test;
            return tests.Count();
        }

        public bool TraineeAvailableForTestAt(DateTime dateTime, XElement trainee)
        {
            if (!File.Exists(testsPath))
                return true;
            XDocument testsRoot = XDocument.Load(testsPath);
            var available = from test in testsRoot.Elements("ArrayOfTest").Elements("Test") where (test.Element("TraineeId").Value == trainee.Element("Id").Value && Convert.ToDateTime(test.Element("DateOfTest").Value) == dateTime) select test;
            return (available.Count() > 0) ? false : true;
        }
         
        public bool SuccesfullyPassedTestWithSpecifiedCarTypeAndGearBoxType(XElement trainee)
        {
            if (!File.Exists(testsPath))
                return false;
            XDocument testsRoot = XDocument.Load(testsPath);
            var passed = from test in testsRoot.Elements("ArrayOfTest").Elements("Test") where (test.Element("TraineeId").Value == trainee.Element("Id").Value && Convert.ToBoolean(test.Element("Passed").Value) == true && test.Element("CarType").Value == trainee.Element("CarType").Value && test.Element("GearboxType").Value == trainee.Element("GearboxType").Value) select test;
            return (passed.Count() > 0) ? true : false;
        }

        #endregion

    }
}
