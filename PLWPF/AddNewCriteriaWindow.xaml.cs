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
using System.Windows.Shapes;
using BE;

// שם: דניאל רוטנמר
// ת.ז: 316240837
// שותף: רפאל לרמן
// פלאפון: 0583266859
// מרצה: יאיר גולדשטיין
// קורס: מיני פרוייקט במערכות חלונות

namespace PLWPF
{
    /// <summary>
    /// Interaction logic for AddNewCriteriaWindow.xaml
    /// </summary>
    public partial class AddNewCriteriaWindow : Window
    {
        #region Constructor

        public AddNewCriteriaWindow()
        {
            InitializeComponent();
            NewCriteria = new List<CriteriaOfTheTest>();
        }

        #endregion

        #region Events

        public event DataChangedEventHandler DataChanged;

        #endregion

        #region Properties

        public List<CriteriaOfTheTest> NewCriteria { get; set; }

        #endregion

        #region Events Execution Data

        private void AddCriteriaButton_Click(object sender, RoutedEventArgs e)
        {
            NewCriteria.Add(new CriteriaOfTheTest(criteriaNameTB.Text, (bool)passedCB.IsChecked, remarksTB.Text));
            DataChanged?.Invoke();
        }

        private void NotPassedCB_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)notPassedCB.IsChecked)
                passedCB.IsChecked = false;
            EnableOrDisableAddButton();
        }

        private void PassedCB_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)passedCB.IsChecked)
                notPassedCB.IsChecked = false;
            EnableOrDisableAddButton();
        }

        private void CriteriaNameTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            EnableOrDisableAddButton();
        }

        #endregion

        #region Methods

        private void EnableOrDisableAddButton()
        {
            if (criteriaNameTB.Text != "" && ((bool)passedCB.IsChecked || (bool)notPassedCB.IsChecked))
                addCriteriaButton.IsEnabled = true;
            else
                addCriteriaButton.IsEnabled = false;
        }

        #endregion

    }
}
