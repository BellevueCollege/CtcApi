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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Ctc.Ods
{
	/// <summary>
	/// Extension methods for LINQ
	/// </summary>
	public static class LinqExtensions
	{
		/// <summary>
		/// Applies multiple anonymous <i>.Where()</i> calls as a single query
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <param name="whereClause"></param>
		/// <returns></returns>
		public static IQueryable<T> CompoundWhere <T>(this IQueryable<T> source, params Expression<Func<T, bool>>[] whereClause)
			where T: class
		{
			if (whereClause[0] != null)
			{
				IQueryable<T> filtered = source.Where(whereClause[0]);

				// remove the first clause (because we just applied it), and process the remaining ones
				if(whereClause.Length > 1)
				{
					int clauseCount = whereClause.Length - 1;
			
					Expression<Func<T, bool>>[] newClauses = new Expression<Func<T, bool>>[clauseCount];
					Array.Copy(whereClause, 1, newClauses, 0, clauseCount);
			
					filtered = filtered.CompoundWhere(newClauses);
				}
	
				return filtered;
			}
			
			// invalid whereClause - keep going with unaltered source
			return source;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <param name="selector"></param>
		/// <param name="separator"></param>
		/// <returns></returns>
		/// <remarks>
		/// <para>
		/// This code is copied from <a href="https://petemontgomery.wordpress.com/2008/08/07/caching-the-results-of-linq-queries/">https://petemontgomery.wordpress.com/2008/08/07/caching-the-results-of-linq-queries/</a>
		/// </para>
		/// <para>
		/// Copyright (c) 2010 Pete Montgomery.
		/// http://petemontgomery.wordpress.com
		/// <br/>
		/// Licenced under GNU LGPL v3.
		/// http://www.gnu.org/licenses/lgpl.html
		/// </para>
		/// </remarks>
		public static string ToConcatenatedString<T>(this IEnumerable<T> source, Func<T, string> selector, string separator)
		{
			var b = new StringBuilder();
			bool needSeparator = false;

			foreach (var item in source)
			{
				if (needSeparator)
					b.Append(separator);

				b.Append(selector(item));
				needSeparator = true;
			}

			return b.ToString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <returns></returns>
		/// <remarks>
		/// <para>
		/// This code is copied from <a href="https://petemontgomery.wordpress.com/2008/08/07/caching-the-results-of-linq-queries/">https://petemontgomery.wordpress.com/2008/08/07/caching-the-results-of-linq-queries/</a>
		/// </para>
		/// <para>
		/// Copyright (c) 2010 Pete Montgomery.
		/// http://petemontgomery.wordpress.com
		/// <br/>
		/// Licenced under GNU LGPL v3.
		/// http://www.gnu.org/licenses/lgpl.html
		/// </para>
		/// </remarks>
		public static LinkedList<T> ToLinkedList<T>(this IEnumerable<T> source)
		{
			return new LinkedList<T>(source);
		}
	}
}
