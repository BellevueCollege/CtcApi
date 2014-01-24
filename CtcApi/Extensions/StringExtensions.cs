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
		/// Combines two or more strings into a valid file/folder path.
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
		/// Combines two or more strings into a URL.
		/// </summary>
		/// <param name="src"></param>
		/// <param name="urlPart"></param>
		/// <returns></returns>
		/// <seealso cref="Path.Combine(string[])"/>
		public static string CombineUrl(this string src, params string[] urlPart)
		{
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
		/// Capitalizes the first letter of each word in the string.
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
		/// Joins an array of strings into one.
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
		/// Saves the string to a file.
		/// </summary>
		/// <param name="str"></param>
		/// <param name="filename"></param>
		/// <returns></returns>
		/// <seealso cref="ByteExtensions.ToFile"/>
		public static bool ToFile(this string str, string filename)
		{
			byte[] data = Encoding.UTF8.GetBytes(str);
			return data.ToFile(filename, str.Length);
		}

	  /// <summary>
	  /// Safely converts a string to an int.
	  /// </summary>
	  /// <param name="value"></param>
	  /// <param name="defaultValue">A default value to return if the <param name="value"/> cannot be converted to an int.</param>
	  /// <returns>
	  ///   The <see>parsed<cref>Int32.Parse</cref></see> value of the <param name="value">string</param>,
	  ///   otherwise the <param name="defaultValue"/>.
	  /// </returns>
	  public static int SafeToInt32(this string value, int defaultValue)
    {
      if (!string.IsNullOrWhiteSpace(value))
      {
        int outValue;

        if (int.TryParse(value, out outValue))
        {
          return outValue;
        }
      }
      return defaultValue;
    }

    /// <summary>
    /// Identifies whether or not a string can be converted to a number
    /// </summary>
    /// <param name="value">The <see cref="String"/> to evaluate.</param>
    /// <returns>
    ///   <i>True</i> if <paramref name="value"/> can be successfully converted to a <see cref="Double"/>, otherwise <i>false</i>.
    /// </returns>
    public static bool IsNumber(this string value)
    {
      double bucket;
      if (double.TryParse(value, out bucket))
      {
        return true;
      }
      return false;
    }

    /// <summary>
    /// Identifies whether or not a string can be converted to an <see cref="Int32">int</see>
    /// </summary>
    /// <param name="value">The <see cref="String"/> to evaluate.</param>
    /// <returns>
    ///   <i>True</i> if <paramref name="value"/> can be successfully converted to an <see cref="Int32">int</see>, otherwise <i>false</i>.
    /// </returns>
    public static bool IsInt(this string value)
    {
      int bucket;
      if (int.TryParse(value, out bucket))
      {
        return true;
      }
      return false;
    }

    /// <summary>
    /// Converts an empty <see cref="String"/> to <i>null</i>
    /// </summary>
    /// <param name="value">The <see cref="String"/> to evaluate.</param>
    /// <returns>
    ///   A <i>null</i> <see cref="String"/> if <paramref name="value"/> is either <i>null</i> or contains only whitespace, otherwise <paramref name="value"/>.
    /// </returns>
    public static string Nullify(this string value)
    {
      return String.IsNullOrWhiteSpace(value) ? null : value;
    }
  }
}