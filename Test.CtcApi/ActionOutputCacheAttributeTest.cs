using CtcApi.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Test.CtcApi
{
    
    
    /// <summary>
    ///This is a test class for ActionOutputCacheAttributeTest and is intended
    ///to contain all ActionOutputCacheAttributeTest Unit Tests
    ///</summary>
  [TestClass()]
  public class ActionOutputCacheAttributeTest
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


    /// <summary>
    ///A test for ActionOutputCacheAttribute Constructor
    ///</summary>
    [TestMethod()]
    public void ActionOutputCacheAttributeConstructorTest()
    {
      string cacheSetting = string.Empty; // TODO: Initialize to an appropriate value
      int defaultCacheDuration = 0; // TODO: Initialize to an appropriate value
      ActionOutputCacheAttribute target = new ActionOutputCacheAttribute(cacheSetting, defaultCacheDuration);
      Assert.Inconclusive("TODO: Implement code to verify target");
    }
  }
}
