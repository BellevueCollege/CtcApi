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
using System.Text;
using Ctc.Ods.Data;
using Ctc.Ods.Types;
using System.Collections.Generic;

namespace Ctc.Ods
{
    public class YearQuarterFacet : ISectionFacet
    {
        readonly private IList<String> _yearQuarterIDs;

        public YearQuarterFacet(YearQuarter yearQuarter)
        {
            _yearQuarterIDs = new List<String>();
            _yearQuarterIDs.Add(yearQuarter.ID);
        }
        public YearQuarterFacet(IList<YearQuarter> yearQuarters)
        {
            _yearQuarterIDs = yearQuarters.Select(d => d.ID).ToList<String>();
        }

        /// <summary>
        /// Provides the appropriate anonymous method for a LINQ .Where() call, depending on the option specified
        /// </summary>
        /// <param name="dbContext"></param>
        /// <exception cref="ArgumentNullException"><paramref name="dbContext"/> is null.</exception>
        /// <exception cref="InvalidOperationException">A valid option value was not specified when the class was instantiated.</exception>
        public Expression<Func<T, bool>> GetFilter<T>(DbContext dbContext) where T : SectionEntity
        {
            if (dbContext != null)
            {
                OdsContext db = dbContext as OdsContext;

                if (db != null)
                {
                    return s => _yearQuarterIDs.Contains(s.YearQuarterID);
                }

                throw new ArgumentNullException("dbContext", "Database context is not valid.");
            }

            throw new ArgumentNullException("dbContext", "Database context is null.");
        }
    }
}
