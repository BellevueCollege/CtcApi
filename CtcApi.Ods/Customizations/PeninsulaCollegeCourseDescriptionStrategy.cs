//Copyright (C) 2011 Peninsula College
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
using System.Linq;
using Ctc.Ods.Data;
using Ctc.Ods.Types;

namespace Ctc.Ods.Customizations
{
	/// <summary>
	/// Implements Peninsula College's business logic for determining a <see cref="Course"/>'s description(s)
	/// </summary>
	internal class PeninsulaCollegeCourseDescriptionStrategy : ICourseDescriptionStrategy
	{
		private IEnumerable<CourseDescription1Entity> _PCdescriptionsEntity1;
		private IEnumerable<CourseDescription2Entity> _PCdescriptionsEntity2;

		/// <summary>
		/// Encapsulates Peninsula College's business logic for retrieving course descriptions
		/// </summary>
		/// <param name="PCdescriptionsEntity1">A collection of <see cref="Course"/> descriptions from the first table in the ODS.</param>
		/// <param name="PCdescriptionsEntity2">A collection of <see cref="Course"/> descriptions from the second table in the ODS.</param>
		/// <seealso cref="CourseDescription1Entity"/>
		/// <seealso cref="CourseDescription2Entity"/>
		public PeninsulaCollegeCourseDescriptionStrategy(IEnumerable<CourseDescription1Entity> PCdescriptionsEntity1, IEnumerable<CourseDescription2Entity> PCdescriptionsEntity2)
		{
			_PCdescriptionsEntity1 = PCdescriptionsEntity1;
			_PCdescriptionsEntity2 = PCdescriptionsEntity2;
		}

		/// <summary>
		/// Retrieves current and future course descriptions from the database
		/// </summary>
		/// <param name="courseId">The course ID to retrieve the description for.</param>
		/// <param name="yrq">
		///		The <see cref="YearQuarter"/> to retrieve the description for. If not supplied, will default to the
		///		<see cref="OdsRepository.CurrentYearQuarter"/>.
		/// </param>
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
		/// <exception cref="ArgumentNullException"><paramref name="yrq"/> is null.</exception>
		public IList<CourseDescription> GetDescriptions(ICourseID courseId, YearQuarter yrq)
		{
			if (yrq != null)
			{
			// Where() expressions within Entity Framework queries can only use primitive data types
			string strYrqId = yrq.ID;
			string subject = courseId.Subject;
			string courseNumber = courseId.Number;

			IEnumerable<CourseDescription> list = _PCdescriptionsEntity2.Where(desc => desc.CourseID.StartsWith(subject) && desc.CourseID.EndsWith(courseNumber))
				// get the active description for the current quarter...
																																.Where(desc => desc.YearQuarterBegin.CompareTo(strYrqId) <= 0)
																																.OrderByDescending(d => d.YearQuarterBegin)
																																.Take(1)
				// ...and any upcoming description changes
																																.Union(_PCdescriptionsEntity2.Where(d => d.CourseID.StartsWith(subject) && d.CourseID.EndsWith(courseNumber))
																																					.Where(d => d.YearQuarterBegin.CompareTo(strYrqId) > 0)
																																					.OrderBy(d => d.YearQuarterBegin)
																																).Select(c => new CourseDescription
																																{
																																	CourseID = c.CourseID,
																																	Description = c.Description,
																																	_YearQuarterBegin = c.YearQuarterBegin
																																});
			return list.ToList();
			}

			throw new ArgumentNullException("yrq", "YearQuarter cannot be null.");
		}
	}
}