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
using System.Diagnostics;
using System.Linq;
using Ctc.Ods.Data;
using Ctc.Ods.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ctc.Ods.Tests.RepositoryTests
{
	/// <summary>
	/// Summary description for Repository_TestCourses
	/// </summary>
	[TestClass]
	public class Repository_TestCourses
	{
		#region Test Infrastructure
		private DataVerificationHelper _dataVerifier;

		public Repository_TestCourses()
		{
			_dataVerifier = new DataVerificationHelper();
		}

		~Repository_TestCourses()
		{
			_dataVerifier.Dispose();
		}

		private TestContext testContextInstance;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get {return testContextInstance;}
			set {testContextInstance = value;}
		}

		#region Additional test attributes
		//
		// You can use the following additional attributes as you write your tests:
		//
		// Use ClassInitialize to run code before running the first test in the class
		// [ClassInitialize()]
		// public static void MyClassInitialize(TestContext testContext) { }
		//
		// Use ClassCleanup to run code after all tests in a class have run
		// [ClassCleanup()]
		// public static void MyClassCleanup() { }
		//
		// Use TestInitialize to run code before running each test 
		// [TestInitialize()]
		// public void MyTestInitialize() { }
		//
		// Use TestCleanup to run code after each test has run
		// [TestCleanup()]
		// public void MyTestCleanup() { }
		//
		#endregion

		#endregion

		[TestMethod]
		public void WithSubject_Success()
		{
			AssertCourseCount("NURS");
		}

		[TestMethod]
		public void WithCommonCourseSubject_Success()
		{
			AssertCourseCount("ASTR&");
		}

		[TestMethod]
		public void GetCourses_WithCourseID_Success()
		{
			AssertCourseCount("ART", "101");
		}

		[TestMethod]
		public void WithCourseID_ACCT_CommonCourse_Success()
		{
			AssertCourseCount("ACCT&", "201");
		}

		[TestMethod]
		public void WithCourseID_ACCT_Success()
		{
			AssertCourseCount("ACCT", "101");
		}

		[TestMethod]
		public void GetCourses_VerifyCommonCourseCharacterRemovedFromCourseID()
		{
			using (OdsRepository repository = new OdsRepository())
			{
				IList<Course> courses = repository.GetCourses("CHEM");

				int count = courses.Where(s => s.CourseID.Contains("&")).Count();
				Assert.IsTrue(count == 0, "{0} records found to still contain the CCN character ('&') in the CourseID", count);
			}
		}

		[TestMethod]
		public void GetCourses_VerifySortedByCourseID()
		{
			using (OdsRepository repository = new OdsRepository())
			{
				IList<Course> courses = repository.GetCourses("CHEM");
				string prevCourseID = "    ";

				// TODO: is there a more efficient way to determine that the whole list is sorted?
				foreach (Course course in courses)
				{
					string thisCourseID = course.CourseID;
					Assert.IsTrue(thisCourseID.CompareTo(prevCourseID) >= 0, "Invalid order found: [{0}] is not less than [{1}]", prevCourseID, thisCourseID);

					prevCourseID = thisCourseID;
				}
			}
		}

		/// <summary>
		/// 
		///</summary>
		[TestMethod]
		public void GetCourses_WithSubjectList_Success()
		{
			AssertCourseCount(new List<string> {"ENGL", "ENGL&"});
		}

		/// <summary>
		/// 
		///</summary>
		[TestMethod]
		public void GetCourses_VerifyIsVariableCredit()
		{
			using (OdsRepository repository = new OdsRepository())
			{
				IList<string> subjects = new List<string> {"ENGL", "ENGL&"};
				IList<Course> courses = repository.GetCourses(subjects);
				Assert.IsTrue(courses.Count > 0);

				int count = _dataVerifier.GetCourseCount("(rtrim(left(CourseID, 5)) = 'ENGL' or rtrim(left(CourseID, 5)) = 'ENGL&') AND isnull(VariableCredits, 0) = 1");
				Assert.AreEqual(count, courses.Where(c => c.IsVariableCredits).Count());
			}
		}

    [TestMethod]
    [Ignore]  // specific research, not part of the standard test suite
    public void Research()
    {
      using (OdsRepository repository = new OdsRepository())
      {
        string courseId = "CEO 196";
        IList<Course> courses = repository.GetCourses(CourseID.FromString(courseId));
        Assert.IsTrue(courses.Count > 0);

        var foo = courses.Select(c => c.CourseID);
        Assert.AreEqual(courses.Count, courses.Count(c => c.CourseID == courseId));

      }
    }

		#region Private members
		/// <summary>
		/// 
		/// </summary>
		/// <param name="subject"></param>
		private void AssertCourseCount(string subject)
		{
			using (OdsRepository repository = new OdsRepository())
			{
				IList<Course> courses = repository.GetCourses(subject);
				Assert.IsTrue(courses.Count > 0, "No records were returned for '{0}'!", subject);

				AssertCourseCount(courses, string.Format("rtrim(LEFT(CourseID, 5)) = '{0}'", subject));
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="subject"></param>
		/// <param name="number"></param>
		private void AssertCourseCount(string subject, string number)
		{
			using (OdsRepository repository = new OdsRepository())
			{
				ICourseID courseID = CourseID.FromString(subject, number);
				IList<Course> courses = repository.GetCourses(courseID);
				Assert.IsTrue(courses.Count > 0, "No records were returned for '{0} {1}'!", subject, number);

				AssertCourseCount(courses, string.Format("rtrim(LEFT(CourseID, 5)) = '{0}' and LTRIM(RTRIM(SUBSTRING(CourseID, 6, 5))) = '{1}'", subject, number));
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="subjects"></param>
		private void AssertCourseCount(IList<string> subjects)
		{
			using (OdsRepository repository = new OdsRepository())
			{
				IList<Course> courses = repository.GetCourses(subjects);
				Assert.IsTrue(courses.Count > 0, "No records were returned for '{0}'!", subjects);

				string whereClause = string.Empty;
				if (subjects.Count > 0)
				{
					whereClause = "(";
					for (int i = 0; i < subjects.Count; i++)
					{
						whereClause = string.Concat(whereClause, i > 0 ? " or " : string.Empty, String.Format("rtrim(LEFT(CourseID, 5)) = '{0}'", subjects[i]));
					}
					whereClause = string.Concat(whereClause, ")");
				}

				AssertCourseCount(courses, whereClause);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="courses"></param>
		/// <param name="whereClause"></param>
		private void AssertCourseCount(IList<Course> courses, string whereClause)
		{
#if DEBUG
			Debug.Print("==== All courses returned ({0}) ====", courses.Count);
			foreach (Course course in courses)
			{
				Debug.Print("{0}\tCCN: {1}", course.CourseID, course.IsCommonCourse);
			}
#endif

			int count = _dataVerifier.GetCourseCount(whereClause);
			Assert.AreEqual(count, courses.Count());
		}

		#endregion

	}
}
