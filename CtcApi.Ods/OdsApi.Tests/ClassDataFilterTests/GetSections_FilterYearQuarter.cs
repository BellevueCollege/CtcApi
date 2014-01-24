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
using Ctc.Ods.Types;

namespace Ctc.Ods.Tests.ClassDataFilterTests
{
    /// <summary>
    /// Summary description for GetSections_FilterYearQuarter
    /// </summary>
    [TestClass]
    public class GetSections_FilterYearQuarter
    {
        #region Test infrastructure
		private DataVerificationHelper _dataVerifier;

		public GetSections_FilterYearQuarter()
		{
			_dataVerifier = new DataVerificationHelper();
		}

        ~GetSections_FilterYearQuarter()
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

        #region Single Quarters
        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void GetSections_OneQuarter_Success()
        {
          IList<YearQuarter> yearQuartersToFilter = new List<YearQuarter>();
//          yearQuartersToFilter.Add(YearQuarter.FromString("B121"));
          yearQuartersToFilter.Add(TestHelper.Data.YearQuarterWithSections);

          int returnedCount = TestHelper.GetSectionCountWithFilter(new YearQuarterFacet(yearQuartersToFilter), false);
        	int actualCount = _dataVerifier.GetSectionCount(string.Format("YearQuarterID = '{0}'", yearQuartersToFilter[0].ID));

           Assert.AreEqual(actualCount, returnedCount);
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void GetSections_NoQuarters_Success()
        {
            IList<YearQuarter> yearQuartersToFilter = new List<YearQuarter>();

            int returnedCount = TestHelper.GetSectionCountWithFilter(new YearQuarterFacet(yearQuartersToFilter), false);

            Assert.AreEqual(0, returnedCount);
        }
        #endregion


        #region Multiple Quarters
        [TestMethod]
        public void GetSections_ThreeQuarters_Success()
        {
					IList<YearQuarter> yearQuartersToFilter = new List<YearQuarter>();
					yearQuartersToFilter.Add(YearQuarter.FromString("B014"));
					yearQuartersToFilter.Add(YearQuarter.FromString("B121"));
					yearQuartersToFilter.Add(YearQuarter.FromString("B122"));

          int returnedCount = TestHelper.GetSectionCountWithFilter(new YearQuarterFacet(yearQuartersToFilter), false);
        	int actualCount = _dataVerifier.GetSectionCount(string.Format("YearQuarterID in ('{0}', '{1}', '{2}')",
        																																yearQuartersToFilter[0].ID,
																																				yearQuartersToFilter[1].ID,
        																																yearQuartersToFilter[2].ID));

          Assert.AreEqual(actualCount, returnedCount);
        }
        #endregion

        #region Private Helpers
        private int GetSectionCountForQuarters(string yrq)
        {
            return _dataVerifier.GetSectionCount(string.Format("YearQuarterID = '{0}'", yrq));
        }
        #endregion
    }
}
