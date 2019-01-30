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
    /// Interaction logic for QueriesWindow.xaml
    /// </summary>
    public partial class QueriesWindow : Window
    {
        public QueriesWindow(string q)
        {
            InitializeComponent();
            query = q;
            bl = (Bl_imp)FactoryBl.GetBl();
            RunQuery();
        }

        #region Private Variables

        private Bl_imp bl;
        private string query;

        #endregion

        #region Methods

        private void RunQuery()
        {
            if (query == "futureTests")
            {
                valueLabel.Content = "";
                var futureTests = from test in bl.GetTests() where test.DateOfTest > DateTime.Now select test;
                queryListBox.DataContext = futureTests;
                queryListBox.ItemsSource = futureTests;
            }
        }

        #endregion
    }
}
