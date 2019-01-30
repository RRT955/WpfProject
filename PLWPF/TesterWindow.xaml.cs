using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

// This window will handle the following operations:
// 1) Add new tester.
// 2) Update existing tester.

namespace PLWPF
{
    /// <summary>
    /// Interaction logic for TesterWindow.xaml
    /// </summary>
    public partial class TesterWindow : Window
    {
        #region Constructor

        /// <summary>
        /// method = 1 of {"Add", "Update"}
        /// tester = in case of update => the tester to update, otherwise => null
        /// </summary>
        /// <param name="method"></param>
        /// <param name="tester"></param>
        public TesterWindow(Operations method, Tester tester = null)
        {
            InitializeComponent();
            bl = (Bl_imp)FactoryBl.GetBl();
            SetContent(method, tester);
            RegisterEvents();
            dateOfBirthTB.DisplayDateEnd = DateTime.Now;
            if (method == Operations.Add)
                dateOfBirthTB.DisplayDate = DateTime.Now.AddYears(-Configuration.MinimumTesterAge);
            else
                dateOfBirthTB.IsEnabled = false;
        }

        #endregion

        #region General Methods

        private void RegisterEvents()
        {
            if (CurrentOperation.Method != Operations.View)
            {
                foreach (Control control in innerGrid.Children)
                {
                    if (control.GetType() == typeof(TextBox))
                        ((TextBox)control).TextChanged += InputChangedCheckErrors;
                    else if (control.GetType() == typeof(NumericUpDown))
                        ((NumericUpDown)control).ValueChanged += InputChangedCheckErrors;
                    else if (control.GetType() == typeof(ComboBox) || control.GetType() == typeof(WeeklySchedule))
                        ((ComboBox)control).SelectionChanged += InputChangedCheckErrors;
                    else if (control.GetType() == typeof(DatePicker))
                        ((DatePicker)control).SelectedDateChanged += InputChangedCheckErrors;
                }
                schedule.SelectionChanged += InputChangedCheckErrors;
            }
        }

        // CAHNGES THE WINDOW CONTENT FOR 3 METHODS: ADD & UPDATE & VIEW
        public void SetContent(Operations method, Tester tester = null)
        {
            testerToUpdateOrView = tester;
            waitingLabel.Visibility = Visibility.Hidden;
            testerDetailsPanel.Visibility = Visibility.Visible;
            CurrentOperation = new Operation();
            CurrentOperation.Method = method;
            CurrentOperation.Finished = (method == Operations.View) ? true : false;
            Title = method.ToString() + " tester";
            addOrUpdateTesterButton.Content = method.ToString() + " tester";
            if (method == Operations.Add)
                Reset();
            else if (tester != null) // Update or View, teater != null
            {
                firstNameTB.Text = tester.FirstName;
                lastNameTB.Text = tester.LastName;
                dateOfBirthTB.SelectedDate = tester.DateOfBirth;
                genderCB.SelectedItem = tester.Gender;
                phoneTB.Text = tester.PhoneNumber;
                AddressTB.Text = tester.Address.AddressAsString;
                yearsOfExperience.NumValue = tester.YearsOfExperience;
                carTypeCB.SelectedItem = tester.CarType;
                maxTestsForWeek.NumValue = tester.MaximumPossibleWeeklyTests;
                maxDistanceForTest.NumValue = tester.MaxDistanceOfTestLocation;
                schedule.SetHours(tester.WorkSchedule);
            }
            if (method == Operations.View)
                DisableOrEnableControls(false);
            else
                DisableOrEnableControls(true);
            errorLabel.Visibility = Visibility.Hidden;
        }

        private void DisableOrEnableControls(bool enabled)
        {
            foreach (Control control in innerGrid.Children)
                control.IsEnabled = enabled;
            schedule.IsEnabled = enabled;
            addOrUpdateTesterButton.Visibility = enabled ? Visibility.Visible : Visibility.Hidden;
        }

