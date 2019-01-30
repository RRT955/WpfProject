using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BE;
using BL;

// שם: דניאל רוטנמר
// ת.ז: 316240837
// שותף: רפאל לרמן
// פלאפון: 0583266859
// מרצה: יאיר גולדשטיין
// קורס: מיני פרוייקט במערכות חלונות

namespace PLWPF
{
    /// <summary>
    /// Interaction logic for TestWindow.xaml
    /// </summary>
    public partial class TestWindow : Window
    {

        #region Constructor

        public TestWindow(Operations method, Test test = null)
        {
            InitializeComponent();
            bl = (Bl_imp)FactoryBl.GetBl();
            SetContent(method, test);
            RegisterEvents();
        }

        #endregion

        #region General Methods

        private void RegisterEvents()
        {
            if (CurrentOperation.Method != Operations.View)
            {
                foreach (Control control in upGrid.Children)
                {
                    if (control.GetType() == typeof(TextBox))
                        ((TextBox)control).TextChanged += InputChangedCheckErrors;
                    else if (control.GetType() == typeof(NumericUpDown))
                        ((NumericUpDown)control).ValueChanged += InputChangedCheckErrors;
                    else if (control.GetType() == typeof(ComboBox))
                        ((ComboBox)control).SelectionChanged += InputChangedCheckErrors;
                    else if (control.GetType() == typeof(DatePicker))
                        ((DatePicker)control).SelectedDateChanged += InputChangedCheckErrors;
                    if (CurrentOperation.Method == Operations.Update)
                        if (control.GetType() == typeof(CheckBox))
                            ((CheckBox)control).Checked += InputChangedCBCheckErrors;

                }
                foreach (Control control in bottomGrid.Children)
                {
                    if (control.GetType() == typeof(TextBox))
                        ((TextBox)control).TextChanged += InputChangedCheckErrors;
                    else if (control.GetType() == typeof(NumericUpDown))
                        ((NumericUpDown)control).ValueChanged += InputChangedCheckErrors;
                    else if (control.GetType() == typeof(ComboBox))
                        ((ComboBox)control).SelectionChanged += InputChangedCheckErrors;
                    else if (control.GetType() == typeof(DatePicker))
                        ((DatePicker)control).SelectedDateChanged += InputChangedCheckErrors;
                    if (CurrentOperation.Method == Operations.Update)
                        if (control.GetType() == typeof(CheckBox))
                            ((CheckBox)control).Checked += InputChangedCBCheckErrors;

                }
            }
        }

        // CAHNGES THE WINDOW CONTENT FOR 3 METHODS: ADD & UPDATE & VIEW
        public void SetContent(Operations method, Test test = null)
        {
            testToUpdateOrView = test;
            waitingLabel.Visibility = Visibility.Hidden;
            testDetailsPanel.Visibility = Visibility.Visible;
            CurrentOperation = new Operation();
            CurrentOperation.Method = method;
            CurrentOperation.Finished = (method == Operations.View) ? true : false;
            Title = method.ToString() + " test";
            addOrUpdateButton.Content = method.ToString() + " test";
            if (method == Operations.View)
            {
                addOrUpdateButton.Visibility = Visibility.Hidden;
                DisableOrEnableControls(false); // so the user can't change values just view.
            }
            else // add or update
            {
                addOrUpdateButton.Visibility = Visibility.Visible;
                DisableOrEnableControls(true);
            }
            if (method == Operations.Add)
                Reset();
            else if (testToUpdateOrView != null) // Update or View, teater != null
                FillData();
            errorLabel.Visibility = Visibility.Hidden;
        }

