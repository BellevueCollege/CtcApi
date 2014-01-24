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
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Ctc.Ods.Config;
using Ctc.Ods.Data;
using Ctc.Ods.Types;

namespace Ctc.Ods
{
	/// <summary>
	/// 
	/// </summary>
	public class ModalityFacet : ISectionFacet
	{
		private Options _option;
		private ApiSettings _settings;

		/// <summary>
		/// Options for which <see cref="Section"/>s to include
		/// </summary>
		[Flags]
		public enum Options
		{
			/// <summary>
			/// Include all <see cref="Section"/>s
			/// </summary>
			All = 0x0,

			/// <summary>
			/// Include only online <see cref="Section"/>s
			/// </summary>
			Online = 0x2,

			/// <summary>
			/// Include only hybrid (both <see cref="Online"/> and <see cref="OnCampus"/>) <see cref="Section"/>s
			/// </summary>
			Hybrid = 0x4,

			/// <summary>
			/// Include only telecom <see cref="Section"/>s
			/// </summary>
			Telecourse = 0x10,

			/// <summary>
			/// Include only <see cref="Section"/>s that meet on campus (in a classroom)
			/// </summary>
			OnCampus = 0x20
		}

		/// <summary>
		/// Provides the appropriate anonymous method for a LINQ .Where() call, depending on the option specified
		/// </summary>
		/// <param name="dbContext"></param>
		/// <remarks>
		/// This property assumes the database field is 2 characters long.
		/// </remarks>
		/// <exception cref="InvalidOperationException">A valid <see cref="Options"/> value was not specified when the class was instantiated.</exception>
		/// <seealso cref="Options"/>
		public Expression<Func<T, bool>> GetFilter<T>(DbContext dbContext) where T : SectionEntity
		{
			string onlineFlag = _settings.ClassFlags.Online;
			string hybridFlag = _settings.ClassFlags.Hybrid;
			string telecourseFlag = _settings.ClassFlags.Telecourse;

			if (_option == Options.All)
			{
				return s => true;
			}

			if (dbContext != null)
			{
				OdsContext db = dbContext as OdsContext;

				if (db != null)
				{
					// Construct a list of ClassIDs that match the specified modality/ies...
					IQueryable<SectionEntity> includes = db.Sections.Where(s => false).Select (s => s);	// hack to produce a valid, but empty collection
					
					if ((_option & Options.Online) == Options.Online)
					{
						includes = includes.Union(db.Sections.Where(i => i.SBCTCMisc1.StartsWith(onlineFlag)).Select(i => i));
					}
					if ((_option & Options.Hybrid) == Options.Hybrid)
					{
						includes = includes.Union(db.Sections.Where(i => i.SBCTCMisc1.StartsWith(hybridFlag)).Select(i => i));
					}
					if ((_option & Options.Telecourse) == Options.Telecourse)
					{
						includes = includes.Union(db.Sections.Where(i => i.SBCTCMisc1.StartsWith(telecourseFlag)).Select(i => i));
					}
					if ((_option & Options.OnCampus) == Options.OnCampus)
					{
						includes = includes.Union(db.Sections.Where(i => i.SBCTCMisc1 == null || (!i.SBCTCMisc1.StartsWith(onlineFlag) && !i.SBCTCMisc1.StartsWith(hybridFlag) && !i.SBCTCMisc1.StartsWith(telecourseFlag))).Select(i => i));
					}

					// ... and then filter by that list
					return s => includes.Any(i => i.ClassID == s.ClassID);
				}
				
				throw new ArgumentNullException("dbContext", "Database context is not valid.");
			}
			
			throw new ArgumentNullException("dbContext", "Database context is null.");
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="option"></param>
		public ModalityFacet(Options option)
		{
			_option = option;
			_settings = ConfigurationManager.GetSection(ApiSettings.SectionName) as ApiSettings;
		}
	}
}