        // ERRORS
        private bool HasErrors()
        {
            string[] address = AddressTB.Text.Split(',');
            if (Regex.IsMatch(phoneTB.Text, @"[a-zA-Z^%&$#@!*]"))
            {
                errorLabel.Content = "Phone number can't consist of letters, only numbers.";
                errorLabel.Visibility = Visibility.Visible;
                return true;
            }
            if ((Regex.Matches(AddressTB.Text, ",").Count != 3 && AddressTB.Text != "") || AddressTB.Text.Replace(" ", "").Contains(",,")
                || ((from s in address where s.Replace(" ", "") == "" select s).Count() > 0))
            {
                errorLabel.Content = "Enter your address in the correct format: Country, City, Street, Number";
                errorLabel.Visibility = Visibility.Visible;
                return true;
            }
            if (Regex.IsMatch(AddressTB.Text.Substring(AddressTB.Text.LastIndexOf(",")), @"[a-zA-Z^%&$#@!*]") && AddressTB.Text != "Country, City, Street, Number")
            {
                errorLabel.Content = "Your building number can't consist of letters, only numbers.";
                errorLabel.Visibility = Visibility.Visible;
                return true;
            }
            if (schedule.HasInput() && schedule.NumberOfHours() <= 11)
            {
                errorLabel.Content = "Can't add tester whose working schedule contains less than 11 hours.";
                errorLabel.Visibility = Visibility.Visible;
                return true;
            }
            if (firstNameTB.Text != "" && lastNameTB.Text != "" && dateOfBirthTB.SelectedDate != null && Regex.Matches(AddressTB.Text, ",").Count == 3
                && genderCB.SelectedIndex > -1 && phoneTB.Text != "" && !Regex.IsMatch(phoneTB.Text, @"[a-zA-Z^%&$#@!*]") && !AddressTB.Text.Replace(" ", "").Contains(",,")
                && AddressTB.Text != "Country, City, Street, Number" && AddressTB.Text != "" && !Regex.IsMatch(AddressTB.Text.Substring(AddressTB.Text.LastIndexOf(",")), @"[a-zA-Z^%&$#@!*]") && yearsOfExperience.NumValue > 1 && carTypeCB.SelectedIndex > -1
                && maxTestsForWeek.NumValue > 1 && maxDistanceForTest.NumValue > 1 && schedule.HasInput() && schedule.NumberOfHours() > 11)
            {
                errorLabel.Visibility = Visibility.Hidden;
                return false;
            }
            errorLabel.Content = "Fill required fields!";
            errorLabel.Visibility = Visibility.Visible;
            return true;
        }

        // RESET WINDOW
        private void Reset()
        {
            firstNameTB.Text = "";
            lastNameTB.Text = "";
            dateOfBirthTB.SelectedDate = null;
            genderCB.SelectedIndex = -1;
            phoneTB.Text = "";
            AddressTB.Text = "Country, City, Street, Number";
            yearsOfExperience.NumValue = 0;
            carTypeCB.SelectedIndex = -1;
            maxTestsForWeek.NumValue = 0;
            maxDistanceForTest.NumValue = 0;
            schedule.Reset();
        }

        #endregion

        #region Events

        public event DataChangedEventHandler DataChanged;

        #endregion

        #region Private Variables

        private Bl_imp bl;
        private Tester testerToUpdateOrView = null;

        #endregion

        #region Properties

        public Operation CurrentOperation
        {
            get; set;
        }

        #endregion

        #region Events Execution Data

        private void InputChangedCheckErrors(object sender, EventArgs e)
        {
            if (!CurrentOperation.Finished)
                HasErrors();
        }

        private void AddOrUpdateTesterButton_Click(object sender, RoutedEventArgs e)
        {
            if (!HasErrors())
            {
                string address = AddressTB.Text;
                string country = address.Substring(0, address.IndexOf(','));
                address = address.Substring(address.IndexOf(',') + 1);
                if (address[0] == ' ') address = address.Substring(1);
                string city = address.Substring(0, address.IndexOf(','));
                address = address.Substring(address.IndexOf(',') + 1);
                if (address[0] == ' ') address = address.Substring(1);
                string street = address.Substring(0, address.IndexOf(','));
                address = address.Substring(address.IndexOf(',') + 1);
                int number = Convert.ToInt32(address.Replace(" ", ""));
                //try
                //{
                    if (CurrentOperation.Method == Operations.Add)
                    {
                        bl.AddTester(new Tester(firstNameTB.Text, lastNameTB.Text, (DateTime)dateOfBirthTB.SelectedDate, (Gender)genderCB.SelectedItem,
                            phoneTB.Text.Replace(" ", ""), new Address(country, city, street, number), yearsOfExperience.NumValue, maxTestsForWeek.NumValue,
                            (CarType)carTypeCB.SelectedItem, schedule.Schedule, maxDistanceForTest.NumValue, true, DateTime.MaxValue));
                    }
                    else
                    {
                        Tester tester = new Tester(firstNameTB.Text, lastNameTB.Text, testerToUpdateOrView.DateOfBirth, (Gender)genderCB.SelectedItem,
                            phoneTB.Text.Replace(" ", ""), new Address(country, city, street, number), yearsOfExperience.NumValue, maxTestsForWeek.NumValue,
                            (CarType)carTypeCB.SelectedItem, schedule.Schedule, maxDistanceForTest.NumValue, true, DateTime.MaxValue);
                        tester.Id = testerToUpdateOrView.Id;
                        bl.UpdateTester(tester);
                    }
                    DataChanged?.Invoke();
                    CurrentOperation.Finished = true;
                    Reset();
                    testerDetailsPanel.Visibility = Visibility.Hidden;
                    waitingLabel.Content = "Successfully " + CurrentOperation.Method.ToString() + "ed! Select new operation from the main window.";
                    waitingLabel.Visibility = Visibility.Visible;
                    Title = "Waiting for new operation...";
                /*}
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Exception thrown");
                }*/
            }
        }

        #endregion        
    }
}
