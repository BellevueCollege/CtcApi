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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Ctc.Ods.Data;
using Ctc.Ods.Tests.ClassDataFilterTests;
using Ctc.Ods.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ctc.Ods.Tests
{
	/// <summary>
	/// Summary description for OdsRepositoryTest
	/// </summary>
	[TestClass]
	public class OdsRepositoryTest
	{
		private DataVerificationHelper _dataVerifier;

		public OdsRepositoryTest()
		{
			_dataVerifier = new DataVerificationHelper();
		}

		~OdsRepositoryTest()
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
		// You can use the following additional attributes as you write your tests:
		//
		// Use ClassInitialize to run code before running the first test in the class

		//[ClassInitialize()]
		//public static void MyClassInitialize(TestContext testContext)
		//{
		//  try
		//  {
		//    testContext.DataConnection.Open();

		//    using (DbCommand cmd = testContext.DataConnection.CreateCommand())
		//    {
		//      cmd.CommandText = "select top 1 c.ClassID from vw_Class c where c.SectionStatusID1 like '%Z%' order by newid()";

		//      using (DbDataReader rs = cmd.ExecuteReader())
		//      {
		//        testContext.Properties["SectionID_NonClass"] = rs[0].ToString();
		//      }
		//    }
		//  }
		//  catch (Exception ex)
		//  {
		//    Console.WriteLine(ex);
		//  }
		//}

		//
		// Use ClassCleanup to run code after all tests in a class have run
		// [ClassCleanup()]
		// public static void MyClassCleanup() { }
		//
		// Use TestInitialize to run code before running each test 
		//[TestInitialize()]
		//public void MyTestInitialize()
		//{
		//}

		// Use TestCleanup to run code after each test has run
		// [TestCleanup()]
		// public void MyTestCleanup() { }
		//
		#endregion

		#region YearQuarter tests
		/// <summary>
		/// Confirms that the correct current YearQuarter is being retrieved
		/// </summary>
		[TestMethod]
		public void CurrentYearQuarter_Success()
		{
			using (OdsRepository repository = new OdsRepository())
			{
				YearQuarter actual = repository.CurrentYearQuarter;

				string expected = _dataVerifier.CurrentYrq;

				Assert.AreEqual(expected, actual.ID);
			}
		}

		/// <summary>
		/// Confirms retrieval of current registration quarter, + 2 more
		/// </summary>
		/// <remarks>
		///	This funcionality is specifically needed by the online class schedule
		/// </remarks>
		[TestMethod]
		public void GetRegistrationQuarters_CurrentAnd2More()
		{
			using (OdsRepository repository = new OdsRepository())
			{
				IList<YearQuarter> yrqList = repository.GetRegistrationQuarters(3);

				int listCount = yrqList.Count;
				Assert.IsTrue(listCount == 3, "Returned {0} items, expected 3", listCount);	// NOTE: test does not yet account for SUMMER/FALL overlapping registration period

				int i = 0;
				foreach (YearQuarter yrq in yrqList)
				{
					Assert.IsNotNull(yrq, "List index {0} is null", i);
					Assert.IsFalse(string.IsNullOrWhiteSpace(yrq.ID), "ID for item #{0} is null/empty", i);
					Assert.IsFalse(string.IsNullOrWhiteSpace(yrq.FriendlyName), "FriendlyName for item #{0} is null/empty", i++);
				}

				// TODO: Assert first item in list is current YRQ
			}
		}

		/// <summary>
		/// Confirms retrieval of current registration quarter
		/// </summary>
		/// <remarks>
		///	This funcionality is specifically needed by the online class schedule
		/// </remarks>
		[TestMethod]
		public void GetRegistrationQuarters()
		{
			using (OdsRepository repository = new OdsRepository())
			{
				IList<YearQuarter> yrqList = repository.GetRegistrationQuarters();

				int listCount = yrqList.Count;
				Assert.IsTrue(listCount == 1, "Returned {0} items, expected 1", listCount);	// NOTE: test does not yet account for SUMMER/FALL overlapping registration period
				
				// TODO: Assert first item in list is current YRQ
			}
		}

		/// <summary>
		/// Confirms the default retrieval of the current registration quarter when no parameters are specified.
		/// </summary>
		[TestMethod]
		public void GetFutureQuarters()
		{
			using (OdsRepository repository = new OdsRepository())
			{
				IList<YearQuarter> yrqList = repository.GetFutureQuarters();

				int listCount = yrqList.Count;
				Assert.IsTrue(listCount == 1, "Returned {0} items, expected 1", listCount);	// NOTE: test does not yet account for SUMMER/FALL overlapping registration period

				Assert.AreEqual(repository.CurrentYearQuarter.ID, yrqList[0].ID, "The quarter returned is not the current quarter.");
			}
		}

		/// <summary>
		/// Confirms retrieval of current registration quarter, plus 2 future quarters.
		/// </summary>
		[TestMethod]
		public void GetFutureQuarters_CurrentAnd2More()
		{
			using (OdsRepository repository = new OdsRepository())
			{
				IList<YearQuarter> yrqList = repository.GetFutureQuarters(3);

				int listCount = yrqList.Count;
				Assert.IsTrue(listCount == 3, "Returned {0} items, expected 3", listCount);	// NOTE: test does not yet account for SUMMER/FALL overlapping registration period

				int i = 0;
				foreach (YearQuarter yrq in yrqList)
				{
					Assert.IsNotNull(yrq, "List index {0} is null", i);
					Assert.IsFalse(string.IsNullOrWhiteSpace(yrq.ID), "ID for item #{0} is null/empty", i);
					Assert.IsFalse(string.IsNullOrWhiteSpace(yrq.FriendlyName), "FriendlyName for item #{0} is null/empty", i++);
				}

				Assert.AreEqual(repository.CurrentYearQuarter.ID, yrqList[0].ID, "The quarter returned is not the current quarter.");
			}
		}
		#endregion


		#region GetCourses() tests
		/// <summary>
		/// 
		///</summary>
		[TestMethod]
		public void GetCourses_Success()
		{
			int count;
			using (OdsRepository repository = new OdsRepository())
			{
				count = repository.GetCourses().Count();
			}

			Assert.IsTrue(count > 0);
		}

		/// <summary>
		/// Verify that Course data includes descriptions
		///</summary>
		[TestMethod]
		public void GetCourses_WithDescription_Success()
		{
			using (OdsRepository repository = new OdsRepository())
			{
				IList<Course> courses = repository.GetCourses();
				int allCount = courses.Count;
				Assert.IsTrue(allCount > 0, "No courses found.");

				IEnumerable<Course> withNonNullDesc = courses.Where(c => c.Descriptions != null);
				Assert.IsTrue(withNonNullDesc.Count() > 0, "All Descriptions properties are null");

// TODO: resolve issues with some courses not having descriptions
				//IEnumerable<Course> withDescCounts = withNonNullDesc.Where(d => d.Descriptions.Count() > 0);
				////IEnumerable<Course> withoutDescCounts = withNonNullDesc.Where(d => d.Descriptions.Count() <= 0);
				//Assert.AreEqual(allCount, withDescCounts.Count(), "{0} (out of {1}) Courses missing description entries", allCount - withDescCounts.Count(), allCount);
			}
		}

		/// <summary>
		/// Verify the Common Course Numbering flag is set for designated course subjects
		/// </summary>
		[TestMethod]
		public void GetCourses_VerifyCcnFlag_True()
		{
			using (OdsRepository repository = new OdsRepository())
			{
				IList<Course> courses = repository.GetCourses();
				int expectedCount = _dataVerifier.GetCourseCount("CourseID like '%&%'");

				Assert.AreEqual(expectedCount, courses.Where(c => c.IsCommonCourse).Count());
			}
		}

		/// <summary>
		/// Verify the Common Course Numbering flag is NOT set for regular course subjects
		/// </summary>
		[TestMethod]
		public void GetCourses_VerifyCcnFlag_False()
		{
			using (OdsRepository repository = new OdsRepository())
			{
				IList<Course> courses = repository.GetCourses();
				Course course = courses.Where(s => !(s.Subject.EndsWith("&"))).Take(1).Single();

				Assert.IsFalse(course.IsCommonCourse);
			}
		}

		/// <summary>
		/// 
		///</summary>
		[TestMethod]
		public void GetCourses_ByCourseID_Success()
		{
			using (OdsRepository repository = new OdsRepository())
			{
				IEnumerable<ICourse> courses = repository.GetCourses(CourseID.FromString("art 101"));
				Assert.IsNotNull(courses);

				int allCoursesCount = courses.Count();
				Assert.IsTrue(allCoursesCount > 0);

				int filteredCourseCount = courses.Where(c => Regex.IsMatch(c.CourseID.ToUpper(), @"ART\s+101")).Count();
				Assert.AreEqual(allCoursesCount, filteredCourseCount);
			}

		}

		#region Modality tests
		/// <summary>
		/// 
		///</summary>
		[TestMethod]
		public void GetCourses_ModalityTimes_Morning_Success()
		{
			using (OdsRepository repository = new OdsRepository())
			{
				VerifyModalityTimes("00:00", "11:59", repository);
			}
		}

		/// <summary>
		/// 
		///</summary>
		[TestMethod]
		public void GetCourses_ModalityTimes_Afternoon_Success()
		{
			using (OdsRepository repository = new OdsRepository())
			{
				VerifyModalityTimes("12:00", "16:59", repository);
			}
		}

		/// <summary>
		/// 
		///</summary>
		[TestMethod]
		public void GetCourses_ModalityTimes_Evening_Success()
		{
			using (OdsRepository repository = new OdsRepository())
			{
				VerifyModalityTimes("17:00", "23:59", repository);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <param name="repository"></param>
		private void VerifyModalityTimes(string start, string end, OdsRepository repository)
		{
			TimeSpan startTime = TimeSpan.Parse(start);
			TimeSpan endTime = TimeSpan.Parse(end);

			IList<ISectionFacet> facets = TestHelper.GetFacets(new TimeFacet(startTime, endTime));

			int count = repository.GetCourses(facets).Select(c => c.CourseID).Distinct().Count();
			Assert.IsTrue(count > 0, "No sections were returned");

			int allCount = _dataVerifier.GetCourseIDCountForSections("not ClassID is null");
			Assert.IsTrue(allCount > count, "Filtered course count ({0}) is not less than count of all courses ({1}).", count, allCount);
		}

		/// <summary>
		/// 
		///</summary>
		[TestMethod]
		public void GetCourses_Modality_Online_Success()
		{
			using (OdsRepository repository = new OdsRepository())
			{
				IList<ISectionFacet> facets = TestHelper.GetFacets(new ModalityFacet(ModalityFacet.Options.Online));

				IList<Course> courses = repository.GetCourses(facets);
				Debug.Print("==> Online course count before DISTINCT: {0}", courses.Count);

				int count = courses.Select(c => c.CourseID).Distinct().Count();
				Debug.Print("==> Online course count after DISTINCT: {0}", count);
#if DEBUG
				foreach (string courseid in courses.Select(c => c.CourseID).Distinct())
				{
					Debug.Print(courseid);
				}
#endif

				int expectedCount = _dataVerifier.GetCourseIDCountForSections(String.Format("SBCTCMisc1 like '3%'"));
				Assert.AreEqual(expectedCount, count);

				int allCount = _dataVerifier.GetCourseIDCountForSections("not ClassID is null");
				Assert.IsTrue(allCount > count, "Online course count is not less than count of all courses.");
			}
		}

		/// <summary>
		/// 
		///</summary>
		[TestMethod]
		public void GetCourses_Modality_Hybrid_Success()
		{
			using (OdsRepository repository = new OdsRepository())
			{
				IList<ISectionFacet> facets = TestHelper.GetFacets(new ModalityFacet(ModalityFacet.Options.Hybrid));

				IEnumerable<string> courses = repository.GetCourses(facets).Select(c => c.CourseID).Distinct();
				int count = courses.Count();

				// uncomment to get list for comparing
				foreach (string course in courses)
				{
					Console.WriteLine(course);
				}


				int expectedCount = _dataVerifier.GetCourseIDCountForSections(String.Format("isnull(SBCTCMisc1, '') like '8%'"));
				Assert.AreEqual(expectedCount, count);

				int allCount = _dataVerifier.GetCourseIDCountForSections("not ClassID is null");
				Assert.IsTrue(allCount > count, "Online course count is not less than count of all courses.");
			}
		}

		/// <summary>
		/// 
		///</summary>
		[TestMethod]
		public void GetCourses_Modality_Telecourse_Success()
		{
			using (OdsRepository repository = new OdsRepository())
			{
				IList<ISectionFacet> facets = TestHelper.GetFacets(new ModalityFacet(ModalityFacet.Options.Telecourse));

				int count = repository.GetCourses(facets).Select(c => c.CourseID).Distinct().Count();

				int expectedCount = _dataVerifier.GetCourseIDCountForSections(String.Format("SBCTCMisc1 like '1%'"));
				Assert.AreEqual(expectedCount, count);

				int allCount = _dataVerifier.GetCourseIDCountForSections("not ClassID is null");
				Assert.IsTrue(allCount > count, "Online course count is not less than count of all courses.");
			}
		}

		/// <summary>
		/// 
		///</summary>
		[TestMethod]
		public void GetCourses_Modality_OnCampus_Success()
		{
			using (OdsRepository repository = new OdsRepository())
			{
				IList<ISectionFacet> facets = TestHelper.GetFacets(new ModalityFacet(ModalityFacet.Options.OnCampus));

				int count = repository.GetCourses(facets).Select(c => c.CourseID).Distinct().Count();

				int expectedCount = _dataVerifier.GetCourseIDCountForSections(String.Format("isnull(SBCTCMisc1, '') not like '3%' and isnull(SBCTCMisc1, '') not like '8%' and isnull(SBCTCMisc1, '') not like '1%'"));
				Assert.AreEqual(expectedCount, count);

				int allCount = _dataVerifier.GetCourseIDCountForSections("not ClassID is null");
				Assert.IsTrue(allCount > count, "Online course count is not less than count of all courses.");
			}
		}
		#endregion

		#endregion

		/// <summary>
		/// 
		///</summary>
		[TestMethod]
		public void CourseDescription_CourseIDLookup_Success()
		{
			using (OdsRepository repository = new OdsRepository())
			{
				IEnumerable<CourseDescription> courseDescriptions = repository.GetCourseDescription(CourseID.FromString("art 101"));
				Assert.IsNotNull(courseDescriptions, "Method returned null");

				int allCoursesCount = courseDescriptions.Count();
				Assert.IsTrue(allCoursesCount > 0, "{0} descriptions were returned", allCoursesCount);

				int filteredCourseCount = courseDescriptions.Where(c => Regex.IsMatch(c.CourseID.ToUpper(), @"ART\s+101")).Count();
				Assert.AreEqual(allCoursesCount, filteredCourseCount);
			}

		}
	}
}
