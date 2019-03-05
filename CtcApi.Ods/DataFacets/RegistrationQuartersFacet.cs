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
using CtcApi;

namespace Ctc.Ods
{
	///<summary>
	///</summary>
	public class RegistrationQuartersFacet : ISectionFacet
	{
		private int _quarterCount;
		private DateTime _today;
		private DateTime _registrationDate;
		private string _yrqMax;
		private ApiSettings _settings;
	  /// <summary>
		/// 
		/// </summary>
		private ApiSettings Settings
		{
			get
			{
				_settings = _settings ?? ConfigurationManager.GetSection(ApiSettings.SectionName) as ApiSettings;
				return _settings;
			}
		}

		///<summary>
		///</summary>
		public RegistrationQuartersFacet() : this(1, new ApplicationContext())
		{
		}

    /// <summary>
    /// 
    /// </summary>
    /// <param name="appContext"></param>
    public RegistrationQuartersFacet(ApplicationContext appContext) : this(1, appContext)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <paramref name="quarterCount"/>
    public RegistrationQuartersFacet(int quarterCount) : this(quarterCount, new ApplicationContext())
    {
    }

	  /// <summary>
	  /// </summary>
	  /// <param name="quarterCount"></param>
	  /// <param name="appContext"></param>
	  /// <exception cref="InvalidOperationException"><paramref name="quarterCount"/> cannot be zero (0).</exception>
	  public RegistrationQuartersFacet(int quarterCount, ApplicationContext appContext)
		{
			if (quarterCount == 0) {
				throw new InvalidOperationException("Quarter count cannot be zero (0). It must be a positive or negative number.");
			}
			_quarterCount = quarterCount;

			// LINQ for EF only supports primitive variables
			_today = appContext.CurrentDate ?? DateTime.Now;
			_yrqMax = Settings.YearQuarter.Max;
            // Registration information should be available *before* registration begins
            // NOTE: we jump ahead n days to simulate date lookup n days prior to the registration date
            _registrationDate = _today.Add(new TimeSpan(_settings.YearQuarter.RegistrationLeadDays, 0, 0, 0));
		}

		/// <summary>
		/// Provides the appropriate anonymous method for a LINQ .Where() call, depending on the option specified
		/// </summary>
		/// <param name="dbContext"></param>
		/// <exception cref="ArgumentNullException"><paramref name="dbContext"/>Is null, or is not a valid <see cref="OdsContext"/> object.</exception>
		public Expression<Func<T, bool>> GetFilter <T>(DbContext dbContext) where T : SectionEntity
		{
			if (dbContext != null)
			{
				OdsContext db = dbContext as OdsContext;

				if (db != null)
				{
					Expression<Func<T, bool>> filter;

					// TODO: refactor how we count quarters (forward and back)
					// idea:
					//	0		- 1 quarter, current
					// < 0	- that many *additional* previous quarters
					// > 0	- that many *additional* future quarters
					if (_quarterCount < 0)
					{
						// include current and PREVIOUS quarters
						int quarterCount = Math.Abs(_quarterCount);
						filter = s => db.YearQuarters.Join(db.WebRegistrationSettings, y => y.YearQuarterID, r => r.YearQuarterID, (y, r) => new {y, r})
						                						 .DefaultIfEmpty()
						                						 .Where(c => (c.r.FirstRegistrationDate != null && c.r.FirstRegistrationDate <= _registrationDate || c.y.FirstClassDay <= _today)
						                						 							&& c.y.YearQuarterID != _yrqMax)
						                						 .OrderByDescending(c => c.y.YearQuarterID)
						                						 .Take(quarterCount)
						                						 .Any(c => c.y.YearQuarterID == s.YearQuarterID);
					}
					else
					{
						// include current and FUTURE quarters
						filter = s => db.YearQuarters.Join(db.WebRegistrationSettings, y => y.YearQuarterID, r => r.YearQuarterID, (y, r) => new {y, r})
						                						 .DefaultIfEmpty()
						                						 .Where(c => (c.r.LastRegistrationDate != null && c.r.LastRegistrationDate > _registrationDate || c.y.LastClassDay > _today)
						                						 							&& c.y.YearQuarterID != _yrqMax)
						                						 .OrderBy(c => c.y.YearQuarterID)
						                						 .Take(_quarterCount)
						                						 .Any(c => c.y.YearQuarterID == s.YearQuarterID);
					}

					return filter;
				}
				
				throw new ArgumentNullException("dbContext", "Not a valid ODS database context");
			}
			
			throw new ArgumentNullException("dbContext", "Not a valid database context");
		}
	}
}