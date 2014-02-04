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
	/// Defines a subject of study to which <see cref="Course"/>s belong
	/// </summary>
	public interface ICoursePrefix : IEquatable<ICoursePrefix>
	{
		/// <summary>
		/// The five-character abbreviation which identifies a course of study (e.g. ENGL)
		/// </summary>
		/// <seealso cref="Title"/>
		string Subject { get; }

		/// <summary>
		/// The full name of a course of study (e.g. English)
		/// </summary>
		/// <seealso cref="Subject"/>
		string Title { get; }
	}
}
