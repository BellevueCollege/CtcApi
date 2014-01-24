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
using System.Data.Common;
using System.Linq;
using Ctc.Ods.Data;
using Ctc.Ods.Tests.ClassDataFilterTests;
using Ctc.Ods.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ctc.Ods.Tests
{
	/// <summary>
	/// Summary description for OdsRepository_TestSections
	/// </summary>
	[TestClass]
	public class OdsRepository_TestSections
	{
		#region Constructor(s), etc.
		private DataVerificationHelper _dataVerifier;

		public OdsRepository_TestSections()
		{
			_dataVerifier = new DataVerificationHelper();
		}

		~OdsRepository_TestSections()
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

		#endregion

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

		[TestMethod]
		public void GetSections_Success()
		{
			using (OdsRepository repository = new OdsRepository())
			{
				IList<Section> sections = repository.GetSections(TestHelper.GetFacets());

				/* Additional Debugging
				string yrq = "B233";
				string[] ins = {"3468", "3472"};
				IEnumerable<Section> sec = sections.Where(s => ins.Contains(s.ID.ItemNumber) && s.ID.YearQuarter == yrq);
				// */

				Assert.IsTrue(sections.Count > 0);
			}
		}

		/// <summary>
		/// 
		///</summary>
		[TestMethod]
		public void GetSections_VerifyInstructor()
		{
			using (OdsRepository repository = new OdsRepository())
			{
				IList<Section> sections = repository.GetSections(CourseID.FromString("ENGL", "101"));

				ISection sec = sections.Where(s => s.Offered != null && s.Offered.Where(o => !string.IsNullOrWhiteSpace(o.InstructorID) &&
																																										!string.IsNullOrWhiteSpace(o.InstructorName)
																																				)
																																				.Count() > 0)
															.Take(1).Single();

				OfferedItem item = sec.Offered.Where(o => !string.IsNullOrWhiteSpace(o.InstructorID) &&
																									!string.IsNullOrWhiteSpace(o.InstructorName)
																			)
																			.Take(1).Single();

				string expected = _dataVerifier.GetInstructorName(item.InstructorID);

				Assert.AreEqual(expected, item.InstructorName);
			}
		}

		[TestMethod]
		public void GetSections_VerifyCourseDescriptions()
		{
			using (OdsRepository repository = new OdsRepository())
			{
				IList<Section> sections = repository.GetSections(TestHelper.GetFacets());
				Assert.IsTrue(sections.Count > 0);

				IEnumerable<Section> withNonNullDesc = sections.Where(s => s.CourseDescriptions != null && s.CourseDescriptions.Count > 0);
				Assert.IsTrue(withNonNullDesc.Count() > 0, "All CourseDescriptions properties are null");
			}
		}

		[TestMethod]
		public void GetSections_VerifyWaitlist()
		{
			using (OdsRepository repository = new OdsRepository())
			{
				IList<Section> sections = repository.GetSections(TestHelper.GetFacets());

				int actualSectionCount = sections.Where(s => s.WaitlistCount > 0).Count();
				int expectedSectionCount = _dataVerifier.GetSectionCount("ClassID in (select w.ClassID from vw_Waitlist w where w.[Status] = 'W-LISTED')");

				Assert.AreEqual(expectedSectionCount, actualSectionCount);
			}
		}

		[TestMethod]
		public void GetSections_VerifySortedByYearQuarterID()
		{
			using (OdsRepository repository = new OdsRepository())
			{
				IList<Section> sections = repository.GetSections(TestHelper.GetFacets());
				string prevYrq = "0000";

				// TODO: is there a more efficient way to determine that the whole list is sorted?
				foreach (ISection section in sections)
				{
					string thisYrq = section.Yrq.ID;
					Assert.IsTrue(thisYrq.CompareTo(prevYrq) >= 0, "Invalid order found: [{0}] is not less than [{1}]", prevYrq, thisYrq);

					prevYrq = thisYrq;
				}
			}
		}

		[TestMethod]
		public void GetSections_VerifySortedByCourseID()
		{
			using (OdsRepository repository = new OdsRepository())
			{
				IList<Section> sections = repository.GetSections("ENGL", YearQuarter.FromString("B122"));
				string prevCourseID = "    ";

				// TODO: is there a more efficient way to determine that the whole list is sorted?
				foreach (ISection section in sections)
				{
					string thisCourseID = section.CourseID;
					Assert.IsTrue(thisCourseID.CompareTo(prevCourseID) >= 0, "Invalid order found: [{0}] is not less than [{1}]", prevCourseID, thisCourseID);

					prevCourseID = thisCourseID;
				}
			}
		}

		[TestMethod]
		public void GetSections_VerifyCommonCourseCharacterRemovedFromCourseID()
		{
			using (OdsRepository repository = new OdsRepository())
			{
				IList<Section> sections = repository.GetSections("ENGL", YearQuarter.FromString("B122"));

				int count = sections.Where(s => s.CourseID.Contains("&")).Count();
				Assert.IsTrue(count == 0, "{0} records found to still contain the CCN character ('&') in the CourseID", count);
			}
		}

		/// <summary>
		/// Test exclusion of classes that have a 'Z' in the SectionStatusID1 field
		/// </summary>
		[TestMethod]
		public void GetSections_ExcludeNonClassSections()
		{
			string excludedSection = _dataVerifier.GetRandomClassID(@"isnull(SectionStatusID1, '') like '%Z%'");
			// No ClassID meets all of the following for this test
																																//and isnull(SectionStatusID1, '') not like '%X%'
																																//and isnull(SectionStatusID2, '') not like '%A%'
																																//and isnull(SectionStatusID4, '') not like '%M%'
																																//and isnull(SectionStatusID4, '') not like '%N%'");

			AssertExcludedSection(excludedSection);
		}

		/// <summary>
		/// Test exclusion of classes that have an 'X' in the SectionStatusID1 field
		/// </summary>
		[TestMethod]
		public void GetSections_ExcludeCancelledSections()
		{
			string excludedSection = _dataVerifier.GetRandomClassID(@"isnull(SectionStatusID1, '') like '%X%'
																																and isnull(SectionStatusID1, '') not like '%Z%'
																																and isnull(SectionStatusID4, '') not like '%M%'
																																and isnull(SectionStatusID4, '') not like '%N%'");

			AssertExcludedSection(excludedSection);
		}

		/// <summary>
		/// Test exclusion of classes that have an 'M' in the SectionStatusID4 field
		/// </summary>
		[TestMethod]
//		[Ignore]	// Test is not properly failing
		public void GetSections_ExcludeSectionsSuppressedFromSchedule_M()
		{
			string excludedSection = _dataVerifier.GetRandomClassID(@"isnull(SectionStatusID4, '') like '%M%'
																																and isnull(SectionStatusID1, '') not like '%Z%'
																																and isnull(SectionStatusID1, '') not like '%X%'
																																and isnull(SectionStatusID4, '') not like '%N%'");

			AssertExcludedSection(excludedSection);
		}

		/// <summary>
		/// Test exclusion of classes that have an 'N' in the SectionStatusID4 field
		/// </summary>
		[TestMethod]
		[Ignore]	// NOTE: The ODS contains no records with this flag that isn't already caught by the other flags
		public void GetSections_ExcludeSectionsSuppressedFromSchedule_N()
		{
			string excludedSection = _dataVerifier.GetRandomClassID(@"isnull(SectionStatusID4, '') like '%N%'
																																and isnull(SectionStatusID1, '') not like '%X%'
																																and isnull(SectionStatusID4, '') not like '%M%'");
			// Adding the following filters result in no record being returned.
																																//and isnull(SectionStatusID1, '') not like '%Z%'
																																//and isnull(SectionStatusID2, '') not like '%A%'

			AssertExcludedSection(excludedSection);
		}

		/// <summary>
		/// 
		///</summary>
		[TestMethod]
		public void GetSectionsByYearQuarter_Success()
		{
			using (OdsRepository repository = new OdsRepository())
			{
				string yearQuarterId = "B012";
				YearQuarter yrq = YearQuarter.FromString(yearQuarterId);
				
				IList<Section> sections = repository.GetSections(yrq);
				int count = sections.Where(s => s.Yrq.ID == yearQuarterId).Count();

				Assert.AreEqual(sections.Count, count);
			}
		}

		/// <summary>
		/// 
		///</summary>
		[TestMethod]
		public void GetSectionsBySubject_WithQuarterFilter_Success()
		{
			using (OdsRepository repository = new OdsRepository())
			{
				YearQuarter yrq = YearQuarter.FromString("B122");
				IList<Section> sections = repository.GetSections("ENGL", yrq);
				int count = sections.Where(s => s.Yrq.ID == "B122" && s.CourseID.StartsWith("ENGL")).Count();

				Assert.AreEqual(sections.Count, count);
			}
		}

		/// <summary>
		/// 
		///</summary>
		[TestMethod]
		public void GetSectionsBySubjectList_Success()
		{
			using (OdsRepository repository = new OdsRepository())
			{
				IList<ISectionFacet> facets = TestHelper.GetFacets();

				IList<string> subjects = new List<string>(2) {"ENGL", "ENGL&"};

				YearQuarter yrq = YearQuarter.FromString("B122");
				IList<Section> sections = repository.GetSections(subjects, yrq);
				int count = sections.Where(s => s.CourseSubject == "ENGL").Count();

				Assert.AreEqual(sections.Count, count);
			}
		}

		/// <summary>
		/// 
		///</summary>
		[TestMethod]
		public void GetSectionsBySubjectList_OneSubject_Success()
		{
			using (OdsRepository repository = new OdsRepository())
			{
				IList<ISectionFacet> facets = TestHelper.GetFacets();

				IList<string> subjects = new List<string>(1) {"CHEM"};

				IList<Section> sections = repository.GetSections(subjects, facetOptions: facets);
				int count = sections.Where(s => s.CourseSubject == "CHEM").Count();

				Assert.AreEqual(sections.Count, count);
			}
		}

		/// <summary>
		/// 
		///</summary>
		[TestMethod]
		public void GetSectionsBySubject_WithQuarterFilter_OnCampus_Success()
		{
			using (OdsRepository repository = new OdsRepository())
			{
				YearQuarter yrq = YearQuarter.FromString("B122");
				IList<ISectionFacet> facets = new List<ISectionFacet>(1);
				facets.Add(new ModalityFacet(ModalityFacet.Options.OnCampus));

				IList<Section> sections = repository.GetSections("ART", yrq, facets);
				Assert.IsTrue(sections.Count > 0, "No records found.");

				int count = sections.Where(s => s.Yrq.ID == yrq.ID && s.CourseID.StartsWith("ART")).Count();
				Assert.AreEqual(sections.Count, count);
			}
		}


		/// <summary>
		/// 
		///</summary>
		[TestMethod]
		public void GetSections_ByClassIDCollection_Success()
		{
			int sectionCount = 3;
			IList<ISectionID> ids = _dataVerifier.GetSectionIDListFromQuery("english", sectionCount);

			using (OdsRepository repository = new OdsRepository())
			{
				IList<Section> sections = repository.GetSections(ids, TestHelper.GetFacets());

				Assert.AreEqual(sectionCount, sections.Count);
			}
		}

		/// <summary>
		/// 
		///</summary>
		[TestMethod]
		public void GetSections_ByCourseIDCollection_Success()
		{
			IList<ICourseID> ids = _dataVerifier.GetCourseIDListFromQuery("stud", 3);
			int expectedCount = ids.Count;

			using (OdsRepository repository = new OdsRepository())
			{
				IList<Section> sections = repository.GetSections(ids, facetOptions: TestHelper.GetFacets());
				int sectionCount = sections.Count;

				Assert.IsTrue(expectedCount <= sectionCount, "Sections returned ({0}) is less than # of CourseIDs ({1})", sectionCount, expectedCount);
			}
		}

		/// <summary>
		/// 
		///</summary>
		[TestMethod]
		public void GetSections_ByCourseIDCollection_WithYrq_Success()
		{

			using (OdsRepository repository = new OdsRepository())
			{
				YearQuarter yrq = repository.CurrentYearQuarter;

				int courseIdCount = 3;
				IList<ICourseID> ids = _dataVerifier.GetCourseIDListFromQuery("eng", courseIdCount, string.Format(" and YearQuarterID = '{0}' ", yrq));

				IList<Section> sections = repository.GetSections(ids, yrq, TestHelper.GetFacets());
				int sectionCount = sections.Count;

				Assert.IsTrue(courseIdCount <= sectionCount, "Sections returned ({0}) is less than # of CourseIDs ({1})", sectionCount, courseIdCount);
			}
		}

		/// <summary>
		/// 
		///</summary>
		[TestMethod]
		public void GetSectionsByCourseID_Success()
		{
			using (OdsRepository repository = new OdsRepository())
			{
				IList<Section> sections = repository.GetSections(CourseID.FromString("ART 101"), facetOptions: TestHelper.GetFacets());

				Assert.IsTrue(sections.Count > 0);
			}
		}

		/// <summary>
		/// 
		///</summary>
		[TestMethod]
		public void GetSectionsByCourseID_WithYrq_Success()
		{
			using (OdsRepository repository = new OdsRepository())
			{
				YearQuarter yrq = YearQuarter.FromString("B012");
				IList<Section> sections = repository.GetSections(CourseID.FromString("ART 101"), yrq);

				Assert.IsTrue(sections.Count > 0);
			}
		}

		/// <summary>
		/// Verify instructor information includes e-mail address
		///</summary>
		[TestMethod]
//		[Conditional("BC_ONLY")]
		public void GetSectionsByCourseID_WithYrq_VerifyInstructorEmail()
		{
			using (OdsRepository repository = new OdsRepository())
			{
				YearQuarter yrq = YearQuarter.FromString("B233");
				string courseId = "ART 101";

				IList<Section> sections = repository.GetSections(CourseID.FromString(courseId), yrq, TestHelper.GetFacets());
				Assert.IsTrue(sections.Count > 0, "No sections found for {0} in {1}", courseId, yrq.FriendlyName);

				int emailCount = sections.Where(s => s.Offered.Where(o => !string.IsNullOrWhiteSpace(o.InstructorEmail)).Count() > 0).Count();
				Assert.IsTrue(emailCount > 0, "No instructor e-mails found.");
			}
		}

		/// <summary>
		/// 
		///</summary>
		[TestMethod]
		public void GetSectionsBy_CourseID_Yrq_VerifyOffered()
		{
			using (OdsRepository repository = new OdsRepository())
			{
				YearQuarter yrq = repository.CurrentYearQuarter;

				IList<Section> sections = repository.GetSections(TestHelper.Data.CourseIDOfferedEveryQuarter, yrq, TestHelper.GetFacets());

				Assert.IsTrue(sections.Where(s => s.Offered != null).Count() > 0, "InstructionDetails are not being populated from the database");	// check for all NULL from the database

				int sectionsWithOffered = sections.Where(s => s.Offered.Count() > 0).Count();
				Assert.IsTrue(sectionsWithOffered > 0, "One or more Sections is missing an Offered collection");

				int tooManyOffered = sections.Where(s => s.Offered.Count() > 20).Count();
				Assert.IsFalse(tooManyOffered > 0, "One or more Sections has more than 20 Offered items, which is more than is reasonable to expect");
			}
		}

		/// <summary>
		/// 
		///</summary>
		[TestMethod]
		public void GetSections_VerifyIsOnline()
		{
			IList<Section> sections = TestHelper.GetSectionsWithFilter(new ModalityFacet(ModalityFacet.Options.Online));

			int count = sections.Count;
			int areOnline = sections.Where(s => s.IsOnline).Count();
			Assert.AreEqual(count, areOnline);
		}

		/// <summary>
		/// 
		///</summary>
		[TestMethod]
		public void GetSections_VerifyIsOnCampus()
		{
			IList<Section> sections = TestHelper.GetSectionsWithFilter(new ModalityFacet(ModalityFacet.Options.OnCampus));

			int count = sections.Count;
			int areOnCampus = sections.Where(s => s.IsOnCampus).Count();
			Assert.AreEqual(count, areOnCampus);
		}

		/// <summary>
		/// 
		///</summary>
		[TestMethod]
		public void GetSections_VerifyIsHybrid()
		{
			IList<Section> sections = TestHelper.GetSectionsWithFilter(new ModalityFacet(ModalityFacet.Options.Hybrid));

			int count = sections.Count;
			int areHybrid = sections.Where(s => s.IsHybrid).Count();
			Assert.AreEqual(count, areHybrid);
		}

		/// <summary>
		/// 
		///</summary>
		[TestMethod]
		public void GetSections_VerifyIsTelecourse()
		{
			IList<Section> sections = TestHelper.GetSectionsWithFilter(new ModalityFacet(ModalityFacet.Options.Telecourse));

			int count = sections.Count;
			int areTelecourse = sections.Where(s => s.IsTelecourse).Count();
			Assert.AreEqual(count, areTelecourse);
		}

		/// <summary>
		/// 
		///</summary>
		[TestMethod]
		public void GetSections_VerifyIsLinkedClasses()
		{
			IList<ISectionFacet> facets = TestHelper.GetFacets();

			using (OdsRepository repository = new OdsRepository())
			{
				IList<Section> sections = repository.GetSections(facets);

				int expected = _dataVerifier.GetSectionCount("isnull(ItemYRQLink, '') <> '' and ItemYRQLink <> ItemNumber");
				int actual = sections.Where(s => s.IsLinked).Count();

				Assert.AreEqual(expected, actual);
			}
		}

		/* This test exists for spot-checking research. It should NOT be included in the standard test run. - 3/22/2012, shawn.south@bellevuecollege.edu
		[TestMethod]
		public void GetSections_VerifyLinkedSections()
		{
			IList<ISectionFacet> facets = TestHelper.GetFacets();

			using (OdsRepository repository = new OdsRepository())
			{
				IList<string> subjects = new List<string> {"ENGL", "ENGL&"};
				IList<Section> sections = repository.GetSections(subjects, YearQuarter.FromString("B124"), facets);
				Assert.IsTrue(sections.Count > 0, "No Sections were returned!");

				IList<Section> linked = sections.Where(s => !string.IsNullOrWhiteSpace(s.LinkedTo)).ToList();
				Assert.IsTrue(linked.Count > 0, "No linked Sections found!");
			}
		}
//		 */

		[TestMethod]
		public void GetSections_VerifyContinuousEnrollementHasLastRegistrationDate()
		{
			IList<ISectionFacet> facets = TestHelper.GetFacets();

			using (OdsRepository repository = new OdsRepository())
			{
				IList<Section> sections = repository.GetSections(facets);
				IList<Section> continuousEnrollment = sections.Where(s => s.IsContinuousEnrollment).ToList();

				int total = continuousEnrollment.Count;
				Assert.IsTrue(total > 0, "No continuous enrollment Sections found!");

				int missingDate = continuousEnrollment.Where(s => s.LastRegistrationDate == DateTime.MinValue).Count();
				Assert.IsTrue(missingDate == 0, "Found [{0} out of {1}] continuous enrollment Sections without a LastRegistrationDate", missingDate, total);
			}
		}

		[TestMethod]
		public void GetSections_StartDateDifferentFromQuarter()
		{
			using (OdsRepository repository = new OdsRepository())
			{
				IList<Section> sections = repository.GetSections(TestHelper.GetFacets());
				Assert.IsTrue(sections.Count > 0, "No records were returned.");

				int count = sections.Where(s => s.IsDifferentStartDate).Count();
				Assert.IsTrue(count > 0, "No DifferentStartDate records were found.");

				int allSectionCount = _dataVerifier.AllSectionsCount;
				Assert.IsTrue(count < allSectionCount, "ALL Section records were returned.");

				string whereClause = "NOT StartDate IS NULL AND StartDate <> (select top 1 yq.FirstClassDay from vw_YearQuarter yq where yq.YearQuarterID = Vw_Class.YearQuarterID)";
				int expectedCount = _dataVerifier.GetSectionCount(whereClause);
				Assert.AreEqual(expectedCount, count);
			}
		}

		[TestMethod]
		public void GetSections_EndDateDifferentFromQuarter()
		{
			using (OdsRepository repository = new OdsRepository())
			{
				IList<Section> sections = repository.GetSections(TestHelper.GetFacets());
				Assert.IsTrue(sections.Count > 0, "No records were returned.");

				int count = sections.Where(s => s.IsDifferentEndDate).Count();
				Assert.IsTrue(count > 0, "No DifferentEndDate records were found.");

				int allSectionCount = _dataVerifier.AllSectionsCount;
				Assert.IsTrue(count < allSectionCount, "ALL Section records were returned.");

				string whereClause = "NOT EndDate IS NULL AND EndDate <> (select top 1 yq.LastClassDay from vw_YearQuarter yq where yq.YearQuarterID = Vw_Class.YearQuarterID)";
				int expectedCount = _dataVerifier.GetSectionCount(whereClause);
				Assert.AreEqual(expectedCount, count);
			}
		}

		#region Private members
		/// <summary>
		/// 
		/// </summary>
		/// <param name="excludedSection"></param>
		private void AssertExcludedSection(string excludedSection)
		{
			using (OdsRepository repository = new OdsRepository())
			{
				IList<Section> sections = repository.GetSections(TestHelper.GetFacets());
				Assert.IsTrue(sections.Count > 0, "No Sections were returned.");

				int count = sections.Where(s => s.ID.ToString() == excludedSection).Count();
				Assert.IsTrue(count <= 0, "Sections collection contains a record that should be filtered out: ({0})", excludedSection);
			}
		}

		#endregion
	}
}
