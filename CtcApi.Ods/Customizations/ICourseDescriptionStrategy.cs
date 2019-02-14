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
using System.Collections.Generic;
using Ctc.Ods.Data;
using Ctc.Ods.Types;

namespace Ctc.Ods.Customizations
{
	/// <summary>
	/// Defines a class which encapsulates the business logic for retrieving <see cref="Course"/> descriptions.
	/// </summary>
	/// <remarks>
	/// This customization point was implemented by following the <a href="http://www.dofactory.com/Patterns/PatternStrategy.aspx"
	/// ><i>Strategy</i> design pattern</a>.
	/// </remarks>
	internal interface ICourseDescriptionStrategy
	{
		/// <summary>
		/// Retrieves current and future course descriptions from the database
		/// </summary>
		/// <param name="courseId">The course ID to retrieve the description for.</param>
		/// <param name="yrq">
		///		The <see cref="YearQuarter"/> to retrieve the description for. If not supplied, will default to the
		///		<see cref="OdsRepository.CurrentYearQuarter"/>.
		/// </param>
		/// <remarks>
		///		<para>
		///		This interface provides a customization point for schools to provide their own business logic
		///		for retrieving current and future course descriptions from the ODS. It was necessary to create
		///		this customization because the HP (and thus the ODS) contains two course description tables
		///		(<i>CourseDescription</i> and <i>CourseDescription2</i> in the ODS), and different schools
		///		utilize these in different ways.
		///		</para>
		///		<note type="important">
		///		For compatibility, schools implementing <see cref="ICourseDescriptionStrategy"/> should ensure
		///		that their implementation returns records in the expected order.
		///		</note>
		/// </remarks>
		/// <returns>
		/// A collection of <see cref="CourseDescriptionEntityBase">course description entites</see>:
		/// <list type="table">
		///		<item>
		///			<term>First record</term>
		///			<description>
		///				The course description for the specified <paramref name="yrq"/> (or <see cref="OdsRepository.CurrentYearQuarter"/>
		///				if not supplied.
		///			</description>
		///		</item>
		///		<item>
		///			<term>Successive records</term>
		///			<description>
		///				Future course description updates, in the order they will be updated (by YearQuarter).
		///			</description>
		///		</item>
		/// </list>
		/// </returns>
		IList<CourseDescription> GetDescriptions(ICourseID courseId, YearQuarter yrq);
	}
}