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
using System.Linq;
using System.Linq.Expressions;
using Ctc.Ods.Data;
using Ctc.Ods.Types;
using Ctc.Ods.Config;
using System.Configuration;

namespace Ctc.Ods
{
	///<summary>
	/// Defines which <see cref="Section"/>s to include based on availability
	///</summary>
	/// <remarks>
	/// An "available" <see cref="Section"/> is currently defined as:
	/// <list type="bullet">
	///		<item>
	///			<description>
	///			<b>Normal Section:</b> <i>ClassCapacity</i> - <i>StudentsEnrolled</i>
	///			</description>
	///		</item>
	///		<item>
	///			<description>
	///			<b>Clustered Section:</b> max(<i>ClusterCapacity</i>) - sum(<i>StudentsEnrolled</i>)
	///			</description>
	///		</item>
	///		<item>
	///			<description>
	///			<b>And</b> Does not have a <see cref="OdsContext.WaitListCounts">Waitlist</see>
	///			</description>
	///		</item>
	/// </list>
	/// 
	/// NOTE: If these rules do not match the logic needed by your school, create a custom "availability" <see cref="ISectionFacet">Facet</see> to use instead.
	/// 
	/// </remarks>
	public class AvailabilityFacet : ISectionFacet
	{
		private Options _options;
		private string _waitlistStatus;

		///<summary>
		///</summary>
		public enum Options
		{
			///<summary>
			/// All Sections
			///</summary>
			All = 0x0,

			///<summary>
			/// Sections that have available seats
			///</summary>
			Open = 0x2
		}

		///<summary>
		/// Creates a new Facet with the <see cref="Options"/> specified
		///</summary>
		///<param name="options"></param>
		public AvailabilityFacet(Options options)
		{
			ApiSettings settings = ConfigurationManager.GetSection(ApiSettings.SectionName) as ApiSettings;
			_waitlistStatus = settings != null ? settings.Waitlist.Status : String.Empty;
			_options = options;
		}

		/// <summary>
		/// Gets filter to only include <see cref="Section"/>s which are "available"
		/// </summary>
		/// <param name="dbContext"></param>
		/// <remarks>
		/// An "available" <see cref="Section"/> is currently defined as:
		/// <list type="bullet">
		///		<item>
		///			<description>
		///			<b>Normal Section:</b> <i>ClassCapacity</i> - <i>StudentsEnrolled</i>
		///			</description>
		///		</item>
		///		<item>
		///			<description>
		///			<b>Clustered Section:</b> max(<i>ClusterCapacity</i>) - sum(<i>StudentsEnrolled</i>)
		///			</description>
		///		</item>
		///		<item>
		///			<description>
		///			<b>And</b> Does not have a <see cref="OdsContext.WaitListCounts">Waitlist</see>
		///			</description>
		///		</item>
		/// </list>
		/// 
		/// NOTE: If these rules do not match the logic needed by your school, create a custom "availability" <see cref="ISectionFacet">Facet</see> to use instead.
		/// 
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="dbContext"/>Is null, or is not a valid <see cref="OdsContext"/></exception>
		public Expression<Func<T, bool>> GetFilter <T>(DbContext dbContext) where T : SectionEntity
		{
			if (_options == Options.Open)
			{
				if (dbContext != null)
				{
					OdsContext db = dbContext as OdsContext;
					
					if (db != null)
					{
						return s => db.Sections.Where(section => section.ClusterItemNumber != null)
								     							 .GroupBy(section => section.ClassID, section => section, (id, sec) => new
								     		                                                                      						{
								     		                                                                      							ClassID = id,
								     		                                                                      							Capacity = sec.Max(c => c.ClusterCapacity),
								     		                                                                      							Enrolled = sec.Sum(c => c.StudentsEnrolled)
								     		                                                                      						})
								     							 .Where(cluster => (cluster.Capacity - cluster.Enrolled) > 0)
								     							 .Select(section => section.ClassID)
								     							 .Union(
								     							 		db.Sections.Where(sect => sect.ClusterItemNumber == null)
								     							 							 .Where(sect => sect.ClassCapacity - sect.StudentsEnrolled > 0)
								     							 							 .Select(sect => sect.ClassID)
								     							 )
																	 .Where(id => !db.WaitListCounts.Where(w => w.Status == _waitlistStatus).Select(w => w.ClassID).Contains(id))
								     							 .Contains(s.ClassID);
					}

					throw new ArgumentNullException("dbContext", "Not a valid ODS database context");
				}

				throw new ArgumentNullException("dbContext", "Not a valid database context");
			}

			return s => true;	// accept all records
		}
	}
}