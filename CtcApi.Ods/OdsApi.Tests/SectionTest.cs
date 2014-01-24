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
	[TestClass]
	public class SectionTest
	{
		/// <summary>
		/// 
		/// </summary>
		[TestMethod]
		public void SectionInstantiation_Success()
		{
			Section section = new Section(SectionID.FromString("1234B012"));
		}

		/// <summary>
		/// 
		///</summary>
		[TestMethod]
		public void SectionToString_Success()
		{
			string sectionIdString = "1234B012";
			Section section = new Section(SectionID.FromString(sectionIdString));

			Assert.AreEqual(sectionIdString, section.ToString());
		}

		/// <summary>
		/// 
		///</summary>
		[TestMethod]
		public void ISectionToString_Success()
		{
			string sectionIdString = "1234B012";
			ISection section = new Section(SectionID.FromString(sectionIdString));

			Assert.AreEqual(sectionIdString, section.ToString());
		}

		/// <summary>
		/// 
		///</summary>
		[TestMethod]
		public void Sections_AreEqual()
		{
			Section sec1 = new Section(SectionID.FromString("1234B012"));
			Section sec2 = new Section(SectionID.FromString("1234B012"));

			Assert.AreEqual(sec1, sec2);
		}

		/// <summary>
		/// 
		///</summary>
		[TestMethod]
		public void Sections_AreNotEqual()
		{
			Section sec1 = new Section(SectionID.FromString("1234B012"));
			Section sec2 = new Section(SectionID.FromString("4321B121"));

			Assert.AreNotEqual(sec1, sec2);
		}

		/// <summary>
		/// 
		/// </summary>
		[TestMethod]
		public void ISectionInstantiation_Success()
		{
			ISection section = new Section(SectionID.FromString("1234B012"));
		}

		/// <summary>
		/// 
		///</summary>
		[TestMethod]
		public void ISections_AreEqual()
		{
			ISection sec1 = new Section(SectionID.FromString("1234B012"));
			ISection sec2 = new Section(SectionID.FromString("1234B012"));

			Assert.AreEqual(sec1, sec2);
		}

		/// <summary>
		/// 
		///</summary>
		[TestMethod]
		public void ISections_AreNotEqual()
		{
			ISection sec1 = new Section(SectionID.FromString("1234B012"));
			ISection sec2 = new Section(SectionID.FromString("4321B121"));

			Assert.AreNotEqual(sec1, sec2);
		}

		/// <summary>
		/// 
		///</summary>
		[TestMethod]
		public void ISectionSection_AreEqual()
		{
			ISection sec1 = new Section(SectionID.FromString("1234B012"));
			Section sec2 = new Section(SectionID.FromString("1234B012"));

			Assert.AreEqual(sec1, sec2);
		}

		/// <summary>
		/// 
		///</summary>
		[TestMethod]
		public void ISectionSection_AreNotEqual()
		{
			ISection sec1 = new Section(SectionID.FromString("1234B012"));
			Section sec2 = new Section(SectionID.FromString("4321B121"));

			Assert.AreNotEqual(sec1, sec2);
		}

		/// <summary>
		/// 
		///</summary>
		[TestMethod]
		public void SectionISection_AreEqual()
		{
			Section sec1 = new Section(SectionID.FromString("1234B012"));
			ISection sec2 = new Section(SectionID.FromString("1234B012"));

			Assert.AreEqual(sec1, sec2);
		}

		/// <summary>
		/// 
		///</summary>
		[TestMethod]
		public void SectionISection_AreNotEqual()
		{
			Section sec1 = new Section(SectionID.FromString("1234B012"));
			ISection sec2 = new Section(SectionID.FromString("4321B121"));

			Assert.AreNotEqual(sec1, sec2);
		}

	}
}
