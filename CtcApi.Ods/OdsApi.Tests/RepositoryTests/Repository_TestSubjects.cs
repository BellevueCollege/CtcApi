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
using System.Globalization;
using System.Linq;
using Ctc.Ods.Data;
using Ctc.Ods.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ctc.Ods.Tests.RepositoryTests
{
	/// <summary>
	/// Summary description for Repository_TestSubjects
	/// </summary>
	[TestClass]
	public class Repository_TestSubjects
	{
		#region Test Infrastructure
		private DataVerificationHelper _dataVerifier;

		public Repository_TestSubjects()
		{
			_dataVerifier = new DataVerificationHelper();
		}

		~Repository_TestSubjects()
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

		/// <summary>
		/// 
		///</summary>
		[TestMethod]
		public void GetSubjects()
		{
			using (OdsRepository repository = new OdsRepository())
			{
				IList<CoursePrefix> subjects = repository.GetCourseSubjects();
				Assert.IsTrue(subjects.Count > 0);
			}
		}

		/// <summary>
		/// Test that we can return a list of Subject,Title by first char in subject
		/// </summary>
		[TestMethod]
		public void GetSubjectsWithChar()
		{
			using (OdsRepository repository = new OdsRepository())
			{
				char firstChar = 'b';
				IList<CoursePrefix> actual = repository.GetCourseSubjects(firstChar);
				
				int actualCount = actual.Count;
				Assert.IsTrue(actualCount > 0);
				
				int count = actual.Where(c => c.Subject.StartsWith(firstChar.ToString(), true, CultureInfo.CurrentCulture)).Count();
				Assert.AreEqual(actualCount, count);
			}

		}

		/// <summary>
		/// Test that we can return a list of course subjects which have active sections in the specified quarter
		/// </summary>
		[TestMethod]
		public void GetSubjectsWithYQ_Success()
		{
			using (OdsRepository repo = new OdsRepository())
			{
				YearQuarter yearQuarter = repo.CurrentYearQuarter;
				IList<CoursePrefix> actual = repo.GetCourseSubjects(yearQuarter);

				int expectedCount = _dataVerifier.GetCourseSubjectCountForSections(string.Format("YearQuarterID = '{0}'", yearQuarter.ID), true);
				Assert.AreEqual(expectedCount, actual.Count);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		[TestMethod]
		public void GetSubjects_VerifyCourseSubjectIsCorrectlyExcluded()
		{
			YearQuarter yrq = TestHelper.Data.YearQuarterWithSections;

			using (OdsRepository repo = new OdsRepository())
			{
				IList<CoursePrefix> actual = repo.GetCourseSubjects(yrq);
				Assert.IsTrue(actual.Count > 0, "No subjects were returned for {0} ({1})", yrq.FriendlyName, yrq);

				string subject = _dataVerifier.GetRandomCourseSubject(string.Format("YearQuarterID <> '{0}'", yrq.ID), true);

				IEnumerable<CoursePrefix> shouldBeEmpty = actual.Where(s => s.Subject.ToUpper() == subject.ToUpper());
				int emptyCount = shouldBeEmpty.Count();
			  if (emptyCount > 0)
			  {
			    Assert.Fail("{0} found in {1} ({2})", emptyCount, yrq.FriendlyName, yrq);
			  }
			}
		}

		// TODO: Create tests for the rest of the modality options
		/// <summary>
		/// Test that we can return a list of course subjects which have active online sections in the specified quarter
		/// </summary>
		[TestMethod]
		public void GetSubjectsWithYrq_Modality_Online_Success()
		{
			using (OdsRepository repo = new OdsRepository())
			{
				YearQuarter yearQuarter = YearQuarter.FromString("B122");
				IList<ISectionFacet> facets = new List<ISectionFacet>();

				facets.Add(new ModalityFacet(ModalityFacet.Options.Online));
				IList<CoursePrefix> actual = repo.GetCourseSubjects(yearQuarter, facets);

				int expectedCount = _dataVerifier.GetCourseSubjectCountForSections(string.Format("YearQuarterID = '{0}' and SBCTCMisc1 like '3%'", yearQuarter.ID), true);
				Assert.AreEqual(expectedCount, actual.Count);

				int allCount = _dataVerifier.GetCourseSubjectCountForSections(string.Format("YearQuarterID = '{0}'", yearQuarter.ID));
				Assert.IsTrue(allCount > actual.Count, "Count of online sections is not less than all sections");
			}
		}
	}
}
