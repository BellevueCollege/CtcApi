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
using Ctc.Ods.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ctc.Ods.Tests
{
    
    
	[TestClass()]
	public class CourseIDTest
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


		[TestMethod()]
		public void FromString_SingleParameter()
		{
		  string subject = "ART";
		  string number = "101";
		  string courseId = string.Concat(subject, " ", number);

		  ICourseID actual = CourseID.FromString(courseId);
			
      Assert.AreEqual(subject, actual.Subject);
			Assert.AreEqual(number, actual.Number);
      Assert.AreEqual(courseId, actual.ToString());
			
      Assert.IsFalse(actual.IsCommonCourse);
		}

		[TestMethod()]
		public void FromString_SingleParameter_TabSeparator()
		{
		  string subject = "ART";
		  string number = "101";
      string realCourseID = string.Concat(subject, "\t", number);

		  ICourseID actual = CourseID.FromString(realCourseID);
			
      Assert.AreEqual(subject, actual.Subject);
			Assert.AreEqual(number, actual.Number);
      Assert.AreEqual(string.Concat(subject, " ", number), actual.ToString());
			
      Assert.IsFalse(actual.IsCommonCourse);
		}

		[TestMethod()]
		public void FromString_SingleParameter_CommonCourse_SpaceSeparator()
		{
      string subject = "ART";
      string number = "101";
      string courseId = string.Concat(subject, " ", number);
      string realCourseID = string.Concat(subject, "& ", number);

      ICourseID actual = CourseID.FromString(realCourseID);
			Assert.AreEqual(subject, actual.Subject);
			Assert.AreEqual(number, actual.Number);
      Assert.AreEqual(courseId, actual.ToString());

			Assert.IsTrue(actual.IsCommonCourse);
		}

		[TestMethod()]
		public void FromString_SingleParameter_CommonCourse_TabSeparator()
		{
      string subject = "ART";
      string number = "101";
      string courseId = string.Concat(subject, " ", number);
      string realCourseID = string.Concat(subject, "&\t", number);

      ICourseID actual = CourseID.FromString(realCourseID);
			Assert.AreEqual(subject, actual.Subject);
			Assert.AreEqual(number, actual.Number);
      Assert.AreEqual(courseId, actual.ToString());

			Assert.IsTrue(actual.IsCommonCourse);
		}

    [TestMethod]
	  public void FromString_SingleParameter_CommonCourse_NoSeparatingSpace()
	  {
      string subject = "ACCT";
      string number = "101";
      string courseId = string.Concat(subject, " ", number);
      string realCourseID = string.Concat(subject, "&", number);

      ICourseID actual = CourseID.FromString(realCourseID);
      Assert.AreEqual(subject, actual.Subject);
      Assert.AreEqual(number, actual.Number);
      Assert.AreEqual(courseId, actual.ToString());

      Assert.IsTrue(actual.IsCommonCourse);
    }

    [TestMethod()]
    public void FromString_SingleParameter_WithSpaces()
    {
      string subject = "C J";
      string number = "100";
      string courseId = string.Concat(subject, " ", number);

      ICourseID actual = CourseID.FromString(courseId);

      Assert.AreEqual(subject, actual.Subject);
      Assert.AreEqual(number, actual.Number);
      Assert.AreEqual(courseId, actual.ToString());

      Assert.IsFalse(actual.IsCommonCourse);
    }

    [TestMethod()]
    public void FromString_SingleParameter_WithSpaces_TabSeparated()
    {
      string subject = "C J";
      string number = "100";
      string realCourseID = string.Concat(subject, "\t", number);

      ICourseID actual = CourseID.FromString(realCourseID);

      Assert.AreEqual(subject, actual.Subject);
      Assert.AreEqual(number, actual.Number);
      Assert.AreEqual(string.Concat(subject, " ", number), actual.ToString());

      Assert.IsFalse(actual.IsCommonCourse);
    }

    [TestMethod()]
    public void FromString_SingleParameter_WithSpaces_CommonCourse()
    {
      string subject = "C J";
      string number = "100";
      string courseId = string.Concat(subject, " ", number);
      string realCourseID = string.Concat(subject, "& ", number);

      ICourseID actual = CourseID.FromString(realCourseID);

      Assert.AreEqual(subject, actual.Subject);
      Assert.AreEqual(number, actual.Number);
      Assert.AreEqual(courseId, actual.ToString());

      Assert.IsTrue(actual.IsCommonCourse);
    }

    [TestMethod()]
    public void FromString_SingleParameter_WithSpaces_CommonCourse_TabSeparated()
    {
      string subject = "C J";
      string number = "100";
      string courseId = string.Concat(subject, " ", number);
      string realCourseID = string.Concat(subject, "&\t", number);

      ICourseID actual = CourseID.FromString(realCourseID);

      Assert.AreEqual(subject, actual.Subject);
      Assert.AreEqual(number, actual.Number);
      Assert.AreEqual(courseId, actual.ToString());

      Assert.IsTrue(actual.IsCommonCourse);
    }

    [TestMethod()]
		public void FromString_SubjectAndNumber()
		{
			string subject = "CEO";
			string number = "196";

			ICourseID actual = CourseID.FromString(subject, number);
			Assert.AreEqual(subject, actual.Subject);
			Assert.AreEqual(number, actual.Number);
      Assert.AreEqual(string.Concat(subject, " ", number), actual.ToString());

			Assert.IsFalse(actual.IsCommonCourse);
		}

		[TestMethod()]
		public void FromString_SubjectAndNumber_CommonCourse()
		{
			string subject = "ART";
			string number = "101";
			ICourseID actual = CourseID.FromString(string.Concat(subject, "&"), number);
			Assert.AreEqual(subject, actual.Subject);
			Assert.AreEqual(number, actual.Number);
			Assert.IsTrue(actual.IsCommonCourse, "Common Course flag is not set");
		}

    [TestMethod()]
    public void FromString_SubjectAndNumber_WithSpaces()
    {
      string subject = "C J";
      string number = "100";

      ICourseID actual = CourseID.FromString(subject, number);

      Assert.AreEqual(subject, actual.Subject);
      Assert.AreEqual(number, actual.Number);
      Assert.AreEqual(string.Concat(subject, " ", number), actual.ToString());

      Assert.IsFalse(actual.IsCommonCourse);
    }

    [TestMethod()]
    public void FromString_SubjectAndNumber_WithSpaces_CommonCourse()
    {
      string subject = "P E";
      string number = "100";

      ICourseID actual = CourseID.FromString(string.Concat(subject, "&"), number);

      Assert.AreEqual(subject, actual.Subject);
      Assert.AreEqual(number, actual.Number);
      Assert.AreEqual(string.Concat(subject, " ", number), actual.ToString());

      Assert.IsTrue(actual.IsCommonCourse);
    }

    [TestMethod()]
		public void FromString_SubjectAndNumber_CommonCourse_2charSubject()
		{
			// this test comes from a real-world scenario which revealed a bug. 5/07/2012, shawn.south@bellevuecollege.edu
			string subject = TestHelper.Data.ShortCourseSubject;
			string number = TestHelper.Data.ShortCourseSubjectNumber;

			ICourseID actual = CourseID.FromString(string.Concat(subject, TestHelper.Data.CommonCourseCharacter), number);

			Assert.AreEqual(subject, actual.Subject);
			Assert.AreEqual(number, actual.Number);
			Assert.IsTrue(actual.IsCommonCourse, "Common Course flag is not set");
		}

		[TestMethod()]
		public void ToStringConversion()
		{
			ICourseID target = CourseID.FromString("ART", "101");
			string expected = "ART 101";
			string actual = target.ToString();
			Assert.AreEqual(expected, actual);
			Assert.IsFalse(target.IsCommonCourse);
		}

		public void ToStringConversion_SpacesInSubject()
		{
			ICourseID target = CourseID.FromString("C S C", "112");
			string expected = "C S C 112";
			string actual = target.ToString();
			Assert.AreEqual(expected, actual);
			Assert.IsFalse(target.IsCommonCourse);
		}

		[TestMethod()]
		public void ToStringConversion_CommonCourse()
		{
			ICourseID target = CourseID.FromString("ART&", "101");
			string expected = "ART 101";
			string actual = target.ToString();
			Assert.AreEqual(expected, actual);
			Assert.IsTrue(target.IsCommonCourse);
		}

		[TestMethod()]
		public void ToStringConversion_CommonCourse_SpacesInSubject()
		{
			ICourseID target = CourseID.FromString("C J&", "101");
			string expected = "C J 101";
			string actual = target.ToString();
			Assert.AreEqual(expected, actual);
			Assert.IsTrue(target.IsCommonCourse);
		}





		// TODO: equality tests
	}
}
