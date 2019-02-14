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
using System.Linq;
using Ctc.Ods.Data;
using Ctc.Ods.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ctc.Ods.Tests.ClassDataFilterTests
{
	/// <summary>
	/// Summary description for GetSections_FilterTime
	/// </summary>
	[TestClass]
	public class GetSections_FilterTime
	{
		#region test infrastructure
		private DataVerificationHelper _dataVerifier;

		public GetSections_FilterTime()
		{
			_dataVerifier = new DataVerificationHelper();
		}

		~GetSections_FilterTime()
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

		/// <summary>
		/// 
		/// </summary>
		[TestMethod]
		public void GetSections_FilterByTimes_Morning_Success()
		{
			VerifyTimeFacet("00:00", "11:59");
		}

		/// <summary>
		/// 
		/// </summary>
		[TestMethod]
		public void GetSections_FilterByTimes_Afternoon_Success()
		{
			VerifyTimeFacet("12:00", "16:59");
		}

		/// <summary>
		/// 
		/// </summary>
		[TestMethod]
		public void GetSections_FilterByTimes_Evening_Success()
		{
			VerifyTimeFacet("17:00", "23:59");
		}

		/// <summary>
		/// 
		/// </summary>
		[TestMethod]
		public void GetSections_FilterByTimes_WideRange_Success()
		{
			VerifyTimeFacet("02:55", "22:55");
		}

		#region helper methods
		/// <summary>
		/// 
		/// </summary>
		/// <param name="start"></param>
		/// <param name="end"></param>
		private void VerifyTimeFacet(string start, string end)
		{
			using (OdsRepository repository = new OdsRepository())
			{
				TimeSpan startTime = TimeSpan.Parse(start);
				TimeSpan endTime = TimeSpan.Parse(end);

				IList<ISectionFacet> facets = TestHelper.GetFacets(new TimeFacet(startTime, endTime));

				IList<Section> sections = repository.GetSections(facetOptions: facets);
				Assert.IsTrue(sections.Count > 0, "No sections were returned");

				// make sure we have sections that fall in the specified range...
				int rightSectionCount = sections.Where(s => s.Offered.Where(o => (o.StartTime.HasValue && o.StartTime.Value.TimeOfDay.CompareTo(startTime) >= 0 && o.StartTime.Value.TimeOfDay.CompareTo(endTime) <= 0)).Count() > 0).Count();
				Assert.IsTrue(rightSectionCount > 0, "Section records do not include start/end times in the specified range.");

				// ... and that we don't have any that fall outside that range
				int wrongSectionCount = sections.Where(s => s.Offered.Where(o => (o.StartTime.HasValue && o.StartTime.Value.TimeOfDay.CompareTo(startTime) < 0 && o.StartTime.Value.TimeOfDay.CompareTo(endTime) > 0)).Count() > 0).Count();
				Assert.IsTrue(wrongSectionCount <= 0, "Found {0} sections with start/end times outside of {1} - {2}", wrongSectionCount, startTime, endTime);
			}
		}

		#endregion
	}
}
