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

namespace Ctc.Ods
{
	/// <summary>
	/// 
	/// </summary>
	public class DaysFacet : ISectionFacet
	{
		readonly private Options _option;

		/// <summary>
		/// Options for which <see cref="Section"/>s to include
		/// </summary>
		[Flags]
		public enum Options
		{
			/// <summary>
			/// All days
			/// </summary>
			All = 0x0,
			
			/// <summary>
			/// Sunday
			/// </summary>
			Sunday = 0x2,
			
			/// <summary>
			/// Monday
			/// </summary>
			Monday = 0x4,
			
			/// <summary>
			/// Tuesday
			/// </summary>
			Tuesday = 0x10,
			
			/// <summary>
			/// Wednesday
			/// </summary>
			Wednesday = 0x20,

			/// <summary>
			/// Thursday
			/// </summary>
			Thursday = 0x40,

			/// <summary>
			/// Friday
			/// </summary>
			Friday = 0x80,

			/// <summary>
			/// Saturday
			/// </summary>
			Saturday = 0x100
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="option"></param>
		public DaysFacet(Options option)
		{
			_option = option;
		}

		/// <summary>
		/// Provides the appropriate anonymous method for a LINQ .Where() call, depending on the option specified
		/// </summary>
		/// <note>This method assumes that the <b>Title</b> field in the database contains unique values AND the existence of certain values.</note>
		/// <param name="dbContext"></param>
		/// <exception cref="ArgumentNullException"><paramref name="dbContext"/> is null.</exception>
		/// <exception cref="InvalidOperationException">A valid option value was not specified when the class was instantiated.</exception>
		public Expression<Func<T, bool>> GetFilter <T>(DbContext dbContext) where T : SectionEntity
		{
			if (dbContext != null)
			{
				OdsContext db = dbContext as OdsContext;
				
				if (db != null)
				{
					if (_option == Options.All) {
						// don't filter by days
						return s => true;
					}
					
					string days = GetDaysTitle();

					if (days == "MTWThF")
					{
						return s => db.InstructionDetails.Join(db.Days, i => i.DayID, d => d.DayID, (i, d) => new {i, d})
					            											 .Any(h => h.i.ClassID == s.ClassID && (h.d.Title == days || h.d.Title.ToUpper() == "DAILY"));
					}
					// else
					return s => db.InstructionDetails.Join(db.Days, i => i.DayID, d => d.DayID, (i, d) => new {i, d})
					            										 .Any(h => h.d.Title == days && h.i.ClassID == s.ClassID);
				}
				
				throw new ArgumentNullException("dbContext", "Database context is not valid.");
			}
			
			throw new ArgumentNullException("dbContext", "Database context is null.");
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private string GetDaysTitle()
		{
			StringBuilder title = new StringBuilder(10);
			
			if ((_option & Options.Monday) == Options.Monday) {
				title.Append("M");
			}
			if ((_option & Options.Tuesday) == Options.Tuesday) {
				title.Append("T");
			}
			if ((_option & Options.Wednesday) == Options.Wednesday) {
				title.Append("W");
			}
			if ((_option & Options.Thursday) == Options.Thursday) {
				title.Append("Th");
			}
			if ((_option & Options.Friday) == Options.Friday) {
				title.Append("F");
			}
			if ((_option & Options.Saturday) == Options.Saturday) {
				title.Append("Sa");
			}
			if ((_option & Options.Sunday) == Options.Sunday) {
				title.Append("Su");
			}

			return title.ToString();
		}
	}
}
