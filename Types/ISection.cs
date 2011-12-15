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
using System.Collections.Generic;

namespace Ctc.Ods.Types
{
	/// <summary>
	/// Defines an instance of a college class (<see cref="Course"/>)
	/// </summary>
	public interface ISection : IEquatable<ISection>
	{
		/// <summary>
		/// 
		/// </summary>
		ISectionID ID{get;}

		/// <summary>
		/// 
		/// </summary>
		string CourseSubject{get;}

		/// <summary>
		/// 
		/// </summary>
		string CourseNumber{get;}

		/// <summary>
		/// 
		/// </summary>
		IList<CourseDescription> CourseDescriptions{get;}

		/// <summary>
		/// 
		/// </summary>
		YearQuarter Yrq {get;}

		/// <summary>
		/// 
		/// </summary>
		string CourseID{get;}

		/// <summary>
		/// 
		/// </summary>
		string CourseTitle{get;}

		/// <summary>
		/// 
		/// </summary>
		Decimal Credits{get;}

		/// <summary>
		/// 
		/// </summary>
		string SectionCode {get;}

		/// <summary>
		/// 
		/// </summary>
		IEnumerable<string> Footnotes { get; }

		// TODO: Seat availability

		/// <summary>
		/// Number of students on the waitlist
		/// </summary>
		/// <remarks>
		/// A non-zero value is also an indication that the <see cref="Section"/> is <i>closed</i>.
		/// </remarks>
		int WaitlistCount{get;}

		/// <summary>
		/// 
		/// </summary>
		bool IsOnline { get;}

		/// <summary>
		/// 
		/// </summary>
		bool IsCommonCourse{get;}

		/// <summary>
		/// 
		/// </summary>
		IEnumerable<OfferedItem> Offered{get;}
	}
}
