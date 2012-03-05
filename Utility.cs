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
using System.Configuration;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Ctc.Ods.Config;
using Ctc.Ods.Types;

namespace Ctc.Ods
{
	///<summary>
	/// Contains a collections of useful methods/properties/objects.
	///</summary>
	public class Utility
	{
		// AppSettings keys
		/// <summary>
		/// If present in &gt;appSettings&lt; this value will override <see cref="Today"/>
		/// </summary>
		/// <remarks>
		/// This &gt;appSettings&lt; key is currently <i>CurrentDateOverride</i>
		/// </remarks>
		private const string DATE_OVERRIDE_KEY = "CurrentDateOverride";	// optional

		private static DateTime _today = DateTime.MinValue;

		/// <summary>
		/// The canononical name of the currently executing <see cref="Assembly"/>
		/// </summary>
		/// <remarks>
		///		<note type="caution">THIS MEMBER DOES NOT YET HAVE UNIT TESTS</note>
		/// </remarks>
		static public string AssemblyName
		{
			get { return Assembly.GetExecutingAssembly().GetName().ToString(); }
		}

		/// <summary>
		/// Replacement for <see cref="DateTime.Today"/> which can be overriden in the .config file
		/// </summary>
		/// <remarks>
		///		<note type="caution">THIS MEMBER DOES NOT YET HAVE UNIT TESTS</note>
		/// Use this property in place of <see cref="DateTime.Today"/> becuase it permits overriding the
		/// current date by setting the <see cref="DATE_OVERRIDE_KEY"/> in the application's .config
		/// file. This can be very useful for testing and/or troubleshooting.
		/// </remarks>
		/// <value>Either the <see cref="DateTime">Date</see> provided by <see cref="DATE_OVERRIDE_KEY"/> or <see cref="DateTime.Today"/></value>
		static public DateTime Today
		{
			get
			{
				if (_today == DateTime.MinValue)
				{
					string todayOverride = ConfigurationManager.AppSettings[DATE_OVERRIDE_KEY];
					if (string.IsNullOrWhiteSpace(todayOverride) || !DateTime.TryParse(todayOverride, out _today))
					{
						_today = DateTime.Today;
					}
				}
				return _today;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		static public ApiSettings GetApiSettings()
		{
			ApiSettings settings = ConfigurationManager.GetSection(ApiSettings.SectionName) as ApiSettings;
			// TODO: return a useful/default object if no settings are found in config file
			// (we might only need to set default values in the ApiSettings object to accomplish this)
			return settings;
		}

		/// <summary>
		/// Creates a <see cref="DateTime"/> object which conforms to HP fields - which only contain meaningful time information.
		/// </summary>
		/// <param name="timeSpan"></param>
		/// <returns>
		/// A <see cref="DateTime"/> value for 1/1/1900 (the default for an "empty" date in
		/// the HP3000) with the <paramref name="timeSpan">time</paramref> supplied.
		/// </returns>
		/// <remarks>
		///		<note type="caution">THIS MEMBER DOES NOT YET HAVE UNIT TESTS</note>
		/// </remarks>
		static public DateTime GetHpTime(TimeSpan timeSpan)
		{
			return new DateTime(1900, 1, 1, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
		}

		/// <summary>
		/// Converts a single digit to a <see langref="char"/> value
		/// </summary>
		/// <param name="value">A single digit unsigned number</param>
		/// <returns>
		///		A <see langref="char"/> representation of the number provided.
		///		<note type="important">This is <b>NOT</b> the ASCII character.</note>
		///	</returns>
		/// <remarks>
		///		<note type="caution">THIS MEMBER DOES NOT YET HAVE UNIT TESTS</note>
		///		<para>
		///		This method was originally created to support <see cref="YearQuarter"/> calculations.
		///		</para>
		/// </remarks>
		static public char DigitToChar(ushort value)
		{
			// TODO: assert incoming value is a single digit
			return Convert.ToChar(value.ToString());
		}

		/// <summary>
		/// Performs addition on a single-digit value, wrapping around the boundries
		/// </summary>
		/// <param name="value">A single digit unsigned number</param>
		/// <param name="adder">How much to add to <paramref name="value"/></param>
		/// <returns>
		///		If <paramref name="value"/> is at either end of the single digit range (0 or 9)
		///		<paramref name="adder"/> is applied to the opposite end, so that the incrementation
		///		wraps around the range. Otherwise the sum of <paramref name="value"/> and
		///		<paramref name="adder"/>
		/// </returns>
		/// <remarks>
		///		<note type="caution">THIS MEMBER DOES NOT YET HAVE UNIT TESTS</note>
		///		<note type="caution">
		///		At the moment, this method only supports an <paramref name="adder"/> value of
		///		1 or -1.
		///		</note>
		///		<para>
		///		This method was originally created to support <see cref="YearQuarter"/> calculations.
		///		</para>
		/// </remarks>
		static public ushort CircularDigitSum(ushort value, short adder)
		{
			if (adder < 0)
			{
				return (ushort)(value == 0 ? 9 : value + adder);
			}
			return (ushort)(value == 9 ? 0 : value + adder);
		}

		/// <summary>
		/// Safely converts a <see langref="string"/> value to a <see langref="bool"/>
		/// </summary>
		/// <param name="value">The value to attempt to convert</param>
		/// <returns>
		///		<i>true</i> if <paramref name="value"/> is equivalent to <see cref="bool.TrueString"/>,
		///		otherwise <i>false</i>.
		/// </returns>
		/// <remarks>
		/// Unlike the standard <see cref="Boolean"/> conversion methods, <b>SafeConvertToBool</b>
		/// will silentsly return <i>false</i> if it encounters an error during the conversion attempt.
		/// </remarks>
		static public bool SafeConvertToBool(string value)
		{
			bool result;
			return bool.TryParse(value, out result) ? result : false;
		}

		///<summary>
		/// Retrieves the various components of an HP3000 CourseID value
		///</summary>
		///<param name="value">The <see langref="string"/> value to parse.</param>
		///<param name="isCommonCourse"><i>True</i> if the CourseID is for a Common Course, otherwise <i>false</i>.</param>
		///<param name="subject">The subject abbreviation for the <see cref="Course"/> (e.g. ENGL).</param>
		///<param name="number">The number for the <see cref="Course"/> (e.g. 101).</param>
		///<param name="commonCourseChar">The character(s) to use to identify whether or not the CourseID represents a Common Course.</param>
		///<returns>A <see langref="string"/> representing the CourseID without the Common Course Identifier.</returns>
		/// <remarks>
		///		<note type="caution">THIS MEMBER DOES NOT YET HAVE UNIT TESTS</note>
		///		<para>A standard HP3000 CourseID is represented by:</para>
		///		<example>
		///			[<i>subject</i>][<i>&amp;</i>] [<i>number</i>]
		///		</example>
		///		<para>Where &amp; denotes a Common Course. Ex: <b>ENGL& 101</b></para>
		///	</remarks>
		static public string ParseCourseID(string value, out bool isCommonCourse, out string subject, out string number, string commonCourseChar)
		{
			isCommonCourse = value.Contains(commonCourseChar);

			char[] trimChars = commonCourseChar.ToCharArray();

			subject = isCommonCourse ? value.Substring(0, 5).Trim().Trim(trimChars) : value.Substring(0, 5).Trim();
			number = value.Length > 5 ? (isCommonCourse ? value.Substring(5).Trim().Trim(trimChars) : value.Substring(5).Trim()) : string.Empty;

			return String.Format("{0} {1}", subject, number);
		}

		/// <summary>
		/// Identifies whether or not the supplied value contains letters and/or numbers
		/// </summary>
		/// <param name="strValue">The value to check.</param>
		/// <returns>
		///		<i>True</i> if <paramref name="strValue"/> contains alphanumeric characters,
		///		otherwise <i>false</i>.
		/// </returns>
		/// <remarks>
		///		<note type="caution">THIS MEMBER DOES NOT YET HAVE UNIT TESTS</note>
		/// </remarks>
		public static bool IsAlphaNumeric(string strValue)
		{
			return Regex.IsMatch(strValue, "[A-Za-z0-9]");
		}

		/// <summary>
		/// Identifies whether the specified <see cref="String"/> can be converted to a number
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		/// <remarks>
		///		<note type="caution">THIS MEMBER DOES NOT YET HAVE UNIT TESTS</note>
		/// </remarks>
		static public bool IsNumber(string str)
		{
			int bucket;
			if (int.TryParse(str, out bucket))
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// Capitalizes the first letter of each word in a <see cref="String"/>
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		/// <remarks>
		///		<note type="caution">THIS MEMBER DOES NOT YET HAVE UNIT TESTS</note>
		///		This method utilizes code taken from <a href="http://channel9.msdn.com/forums/TechOff/252814-Howto-Capitalize-first-char-of-words-in-a-string-NETC/">this
		///		forum post</a>.
		/// </remarks>
		static public string TitleCase(string str)
		{
			if(String.IsNullOrWhiteSpace(str)) {
				throw new ArgumentNullException("value");
			}

			StringBuilder result = new StringBuilder(str.ToLower());
			result[0] = char.ToUpper(result[0]);
			 
			for( int i = 1; i < result.Length; ++i )
			{
				if( char.IsWhiteSpace(result[i - 1]) ) {
					result[i] = char.ToUpper(result[i]);
				}
			}

			return result.ToString();
		}

		/// <summary>
		/// Retrieve a <paramref name="dll"/>'s <see cref="Version"/>, with formatting if supplied
		/// </summary>
		/// <param name="dll"></param>
		/// <param name="format">
		///		A simple <see langref="string"/> template. The following token placeholders are recognized:
		///		<list	type="table">
		///			<listheader>
		///				<term>Token</term>
		///				<description>Replaced with</description>
		///			</listheader>
		///			<item>
		///				<term>{MAJOR}</term>
		///				<description><see cref="Version.Major"/></description>
		///			</item>
		///			<item>
		///				<term>{MINOR}</term>
		///				<description><see cref="Version.Minor"/></description>
		///			</item>
		///			<item>
		///				<term>{REVISION}</term>
		///				<description><see cref="Version.Revision"/></description>
		///			</item>
		///			<item>
		///				<term>{BUILD}</term>
		///				<description><see cref="Version.Build"/></description>
		///			</item>
		///		</list>
		/// </param>
		/// <returns>A string representation of the <paramref name="dll"/>'s <see cref="Version"/></returns>
		/// <remarks>
		///		<note type="caution">THIS MEMBER DOES NOT YET HAVE UNIT TESTS</note>
		/// </remarks>
		static public string GetApplicationVersion(Assembly dll, string format = null)
		{
//			LogHelper.Log.Trace("Toolkit::GetApplicationVersion(dll=[{0}], format='{1}')", dll != null ? dll.GetName().FullName : "null", format);

			Version ver = dll.GetName().Version;
			string versionOutput = format;

			if (String.IsNullOrWhiteSpace(format))
			{
				versionOutput = ver.ToString();
			}
			else
			{
				ReplaceIfPresent(ref versionOutput, "{MAJOR}", ver.Major);
				ReplaceIfPresent(ref versionOutput, "{MINOR}", ver.Minor);
				ReplaceIfPresent(ref versionOutput, "{REVISION}", ver.Revision);
				ReplaceIfPresent(ref versionOutput, "{BUILD}", ver.Build);
			}

//			LogHelper.Log.Debug("Formatted application version as '{0}'", versionOutput);
			return versionOutput;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="token"></param>
		/// <param name="component"></param>
		private static void ReplaceIfPresent(ref string buffer, string token, int component)
		{
//			LogHelper.Log.Trace("=> Version::ReplaceIfPresent(buffer='{0}', token='{1}', component={2})", buffer, token, component);

			if (buffer.Contains(token))
			{
//				LogHelper.Log.Trace("buffer contains '{0}' -> replacing with [{1}]...", token, component);
				buffer = buffer.Replace(token, component.ToString());
			}
		}

	}
}


