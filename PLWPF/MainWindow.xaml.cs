using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using BE;
using BL;
using System.Xml.Linq;

// שם: דניאל רוטנמר
// ת.ז: 316240837
// שותף: רפאל לרמן
// פלאפון: 0583266859
// מרצה: יאיר גולדשטיין
// קורס: מיני פרוייקט במערכות חלונות

namespace PLWPF
{
    public delegate void DataChangedEventHandler();

    public enum Operations { Add, Update, View };

    public class Operation
    {
        public Operations Method { get; set; }
        public bool Finished { get; set; }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Constructor

        public MainWindow()
        {
            InitializeComponent();
            /*if (File.Exists("Tests.xml"))
                File.Delete("Tests.xml");
            if (File.Exists("Trainees.xml"))
                File.Delete("Trainees.xml");
            if (File.Exists("Testers.xml"))
                File.Delete("Testers.xml");*/
            XDocument config = XDocument.Load("config.xml");
            var testId = from t in config.Element("Configuration").Elements("Identification").Elements("TestId") select t;
            testId.ElementAt(0).Value = "1";
            var testerId = from t in config.Element("Configuration").Elements("Identification").Elements("TesterId") select t;
            testerId.ElementAt(0).Value = "1";
            var traineeId = from t in config.Element("Configuration").Elements("Identification").Elements("TraineeId") select t;
            traineeId.ElementAt(0).Value = "1";
            config.Save("config.xml");
            bl = (Bl_imp)FactoryBl.GetBl();
            //AddTesters();
            //AddTrainees();
           // AddTest();
            UpdateData();
        }

        #endregion

        void AddTesters()
        {
            Tester tester = new Tester("moshe", "joe", new DateTime(1960, 11, 17, 05, 05, 05), Gender.Female, "645454", new Address("Israel", "jerusalem", "HaPisga", 45), 25, 24, CarType.PrivateCar,
                new bool[6, 5]
                {
                    {true, false, false, false, true },
                    {true, false, false, false, true },
                    {true, false, false, false, true },
                    {true, false, false, false, true },
                    {true, false, false, false, true },
                    {true, false, false, false, true },
                }, 65, true, DateTime.MaxValue);
            bl.AddTester(tester);
            Tester tester1 = new Tester("yair", "goldsht", new DateTime(1950, 11, 17, 05, 05, 05), Gender.Female, "545466546", new Address("Israel", "jerusalem", "brazil", 1), 25, 24, CarType.PrivateCar,
                new bool[6, 5]
                {
                    {true, true, false, false, true },
                    {true, true, false, false, true },
                    {true, true, false, false, true },
                    {true, true, false, false, true },
                    {true, true, false, false, true },
                    {true, true, false, false, true },
                }, 65, true, DateTime.MaxValue);
            bl.AddTester(tester1);
        }

        void AddTrainees()
        {
            Trainee trainee = new Trainee("dani", "levi", new DateTime(1980, 11, 17, 05, 05, 05), Gender.Female, "5454554", new Address("israel", "jerusalem", "Pinsker", 15), CarType.PrivateCar, GearboxType.Auto, "best test", 30, "yossi");
            bl.AddTrainee(trainee);
            Trainee trainee1 = new Trainee("sasson", "sassoni", new DateTime(1980, 11, 17, 05, 05, 05), Gender.Female, "5454554", new Address("israel", "jerusalem", "Dubnov", 15), CarType.PrivateCar, GearboxType.Auto, "or yarok", 30, "david");
            bl.AddTrainee(trainee1);
        }

        void AddTest()
        {
            Test test = new Test(CarType.PrivateCar, GearboxType.Auto, 1, 1, new DateTime(2019, 02, 3, 10, 0, 0));
            test.TestDepartureAddress = new Address("Israel", "Jerusalem", "Uziel", 32);
            bl.AddTest(test);
            //Test test1 = new Test(CarType.PrivateCar, GearboxType.Auto, 2, 2, new DateTime(2019, 04, 10, 9, 0, 0));
            //bl.AddTest(test1);
        }

        #region Private Variables

