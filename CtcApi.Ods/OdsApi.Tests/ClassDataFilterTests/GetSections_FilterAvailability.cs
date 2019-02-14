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

using Ctc.Ods.Config;
using System.Configuration;

namespace Ctc.Ods.Tests.ClassDataFilterTests
{
	/// <summary>
	/// Summary description for GetSections_FilterAvailability
	/// </summary>
	[TestClass]
	public class GetSections_FilterAvailability
	{
		#region Test Infrastructure
		private DataVerificationHelper _dataVerifier;

		public GetSections_FilterAvailability()
		{
			_dataVerifier = new DataVerificationHelper();
		}

		~GetSections_FilterAvailability()
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

		[TestMethod]
		public void GetSections_All_Success()
		{
			int count = TestHelper.GetSectionCountWithFilter(new AvailabilityFacet(AvailabilityFacet.Options.All));

			int allSectionCount = _dataVerifier.GetSectionCount("ClassID = ClassID");
			Assert.AreEqual(allSectionCount, count);
		}

		[TestMethod]
		public void GetSections_Open_Success()
		{
			ApiSettings settings = (ApiSettings)ConfigurationManager.GetSection(ApiSettings.SectionName);
			Assert.IsNotNull(settings, "Unable to create an instance of the ApiSettings.");

			int count = TestHelper.GetSectionCountWithFilter(new AvailabilityFacet(AvailabilityFacet.Options.Open));

			string openClusteredSections = "select c.ClassID from vw_Class c where not c.ClusterItemNumber is null group by c.ClassID having (max(c.ClusterCapacity) - sum(c.StudentsEnrolled)) > 0";
			string openRegularSections = "select c2.ClassID from vw_Class c2 where c2.ClusterItemNumber is null and ((c2.ClassCapacity - c2.StudentsEnrolled) > 0)";
			int allSectionCount = _dataVerifier.GetSectionCount(string.Format("ClassID in ({0} union {1}) and not ClassID in (select w.ClassID from vw_Waitlist w where w.Status = '{2}')", openClusteredSections, openRegularSections, settings.Waitlist.Status));

			Assert.AreEqual(allSectionCount, count);
		}
	}
}
