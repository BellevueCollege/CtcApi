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
        private int _credits;

        ///<summary>
        /// 
        ///</summary>
        ///<param name="credits">Number of credits to filter for</param>
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
                    return s => (s.Credits < 1 ? 1 : Math.Floor(s.Credits)) == _credits || (s.VariableCredits == true && Math.Floor(s.Credits) >= _credits);
                }

                throw new ArgumentNullException("dbContext", "Database context is not valid.");
            }

            throw new ArgumentNullException("dbContext", "Database context is null.");
        }
    }
}