        private Bl_imp bl;
        private TesterWindow testerInstance = null;
        private TestWindow testInstance = null;
        private TraineeWindow traineeInstance = null;

        #endregion

        #region Properties

        public IEnumerable<Test> Tests { get { return bl.GetTests(); } }
        public IEnumerable<Tester> Testers { get { return bl.GetTesters(); } }
        public IEnumerable<Trainee> Trainees { get { return bl.GetTrainees(); } }

        #endregion

        #region Events Execution Data

        private void TesterInstaceClosed(object sender, EventArgs e)
        {
            testerInstance = null;
        }

        private void TestInstaceClosed(object sender, EventArgs e)
        {
            testInstance = null;
        }

        private void TraineeInstaceClosed(object sender, EventArgs e)
        {
            traineeInstance = null;
        }

        private void UpdateData() // binding
        {
            testersDataGrid.ItemsSource = bl.GetTesters();
            testersDataGrid.DataContext = bl.GetTesters();
            testsDataGrid.ItemsSource = bl.GetTests();
            testsDataGrid.DataContext = bl.GetTests();
            traineesDataGrid.ItemsSource = bl.GetTrainees();
            traineesDataGrid.DataContext = bl.GetTrainees();
            if (bl.GetTesters().Count > 0 && bl.GetTesters()[0].NotAvailableAtTimes.Count > 0)
                Title = bl.GetTesters()[0].NotAvailableAtTimes[0].ToString();
        }

        private void RemoveTesterItem_Click(object sender, RoutedEventArgs e)
        {
            Tester testerToRemove = (Tester)testersDataGrid.SelectedItem;
            if (testerToRemove == null)
                MessageBox.Show("Select tester to remove and try again.", "No tester selected");
            else
            {
                try
                {
                    bl.RemoveTester(testerToRemove);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Exception thrown");
                }
            }
            UpdateData();
        }

        private void AddTesterItem_Click(object sender, RoutedEventArgs e)
        {
            if (testerInstance == null)
            {
                testerInstance = new TesterWindow(Operations.Add);
                testerInstance.Closed += TesterInstaceClosed;
                testerInstance.DataChanged += UpdateData;
                testerInstance.Show();
            }
            else
            {
                if (!testerInstance.CurrentOperation.Finished)
                {
                    testerInstance.Activate(); // ACTIVATE THE WINDOW BEFORE THE MESSAGE BOX IS SHOWN
                    MessageBox.Show("You haven't completed your operation yet, to cancel: close the window.", "Opearion error");
                }
                else
                {
                    testerInstance.SetContent(Operations.Add);
                    testerInstance.Activate(); // ACTIVATE THE WINDOW AFTER SETTING THE NEW CONTENT
                }
            }
        }

        private void UpdateTesterItem_Click(object sender, RoutedEventArgs e)
        {
            Tester testerToUpdate = (Tester)testersDataGrid.SelectedItem;
            if (testerToUpdate == null)
                MessageBox.Show("Select tester to update and try again.", "No tester selected");
            else
            {
                if (testerInstance == null)
                {
                    testerInstance = new TesterWindow(Operations.Update, testerToUpdate);
                    testerInstance.Closed += TesterInstaceClosed;
                    testerInstance.DataChanged += UpdateData;
                    testerInstance.Show();
                }
                else
                {
                    if (!testerInstance.CurrentOperation.Finished)
                    {
                        testerInstance.Activate(); // ACTIVATE THE WINDOW BEFORE THE MESSAGE BOX IS SHOWN
                        MessageBox.Show("You haven't completed your operation yet, to cancel: close the window.", "Opearion error");
                    }
                    else
                    {
                        testerInstance.SetContent(Operations.Update, testerToUpdate);
                        testerInstance.Activate(); // ACTIVATE THE WINDOW AFTER SETTING THE NEW CONTENT
                    }
                }
            }
        }

