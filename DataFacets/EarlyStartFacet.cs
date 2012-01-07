using System;
using System.Data.Entity;
using System.Linq.Expressions;
using Ctc.Ods.Data;

namespace Ctc.Ods
{
	public class EarlyStartFacet : ISectionFacet
	{
		/// <summary>
		/// Provides the appropriate anonymous method for a LINQ .Where() call, depending on the option specified
		/// </summary>
		/// <param name="dbContext"></param>
		public Expression<Func<T, bool>> GetFilter <T>(DbContext dbContext) where T : SectionEntity
		{
			throw new NotImplementedException();
		}
	}
}