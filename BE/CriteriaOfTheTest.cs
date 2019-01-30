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
    public class CriteriaOfTheTest
    {

        #region Constructor

        public CriteriaOfTheTest() { CriterionDescription = ""; SuccessfullyPassed = false; Remarks = ""; }

        public CriteriaOfTheTest(string description, bool passed, string remarks)
        {
            CriterionDescription = description;
            SuccessfullyPassed = passed;
            Remarks = remarks;
        }

        #endregion

        #region Properties

        public string Remarks { get; set; }
        public string CriterionDescription { get; set; }
        public bool SuccessfullyPassed { get; set; }

        #endregion
    }
}
