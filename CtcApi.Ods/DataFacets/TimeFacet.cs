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
using System.Data.Entity;
using System.Linq.Expressions;
using Ctc.Ods.Data;
using Ctc.Ods.Types;

namespace Ctc.Ods
{
	/// <summary>
	/// Defines a <see cref="Section"/> filter by start and end times
	/// </summary>
	public class TimeFacet : ISectionFacet
	{
		readonly private TimeSpan _startTimeRange;
		readonly private TimeSpan _endTimeRange;

		/// <summary>
		/// Creates a new <see cref="Section"/> filter by specifying a start and end time range
		/// </summary>
		/// <param name="startTimeRange">Exclude <see cref="Section"/>s which start before this time of day.</param>
		/// <param name="endTimeRange"></param>
		public TimeFacet(TimeSpan startTimeRange, TimeSpan endTimeRange)
		{
			_startTimeRange = startTimeRange;
			_endTimeRange = endTimeRange;
		}

		/// <summary>
		/// Filters the <see cref="SectionEntity"/> query to only include records between the specified start and end times
		/// </summary>
		/// <param name="dbContext"></param>
		public Expression<Func<T, bool>> GetFilter <T>(DbContext dbContext) where T : SectionEntity
		{
			OdsContext db = dbContext as OdsContext;

			DateTime startTime = Utility.GetHpTime(_startTimeRange);
			DateTime endTime = Utility.GetHpTime(_endTimeRange);

			return
					s => (( ! s.StartTime.HasValue) || (s.StartTime.Value.CompareTo(startTime) >= 0 && s.StartTime.Value.CompareTo(endTime) <= 0));
		}
	}
}