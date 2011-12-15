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

namespace Ctc.Ods
{
	///<summary>
	/// Contains a collections of useful methods/properties/objects.
	///</summary>
	public class Utility
	{
		// AppSettings keys
		private const string DATE_OVERRIDE_KEY = "CurrentDateOverride";	// optional

		private static DateTime _today = DateTime.MinValue;

		/// <summary>
		/// Parse the 4 char's in QuarterID to create the human readable Year Quarter
		/// </summary>
		/// <param name="quarterID">YearQuarterID ie. "B014"</param>
		/// <returns>Human readable Year Quarter, ie. "Spring 2010-2011"</returns>
		public string convertYQID(string quarterID)
		{
			string CharList = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"; // Base36 array

			// Break up the quarterID into 4 components
			char digit1 = quarterID[0]; // Decade multiplier -10
			int digit2 = Int32.Parse(quarterID.Substring(1, 1)); // Year start indicator
			int digit3 = Int32.Parse(quarterID.Substring(2, 1)); // Year end indicator
			int digit4 = Int32.Parse(quarterID.Substring(3, 1)); // Quarter/Session indicator

			int dec = CharList.IndexOf(digit1) - 10;
			int YearStart = 2000 + (dec * 10) + digit2;

			int YearEnd;

			if (digit2 > digit3)
			{
				YearEnd = 2000 + (dec * 10) + digit3 + 10;
			}
			else
				YearEnd = 2000 + (dec * 10) + digit3;

			// Determine Quarter Session
			string quarterName;
			switch (digit4)
			{
				case 1:
					quarterName = "Summer";
					break;
				case 2:
					quarterName = "Fall";
					break;
				case 3:
					quarterName = "Winter";
					break;
				case 4:
					quarterName = "Spring";
					break;
				default:
					quarterName = "Invalid";
					break;
			}
			// Return concat name and years
			string title = quarterName + " " + YearStart + "-" + YearEnd;
			return title;
		}

		/// <summary>
		/// The canononical name of the currently executing <see cref="Assembly"/>
		/// </summary>
		static public string AssemblyName
		{
			get { return Assembly.GetExecutingAssembly().GetName().ToString(); }
		}

		/// <summary>
		/// 
		/// </summary>
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
		/// Creates a <see cref="DateTime"/> object which conforms to HP fields which only contain meaningful time information.
		/// </summary>
		/// <param name="timeSpan"></param>
		/// <returns></returns>
		static public DateTime GetHpTime(TimeSpan timeSpan)
		{
			return new DateTime(1900, 1, 1, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		static public char DigitToChar(ushort value)
		{
			// TODO: assert incoming value is a single digit
			return Convert.ToChar(value.ToString());
		}

		/// <summary>
		/// Performs addition on a single-digit value, wrapping around the boundries
		/// </summary>
		/// <param name="value"></param>
		/// <param name="adder"></param>
		/// <returns></returns>
		static public ushort CircularDigitSum(ushort value, short adder)
		{
			if (adder < 0)
			{
				return (ushort)(value == 0 ? 9 : value + adder);
			}
			return (ushort)(value == 9 ? 0 : value + adder);
		}

		///<summary>
		///</summary>
		///<param name="value"></param>
		///<returns></returns>
		static public bool SafeConvertToBool(string value)
		{
			bool result;
			return bool.TryParse(value, out result) ? result : false;
		}

		///<summary>
		///</summary>
		///<param name="value"></param>
		///<param name="isCommonCourse"></param>
		///<param name="subject"></param>
		///<param name="number"></param>
		///<param name="commonCourseChar"></param>
		///<returns></returns>
		static public string ParseCourseID(string value, out bool isCommonCourse, out string subject, out string number, string commonCourseChar)
		{
			isCommonCourse = value.Contains(commonCourseChar);

			char[] trimChars = commonCourseChar.ToCharArray();

			subject = isCommonCourse ? value.Substring(0, 5).Trim().Trim(trimChars) : value.Substring(0, 5).Trim();
			number = value.Length > 5 ? (isCommonCourse ? value.Substring(5).Trim().Trim(trimChars) : value.Substring(5).Trim()) : string.Empty;

			return String.Format("{0} {1}", subject, number);
		}
	}
}


