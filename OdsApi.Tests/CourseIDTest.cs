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
			ICourseID actual = CourseID.FromString("art 101");
			Assert.AreEqual("ART", actual.Subject);
			Assert.AreEqual("101", actual.Number);
			Assert.IsFalse(actual.IsCommonCourse);
		}

		[TestMethod()]
		public void FromString_SingleParameter_CommonCourse()
		{
			ICourseID actual = CourseID.FromString("art& 101");
			Assert.AreEqual("ART", actual.Subject);
			Assert.AreEqual("101", actual.Number);
			Assert.IsTrue(actual.IsCommonCourse);
		}

		[TestMethod()]
		public void FromString_SubjectAndNumber()
		{
			string subject = "ART";
			string number = "101";
			ICourseID actual = CourseID.FromString(subject, number);
			Assert.AreEqual(subject, actual.Subject);
			Assert.AreEqual(number, actual.Number);
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
		public void ToStringConversion()
		{
			ICourseID target = CourseID.FromString("ART", "101");
			string expected = "ART 101";
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

		// TODO: equality tests
	}
}