        private void FillData()
        {
            testDepartureAddressTB.Visibility = Visibility.Visible;
            testDepartueLabel.Visibility = Visibility.Visible;
            testDepartureAddressTB.IsEnabled = false;
            dateOfTest.IsEnabled = false;
            hourTB.IsEnabled = false;
            testerId.IsEnabled = false;
            traineeId.IsEnabled = false;
            try
            {
                string exceptionMessage = "";
                var trainee = bl.GetTrainees().FirstOrDefault(t => t.Id == testToUpdateOrView.TraineeId);
                var tester = bl.GetTesters().FirstOrDefault(t => t.Id == testToUpdateOrView.TesterId);
                if (trainee == null)
                    exceptionMessage += "The specified trainee not found in the system.";
                if (tester == null)
                    exceptionMessage += "\nThe specified tester not found in the system.";
                if (trainee == null || tester == null)
                    throw new Exception(exceptionMessage);

                traineeId.NumValue = trainee.Id;
                testerId.NumValue = tester.Id;
                hourTB.NumValue = testToUpdateOrView.DateOfTest.Hour;
                dateOfTest.SelectedDate = testToUpdateOrView.DateOfTest;
                addCriteriaButton.Visibility = Visibility.Visible;
                if (!testToUpdateOrView.Finished && testToUpdateOrView.CriteriaOfTheTest.Count == 0)
                {
                    testDepartureAddressTB.Text = testToUpdateOrView.TestDepartureAddress.AddressAsString;
                    addCriteriaButton.Content = "Add criteria";
                    passedCB.Visibility = Visibility.Visible;
                    notPassedCB.Visibility = Visibility.Visible;
                }
                else
                {
                    addCriteriaButton.Content = "View criteria";
                    testDepartureAddressTB.Text = testToUpdateOrView.TestDepartureAddress.AddressAsString;
                    passedCB.IsChecked = testToUpdateOrView.Passed ? true : false;
                    passedCB.Visibility = testToUpdateOrView.Passed ? Visibility.Visible : Visibility.Hidden;
                    notPassedCB.IsChecked = testToUpdateOrView.Passed ? false : true;
                    notPassedCB.Visibility = testToUpdateOrView.Passed ? Visibility.Hidden : Visibility.Visible;
                }
                numOfCriteriaLabel.Content = testToUpdateOrView.CriteriaOfTheTest.Count.ToString() + " Criteria, Only " +
                    ((from c in testToUpdateOrView.CriteriaOfTheTest where c.SuccessfullyPassed select c).Count()).ToString() + " successfully passed.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception thrown");
            }
        }

        private void DisableOrEnableControls(bool enabled)
        {
            foreach (Control control in upGrid.Children)
                if (control.GetType() != typeof(Label))
                    control.IsEnabled = enabled;
            foreach (Control control in bottomGrid.Children)
                if (control.GetType() != typeof(Label))
                    control.IsEnabled = enabled;
        }

        // ERRORS
        private bool HasErrors()
        {
            bool error = false;
            string[] address = testDepartureAddressTB.Text.Split(',');
            if (CurrentOperation.Method == Operations.Add)
            {
                if (testDepartureAddressTB.Text.Contains("Country") || testDepartureAddressTB.Text.Contains("City") || testDepartureAddressTB.Text.Contains("Street") || testDepartureAddressTB.Text.Contains("Number"))
                {
                    errorLabel.Content = "Enter your address in the correct format: Country, City, Street, Number";
                    errorLabel.Visibility = Visibility.Visible;
                    return true;
                }
                if ((Regex.Matches(testDepartureAddressTB.Text, ",").Count != 3 && testDepartureAddressTB.Text != "") || testDepartureAddressTB.Text.Replace(" ", "").Contains(",,")
                || ((from s in address where s.Replace(" ", "") == "" select s).Count() > 0))
                {
                    errorLabel.Content = "Enter your address in the correct format: Country, City, Street, Number";
                    errorLabel.Visibility = Visibility.Visible;
                    return true;
                }
                if (Regex.IsMatch(testDepartureAddressTB.Text.Substring(testDepartureAddressTB.Text.LastIndexOf(",")), @"[a-zA-Z^%&$#@!*]") && testDepartureAddressTB.Text != "Country, City, Street, Number")
                {
                    errorLabel.Content = "Your building number can't consist of letters, only numbers.";
                    errorLabel.Visibility = Visibility.Visible;
                    return true;
                }
                errorLabel.Content = "";
            }
            if (testerId.NumValue > bl.GetTesters().Count() || testerId.NumValue <= 0 && CurrentOperation.Method == Operations.Add)
            {
                errorLabel.Content = "Enter id of an existing tester.";
                errorLabel.Visibility = Visibility.Visible;
                error = true;
            }
            else if (traineeId.NumValue > bl.GetTrainees().Count() || traineeId.NumValue <= 0 && CurrentOperation.Method == Operations.Add)
            {
                errorLabel.Content = "Enter id of an existing trainee.";
                errorLabel.Visibility = Visibility.Visible;
                error = true;
            }
            else
                errorLabel.Content = "";
            if (CurrentOperation.Method == Operations.Update && !CurrentOperation.Finished)
                if ((bool)!passedCB.IsChecked && (bool)!notPassedCB.IsChecked)
                {
                    errorLabel.Content = "Please specify the grade of the test.";
                    errorLabel.Visibility = Visibility.Visible;
                    error = true;
                }
                else
                    errorLabel.Content = "";
            if (CurrentOperation.Method == Operations.Update && !CurrentOperation.Finished)
                if (testToUpdateOrView.CriteriaOfTheTest.Count == 0)
                {
                    errorLabel.Content = "Please specify the criteria of this test.";
                    errorLabel.Visibility = Visibility.Visible;
                    error = true;
                }
                else
                    errorLabel.Content = "";
            if ((hourTB.NumValue < 9 || hourTB.NumValue > 14) && CurrentOperation.Method == Operations.Add)
            {
                errorLabel.Content = "Hour of test can be between 09:00 and 15:00";
                errorLabel.Visibility = Visibility.Visible;
                error = true;
            }
            else if (!error)
                errorLabel.Content = "";
            return error;
        }

