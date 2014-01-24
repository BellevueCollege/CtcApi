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
using System.Runtime.Serialization;

namespace Ctc.Ods.Types
{
	/// <summary>
	/// 
	/// </summary>
    [DataContract]
	public class CourseDescription : ICourseDescription
	{
		/// <summary>
		/// Non-unique field that maps to CourseID in Course
		/// </summary>
        [DataMember]
        public string CourseID { get; internal set; }

		/// <summary>
		/// Long course description describing a course
		/// </summary>
        [DataMember]
        public string Description { get; internal set; }

		/// <summary>
		/// Effective Year/Quarter that the description applies to
		/// </summary>
        [DataMember]
        public YearQuarter YearQuarterBegin { get; internal set; }

		/// <summary>
		/// Provided for compatiblity with Entity Framework
		/// </summary>
		/// <remarks>
		/// Instantiating object properties from Entity Framework <i>selects</i> does not
		/// support parameter constructors or static methods, so we need to provide a
		/// property of a primitive data type to assign to.
		/// </remarks>
		/// <seealso cref="YearQuarterBegin"/>
		internal string _YearQuarterBegin
		{
			set {YearQuarterBegin = YearQuarter.FromString(value);}
		}
	}
}