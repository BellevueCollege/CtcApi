using System;
using System.Data.Entity;
using System.Data.Objects.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using Ctc.Ods.Data;
using System.Configuration;
using Ctc.Ods.Config;

namespace Ctc.Ods
{
	///<summary>
	///</summary>
	public class LateStartFacet : ISectionFacet
	{
		private ushort _days;

		///<summary>
		///</summary>
		public LateStartFacet()
		{
      ApiSettings settings = ConfigurationManager.GetSection(ApiSettings.SectionName) as ApiSettings;
			_days = settings.ClassFlags.LateStartDaysCount; // TODO: what happens when flag dows not exist
		}

		/// <summary>
		/// Provides the appropriate anonymous method for a LINQ .Where() call, depending on the option specified
		/// </summary>
		/// <param name="dbContext"></param>
		public Expression<Func<T, bool>> GetFilter <T>(DbContext dbContext) where T : SectionEntity
		{
			if (dbContext != null)
			{
				OdsContext db = dbContext as OdsContext;

				if (db != null)
				{
					return s => s.StartDate.HasValue &&
											SqlFunctions.DateAdd("day", (_days * -1), s.StartDate) >= db.YearQuarters.Where(y => y.YearQuarterID == s.YearQuarterID)
							  																																		 .Select(y => y.FirstClassDay)
																																										 .FirstOrDefault();
				}
				
				throw new ArgumentNullException("dbContext", "Database context is not valid.");
			}
			
			throw new ArgumentNullException("dbContext", "Database context is null.");
		}
	}
}