        // RESET WINDOW
        private void Reset()
        {
            foreach (Control control in bottomGrid.Children)
            {
                if (control.GetType() == typeof(TextBox))
                    ((TextBox)control).Text = "";
                else if (control.GetType() == typeof(NumericUpDown))
                    ((NumericUpDown)control).NumValue = 0;
                else if (control.GetType() == typeof(ComboBox))
                    ((ComboBox)control).SelectedIndex = -1;
                else if (control.GetType() == typeof(DatePicker))
                    ((DatePicker)control).SelectedDate = null;
                else if (control.GetType() == typeof(CheckBox))
                    ((CheckBox)control).IsChecked = false;
            }
            foreach (Control control in upGrid.Children)
            {
                if (control.GetType() == typeof(TextBox))
                    ((TextBox)control).Text = "";
                else if (control.GetType() == typeof(NumericUpDown))
                    ((NumericUpDown)control).NumValue = 0;
                else if (control.GetType() == typeof(ComboBox))
                    ((ComboBox)control).SelectedIndex = -1;
                else if (control.GetType() == typeof(DatePicker))
                    ((DatePicker)control).SelectedDate = null;
                else if (control.GetType() == typeof(CheckBox))
                    ((CheckBox)control).IsChecked = false;
            }
            numOfCriteriaLabel.Content = "";
            DisableOrEnableControls(true);
            testDepartureAddressTB.Visibility = Visibility.Visible;
            testDepartueLabel.Visibility = Visibility.Visible;
            waitingLabel.Visibility = Visibility.Hidden;
            testDetailsPanel.Visibility = Visibility.Visible;
            addCriteriaButton.Visibility = Visibility.Hidden;
            passedCB.Visibility = Visibility.Hidden;
            notPassedCB.Visibility = Visibility.Hidden;
            testDepartureAddressTB.Text = "Country, City, Street, Number";
            testDepartureAddressTB.IsEnabled = true;
            testerId.IsEnabled = true;
            traineeId.IsEnabled = true;
            dateOfTest.IsEnabled = true;
            hourTB.IsEnabled = true;
        }

        #endregion

        #region Events

        public DataChangedEventHandler DataChanged;

        #endregion

        #region Private Variables

        private Bl_imp bl;
        private Test testToUpdateOrView = null;
        private AddNewCriteriaWindow addCriteriaInstance = null;

        #endregion

        #region Properties

        public Operation CurrentOperation
        {
            get; set;
        }

        #endregion

        #region Events Execution Data

        private void AddCriteriaInstanceClosed(object sender, EventArgs e)
        {
            addCriteriaInstance = null;
        }

        public void AddCriteria()
        {
            testToUpdateOrView.CriteriaOfTheTest = addCriteriaInstance.NewCriteria;
            numOfCriteriaLabel.Visibility = Visibility.Visible;
            numOfCriteriaLabel.Content = testToUpdateOrView.CriteriaOfTheTest.Count.ToString() + " Criteria, Only " +
                ((from c in testToUpdateOrView.CriteriaOfTheTest where c.SuccessfullyPassed select c).Count()).ToString() + " successfully passed.";
        }

