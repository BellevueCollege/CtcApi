using CtcApi.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.CtcApi
{
    
    
    /// <summary>
    ///This is a test class for StringExtensionsTest and is intended
    ///to contain all StringExtensionsTest Unit Tests
    ///</summary>
  [TestClass()]
  public class StringExtensionsTest
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


      #region Test IsNumber()

      #region Integer
      [TestMethod()]
      public void IsNumber_True_Integer_Positive()
      {
        string value = "832";
        bool actual = value.IsNumber();
        Assert.IsTrue(actual);
      }

      [TestMethod()]
      public void IsNumber_True_Integer_Zero()
      {
        string value = "0";
        bool actual = value.IsNumber();
        Assert.IsTrue(actual);
      }

      [TestMethod()]
      public void IsNumber_True_Integer_Negative()
      {
        string value = "-3423";
        bool actual = value.IsNumber();
        Assert.IsTrue(actual);
      }

      #endregion

      #region Decimal
      [TestMethod()]
      public void IsNumber_True_Decimal_Positive()
      {
        string value = "8.32";
        bool actual = value.IsNumber();
        Assert.IsTrue(actual);
      }

      [TestMethod()]
      public void IsNumber_True_Decimal_Zero()
      {
        string value = "0.0";
        bool actual = value.IsNumber();
        Assert.IsTrue(actual);
      }

      [TestMethod()]
      public void IsNumber_True_Decimal_Negative()
      {
        string value = "-3.423";
        bool actual = value.IsNumber();
        Assert.IsTrue(actual);
      }

      #endregion

      #region Not a number
      [TestMethod()]
      public void IsNumber_False_NaN_Alpha()
      {
        string value = "herkher";
        bool actual = value.IsNumber();
        Assert.IsFalse(actual);
      }

      [TestMethod()]
      public void IsNumber_False_NaN_Empty()
      {
        string value = "";
        bool actual = value.IsNumber();
        Assert.IsFalse(actual);
      }

      [TestMethod()]
      public void IsNumber_False_NaN_AlphaNumeric()
      {
        string value = "A-3.423";
        bool actual = value.IsNumber();
        Assert.IsFalse(actual);
      }

      #endregion

      #endregion

    [TestMethod()]
    public void Nullify_NonEmptyString()
    {
      string value = "not an empty string";
      string actual = value.Nullify();
      Assert.IsNotNull(actual);
      Assert.AreEqual(value, actual);
    }

    [TestMethod()]
    public void Nullify_EmptyString()
    {
      string value = "";
      string actual = value.Nullify();
      Assert.IsNull(actual);
    }

    [TestMethod()]
    public void Nullify_NullString()
    {
      string value = null;
      string actual = value.Nullify();
      Assert.IsNull(actual);
    }
  }
}
