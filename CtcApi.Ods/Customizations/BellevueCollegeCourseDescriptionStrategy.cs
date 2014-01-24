//Copyright (C) 2011 Bellevue College
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
using System.Configuration;
using System.Linq;
using Ctc.Ods.Config;
using Ctc.Ods.Data;
using Ctc.Ods.Types;

namespace Ctc.Ods.Customizations
{
	/// <summary>
	/// Implements Bellevue College's business logic for determining a <see cref="Course"/>'s description(s)
	/// </summary>
	internal class BellevueCollegeCourseDescriptionStrategy : ICourseDescriptionStrategy
	{
		private IEnumerable<CourseDescription1Entity> _descriptionsEntity;
	  private readonly ApiSettings _settings = ConfigurationManager.GetSection(ApiSettings.SectionName) as ApiSettings;

	  /// <summary>
		/// Encapsulates Bellevue College's business logic for retrieving course descriptions
		/// </summary>
		/// <param name="descriptionsEntity">A collection of <see cref="Course"/> descriptions from the first table in the ODS.</param>
		/// <param name="dummyEntity">THIS VALUE IS IGNORED.  (Bellevue College does not use the second description table in the ODS.)</param>
		/// <seealso cref="CourseDescription1Entity"/>
		public BellevueCollegeCourseDescriptionStrategy(IEnumerable<CourseDescription1Entity> descriptionsEntity, IEnumerable<CourseDescription2Entity> dummyEntity)
		{
			_descriptionsEntity = descriptionsEntity;
			// ignore dummyEntity - we only use table 1
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
			if (_descriptionsEntity != null)
			{
				if (yrq != null)
				{
					// Where() expressions within Entity Framework queries can only use primitive data types
					string strYrqId = yrq.ID;
				  string subject = courseId.IsCommonCourse ? String.Concat(courseId.Subject, _settings.RegexPatterns.CommonCourseChar) : courseId.Subject;
					string courseNumber = courseId.Number;

					IEnumerable<CourseDescription> list = _descriptionsEntity.Where(desc => desc.CourseID.StartsWith(subject) && desc.CourseID.EndsWith(courseNumber))
																																		// get the active description for the current quarter...
																																		.Where(desc => desc.YearQuarterBegin.CompareTo(strYrqId) <= 0 )
																																		.OrderByDescending(d => d.YearQuarterBegin)
																																		.Take(1)
																																		// ...and any upcoming description changes
																																		.Union(_descriptionsEntity.Where(d => d.CourseID.StartsWith(subject) && d.CourseID.EndsWith(courseNumber))
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
			}
			else
			{
				// if we weren't given a valid _descriptionsEntity, return an empty list
				return new List<CourseDescription>();
			}
			
			throw new ArgumentNullException("yrq", "YearQuarter cannot be null.");
		}
	}
}