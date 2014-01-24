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
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using Ctc.Ods.Config;

/*****************************************************************************************
 * These inheritence relationships were created following the examples in this blog post:
 * http://weblogs.asp.net/manavi/archive/2011/01/03/inheritance-mapping-strategies-with-entity-framework-code-first-ctp5-part-3-table-per-concrete-type-tpc-and-choosing-strategy-guidelines.aspx
 *****************************************************************************************/
namespace Ctc.Ods.Data
{
	/// <summary>
	/// Represents the vw_CourseDescription view in ODS
	/// </summary>
	/// <seealso cref="CourseDescriptionEntityBase"/>
	internal sealed class CourseDescription1Entity : CourseDescriptionEntityBase
	{
		// members are inherited from base class
	}

	/// <summary>
	/// Represents the vw_CourseDescription2 view in ODS
	/// </summary>
	/// <seealso cref="CourseDescriptionEntityBase"/>
	internal sealed class CourseDescription2Entity : CourseDescriptionEntityBase
	{
		// members are inherited from base class
	}

	/// <summary>
	/// Base implementation of the <see cref="CourseDescription1Entity"/> and <see cref="CourseDescription2Entity"/> classes
	/// </summary>
	/// <remarks>
	/// This class exists because <see cref="CourseDescription1Entity"/> and <see cref="CourseDescription2Entity"/> are
	/// identical, and this way we can maintain them both at a single point.
	/// </remarks>
	/// <seealso cref="CourseDescription1Entity"/>
	/// <seealso cref="CourseDescription2Entity"/>
	public abstract class CourseDescriptionEntityBase
	{
		private string _yearQuarterBegin;
		private ApiSettings _settings;

		/// <summary>
		/// Provides access to the configuration settings for the ODS API.
		/// </summary>
		protected ApiSettings Settings
		{
			get
			{
				return _settings = _settings ?? ConfigurationManager.GetSection(ApiSettings.SectionName) as ApiSettings;
			}
		}

		/// <summary>
		/// The unique ID for the CourseDescription record
		/// </summary>
		/// <remarks>
		/// WARNING: This value is guaranteed to be unique for each record in the database,
		/// but IS NOT guaranteed to remain constant for each record - DO NOT rely on this
		/// value to reference any given record.
		/// </remarks>
		[Key]
		public int CourseDescriptionID { get; set; }

		/// <summary>
		/// The unique ID for the course
		/// </summary>
		public string CourseID { get; internal set; }

		/// <summary>
		/// The long description for the course
		/// </summary>
		public string Description { get; internal set; }

		/// <summary>
		/// College Code ID
		/// </summary>
		public string ColCode { get; internal set; }

		/// <summary>
		/// YearQuarter this description takes/took effect
		/// </summary>
		/// <remarks>
		/// NOTE: if this value is null or an empty string it is replaced with <see cref="ApiSettings.YearQuarter.Min"/>
		/// </remarks>
		[Column("EffectiveYearQuarterBegin")]
		public string YearQuarterBegin
		{
			get {return (string.IsNullOrWhiteSpace(_yearQuarterBegin) ? Settings.YearQuarter.Min : _yearQuarterBegin);}
			internal set {_yearQuarterBegin = (string.IsNullOrWhiteSpace(value) ? Settings.YearQuarter.Min : value);}
		}
	}
}