using System;
using System.Data.Entity;
using System.Data.Objects.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using Ctc.Ods.Data;

namespace Ctc.Ods
{
    ///<summary>
    ///</summary>
    public class CreditsFacet : ISectionFacet
    {
        // TODO:
        // - Take variable credit courses into account
        // - Fix documentation
        // - Add unit tests

        private int  _credits;

        ///<summary>
        ///</summary>
        ///<param name="days">Number of credits to filter for</param>
        public CreditsFacet(int credits)
        {
            _credits = credits;
        }

        /// <summary>
        /// Provides the appropriate anonymous method for a LINQ .Where() call, depending on the option specified
        /// </summary>
        /// <param name="dbContext"></param>
        public Expression<Func<T, bool>> GetFilter<T>(DbContext dbContext) where T : SectionEntity
        {
            if (dbContext != null)
            {
                OdsContext db = dbContext as OdsContext;

                if (db != null)
                {
                    return s => Math.Max(Math.Floor(s.Credits), 1) == _credits;
                }

                throw new ArgumentNullException("dbContext", "Database context is not valid.");
            }

            throw new ArgumentNullException("dbContext", "Database context is null.");
        }
    }
}