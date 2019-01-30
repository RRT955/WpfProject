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

// שם: דניאל רוטנמר
// ת.ז: 316240837
// שותף: רפאל לרמן
// פלאפון: 0583266859
// מרצה: יאיר גולדשטיין
// קורס: מיני פרוייקט במערכות חלונות

namespace PLWPF
{
    public class WorkHour
    {
        public WorkHour(string work) { Work = work; }
        public string Work { get; set; }
    }

    /// <summary>
    /// Interaction logic for WeeklySchedule.xaml
    /// </summary>
    public partial class WeeklySchedule : UserControl
    {
        public event EventHandler SelectionChanged;

        public WeeklySchedule()
        {
            InitializeComponent();
            RegisterEvents();
        }

        private void RegisterEvents()
        {
            foreach (Control ctrl in schedule.Children)
                if (ctrl.GetType() == typeof(CheckBox))
                    ((CheckBox)ctrl).Checked += TriggerSelectionChanged;
        }

        private void TriggerSelectionChanged(object sender, EventArgs e)
        {
            SelectionChanged?.Invoke(this, new EventArgs());
        }

        private bool[,] scheduleBindingArray = null;

        public bool[,] Schedule
        {
            get
            {
                bool[] temp = new bool[30];
                int i = 0;
                foreach (Control ctrl in schedule.Children)
                    if (ctrl.GetType() == typeof(CheckBox))
                    {
                        temp[i] = (bool)((CheckBox)ctrl).IsChecked;
                        i++;
                    }
                scheduleBindingArray = new bool[6, 5];
                for (int a = 0; a < 5; a++)
                    for (int b = 0; b < 6; b++)
                        scheduleBindingArray[b, a] = temp[a * 6 + b];
                return scheduleBindingArray;
            }
        }

        // DETERMINES IF ARE ANY CHECKBOXES CHECKED OR NOT
        public bool HasInput()
        {
            bool hasInput = false;
            foreach (Control ctrl in schedule.Children)
                if (ctrl.GetType() == typeof(CheckBox))
                    if ((bool)((CheckBox)ctrl).IsChecked)
                    {
                        hasInput = true;
                        break;
                    }
            return hasInput;
        }

        // RETURNS THE NUMBER OF HOURS SELECTED
        public int NumberOfHours()
        {
            int hours = 0;
            foreach (Control ctrl in schedule.Children)
                if (ctrl.GetType() == typeof(CheckBox))
                    if ((bool)((CheckBox)ctrl).IsChecked)
                        hours++;
            return hours;
        }

        public void SetHours(bool[,] hours)
        {
            bool[] temp = new bool[30];
            for (int a = 0; a < 5; a++)
                for (int b = 0; b < 6; b++)
                    temp[a * 6 + b] = hours[b, a];
            int i = 0;
            foreach (Control ctrl in schedule.Children)
                if (ctrl.GetType() == typeof(CheckBox))
                {
                    ((CheckBox)ctrl).IsChecked = temp[i];
                    i++;
                }
        }

        // RESETS THE ALL CHECKBOXES
        public void Reset()
        {
            foreach (Control ctrl in schedule.Children)
                if (ctrl.GetType() == typeof(CheckBox))
                    if ((bool)((CheckBox)ctrl).IsChecked)
                        ((CheckBox)ctrl).IsChecked = false;
        }
    }
}