        private void ViewTesterMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Tester testerToView = (Tester)testersDataGrid.SelectedItem;
            if (testerToView == null)
                MessageBox.Show("Select tester to view and try again.", "No tester selected");
            else
            {
                if (testerInstance == null)
                {
                    testerInstance = new TesterWindow(Operations.View, testerToView);
                    testerInstance.Closed += TesterInstaceClosed;
                    testerInstance.DataChanged += UpdateData;
                    testerInstance.Show();
                }
                else
                {
                    if (!testerInstance.CurrentOperation.Finished)
                    {
                        testerInstance.Activate(); // ACTIVATE THE WINDOW BEFORE THE MESSAGE BOX IS SHOWN
                        MessageBox.Show("You haven't completed your operation yet, to cancel: close the window.", "Opearion error");
                    }
                    else
                    {
                        testerInstance.SetContent(Operations.View, testerToView);
                        testerInstance.Activate(); // ACTIVATE THE WINDOW AFTER SETTING THE NEW CONTENT
                    }
                }
            }
        }

        private void AddTestItem_Click(object sender, RoutedEventArgs e)
        {
            if (testInstance == null)
            {
                testInstance = new TestWindow(Operations.Add);
                testInstance.Closed += TestInstaceClosed;
                testInstance.DataChanged += UpdateData;
                testInstance.Show();
            }
            else
            {
                if (!testInstance.CurrentOperation.Finished)
                {
                    testInstance.Activate(); // ACTIVATE THE WINDOW BEFORE THE MESSAGE BOX IS SHOWN
                    MessageBox.Show("You haven't completed your operation yet, to cancel: close the window.", "Opearion error");
                }
                else
                {
                    testInstance.SetContent(Operations.Add);
                    testInstance.Activate(); // ACTIVATE THE WINDOW AFTER SETTING THE NEW CONTENT
                }
            }
        }

        private void UpdateTestItem_Click(object sender, RoutedEventArgs e)
        {
            Test testToUpdate = (Test)testsDataGrid.SelectedItem;
            if (testToUpdate == null)
                MessageBox.Show("Select test to update and try again.", "No test selected");
            else if (testToUpdate.Finished)
                MessageBox.Show("Can't update test that is already finished.");
            else
            {
                if (testInstance == null)
                {
                    testInstance = new TestWindow(Operations.Update, testToUpdate);
                    testInstance.Closed += TestInstaceClosed;
                    testInstance.DataChanged += UpdateData;
                    testInstance.Show();
                }
                else
                {
                    if (!testInstance.CurrentOperation.Finished)
                    {
                        testInstance.Activate(); // ACTIVATE THE WINDOW BEFORE THE MESSAGE BOX IS SHOWN
                        MessageBox.Show("You haven't completed your operation yet, to cancel: close the window.", "Opearion error");
                    }
                    else
                    {
                        testInstance.SetContent(Operations.Update, testToUpdate);
                        testInstance.Activate(); // ACTIVATE THE WINDOW AFTER SETTING THE NEW CONTENT
                    }
                }
            }
        }

