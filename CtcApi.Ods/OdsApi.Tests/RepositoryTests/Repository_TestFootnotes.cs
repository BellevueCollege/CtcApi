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
using System.Diagnostics;
using System.Linq;
using Ctc.Ods.Data;
using Ctc.Ods.Tests.ClassDataFilterTests;
using Ctc.Ods.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ctc.Ods.Tests
{
	/// <summary>
	///This is a test class for CoursesTest and is intended
	///to contain all CoursesTest Unit Tests
	///</summary>
	[TestClass()]
	public class FootnoteTest
	{
		private DataVerificationHelper _dataVerifier;

		public FootnoteTest()
		{
			_dataVerifier = new DataVerificationHelper();
		}

		~FootnoteTest()
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
/* Commented out due to property name collision: Class Schedule's inherited class defines a .CourseFootnotes property.
 *	2/23/2012, shawn.south@bellevuecollege.edu
 *	
		[TestMethod]
		[Ignore]	// NOTE: Added course footnotes to GetSections() calls causes the DB query to timeout. See Task #16.
		public void CourseFootnotesForSections_Success()
		{
			using (OdsRepository repository = new OdsRepository())
			{
				IList<Section> sections = repository.GetSections(TestHelper.GetFacets());
//				IList<Section> sections = repository.GetSections(CourseID.FromString("engl", "093"), facetOptions:TestHelper.GetFacets());
				Assert.IsTrue(sections.Count > 0, "No sections returned");

				IEnumerable<Section> withCourseFootnotes = sections.Where(s => s.CourseFootnotes.Count > 0);
				int actual = withCourseFootnotes.Count();

				int expected = _dataVerifier.GetSectionCount("CourseID in (select c.CourseID from vw_Course c where c.EffectiveYearQuarterEnd <= YearQuarterID and (isnull(c.FootnoteID1, '') <> '' or ISNULL(c.FootnoteID2, '') <> ''))");
				Assert.AreEqual(expected, actual);
			}
		}
*/
		[TestMethod]
//		[Ignore]	// NOTE: Proper implementation requires restructuring the GetCourses() calls. See Task #16.
		public void CourseFootnotes_Success()
		{
			using (OdsRepository repository = new OdsRepository())
			{
				IList<Course> courses = repository.GetCourses();
//				IList<Course> courses = repository.GetCourses("engl");
				Assert.IsTrue(courses.Count > 0, "No courses returned");

#if DEBUG
				Debug.Print("==== All courses returned ({0}) ====", courses.Count);
				foreach (Course course in courses)
				{
					Debug.Print("{0}", course.CourseID);
				}
#endif

				IEnumerable<Course> withFootnotes = courses.Where(c => c.Footnotes.Count > 0);

#if DEBUG
				Debug.Print("==== Courses with footnotes ({0}) ====", withFootnotes.Count());
				foreach (Course course in withFootnotes)
				{
					Debug.Print("{0}\t({1})", course.CourseID, course.Footnotes.Count);
				}
#endif
				int actual = withFootnotes.Count();
				int expected = _dataVerifier.GetCourseCount("((FootnoteID1 IN (SELECT f.footnoteid FROM dbo.vw_Footnote f) or FootnoteID2 IN (SELECT f.FootnoteID FROM dbo.vw_Footnote f)))");
//				int expected = _dataVerifier.GetCourseCount("(isnull(FootnoteID1, '') <> '' or ISNULL(FootnoteID2, '') <> '') and rtrim(left(replace(CourseID, '&', ' '), 5)) = 'engl'");
				Assert.AreEqual(expected, actual);
			}
		}

	}
}
