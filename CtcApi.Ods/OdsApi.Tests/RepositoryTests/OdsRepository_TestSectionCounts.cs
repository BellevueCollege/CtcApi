using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Ctc.Ods.Data;
using Ctc.Ods.Tests.ClassDataFilterTests;
using Ctc.Ods.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ctc.Ods.Tests.RepositoryTests
{
	/// <summary>
	/// Summary description for OdsRepository_TestSectionCounts
	/// </summary>
	[TestClass]
	public class OdsRepository_TestSectionCounts
	{
		#region Constructor(s), etc.
		private DataVerificationHelper _dataVerifier;

		public OdsRepository_TestSectionCounts()
		{
			_dataVerifier = new DataVerificationHelper();
		}

		~OdsRepository_TestSectionCounts()
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
		public void ForCourseID_CurrentRegistrationQuarter()
		{
			using (OdsRepository repository = new OdsRepository())
			{
				ICourseID courseID = CourseID.FromString("ACCT 101");
				YearQuarter yrq = repository.CurrentRegistrationQuarter;

				int sectionCount = repository.SectionCountForCourse(courseID, yrq, TestHelper.GetFacets(false));
				Assert.IsTrue(sectionCount > 0, string.Format("No sections found for '{0}' in '{1}'", courseID, yrq.FriendlyName));

				int expectedCount = _dataVerifier.GetSectionCount(string.Format("YearQuarterID = '{0}' AND CourseID LIKE '{1}%{2}' AND NOT CourseID LIKE '%&%'", yrq, courseID.Subject, courseID.Number));
				Assert.AreEqual(expectedCount, sectionCount);
			}
		}

		[TestMethod]
		public void ForCourseIDCommonCourse_CurrentRegistrationQuarter()
		{
			using (OdsRepository repository = new OdsRepository())
			{
				ICourseID courseID = CourseID.FromString("ENGL& 101");
				YearQuarter yrq = repository.CurrentRegistrationQuarter;

				int sectionCount = repository.SectionCountForCourse(courseID, yrq, TestHelper.GetFacets(false));
				Assert.IsTrue(sectionCount > 0, string.Format("No sections found for '{0}' in '{1}'", courseID, yrq.FriendlyName));

				int expectedCount = _dataVerifier.GetSectionCount(string.Format("YearQuarterID = '{0}' AND CourseID LIKE '{1}%{2}' AND CourseID LIKE '%&%'", yrq, courseID.Subject, courseID.Number));
				Assert.AreEqual(expectedCount, sectionCount);
			}
		}

		[TestMethod]
		public void ForSubject_CurrentRegistrationQuarter()
		{
			using (OdsRepository repository = new OdsRepository())
			{
				string subject = "ENGL";
				YearQuarter yrq = repository.CurrentRegistrationQuarter;

				int sectionCount = repository.SectionCountForCourse(subject, yrq, TestHelper.GetFacets(false));
				Assert.IsTrue(sectionCount > 0, string.Format("No sections found for '{0}' in '{1}'", subject, yrq.FriendlyName));

				int expectedCount = _dataVerifier.GetSectionCount(string.Format("YearQuarterID = '{0}' AND CourseID LIKE '{1}%' AND NOT CourseID LIKE '%&%'", yrq, subject));
				Assert.AreEqual(expectedCount, sectionCount);
			}
		}

		[TestMethod]
		public void ForSubjectCommonCourse_CurrentRegistrationQuarter()
		{
			using (OdsRepository repository = new OdsRepository())
			{
				string subject = "ENGL&";
				YearQuarter yrq = repository.CurrentRegistrationQuarter;

				int sectionCount = repository.SectionCountForCourse(subject, yrq, TestHelper.GetFacets(false));
				Assert.IsTrue(sectionCount > 0, string.Format("No sections found for '{0}' in '{1}'", subject, yrq.FriendlyName));

				int expectedCount = _dataVerifier.GetSectionCount(string.Format("YearQuarterID = '{0}' AND CourseID LIKE '{1}%' AND CourseID LIKE '%&%'", yrq, subject));
				Assert.AreEqual(expectedCount, sectionCount);
			}
		}

	}
}