        private void ViewTestMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Test testToView = (Test)testsDataGrid.SelectedItem;
            if (testToView == null)
                MessageBox.Show("Select test to view and try again.", "No test selected");
            else
            {
                if (testInstance == null)
                {
                    testInstance = new TestWindow(Operations.View, testToView);
                    testInstance.Closed += TestInstaceClosed;
                    testInstance.DataChanged += UpdateData;
                    testInstance.Show();
                }
                else
                {
                    if (!testInstance.CurrentOperation.Finished)
                    {
                        testInstance.Activate(); // ACTIVATE THE WINDOW BEFORE THE MESSAGE BOX IS SHOWN
                        MessageBox.Show("You haven't completed your operation yet, to cancel: close the window.", "Opearion error");
                    }
                    else
                    {
                        testInstance.SetContent(Operations.View, testToView);
                        testInstance.Activate(); // ACTIVATE THE WINDOW AFTER SETTING THE NEW CONTENT
                    }
                }
            }
        }

        private void AddTraineeItem_Click(object sender, RoutedEventArgs e)
        {
            if (traineeInstance == null)
            {
                traineeInstance = new TraineeWindow(Operations.Add);
                traineeInstance.Closed += TraineeInstaceClosed;
                traineeInstance.DataChanged += UpdateData;
                traineeInstance.Show();
            }
            else
            {
                if (!traineeInstance.CurrentOperation.Finished)
                {
                    traineeInstance.Activate(); // ACTIVATE THE WINDOW BEFORE THE MESSAGE BOX IS SHOWN
                    MessageBox.Show("You haven't completed your operation yet, to cancel: close the window.", "Opearion error");
                }
                else
                {
                    traineeInstance.SetContent(Operations.Add);
                    traineeInstance.Activate(); // ACTIVATE THE WINDOW AFTER SETTING THE NEW CONTENT
                }
            }
        }

        private void UpdateTraineeItem_Click(object sender, RoutedEventArgs e)
        {
            Trainee traineeToUpdate = (Trainee)traineesDataGrid.SelectedItem;
            if (traineeToUpdate == null)
                MessageBox.Show("Select trainee to update and try again.", "No trainee selected");
            else
            {
                if (traineeInstance == null)
                {
                    traineeInstance = new TraineeWindow(Operations.Update, traineeToUpdate);
                    traineeInstance.Closed += TraineeInstaceClosed;
                    traineeInstance.DataChanged += UpdateData;
                    traineeInstance.Show();
                }
                else
                {
                    if (!traineeInstance.CurrentOperation.Finished)
                    {
                        testInstance.Activate(); // ACTIVATE THE WINDOW BEFORE THE MESSAGE BOX IS SHOWN
                        MessageBox.Show("You haven't completed your operation yet, to cancel: close the window.", "Opearion error");
                    }
                    else
                    {
                        traineeInstance.SetContent(Operations.Update, traineeToUpdate);
                        traineeInstance.Activate(); // ACTIVATE THE WINDOW AFTER SETTING THE NEW CONTENT
                    }
                }
            }
        }

        private void ViewTraineeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Trainee traineeToView = (Trainee)traineesDataGrid.SelectedItem;
            if (traineeToView == null)
                MessageBox.Show("Select trainee to view and try again.", "No trainee selected");
            else
            {
                if (traineeInstance == null)
                {
                    traineeInstance = new TraineeWindow(Operations.View, traineeToView);
                    traineeInstance.Closed += TraineeInstaceClosed;
                    traineeInstance.DataChanged += UpdateData;
                    traineeInstance.Show();
                }
                else
                {
                    if (!traineeInstance.CurrentOperation.Finished)
                    {
                        traineeInstance.Activate(); // ACTIVATE THE WINDOW BEFORE THE MESSAGE BOX IS SHOWN
                        MessageBox.Show("You haven't completed your operation yet, to cancel: close the window.", "Opearion error");
                    }
                    else
                    {
                        traineeInstance.SetContent(Operations.View, traineeToView);
                        traineeInstance.Activate(); // ACTIVATE THE WINDOW AFTER SETTING THE NEW CONTENT
                    }
                }
            }
        }

        private void RemoveTestItem_Click(object sender, RoutedEventArgs e)
        {
            Test testToRemove = (Test)testsDataGrid.SelectedItem;
            if (testToRemove == null)
                MessageBox.Show("Select test to remove and try again.", "No test selected");
            else
            {
                try
                {
                    bl.RemoveTest(testToRemove);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Exception thrown");
                }
            }
            UpdateData();
        }

        private void RemoveTraineeItem_Click(object sender, RoutedEventArgs e)
        {
            Trainee traineeToRemove = (Trainee)traineesDataGrid.SelectedItem;
            if (traineeToRemove == null)
                MessageBox.Show("Select trainee to remove and try again.", "No test selected");
            else
            {
                try
                {
                    bl.RemoveTrainee(traineeToRemove);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Exception thrown");
                }
            }
            UpdateData();
        }

        private void TestersDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ViewTesterMenuItem_Click(this, new RoutedEventArgs());
        }

        private void TestsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ViewTestMenuItem_Click(this, new RoutedEventArgs());
        }

        private void TraineesDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ViewTraineeMenuItem_Click(this, new RoutedEventArgs());
        }

        private void FutureTestsItem_Click(object sender, RoutedEventArgs e)
        {
            QueriesWindow q = new QueriesWindow("futureTests");
            q.Show();
        }

        #endregion        
    }
}
