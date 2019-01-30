using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// שם: דניאל רוטנמר
// ת.ז: 316240837
// שותף: רפאל לרמן
// פלאפון: 0583266859
// מרצה: יאיר גולדשטיין
// קורס: מיני פרוייקט במערכות חלונות

namespace BE
{
    public static class Configuration
    {
        public static int MinimumNumberOfLessons = 20;
        public static int MinimumTraineeAge = 18;
        public static int MinimumTesterAge = 40;
        public static int MinimumIntervalOfDaysBetween2Tests = 7;

        private static int testId = 0;
        private static int traineeId = 0;
        private static int testerId = 0;

        public static int GetTestId()
        {
            testId++;
            return testId;
        }

        public static int GetTesterId()
        {
            testerId++;
            return testerId;
        }

        public static int GetTraineeId()
        {
            traineeId++;
            return traineeId;
        }
    }
}
