//Copyright (C) 2011 Bellevue College and Peninsula College
//
//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU Lesser General Public License as
//published by the Free Software Foundation, either version 3 of the
//License, or (at your option) any later version.
//
//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU Lesser General Public License for more details.
//
//You should have received a copy of the GNU Lesser General Public
//License and GNU General Public License along with this program.
//If not, see <http://www.gnu.org/licenses/>.

using Ctc.Ods.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ctc.Ods.Tests
{
    /// <summary>
    ///This is a test class for CoursesTest and is intended
    ///to contain all CoursesTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CourseTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }


    [TestMethod]
    public void CourseID_PropertyAssignment()
    {
      string courseID = "CEO 196";
      DerivedCourse actual = new DerivedCourse(courseID);
      Assert.AreEqual(courseID, actual.CourseID);
    }

      // Course objects do not allow external setting of CourseID by design
      private class DerivedCourse : Course
      {
        public DerivedCourse(string courseID)
        {
          CourseID = courseID;
        }
      }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

    }
}
