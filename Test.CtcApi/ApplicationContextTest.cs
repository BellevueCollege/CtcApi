using System.Configuration;
using CtcApi;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Test.CtcApi
{
  /// <summary>
  ///This is a test class for ApplicationContextTest and is intended
  ///to contain all ApplicationContextTest Unit Tests
  ///</summary>
  [TestClass()]
  public class ApplicationContextTest
  {


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


    #region CurrentDate tests
    [TestMethod()]
    public void CurrentDateTest_DefaultNow()
    {
      ApplicationContext target = new ApplicationContext();
      DateTime? expected = DateTime.Now;
      DateTime? actual = target.CurrentDate;

      Assert.IsTrue(actual.HasValue, "CurrentDate returned NULL.");
      Assert.AreEqual(expected.Value.ToShortDateString(), actual.Value.ToShortDateString());
    }

    [TestMethod()]
    public void CurrentDate_SetProgrammatically()
    {
      DateTime? expected = DateTime.Parse("10/7/2013");
      ApplicationContext target = new ApplicationContext
                                    {
                                      CurrentDate = expected
                                    };
      DateTime? actual = target.CurrentDate;

      Assert.IsTrue(actual.HasValue, "CurrentDate returned NULL.");

      string shortDateString = actual.Value.ToShortDateString();

      Assert.AreNotEqual(DateTime.Now.ToShortDateString(), shortDateString);
      Assert.AreEqual(expected.Value.ToShortDateString(), shortDateString);
    }

    [TestMethod()]
    public void CurrentDate_FromConfig()
    {
      ConfigurationManager.AppSettings["CurrentDate"] = "7/20/2010";

      DateTime? expected = DateTime.Parse(ConfigurationManager.AppSettings["CurrentDate"]);
      ApplicationContext target = new ApplicationContext();
      DateTime? actual = target.CurrentDate;

      Assert.IsTrue(actual.HasValue, "CurrentDate returned NULL.");

      string shortDateString = actual.Value.ToShortDateString();

      Assert.AreNotEqual(DateTime.Now.ToShortDateString(), shortDateString);
      Assert.AreEqual(expected.Value.ToShortDateString(), shortDateString);
    }

    [TestMethod()]
    public void CurrentDate_InvalidDateFromConfig()
    {
      ConfigurationManager.AppSettings["CurrentDate"] = "foo";

      ApplicationContext target = new ApplicationContext();
      DateTime? actual = target.CurrentDate;

      Assert.IsTrue(actual.HasValue, "CurrentDate returned NULL.");

      Assert.AreEqual(DateTime.Now.ToShortDateString(), actual.Value.ToShortDateString());
    }

    [TestMethod()]
    public void CurrentDate_ProgrammaticallyResetToNow()
    {
      ApplicationContext target = new ApplicationContext();
      target.CurrentDate = null;
      DateTime? actual = target.CurrentDate;

      Assert.IsTrue(actual.HasValue, "CurrentDate returned NULL.");

      Assert.AreEqual(DateTime.Now.ToShortDateString(), actual.Value.ToShortDateString());
    }

    #endregion

    /// <summary>
    ///A test for BaseDirectory
    ///</summary>
    [TestMethod()]
    [Ignore]
    public void BaseDirectoryTest()
    {
      ApplicationContext target = new ApplicationContext(); // TODO: Initialize to an appropriate value
      string expected = string.Empty; // TODO: Initialize to an appropriate value
      string actual;
      target.BaseDirectory = expected;
      actual = target.BaseDirectory;
      Assert.AreEqual(expected, actual);
      Assert.Inconclusive("Verify the correctness of this test method.");
    }
  }
}