        private void InputChangedCBCheckErrors(object sender, EventArgs e)
        {
            if ((bool)passedCB.IsChecked)
                notPassedCB.IsChecked = false;
            else if ((bool)notPassedCB.IsChecked)
                passedCB.IsChecked = false;
            if (!CurrentOperation.Finished)
                HasErrors();
        }

        private void InputChangedCheckErrors(object sender, EventArgs e)
        {
            if (!CurrentOperation.Finished)
                HasErrors();
        }

        private void AddOrUpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (!HasErrors())
            {
                errorLabel.Visibility = Visibility.Hidden;
                try
                {
                    string address = testDepartureAddressTB.Text;
                    string country = address.Substring(0, address.IndexOf(','));
                    address = address.Substring(address.IndexOf(',') + 1);
                    if (address[0] == ' ') address = address.Substring(1);
                    string city = address.Substring(0, address.IndexOf(','));
                    address = address.Substring(address.IndexOf(',') + 1);
                    if (address[0] == ' ') address = address.Substring(1);
                    string street = address.Substring(0, address.IndexOf(','));
                    address = address.Substring(address.IndexOf(',') + 1);
                    int number = Convert.ToInt32(address.Replace(" ", ""));

                    var tester = bl.GetTesters().First(t => t.Id == testerId.NumValue); // can't be null because we already checked it in the 'HasErrors()' function
                    var trainee = bl.GetTrainees().First(t => t.Id == traineeId.NumValue); // can't be null because we already checked it in the 'HasErrors()' function
                    if (CurrentOperation.Method == Operations.Add)
                    {
                        DateTime d = (DateTime)dateOfTest.SelectedDate;
                        DateTime dt = new DateTime(d.Year, d.Month, d.Day, hourTB.NumValue, 0, 0);                        
                        Test test = new Test(tester.CarType, trainee.GearboxType, trainee.Id, tester.Id, dt);
                        test.TestDepartureAddress = new Address(country, city, street, number);
                        test.Finished = false;
                        bl.AddTest(test);
                    }
                    else // update
                    {                        
                        testToUpdateOrView.TestDepartureAddress = new Address(country, city, street, number);
                        testToUpdateOrView.Passed = (bool)passedCB.IsChecked;
                        testToUpdateOrView.Finished = true;
                        Test test = new Test(testToUpdateOrView.CarType, testToUpdateOrView.GearboxType, testToUpdateOrView.TraineeId, testToUpdateOrView.TesterId, testToUpdateOrView.DateOfTest);
                        test.Finished = true;
                        test.Passed = testToUpdateOrView.Passed;
                        test.TestDepartureAddress = testToUpdateOrView.TestDepartureAddress;
                        test.TestId = testToUpdateOrView.TestId;
                        test.CriteriaOfTheTest = testToUpdateOrView.CriteriaOfTheTest;
                        bl.UpdateTest(test);
                    }
                    DataChanged?.Invoke();
                    CurrentOperation.Finished = true;
                    testToUpdateOrView = null;
                    Reset();
                    testDetailsPanel.Visibility = Visibility.Hidden;
                    waitingLabel.Content = "Successfully " + CurrentOperation.Method.ToString() + "ed! Select new operation from the main window.";
                    waitingLabel.Visibility = Visibility.Visible;
                    errorLabel.Visibility = Visibility.Hidden;
                    Title = "Waiting for new operation...";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Exception thrown");
                }
            }
        }

        private void AddCriteriaButton_Click(object sender, RoutedEventArgs e)
        {
            if (addCriteriaInstance == null)
            {
                addCriteriaInstance = new AddNewCriteriaWindow();
                addCriteriaInstance.DataChanged += AddCriteria;
                addCriteriaInstance.Closed += AddCriteriaInstanceClosed;
                addCriteriaInstance.Show();
            }
            else
            {
                addCriteriaInstance.Show();
            }
            addCriteriaInstance.Activate();
        }

        private void HourTB_ValueChanged(object sender, EventArgs e)
        {
            if (hourTB.NumValue < 9 || hourTB.NumValue > 14)
            {
                errorLabel.Content = "Hour of test can be between 09:00 and 15:00";
                errorLabel.Visibility = Visibility.Visible;
                addOrUpdateButton.IsEnabled = false;
            }
            else
                addOrUpdateButton.IsEnabled = true;
        }

        #endregion
    }
}
