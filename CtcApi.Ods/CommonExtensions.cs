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
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Ctc.Ods 
{
	///<summary>
	///</summary>
	public static class CommonExtensions
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="val"></param>
		/// <param name="append"></param>
		/// <remarks>
		/// Code taken from
		/// <a href="http://stackoverflow.com/questions/372865/path-combine-for-urls/4275109#4275109">http://stackoverflow.com/questions/372865/path-combine-for-urls/4275109#4275109</a>
		/// </remarks>
		/// <returns></returns>
		public static string UriCombine (this string val, string append)
    {
        if (String.IsNullOrEmpty(val)) return append;
        if (String.IsNullOrEmpty(append)) return val;
        return val.TrimEnd('/') + "/" + append.TrimStart('/');
    } 

		/// <summary>
		/// Creates an MD5 fingerprint of the string.
		/// </summary>
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
		public static string ToMd5Fingerprint(this string s)
		{
			var bytes = Encoding.Unicode.GetBytes(s.ToCharArray());
			var hash = new MD5CryptoServiceProvider().ComputeHash(bytes);

			// concat the hash bytes into one long string
			return hash.Aggregate(new StringBuilder(32),
					(sb, b) => sb.Append(b.ToString("X2")))
					.ToString();
		}
	}
}
