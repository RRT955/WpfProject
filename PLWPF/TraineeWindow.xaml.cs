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

namespace PLWPF
{
    /// <summary>
    /// Interaction logic for TraineeWindow.xaml
    /// </summary>
    public partial class TraineeWindow : Window
    {

        #region Constructor

        public TraineeWindow(Operations method, Trainee tester = null)
        {
            InitializeComponent();
            bl = (Bl_imp)FactoryBl.GetBl();
            SetContent(method, tester);
            RegisterEvents();
            dateOfBirthTB.DisplayDateEnd = DateTime.Now;
            if (method == Operations.Add)
                dateOfBirthTB.DisplayDate = DateTime.Now.AddYears(-Configuration.MinimumTraineeAge);
            else
                dateOfBirthTB.IsEnabled = false;
        }

        #endregion

        #region Events

        public DataChangedEventHandler DataChanged;

        #endregion

        #region Private Variables

        private Bl_imp bl;
        private Trainee traineeToViewOrUpdate = null;

        #endregion

        #region Properties

        public Operation CurrentOperation
        {
            get; set;
        }

        #endregion

        #region Methods

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
            }
        }

        public void SetContent(Operations method, Trainee trainee = null)
        {
            traineeToViewOrUpdate = trainee;
            waitingLabel.Visibility = Visibility.Hidden;
            traineeDetailsPanel.Visibility = Visibility.Visible;
            CurrentOperation = new Operation();
            CurrentOperation.Method = method;
            CurrentOperation.Finished = (method == Operations.View) ? true : false;
            Title = method.ToString() + " trainee";
            addOrUpdateTraineeButton.Content = method.ToString() + " trainee";
            if (method == Operations.Add)
                Reset();
            else if (trainee != null) // Update or View, teater != null
            {
                firstNameTB.Text = trainee.FirstName;
                lastNameTB.Text = trainee.LastName;
                dateOfBirthTB.SelectedDate = trainee.DateOfBirth;
                genderCB.SelectedItem = trainee.Gender;
                phoneTB.Text = trainee.PhoneNumber;
                AddressTB.Text = trainee.Address.AddressAsString; 
                carTypeCB.SelectedItem = trainee.CarType;
                numberOfDrivingLessons.NumValue = trainee.NumberOfDrivingLessons;
                schoolName.Text = trainee.DrivingSchoolName;
                teacherName.Text = trainee.TeacherName;
                gearboxCB.SelectedItem = trainee.GearboxType;
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
            addOrUpdateTraineeButton.Visibility = enabled ? Visibility.Visible : Visibility.Hidden;
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
            if (firstNameTB.Text != "" && lastNameTB.Text != "" && dateOfBirthTB.SelectedDate != null && Regex.Matches(AddressTB.Text, ",").Count == 3
                && genderCB.SelectedIndex > -1 && phoneTB.Text != "" && !Regex.IsMatch(phoneTB.Text, @"[a-zA-Z^%&$#@!*]") && !AddressTB.Text.Replace(" ", "").Contains(",,")
                && AddressTB.Text != "Country, City, Street, Number" && AddressTB.Text != "" && !Regex.IsMatch(AddressTB.Text.Substring(AddressTB.Text.LastIndexOf(",")), @"[a-zA-Z^%&$#@!*]")
                && schoolName.Text != "" && teacherName.Text != "" && numberOfDrivingLessons.NumValue > 0 && gearboxCB.SelectedIndex > -1)
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
            gearboxCB.SelectedIndex = -1;
            phoneTB.Text = "";
            AddressTB.Text = "Country, City, Street, Number";
            numberOfDrivingLessons.NumValue = 0;
            carTypeCB.SelectedIndex = -1;
            schoolName.Text = "";
            teacherName.Text = "";
        }

        #endregion

        #region Events Execution Data

        private void InputChangedCheckErrors(object sender, EventArgs e)
        {
            if (!CurrentOperation.Finished)
                HasErrors();
        }

        private void AddOrUpdateTraineeButton_Click(object sender, RoutedEventArgs e)
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
                try
                {
                    if (CurrentOperation.Method == Operations.Add)
                    {
                        bl.AddTrainee(new Trainee(firstNameTB.Text, lastNameTB.Text, (DateTime)dateOfBirthTB.SelectedDate, (Gender)genderCB.SelectedItem,
                            phoneTB.Text, new Address(country, city, street, number), (CarType)carTypeCB.SelectedItem, (GearboxType)gearboxCB.SelectedItem,
                            schoolName.Text, numberOfDrivingLessons.NumValue, teacherName.Text));
                    }
                    else
                    {
                        Trainee trainee = new Trainee(firstNameTB.Text, lastNameTB.Text, (DateTime)dateOfBirthTB.SelectedDate, (Gender)genderCB.SelectedItem,
                            phoneTB.Text, new Address(country, city, street, number), (CarType)carTypeCB.SelectedItem, (GearboxType)gearboxCB.SelectedItem,
                            schoolName.Text, numberOfDrivingLessons.NumValue, teacherName.Text);
                        trainee.Id = traineeToViewOrUpdate.Id;
                        bl.UpdateTrainee(trainee);
                    }
                    DataChanged?.Invoke();
                    CurrentOperation.Finished = true;
                    Reset();
                    traineeDetailsPanel.Visibility = Visibility.Hidden;
                    waitingLabel.Content = "Successfully " + CurrentOperation.Method.ToString() + "ed! Select new operation from the main window.";
                    waitingLabel.Visibility = Visibility.Visible;
                    Title = "Waiting for new operation...";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Exception thrown");
                }
            }
        }

        #endregion
    }
}
