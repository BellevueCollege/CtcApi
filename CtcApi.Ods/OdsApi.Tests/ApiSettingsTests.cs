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
using System.Configuration;
using Ctc.Ods.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ctc.Ods.Tests
{
	/// <summary>
	/// Summary description for ApiSettingsTests
	/// </summary>
	[TestClass]
	public class ApiSettingsTests
	{
		#region Test Infrastructure
		private DataVerificationHelper _dataVerifier;

		public ApiSettingsTests()
		{
			_dataVerifier = new DataVerificationHelper();
		}

		~ApiSettingsTests()
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
		public void LoadConfigSettings()
		{
			ApiSettings settings = (ApiSettings)ConfigurationManager.GetSection(ApiSettings.SectionName);
			Assert.IsNotNull(settings, "ApiSettings object is null");
		}

		[TestMethod]
		public void VerifyClassFlagsExist()
		{
			ApiSettings settings = (ApiSettings)ConfigurationManager.GetSection(ApiSettings.SectionName);
			Assert.IsNotNull(settings.ClassFlags, "ClassFlags object is null");
			Assert.IsNotNull(settings.ClassFlags.Rules, "ClassFlags.Rules is null");
			Assert.IsTrue(settings.ClassFlags.Rules.Count > 0, "No ClassRules found");
		}

		[TestMethod]
		public void RegexPatternsExist()
		{
			ApiSettings settings = (ApiSettings)ConfigurationManager.GetSection(ApiSettings.SectionName);
			Assert.IsNotNull(settings.RegexPatterns, "RegexPatterns object is null");
			Assert.IsNotNull(settings.RegexPatterns.CommonCourseChar, "CommonCourseChar is null");
			Assert.IsFalse(string.IsNullOrWhiteSpace(settings.RegexPatterns.CommonCourseChar), "CommonCourseChar is empty");
		}
	}
}
