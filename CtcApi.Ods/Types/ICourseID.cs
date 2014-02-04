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

namespace Ctc.Ods.Types
{
	/// <summary>
	/// Representa the unique ID for a <see cref="Course"/>
	/// </summary>
	/// <remarks>
	/// This is a combination of the <see cref="Subject"/> and <see cref="Number"/>
	/// </remarks>
	public interface ICourseID : IEquatable<ICourseID>
	{
		/// <summary>
		/// Departmental prefix for the course (e.g. ART)
		/// </summary>
		/// <seealso cref="Number"/>
		/// <seealso cref="IsCommonCourse"/>
		string Subject{get;set;}

		/// <summary>
		/// The (typically numeric) identifier for the course (e.g. 101)
		/// </summary>
		/// <seealso cref="Subject"/>
		string Number{get;set;}

		/// <summary>
		/// Indicates whether this CourseID represents a <a href="http://www.sbctc.ctc.edu/college/e_commoncoursenumbering.aspx">Common Course</a>
		/// </summary>
		/// <seealso cref="Subject"/>
		bool IsCommonCourse{get;set;}
	}
}