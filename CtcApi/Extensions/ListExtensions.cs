//Copyright (C) 2012 Bellevue College
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
using System.Collections.Generic;

namespace CtcApi.Extensions
{
	public static class ListExtensions
	{
		/// <summary>
		/// Safely adds to the <paramref name="list"/> if <paramref name="range"/> is not null
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="range"></param>
		public static void AddRangeIfNotNull<T>(this List<T> list, IEnumerable<T> range) where T: class
		{
			if (range != null && list != null)
			{
				list.AddRange(range);
			}
		}
	}
}