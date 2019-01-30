using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE;

// שם: דניאל רוטנמר
// ת.ז: 316240837
// שותף: רפאל לרמן
// פלאפון: 0583266859
// מרצה: יאיר גולדשטיין
// קורס: מיני פרוייקט במערכות חלונות

namespace DAL
{
    public interface Idal
    {
        /// <summary>
        /// Adds a new tester to the data source (If satisfied all conditions)
        /// </summary>
        /// <param name="tester"></param>
        void AddTester(Tester tester);

        /// <summary>
        /// Removes a tester from the data source
        /// </summary>
        /// <param name="tester"></param>
        void RemoveTester(Tester tester);

        /// <summary>
        /// Updates a tester in the data source
        /// </summary>
        /// <param name="tester"></param>
        void UpdateTester(Tester tester);

        /// <summary>
        /// Adds a new trainee to the data source (If satisfied all conditions)
        /// </summary>
        /// <param name="trainee"></param>
        void AddTrainee(Trainee trainee);

        /// <summary>
        /// Removes a trainee from the data source
        /// </summary>
        /// <param name="trainee"></param>
        void RemoveTrainee(Trainee trainee);

        /// <summary>
        /// Updates a trainee in the data source
        /// </summary>
        /// <param name="trainee"></param>
        void UpdateTrainee(Trainee trainee);

        /// <summary>
        /// Adds a new test to the data source (If satisfied all conditions)
        /// </summary>
        /// <param name="test"></param>
        void AddTest(Test test);

        /// <summary>
        /// Updates a test in the data source (Usually when finished)
        /// </summary>
        /// <param name="test"></param>
        void UpdateTest(Test test);

        /// <summary>
        /// Removes a test in the data source (Usually when finished)
        /// </summary>
        /// <param name="test"></param>
        void RemoveTest(Test test);

        // Get Data Methods
        List<Tester> GetTesters();
        List<Tester> GetRetiredTesters();
        List<Trainee> GetRemovedTrainees();
        List<Trainee> GetTrainees();
        List<Test> GetRemovedTests();
        List<Test> GetTests();
    }
}
