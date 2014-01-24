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
	/// </summary>
	public interface ICourse : IRichDataObject, IEquatable<ICourse>
	{
		/// <summary>
		/// The unique ID for the course
		/// </summary>
		/// <remarks>
		/// This is a combination of the <see cref="Subject"/> and <see cref="Number"/>
		/// </remarks>
		string CourseID { get;}

		/// <summary>
		/// Departmental prefix for the course (e.g. ART)
		/// </summary>
		/// <seealso cref="Number"/>
		/// <seealso cref="IsCommonCourse"/>
		string Subject { get; }

		/// <summary>
		/// The (typically numeric) identifier for the course (e.g. 101)
		/// </summary>
		/// <seealso cref="Subject"/>
		string Number { get; }

		/// <summary>
		/// Short title of the course (e.g. Beginning Art)
		/// </summary>
		string Title { get; }

		/// <summary>
		/// One or more long descriptions (defined by <see cref="ICourseDescription.YearQuarterBegin">begin date</see>) for the course
		/// </summary>
		IEnumerable<CourseDescription> Descriptions { get; }

		/// <summary>
		/// Number of credits earned upon completion of the course
		/// </summary>
		Decimal Credits { get; }

		/// <summary>
		/// Indicates whether this is a <a href="http://www.sbctc.ctc.edu/college/e_commoncoursenumbering.aspx">Common Course</a>
		/// </summary>
		/// <seealso cref="Subject"/>
		bool IsCommonCourse { get; }

		/// <summary>
		/// The <see cref="YearQuarter"/> that the current <see cref="Course"/> becomes active.
		/// </summary>
		YearQuarter YearQuarterBegin{get;}

		/// <summary>
		/// The <see cref="YearQuarter"/> that the current <see cref="Course"/> records expires.
		/// </summary>
		YearQuarter YearQuarterEnd{get;}

		/// <summary>
		/// Indicates whether this <see cref="Course"/> can be taken for a variable number of <see cref="Credits"/>.
		/// </summary>
		bool IsVariableCredits{get;}

		/// <summary>
		/// Provides a collection of Footnotes associated with the current <see cref="Course"/>
		/// </summary>
		IList<string> Footnotes{get;}
	}
}
