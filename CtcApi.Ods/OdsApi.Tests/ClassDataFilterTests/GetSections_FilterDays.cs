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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ctc.Ods.Tests.ClassDataFilterTests
{
	/// <summary>
	/// Summary description for GetSections_FilterDays
	/// </summary>
	[TestClass]
	public class GetSections_Filtered
	{
		#region Test infrastructure
		private DataVerificationHelper _dataVerifier;

		public GetSections_Filtered()
		{
			_dataVerifier = new DataVerificationHelper();
		}

		~GetSections_Filtered()
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

		#region Individual days
		/// <summary>
		/// 
		/// </summary>
		[TestMethod]
		public void GetSections_Monday_Success()
		{
			int count = TestHelper.GetSectionCountWithFilter(new DaysFacet(DaysFacet.Options.Monday));

			int allSectionCount = _dataVerifier.AllSectionsCount;
			Assert.IsTrue(allSectionCount > count);

			int expectedCount = GetSectionCountForDays("M");
			Assert.AreEqual(expectedCount, count);
		}

		/// <summary>
		/// 
		/// </summary>
		[TestMethod]
		public void GetSections_Tuesday_Success()
		{
			int count = TestHelper.GetSectionCountWithFilter(new DaysFacet(DaysFacet.Options.Tuesday));

			int allSectionCount = _dataVerifier.AllSectionsCount;
			Assert.IsTrue(allSectionCount > count);

			int expectedCount = GetSectionCountForDays("T");
			Assert.AreEqual(expectedCount, count);
		}

		/// <summary>
		/// 
		/// </summary>
		[TestMethod]
		public void GetSections_Wednesday_Success()
		{
			int count = TestHelper.GetSectionCountWithFilter(new DaysFacet(DaysFacet.Options.Wednesday));

			int allSectionCount = _dataVerifier.AllSectionsCount;
			Assert.IsTrue(allSectionCount > count);

			int expectedCount = GetSectionCountForDays("W");
			Assert.AreEqual(expectedCount, count);
		}

		/// <summary>
		/// 
		/// </summary>
		[TestMethod]
		public void GetSections_Thursday_Success()
		{
			int count = TestHelper.GetSectionCountWithFilter(new DaysFacet(DaysFacet.Options.Thursday));

			int allSectionCount = _dataVerifier.AllSectionsCount;
			Assert.IsTrue(allSectionCount > count);

			int expectedCount = GetSectionCountForDays("Th");
			Assert.AreEqual(expectedCount, count);
		}

		/// <summary>
		/// 
		/// </summary>
		[TestMethod]
		public void GetSections_Friday_Success()
		{
			int count = TestHelper.GetSectionCountWithFilter(new DaysFacet(DaysFacet.Options.Friday));

			int allSectionCount = _dataVerifier.AllSectionsCount;
			Assert.IsTrue(allSectionCount > count);

			int expectedCount = GetSectionCountForDays("F");
			Assert.AreEqual(expectedCount, count);
		}

		/// <summary>
		/// 
		/// </summary>
		[TestMethod]
		public void GetSections_Saturday_Success()
		{
			int count = TestHelper.GetSectionCountWithFilter(new DaysFacet(DaysFacet.Options.Saturday));

			int allSectionCount = _dataVerifier.AllSectionsCount;
			Assert.IsTrue(allSectionCount > count);

			int expectedCount = GetSectionCountForDays("Sa");
			Assert.AreEqual(expectedCount, count);
		}

		/// <summary>
		/// 
		/// </summary>
		[TestMethod]
		public void GetSections_Sunday_Success()
		{
			int count = TestHelper.GetSectionCountWithFilter(new DaysFacet(DaysFacet.Options.Sunday));

			int allSectionCount = _dataVerifier.AllSectionsCount;
			Assert.IsTrue(allSectionCount > count);

			int expectedCount = GetSectionCountForDays("Su");
			Assert.AreEqual(expectedCount, count);
		}

		#endregion

		#region Multiple days
		/// <summary>
		/// 
		/// </summary>
		[TestMethod]
		public void GetSections_Days_MW_Success()
		{
			int count = TestHelper.GetSectionCountWithFilter(new DaysFacet(DaysFacet.Options.Monday | DaysFacet.Options.Wednesday));

			int allSectionCount = _dataVerifier.AllSectionsCount;
			Assert.IsTrue(allSectionCount > count);

			int expectedCount = GetSectionCountForDays("MW");
			Assert.AreEqual(expectedCount, count);
		}

		/// <summary>
		/// 
		/// </summary>
		[TestMethod]
		public void GetSections_Days_TTh_Success()
		{
			int count = TestHelper.GetSectionCountWithFilter(new DaysFacet(DaysFacet.Options.Tuesday | DaysFacet.Options.Thursday));

			int allSectionCount = _dataVerifier.AllSectionsCount;
			Assert.IsTrue(allSectionCount > count);

			int expectedCount = GetSectionCountForDays("TTh");
			Assert.AreEqual(expectedCount, count);
		}

		/// <summary>
		/// 
		/// </summary>
		[TestMethod]
		public void GetSections_Days_MWF_Success()
		{
			int count = TestHelper.GetSectionCountWithFilter(new DaysFacet(DaysFacet.Options.Monday | DaysFacet.Options.Wednesday | DaysFacet.Options.Friday));

			int allSectionCount = _dataVerifier.AllSectionsCount;
			Assert.IsTrue(allSectionCount > count);

			int expectedCount = GetSectionCountForDays("MWF");
			Assert.AreEqual(expectedCount, count);
		}

		/// <summary>
		/// 
		/// </summary>
		[TestMethod]
		public void GetSections_Days_MTWThF_Success()
		{
			int count = TestHelper.GetSectionCountWithFilter(new DaysFacet(DaysFacet.Options.Monday | DaysFacet.Options.Tuesday | DaysFacet.Options.Wednesday | DaysFacet.Options.Thursday | DaysFacet.Options.Friday));

			int allSectionCount = _dataVerifier.AllSectionsCount;
			Assert.IsTrue(allSectionCount > count);

			int expectedCount = _dataVerifier.GetSectionCount("ClassID in (select i.ClassID from vw_Instruction i where i.ClassID = ClassID and i.DayID in (select d.DayID from vw_Day d where d.Title = 'MTWThF' or d.Title = 'DAILY'))");
			Assert.AreEqual(expectedCount, count);
		}

		/// <summary>
		/// 
		/// </summary>
		[TestMethod]
		public void GetSections_Days_SaSu_Success()
		{
			int count = TestHelper.GetSectionCountWithFilter(new DaysFacet(DaysFacet.Options.Saturday | DaysFacet.Options.Sunday));

			int allSectionCount = _dataVerifier.AllSectionsCount;
			Assert.IsTrue(allSectionCount > count);

			int expectedCount = GetSectionCountForDays("SaSu");
			Assert.AreEqual(expectedCount, count);
		}

		#endregion

		#region Private members
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private int GetSectionCountForDays(string days)
		{
			return _dataVerifier.GetSectionCount(string.Format("ClassID in (select i.ClassID from vw_Instruction i where i.ClassID = ClassID and i.DayID in (select d.DayID from vw_Day d where d.Title = '{0}'))", days));
		}

		#endregion
	}
}
