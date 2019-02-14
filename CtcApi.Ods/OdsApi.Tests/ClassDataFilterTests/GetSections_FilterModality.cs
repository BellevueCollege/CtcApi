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

namespace Ctc.Ods.Tests.ClassDataFilterTests
{
	/// <summary>
	/// Summary description for GetSections_FilterModality
	/// </summary>
	[TestClass]
	public class GetSections_FilterModality
	{
		#region Test infrastructure
		private DataVerificationHelper _dataVerifier;

		public GetSections_FilterModality()
		{
			_dataVerifier = new DataVerificationHelper();
		}

		~GetSections_FilterModality()
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

		#region Modality facets
		/// <summary>
		/// 
		/// </summary>
		[TestMethod]
		public void GetSections_Filtered_Modality_All_Success()
		{
			int count = TestHelper.GetSectionCountWithFilter(new ModalityFacet(ModalityFacet.Options.All));

			int allSectionCount = _dataVerifier.GetSectionCount("not ClassID is null");
			Assert.AreEqual(allSectionCount, count);
		}

		/// <summary>
		/// 
		/// </summary>
		[TestMethod]
		public void GetSections_Filtered_Modality_Online_Success()
		{
			int count = TestHelper.GetSectionCountWithFilter(new ModalityFacet(ModalityFacet.Options.Online));

			int expectedSectionCount = _dataVerifier.GetSectionCount("SBCTCMisc1 like '3%'");
			Assert.AreEqual(expectedSectionCount, count);

			int allSectionCount = _dataVerifier.GetSectionCount("not ClassID is null");
			Assert.IsTrue(allSectionCount > count);
		}

		/// <summary>
		/// 
		/// </summary>
		[TestMethod]
		public void GetSections_SimulateClassScheduleApplication()
		{
			// Simulates actual call from Online Class Schedule application - which passes extra 
			IList<ISectionFacet> facets = TestHelper.GetFacets(new ModalityFacet(ModalityFacet.Options.Online));
			facets.Add(new TimeFacet(new TimeSpan(0, 0, 0), new TimeSpan(23, 59, 0)));
			facets.Add(new DaysFacet(DaysFacet.Options.All));

			using (OdsRepository repository = new OdsRepository())
			{
				YearQuarter yrq = repository.CurrentYearQuarter;
				
				string subject = TestHelper.Data.CourseSubjectOfferedEveryQuarter;
				IList<Section> sections = repository.GetSections(subject, yrq, facets);
				
				int count = sections.Count;
				Assert.IsTrue(count > 0, "No records returned.");

				int expectedSectionCount = _dataVerifier.GetSectionCount(string.Format("(CourseID like '{0}%' and SBCTCMisc1 like '3%' and YearQuarterID = '{1}')", subject, yrq.ID));
				Assert.AreEqual(expectedSectionCount, count);

				int allSectionCount = _dataVerifier.GetSectionCount("not ClassID is null");
				Assert.IsTrue(allSectionCount > count, "Query failed: ALL records were returned.");
			}
		}

		/// <summary>
		/// 
		/// </summary>
		[TestMethod]
		public void GetSections_Filtered_Modality_Hybrid_Success()
		{
			int count = TestHelper.GetSectionCountWithFilter(new ModalityFacet(ModalityFacet.Options.Hybrid));

			int expectedSectionCount = _dataVerifier.GetSectionCount("SBCTCMisc1 like '8%'");
			Assert.AreEqual(expectedSectionCount, count);

			int allSectionCount = _dataVerifier.GetSectionCount("not ClassID is null");
			Assert.IsTrue(allSectionCount > count);
		}

		/// <summary>
		/// 
		/// </summary>
		[TestMethod]
		public void GetSections_Filtered_Modality_Telecourse_Success()
		{
			int count = TestHelper.GetSectionCountWithFilter(new ModalityFacet(ModalityFacet.Options.Telecourse));

			int expectedSectionCount = _dataVerifier.GetSectionCount("SBCTCMisc1 like '1%'");
			Assert.AreEqual(expectedSectionCount, count);

			int allSectionCount = _dataVerifier.GetSectionCount("not ClassID is null");
			Assert.IsTrue(allSectionCount > count);
		}

		/// <summary>
		/// 
		/// </summary>
		[TestMethod]
		public void GetSections_Filtered_Modality_OnCampus_Success()
		{
			int count = TestHelper.GetSectionCountWithFilter(new ModalityFacet(ModalityFacet.Options.OnCampus));

			int expectedSectionCount = _dataVerifier.GetSectionCount("isnull(SBCTCMisc1, '') not like '1%' and isnull(SBCTCMisc1, '') not like '3%' and isnull(SBCTCMisc1, '') not like '8%'");
			Assert.AreEqual(expectedSectionCount, count);

			int allSectionCount = _dataVerifier.GetSectionCount("not ClassID is null");
			Assert.IsTrue(allSectionCount > count);
		}

		/// <summary>
		/// 
		/// </summary>
		[TestMethod]
		public void GetSections_Filtered_Modality_Online_And_Hybrid_Success()
		{
			int count = TestHelper.GetSectionCountWithFilter(new ModalityFacet(ModalityFacet.Options.Online | ModalityFacet.Options.Hybrid));

			int expectedSectionCount = _dataVerifier.GetSectionCount("(SBCTCMisc1 like '3%' or SBCTCMisc1 like '8%')");
			Assert.AreEqual(expectedSectionCount, count);

			int allSectionCount = _dataVerifier.GetSectionCount("not ClassID is null");
			Assert.IsTrue(allSectionCount > count);
		}

		#endregion
	}
}
