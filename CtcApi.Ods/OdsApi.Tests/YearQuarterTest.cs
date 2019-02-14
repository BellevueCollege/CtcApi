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
using Ctc.Ods.Tests.ClassDataFilterTests;
using Ctc.Ods.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ctc.Ods.Tests
{
	[TestClass]
	public class YearQuarterTest
	{
		[TestMethod]
		public void YearQuarter_FromString_Success()
		{
			string expected = "B014";
			YearQuarter yrq = YearQuarter.FromString(expected);

			Assert.AreEqual(expected, yrq.ToString());
		}

		[TestMethod]
		public void YearQuarter_FriendlyName_Success()
		{
			string expected = "Spring 2011";
			YearQuarter yrq = YearQuarter.FromString("B014");

			Assert.AreEqual(expected, yrq.FriendlyName);
		}

		[TestMethod]
		public void YearQuarter_ToFriendlyName_Success()
		{
			string expected = "Spring 2011";
			string actual = YearQuarter.ToFriendlyName("B014");

			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void YearQuarter_ToFriendlyName_StringTooShort()
		{
			YearQuarter.ToFriendlyName("B01");

			Assert.Fail("Did not throw expected exception");
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void YearQuarter_ToFriendlyName_StringTooLong()
		{
			YearQuarter.ToFriendlyName("B0142");

			Assert.Fail("Did not throw expected exception");
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void YearQuarter_ToFriendlyName_EmptyString()
		{
			YearQuarter.ToFriendlyName(string.Empty);

			Assert.Fail("Did not throw expected exception");
		}

		[TestMethod]
		[ExpectedException(typeof(NullReferenceException))]
		public void YearQuarter_ToFriendlyName_NullString()
		{
			YearQuarter.ToFriendlyName(null);

			Assert.Fail("Did not throw expected exception");
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidCastException))]
		public void YearQuarter_ToFriendlyName_InvalidQuarter()
		{
			YearQuarter.ToFriendlyName("B019");

			Assert.Fail("Did not throw expected exception");
		}

		[TestMethod]
		public void YearQuarter_ToFriendlyName_UpperBound()
		{
			string expected = "Spring 2259";
			string actual = YearQuarter.ToFriendlyName("Z894");

			Assert.AreEqual(expected, actual);
		}

		#region FromFriendlyName tests

		#region No space
		[TestMethod]
		public void YearQuarter_From_Summer2011()
		{
			AssertYearQuarterFromFriendlyName("Summer2011", "B121");
		}

		[TestMethod]
		public void YearQuarter_From_Fall2011()
		{
			AssertYearQuarterFromFriendlyName("Fall2011", "B122");
		}

		[TestMethod]
		public void YearQuarter_From_Winter2012()
		{
			AssertYearQuarterFromFriendlyName("Winter2012", "B123");
		}

		[TestMethod]
		public void YearQuarter_From_Spring2012()
		{
			AssertYearQuarterFromFriendlyName("Spring2012", "B124");
		}

		#endregion

		#region One space
		[TestMethod]
		public void YearQuarter_From_Summer_2011()
		{
			AssertYearQuarterFromFriendlyName("Summer 2011", "B121");
		}

		[TestMethod]
		public void YearQuarter_From_Fall_2011()
		{
			AssertYearQuarterFromFriendlyName("Fall 2011", "B122");
		}

		[TestMethod]
		public void YearQuarter_From_Winter_2012()
		{
			AssertYearQuarterFromFriendlyName("Winter 2012", "B123");
		}

		[TestMethod]
		public void YearQuarter_From_Spring_2012()
		{
			AssertYearQuarterFromFriendlyName("Spring 2012", "B124");
		}

		#endregion

		#region Crossing decade
		[TestMethod]
		public void YearQuarter_From_Summer_2009()
		{
			AssertYearQuarterFromFriendlyName("Summer 2009", "A901");
		}

		[TestMethod]
		public void YearQuarter_From_Fall_2009()
		{
			AssertYearQuarterFromFriendlyName("Fall 2009", "A902");
		}

		[TestMethod]
		public void YearQuarter_From_Winter_2010()
		{
			AssertYearQuarterFromFriendlyName("Winter 2010", "A903");
		}

		[TestMethod]
		public void YearQuarter_From_Spring_2010()
		{
			AssertYearQuarterFromFriendlyName("Spring 2010", "A904");
		}

		[TestMethod]
		public void YearQuarter_From_Summer_2010()
		{
			AssertYearQuarterFromFriendlyName("Summer 2010", "B011");
		}

		#endregion

		#region Crossing century
		[TestMethod]
		public void YearQuarter_From_Summer_1999()
		{
			AssertYearQuarterFromFriendlyName("Summer 1999", "9901");
		}

		[TestMethod]
		public void YearQuarter_From_Fall_1999()
		{
			AssertYearQuarterFromFriendlyName("Fall 1999", "9902");
		}

		[TestMethod]
		public void YearQuarter_From_Winter_2000()
		{
			AssertYearQuarterFromFriendlyName("Winter 2000", "9903");
		}

		[TestMethod]
		public void YearQuarter_From_Spring_2000()
		{
			AssertYearQuarterFromFriendlyName("Spring 2000", "9904");
		}

		[TestMethod]
		public void YearQuarter_From_Summer_2000()
		{
			AssertYearQuarterFromFriendlyName("Summer 2000", "A011");
		}

		#endregion

		/// <summary>
		/// 
		///</summary>
		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void YearQuarter_FromFriendlyName_FarFuture()
		{
			YearQuarter.FromFriendlyName("Fall 4001");
			Assert.Fail("Did not throw expected exception.");
		}

		/// <summary>
		/// 
		///</summary>
		[TestMethod]
		public void YearQuarter_FromFriendlyName_UpperBound()
		{
			AssertYearQuarterFromFriendlyName("Spring 2259", "Z894");
		}

		/// <summary>
		/// 
		///</summary>
		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void YearQuarter_FromFriendlyName_OverUpperBound()
		{
			YearQuarter.FromFriendlyName("Spring 2260");
			Assert.Fail("Did not throw expected exception.");
		}

		/// <summary>
		/// 
		///</summary>
		[TestMethod]
		public void YearQuarter_Min()
		{
			
			string yrqMin = TestHelper.MinYrq;
			YearQuarter yrq = YearQuarter.FromString(yrqMin);
			Assert.AreEqual(yrqMin, yrq.ID);
			Assert.AreEqual("[Minimum]", yrq.FriendlyName);
		}

		/// <summary>
		/// 
		///</summary>
		[TestMethod]
		public void YearQuarter_Max()
		{
			string yrqMin = TestHelper.MaxYrq;
			YearQuarter yrq = YearQuarter.FromString(yrqMin);
			Assert.AreEqual(yrqMin, yrq.ID);
			Assert.AreEqual("[Maximum]", yrq.FriendlyName);
		}

		#region Helper methods
		private void AssertYearQuarterFromFriendlyName(string friendlyName, string yrqID)
		{
			YearQuarter yrq = YearQuarter.FromFriendlyName(friendlyName);
			Assert.AreEqual(yrqID, yrq.ID);
		}

		#endregion

		#endregion

		#region Equality tests
		[TestMethod]
		public void YearQuarter_Equals_Another()
		{
			YearQuarter yrq1 = YearQuarter.FromString("B121");
			YearQuarter yrq2 = YearQuarter.FromString("B121");

			Assert.AreEqual(yrq1, yrq2);
		}

		[TestMethod]
		public void YearQuarter_Equals_AnotherCreatedFromFriendlyName()
		{
			YearQuarter yrq1 = YearQuarter.FromString("B121");
			YearQuarter yrq2 = YearQuarter.FromFriendlyName("Summer 2011");

			Assert.AreEqual(yrq1, yrq2);
		}

		#endregion

	}
}
