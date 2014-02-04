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
using Ctc.Ods.Config;
using Ctc.Ods.Types;

namespace Ctc.Ods.Data
{
	///<summary>
	/// Internal entity object for use in database access
	///</summary>
	/// <remarks>
	/// DO NOT USE THIS CLASS IN YOUR CODE.  Use <see cref="ICoursePrefix"/> objects.
	/// </remarks>
	[Table("vw_CoursePrefix")]
	internal class CoursePrefixEntity
	{
		/// <summary>
		/// The five-character abbreviation which identifies a course of study (e.g. ENGL)
		/// </summary>
		/// <remarks>
		/// PrefixID values that end in <see cref="ApiSettings.RegexPatterns.CommonCourseChar"/>
		/// only exist in the database to provide a relational lookup for <see cref="Course"/>s to
		/// which the Common Course designation applies. Since the API handles these relationships
		/// for us, and leaving this designator active creates duplicate subjects, it is filtered
		/// out of any list of CoursePrefixes.
		/// </remarks>
		[Key]
		public string CoursePrefixID {get;set;}

		/// <summary>
		/// The full name of a course of study (e.g. English)
		/// </summary>
		public string Title {get;set;}
	}
}