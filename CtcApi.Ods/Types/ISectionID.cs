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
	/// Identifies a specific instance of an <see cref="ICourse"/>
	/// </summary>
	/// <remarks>
	/// The specifications for an <see cref="ISectionID"/> (AKA "ClassID") are defined
	/// by the system(s) maintained by the SBCTC. As of the time of this writing (2011),
	/// a SectionID is currently composed of a 4-digit <see cref="ItemNumber"/> followed by
	/// a 4-character <see cref="YearQuarter"/> ID.
	/// </remarks>
	/// <seealso cref="ItemNumber"/>
	/// <seealso cref="YearQuarter"/>
	public interface ISectionID : IEquatable<ISectionID>
	{
		/// <summary>
		/// A 4-digit number assigned by the <i>Student Management System (SMS)</i>
		/// </summary>
		/// <remarks>
		/// This number in combination with a <see cref="YearQuarter"/> should be a unique
		/// value which represents a specific class.
		/// </remarks>
		/// <seealso cref="ISectionID"/>
		string ItemNumber { get; }
		
		/// <summary>
		/// A 4-character <i>Year-Quarter (YRQ)</i>
		/// </summary>
		/// <seealso cref="ISectionID"/>
		string YearQuarter { get; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		bool Equals(string other);

	  /// <summary>
	  /// 
	  /// </summary>
	  /// <returns></returns>
	  string ToString();
	}
}