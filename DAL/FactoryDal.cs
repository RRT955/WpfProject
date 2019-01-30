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

namespace DAL
{
    public class FactoryDal
    {
        public static Idal GetDalList()
        {
            return Dal_imp.Instance;
        }

        public static Idal GetDalXML()
        {
            return Dal_XML_imp.Instance;
        }
    }

}
