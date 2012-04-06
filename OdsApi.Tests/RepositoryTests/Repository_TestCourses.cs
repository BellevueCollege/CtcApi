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
using System.Collections.Generic;
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
			const string SUBJECT = "ENGL";

			using (OdsRepository repository = new OdsRepository())
			{
				IList<Course> courses = repository.GetCourses(SUBJECT);
				Assert.IsTrue(courses.Count > 0);

				int count = _dataVerifier.GetCourseCount("rtrim(left(replace(CourseID, '&', ' '), 5)) = 'ENGL'");
				Assert.AreEqual(count, courses.Count());
			}
		}

		[TestMethod]
		public void WithCommonCourseSubject_Success()
		{
			const string SUBJECT = "ASTR";

			using (OdsRepository repository = new OdsRepository())
			{
				string ccnSubject = string.Concat(SUBJECT, "&");

				IList<Course> courses = repository.GetCourses(ccnSubject);
				Assert.IsTrue(courses.Count > 0, "No records were returned for '{0}'!", ccnSubject);

				int count = _dataVerifier.GetCourseCount(string.Format("rtrim(left(replace(CourseID, '&', ' '), 5)) = '{0}'", SUBJECT));
				Assert.AreEqual(count, courses.Count());
			}
		}

		[TestMethod]
		public void GetCourses_WithCourseID_Success()
		{
			using (OdsRepository repository = new OdsRepository())
			{
				IList<Course> courses = repository.GetCourses(CourseID.FromString("ASTR 101"));
				Assert.IsTrue(courses.Count > 0);

				int count = _dataVerifier.GetCourseCount("CourseID like 'ASTR%101'");
				Assert.AreEqual(count, courses.Count);
			}
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
			using (OdsRepository repository = new OdsRepository())
			{
				IList<string> subjects = new List<string> {"ENGL", "ENGL&"};
				IList<Course> courses = repository.GetCourses(subjects);
				Assert.IsTrue(courses.Count > 0);

				int count = _dataVerifier.GetCourseCount("(rtrim(left(CourseID, 5)) = 'ENGL' or rtrim(left(CourseID, 5)) = 'ENGL&')");
				Assert.AreEqual(count, courses.Count());
			}
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


	}
}
