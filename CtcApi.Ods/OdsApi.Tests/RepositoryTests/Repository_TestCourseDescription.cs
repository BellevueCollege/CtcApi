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

using System;
using System.Collections.Generic;
using Ctc.Ods.Data;
using Ctc.Ods.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ctc.Ods.Tests
{
  /// <summary>
  ///This is a test class for CoursesTest and is intended
  ///to contain all CoursesTest Unit Tests
  ///</summary>
  [TestClass()]
  public class Repository_TestCourseDescription
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

    [TestMethod]
    public void RetrievingPresentAndFutureDescriptions()
    {
      using (OdsRepository ods = new OdsRepository())
      {
        ICourseID courseID = CourseID.FromString("ECON&201");
        IList<CourseDescription> descs =  ods.GetCourseDescription(courseID, YearQuarter.FromString("B234"));

        Assert.IsNotNull(descs);

        foreach (CourseDescription desc in descs)
        {
          Console.Out.WriteLine("[{0}] {1}", desc.YearQuarterBegin, desc.Description);
        }

        Assert.IsTrue(descs.Count > 1, "ECON& 201 should have 2 descriptions, but only has {0}", descs.Count);
      }
    }

  }
}
