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
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ctc.Ods.Config;
using System.Configuration;

namespace Ctc.Ods.Tests.ClassDataFilterTests
{
	/// <summary>
	/// Summary description for GetSections_FilterEarlyStart
	/// </summary>
	[TestClass]
	public class GetSections_FilterLateStart
	{
		private DataVerificationHelper _dataVerifier;

		public GetSections_FilterLateStart()
		{
		    _dataVerifier = new DataVerificationHelper();
		}

		~GetSections_FilterLateStart()
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

		//Use ClassInitialize to run code before running the first test in the class
		[ClassInitialize]
		public static void MyClassInitialize(TestContext testContext)
		{
		}
		
		//Use ClassCleanup to run code after all tests in a class have run
		[ClassCleanup]
		public static void MyClassCleanup()
		{
		}
		
		#region Additional test attributes
		//
		// You can use the following additional attributes as you write your tests:
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
		public void GetSections_LateStart_Success()
		{
      ApiSettings settings = (ApiSettings)ConfigurationManager.GetSection(ApiSettings.SectionName);
      ushort daysCount = settings.ClassFlags.LateStartDaysCount;

			string whereClause = String.Format("NOT StartDate IS NULL AND dateadd(day, -{0}, StartDate) >= (select top 1 yq.FirstClassDay from vw_YearQuarter yq where yq.YearQuarterID = Vw_Class.YearQuarterID)", daysCount);
      int count = TestHelper.GetSectionCountWithFilter(new LateStartFacet());

			int allSectionCount = _dataVerifier.AllSectionsCount;
			Assert.IsTrue(allSectionCount > count, "Returned all Sections instead of a sub-set.");

			int expectedCount = _dataVerifier.GetSectionCount(whereClause);
			Assert.AreEqual(expectedCount, count);
		}
	}
}
