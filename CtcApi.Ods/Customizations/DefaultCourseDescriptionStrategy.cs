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
using Ctc.Ods.Data;
using Ctc.Ods.Types;

namespace Ctc.Ods.Customizations
{
	/// <summary>
	/// NOT IMPLEMENTED - Default business logic for determining a <see cref="Course"/>'s description(s)
	/// </summary>
	public class DefaultCourseDescriptionStrategy : ICourseDescriptionStrategy
	{
		/// <summary>
		/// NOT IMPLEMENTED - Encapsulates default business logic for retrieving <see cref="Course"/> descriptions
		/// </summary>
		/// <param name="descriptions1Entities">A collection of <see cref="Course"/> descriptions from the first table in the ODS.</param>
		/// <param name="descriptions2Entities">A collection of <see cref="Course"/> descriptions from the second table in the ODS.</param>
		/// <exception cref="NotImplementedException"/>
		/// <seealso cref="CourseDescription1Entity"/>
		/// <seealso cref="CourseDescription2Entity"/>
		public DefaultCourseDescriptionStrategy(IEnumerable<CourseDescriptionEntityBase> descriptions1Entities, IEnumerable<CourseDescriptionEntityBase> descriptions2Entities)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Retrieves current and future course descriptions from the database
		/// </summary>
		/// <param name="courseId">The course ID to retrieve the description for.</param>
		/// <param name="yrq">
		///		The <see cref="YearQuarter"/> to retrieve the description for. If not supplied, will default to the
		///		<see cref="OdsRepository.CurrentYearQuarter"/>.
		/// </param>
		/// <remarks>
		/// This class provides a default fallback in situations where the instantiation of a custom <see cref="ICourseDescriptionStrategy"/> fails.
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
		/// <exception cref="NotImplementedException"/>
		public IList<CourseDescription> GetDescriptions(ICourseID courseId, YearQuarter yrq)
		{
			throw new NotImplementedException();
		}
	}
}