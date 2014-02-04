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
	[TestClass]
	public class SectionIDTest
	{
		[TestMethod]
		public void FromString_Success()
		{
			string itemNumber = "1234";
			string yrq = "B012";
			ISectionID id = SectionID.FromString(String.Concat(itemNumber, yrq));

			Assert.AreEqual(id.ItemNumber, itemNumber);
			Assert.AreEqual(id.YearQuarter, yrq);
		}

		[TestMethod]
		public void FromString_Success_ItemNumberWithLeadingZeroes()
		{
			string itemNumber = "0002";
			string yrq = "B012";
			ISectionID id = SectionID.FromString(String.Concat(itemNumber, yrq));

			Assert.AreEqual(id.ItemNumber, itemNumber);
			Assert.AreEqual(id.YearQuarter, yrq);
		}

		[TestMethod]
		[ExpectedException(typeof(FormatException))]
		public void FromString_EmptyString()
		{
			SectionID.FromString(String.Empty);

			Assert.Fail("Should have thrown a FormatException");
		}

		[TestMethod]
		[ExpectedException(typeof(FormatException))]
		public void FromString_StringTooLong()
		{
			SectionID.FromString("12345B012");

			Assert.Fail("Should have thrown a FormatException");
		}

		/// <summary>
		/// 
		///</summary>
		[TestMethod]
		public void SectionIdToString_Success()
		{
			string expected = "1234B012";
			ISectionID actual = SectionID.FromString(expected);

			Assert.AreEqual(expected, actual.ToString());
		}

		/// <summary>
		/// 
		///</summary>
		[TestMethod]
		public void AreEqual()
		{
			string sectionIdString = "1234B012";
			ISectionID expected = SectionID.FromString(sectionIdString);
			ISectionID actual = SectionID.FromString(sectionIdString);

			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		/// 
		///</summary>
		[TestMethod]
		public void AreEqual_String()
		{
			string sectionIdString = "1234B012";
			ISectionID actual = SectionID.FromString(sectionIdString);

			Assert.AreEqual(actual, sectionIdString);
		}

		/// <summary>
		/// 
		///</summary>
		[TestMethod]
		public void AreNotEqual()
		{
			ISectionID expected = SectionID.FromString("1234B012");
			ISectionID actual = SectionID.FromString("4321A121");

			Assert.AreNotEqual(expected, actual);
		}

		/// <summary>
		/// 
		///</summary>
		[TestMethod]
		public void AreNotEqual_String()
		{
			ISectionID actual = SectionID.FromString("4321A121");

			Assert.AreNotEqual("1234B012", actual);
		}

    [TestMethod]
    public void ID_ToString()
    {
      string sectionId = "4321A121";
      ISectionID actual = SectionID.FromString(sectionId);

      string actualString = actual.ToString();
      Assert.AreEqual(sectionId, actualString);
    }

    [TestMethod]
    public void SubClassed_NewFromISectionID()
    {
      string sectionId = "4321A121";
      ISectionID actual = SectionID.FromString(sectionId);

      SubclassedSectionID subclassed = new SubclassedSectionID(actual);

      Assert.IsNotNull(subclassed);
      Assert.AreEqual(sectionId, subclassed.ToString());
    }

    [TestMethod]
    [Ignore]  // this cast doesn't seem to work. Would like to make it work somehow. shawn.south@bellevuecollege.edu 3/14/2013
    public void SubClassed_SafeCast()
    {
      string sectionId = "4321A121";
      ISectionID actual = SectionID.FromString(sectionId);

      SubclassedSectionID subclassed = actual as SubclassedSectionID;

      Assert.IsNotNull(subclassed);
    }
	}

  public class SubclassedSectionID : SectionID
  {
    /// <summary>
    /// Creates a new <see cref="SectionID"/> from an item # and YRQ
    /// </summary>
    /// <param name="itemNumber"></param>
    /// <param name="yrq"></param>
    protected SubclassedSectionID(string itemNumber, string yrq) : base(itemNumber, yrq)
    {
    }

    public SubclassedSectionID(ISectionID sectionID) : base(sectionID.ItemNumber, sectionID.YearQuarter)
    {
    }
  }
}
