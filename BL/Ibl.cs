using System;
using System.Collections.Generic;
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

namespace BL
{
    public interface Ibl
    {
        /// <summary>
        /// Adds the tester to the data source through the data access layer
        /// </summary>
        /// <param name="tester"></param>
        void AddTester(Tester tester);

        /// <summary>
        /// Removes the tester from the data source through the data access layer
        /// </summary>
        /// <param name="tester"></param>
        void RemoveTester(Tester tester);

        /// <summary>
        /// Updates the tester in  the data source through the data access layer
        /// </summary>
        /// <param name="tester"></param>
        void UpdateTester(Tester tester);

        /// <summary>
        /// Adds a trainee to the data source through the data access layer
        /// </summary>
        /// <param name="trainee"></param>
        void AddTrainee(Trainee trainee);

        /// <summary>
        /// removes a trainee from the data source through the data access layer
        /// </summary>
        /// <param name="trainee"></param>
        void RemoveTrainee(Trainee trainee);

        /// <summary>
        /// removes a test from the data source through the data access layer
        /// </summary>
        /// <param name="trainee"></param>
        void RemoveTest(Test test);

        /// <summary>
        /// Updates a trainee in the data source through the data access layer
        /// </summary>
        /// <param name="trainee"></param>
        void UpdateTrainee(Trainee trainee);

        /// <summary>
        /// Adds a new test to the data source through the data access layer
        /// </summary>
        /// <param name="test"></param>
        void AddTest(Test test);

        /// <summary>
        /// Updates a test in the data source (Usually when finished) through the data access layer
        /// </summary>
        /// <param name="test"></param>
        void UpdateTest(Test test);

        // Get Data Methods
        List<Tester> GetTesters();
        List<Tester> GetRetiredTesters();
        List<Trainee> GetRemovedTrainees();
        List<Trainee> GetTrainees();
        List<Test> GetRemovedTests();
        List<Test> GetTests();
    }
}
