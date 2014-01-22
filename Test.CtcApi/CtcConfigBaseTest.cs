using System.Xml.Serialization;
using CtcApi.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.CtcApi
{
	[TestClass()]
	public class TestCtcConfigBase
	{
		private TestContext testContextInstance;
		private const string CONFIG_NAME = "testConfig";

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
		//You can use the following additional attributes as you write your tests:
		//
		//Use ClassInitialize to run code before running the first test in the class
		//[ClassInitialize()]
		//public static void MyClassInitialize(TestContext testContext)
		//{
		//}
		//
		//Use ClassCleanup to run code after all tests in a class have run
		//[ClassCleanup()]
		//public static void MyClassCleanup()
		//{
		//}
		//
		//Use TestInitialize to run code before running each test
		//[TestInitialize()]
		//public void MyTestInitialize()
		//{
		//}
		//
		//Use TestCleanup to run code after each test has run
		//[TestCleanup()]
		//public void MyTestCleanup()
		//{
		//}
		//
		#endregion

		[TestMethod()]
		public void Load()
		{
			TestConfig config = TestConfig.Load();

			Assert.IsNotNull(config);
			Assert.IsInstanceOfType(config, typeof(TestConfig));
		}

		[TestMethod()]
		public void GetSectionName()
		{
			string configName = TestConfig.GetSectionName();

			Assert.AreEqual(CONFIG_NAME, configName);
		}

		/// <summary>
		/// 
		/// </summary>
		[XmlType(CONFIG_NAME)]
		public class TestConfig : CtcConfigBase<TestConfig>
		{
		}
	}
}
