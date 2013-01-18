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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Common.Logging;

namespace CtcApi.Extensions
{
	/// <summary>
	/// 
	/// </summary>
	/// <remarks>
	///		<note type="tip">
	///		Set the <b>EXTRA_VERBOSE</b> build flag to enable Trace-level log output for
	///		for members of this class.
	///		</note>
	/// </remarks>
	public static class StringExtensions
	{

		private static ILog _log = LogManager.GetCurrentClassLogger();
		private static readonly char _pathSeparator = '\\';
		private static readonly char _urlSeparator = '/';

		/// <summary>
		/// 
		/// </summary>
		/// <param name="src"></param>
		/// <param name="pathParts"></param>
		/// <returns></returns>
		public static string CombinePath(this string src, params string[] pathParts)
		{
			// TODO: add unit test for CombinePath()
			for (int i = 0; i < pathParts.Length; i++)
			{
				if (pathParts[i].StartsWith(_pathSeparator.ToString()))
				{
					// Remove leading path separators because Path.Combine() treats them as absolute paths
					// and thus ignores any previous parameter values.
					pathParts[i] = pathParts[i].TrimStart(_pathSeparator);
				}
			}

			IEnumerable<string> allParts = (new[] {src}).Union(pathParts);
			string path = Path.Combine(allParts.ToArray());
			return path;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="src"></param>
		/// <param name="urlPart"></param>
		/// <returns></returns>
		public static string UriCombine(this string src, params string[] urlPart)
		{
			// TODO: rename to CombineUrl()
			if (urlPart != null && urlPart.Length > 0)
			{
				StringBuilder buffer = new StringBuilder(src.TrimEnd(_urlSeparator));

				foreach (string part in urlPart)
				{
					if (!String.IsNullOrWhiteSpace(part))
					{
						buffer.Append(string.Concat(_urlSeparator.ToString(), part.TrimStart(_urlSeparator)));
					}
				}

				return buffer.ToString().TrimEnd(_urlSeparator);
			}

			return src;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="src"></param>
		/// <remarks>
		///		<para>
		///			See http://tiny.cc/wzawkw
		///		</para>
		/// 	<note type="important">
		///		This method has no affect on strings that are already <see cref="string.ToUpper()">upper case</see>.
		///		</note>
		///		<note type="tip">
		///		Set the <b>EXTRA_VERBOSE</b> build flag to enable Trace-level log output.
		///		</note>
		/// </remarks>
		/// <returns></returns>
		public static string TitleCase(this string src)
		{
#if EXTRA_VERBOSE
			_log.Trace(m => m("TitleCasing '{0}'...", src));
#endif

			if (!string.IsNullOrWhiteSpace(src))
			{
#if EXTRA_VERBOSE
				_log.Trace(m => m("String is a valid value."));
#endif

				StringBuilder result = new StringBuilder(src);
				result[0] = char.ToUpper(result[0]);
				for( int i = 1; i < result.Length; ++i )
				{
#if EXTRA_VERBOSE
					_log.Trace(m => m("{0}: Processing '{1}' -> '{2}'", i, result[i], char.ToUpper(result[i])));
#endif
					if( char.IsWhiteSpace(result[i - 1]) )
							result[i] = char.ToUpper(result[i]);
				}

#if EXTRA_VERBOSE
				_log.Trace(m => m("Returing TitleCased value: '{0}'", result));
#endif
				return result.ToString();				
			}

			return src;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="src"></param>
		/// <param name="seperator"></param>
		/// <returns></returns>
		public static string Mash(this string[] src, string seperator = ",")
		{
			if (src != null && src.Length > 0)
			{
				StringBuilder buffer = new StringBuilder(src[0]);
				for (int i = 1; i < src.Length; i++)
				{
					buffer.AppendFormat("{0}{1}", seperator, src[i]);
				}
				return buffer.ToString();
			}
			return string.Empty;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="str"></param>
		/// <param name="filename"></param>
		/// <returns></returns>
		public static bool ToFile(this string str, string filename)
		{
			byte[] data = Encoding.UTF8.GetBytes(str);
			return data.ToFile(filename, str.Length);
		}
	}
}