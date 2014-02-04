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
using System.Configuration;
using Ctc.Ods.Config;
using Ctc.Ods.Types;

namespace Ctc.Ods.Data
{
	/// <summary>
	/// Internal entity object for use in database access
	/// </summary>
	/// <remarks>
	/// DO NOT USE THIS CLASS IN YOUR CODE.  Use <see cref="ICourse"/> objects.
	/// </remarks>
	internal class CourseEntity
	{
		private string _yearQuarterBegin;
		private ApiSettings _settings;

		/// <summary>
		/// Provides access to configuration <see cref="ApiSettings"/>
		/// </summary>
		private ApiSettings Settings
		{
			get { return _settings = _settings ?? ConfigurationManager.GetSection(ApiSettings.SectionName) as ApiSettings; }
		}

		/// <summary>
		/// A unique record ID - required by Entity Framework
		/// </summary>
		/// <remarks>
		/// WARNING: This value is guaranteed to be unique for each record in the database,
		/// but IS NOT guaranteed to remain constant for each record - DO NOT rely on this
		/// value to reference any given record.
		/// </remarks>
		internal int UniqueId { get; set; }

		/// <summary>
		/// The unique ID for the course
		/// </summary>
		public string CourseID { get;set; }

		/// <summary>
		/// Short title of the course (e.g. Beginning Art)
		/// </summary>
		/// <seealso cref="Title2"/>
		public string Title1{get;set;}
		
		/// <summary>
		/// Alternate title of the course
		/// </summary>
		/// <seealso cref="Title1"/>
		public string Title2{get;set;}

		/// <summary>
		/// Number of credits earned upon completion of the course
		/// </summary>
		public decimal Credits{get;set;}

		/// <summary>
		/// The <see cref="YearQuarter"/> that this <see cref="Course"/> is/was first offered
		/// </summary>
		/// <remarks>
		/// If not specified in the ODS, YearQuarterBegin defaults <see cref="ApiSettings.YearQuarter.Min"/> so that
		/// it will be sure to come before any valid YRQ value during comparisons, etc.
		/// </remarks>
		public string YearQuarterBegin
		{
			get
			{
				return (_yearQuarterBegin ?? Settings.YearQuarter.Min);
			}
			set
			{
				_yearQuarterBegin = (value ?? Settings.YearQuarter.Min);
			}
		}

		/// <summary>
		/// The last <see cref="YearQuarter"/> that this <see cref="Course"/> is/was last offered
		/// </summary>
		public string YearQuarterEnd{get;set;}

		/// <summary>
		/// Identifies whether or not <see cref="SectionEntity">Section</see>s may be offered for
		/// varying <see cref="SectionEntity.Credits">Credit</see> amounts
		/// </summary>
		public bool? VariableCredits {get;set;}

		/// <summary>
		/// Identifies a <see cref="Footnote"/> associated with this <see cref="CourseEntity">Course</see>
		/// </summary>
		/// <remarks>
		/// The HP3000 allows two footnotes per Course.
		/// </remarks>
		/// <seealso cref="FootnoteID2"/>
		public string FootnoteID1{get;set;}

		/// <summary>
		/// Identifies a <see cref="Footnote"/> associated with this <see cref="CourseEntity">Course</see>
		/// </summary>
		/// <remarks>
		/// The HP3000 allows two footnotes per Course.
		/// </remarks>
		/// <seealso cref="FootnoteID1"/>
		public string FootnoteID2{get;set;}
	}
}