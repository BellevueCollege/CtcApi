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
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using Ctc.Ods.Config;

namespace Ctc.Ods.Types
{
	/// <summary>
	/// 
	/// </summary>
	[DataContract]
	public class YearQuarter : IEquatable<YearQuarter>
	{
		private const int MAX_YEAR_ALLOWED = 2259;

		#region Public members
		/// <summary>
		/// The 4-character code that represents this YearQuarterID 
		/// </summary>
		[DataMember]
		public string ID { get; internal set; }

		/// <summary>
		/// The user-friendly name for the quarter (e.g. "Fall 2010")
		/// </summary>
		[IgnoreDataMember]
		public string FriendlyName
		{
			get
			{
				if (String.IsNullOrWhiteSpace(_friendlyName))
				{
					_friendlyName = ToFriendlyName(ID);
				}
				return _friendlyName;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="yearQuarterId"></param>
		/// <seealso cref="FromString"/>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="yearQuarterId"/> is not a 4-character value.</exception>
		/// <exception cref="NullReferenceException"><paramref name="yearQuarterId"/> in <i>null</i>.</exception>
		protected YearQuarter(string yearQuarterId)
		{
			ID = yearQuarterId;
		}

		/// <summary>
		/// Empty constructor required by LINQ to Entity Framework
		/// </summary>
		internal YearQuarter()
		{
		}

		/// <summary>
		/// Adds one quarter to the current <see cref="YearQuarter"/> value
		/// </summary>
		/// <returns>The next <see cref="YearQuarter"/> in the academic year.</returns>
		public YearQuarter Add()
		{
			// TODO: implement adding multiple quarters
			char[] newYrq = new char[4];

			switch (ID[QUARTER])
			{
				case '4':
					IncrementYear(ID, ref newYrq);
					newYrq[QUARTER] = '1';
					break;
				default:	// 1-3
					newYrq[QUARTER] = Convert.ToChar((Convert.ToInt32(ID[QUARTER].ToString()) + 1).ToString());	// increment
					// just copy the rest over
					for (int i = YEAR_MOD; i <= YEAR2; i++)
					{
						newYrq[i] = ID[i];
					}
					break;
			}
			string yrq = new string(newYrq);	// required because newYrq.ToString() == "[System.Char[]]"

			return new YearQuarter(yrq);
		}

		#region Overrides of Object
		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public override string ToString()
		{
			return ID;
		}
		#endregion

		#region Static members

		#region Factory methods
		/// <summary>
		/// Creates a new <see cref="YearQuarter"/> object from a string value
		/// </summary>
		/// <param name="yearQuarterId">The 4-character code that represents this YearQuarterID</param>
		/// <returns></returns>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="yearQuarterId"/> is not a 4-character value.</exception>
		/// <exception cref="NullReferenceException"><paramref name="yearQuarterId"/> in <i>null</i>.</exception>
		static public YearQuarter FromString(string yearQuarterId)
		{
			if (IsValid(yearQuarterId))
			{
				return new YearQuarter(yearQuarterId);
			}
			return null;
		}

		///<summary>
		///</summary>
		///<param name="quarter"></param>
		///<returns></returns>
		///<exception cref="NotImplementedException"></exception>
		static public YearQuarter FromFriendlyName(string quarter)
		{
			return new YearQuarter(ToYearQuarterID(quarter));
		}

		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="quarter">YearQuarter in a "friendly" format (e.g. "Fall2011", "Fall 2011")</param>
		/// <returns>An YearQuarterID value (e.g. "B122")</returns>
		///<exception cref="NotImplementedException"></exception>
		static public string ToYearQuarterID(string quarter)
		{
			quarter = quarter.Trim().ToUpper();

			if (string.IsNullOrWhiteSpace(quarter) || quarter.Length > 15)
			{
				throw new ArgumentOutOfRangeException("quarter", "Provided parameter is either empty or too long");
			}

			Regex quarterFormat = new Regex(@"^(FALL?|(SUM|SPR|WIN)\w{0,3})\s*\d{4}$");

			if (!quarterFormat.IsMatch(quarter))
			{
				throw new ArgumentOutOfRangeException("quarter", "Argument must be a valid quarter title in the form [season]<optional space>[4-digit year]");
			}

			// Should end with 4 digits, if passed previous check
			if (int.Parse(quarter.Substring(quarter.Length - 4)) > MAX_YEAR_ALLOWED)
			{
				throw new ArgumentOutOfRangeException("quarter", string.Format("The specified year cannot be larger than '{0}'", MAX_YEAR_ALLOWED));
			}

			char[] yrqId = new char[4];

			char[] year = quarter.Substring(quarter.Length - 4).ToCharArray();
			ushort yearDigit = ushort.Parse(year[3].ToString());
			ushort decadeDigit = ushort.Parse(year[2].ToString());
			ushort century = ushort.Parse(string.Format("{0}{1}",year[0], year[1]));

			switch (quarter.Substring(0, 3))
			{
				case "SUM":
					yrqId[QUARTER] = '1';
					yrqId[YEAR1] = year[3];
					yrqId[YEAR2] = Utility.DigitToChar(Utility.CircularDigitSum(yearDigit, 1));
					yrqId[YEAR_MOD] = GetYearMod(century, decadeDigit);
					break;
				case "FAL":
					yrqId[QUARTER] = '2';
					yrqId[YEAR1] = year[3];
					yrqId[YEAR2] = Utility.DigitToChar(Utility.CircularDigitSum(yearDigit, 1));
					yrqId[YEAR_MOD] = GetYearMod(century, decadeDigit);
					break;
				case "WIN":
					yrqId[QUARTER] = '3';
					yrqId[YEAR1] = Utility.DigitToChar(Utility.CircularDigitSum(yearDigit, -1));
					yrqId[YEAR2] = year[3];
					if (decadeDigit == 0)
					{
						yrqId[YEAR_MOD] = GetYearMod((ushort)(century - 1), Utility.CircularDigitSum(decadeDigit, -1));
					}
					else
					{
						yrqId[YEAR_MOD] = GetYearMod(century, (ushort)(yearDigit == 0 ? decadeDigit - 1 : decadeDigit));
					}
					break;
				case "SPR":
					yrqId[QUARTER] = '4';
					yrqId[YEAR1] = Utility.DigitToChar(Utility.CircularDigitSum(yearDigit, -1));
					yrqId[YEAR2] = year[3];
					if (decadeDigit == 0)
					{
						yrqId[YEAR_MOD] = GetYearMod((ushort)(century - 1), Utility.CircularDigitSum(decadeDigit, -1));
					}
					else
					{
						yrqId[YEAR_MOD] = GetYearMod(century, (ushort)(yearDigit == 0 ? decadeDigit - 1 : decadeDigit));
					}
					break;
			}

			return new string(yrqId);
		}

		/// <summary>
		/// Gets the user-friendly name for the quarter (e.g. "Fall 2010")
		/// </summary>
		/// <param name="yrq">A 4-character YRQ code.</param>
		/// <returns>
		///		A user-friendly string for the quarter in the form "Fall 2010". Can also return <see cref="MIN_FRIENDLY_NAME"/>
		///		or <see cref="MAX_FRIENDLY_NAME"/>, if the value of <paramref name="yrq"/> represents either of the valid
		///		limits.
		/// </returns>
		/// <seealso cref="ApiSettings.YearQuarter"/>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="yrq"/> is not a 4-character value.</exception>
		/// <exception cref="InvalidCastException"><paramref name="yrq"/> is not a valid YearQuarterID.</exception>
		/// <exception cref="NullReferenceException"><paramref name="yrq"/> in <i>null</i>.</exception>
		/// <remarks>
		/// </remarks>
		public static string ToFriendlyName(string yrq)
		{
			// check for min/max limits (which otherwise are not valid YRQ values)
			ApiSettings settings = ConfigurationManager.GetSection(ApiSettings.SectionName) as ApiSettings;
			if (yrq.ToUpper() == (settings != null ? settings.YearQuarter.Min : "0000")) return MIN_FRIENDLY_NAME;
			if (yrq.ToUpper() == (settings != null ? settings.YearQuarter.Max : "Z999")) return MAX_FRIENDLY_NAME;

			// if not min/max, continue...
			StringBuilder fullName = new StringBuilder();

			if (IsValid(yrq))
			{
				switch (yrq[QUARTER])
				{
					case '1':
						fullName.Append("Summer ");
						fullName.Append(GetYear(yrq[YEAR1], yrq[YEAR_MOD]));
						break;
					case '2':
						fullName.Append("Fall ");
						fullName.Append(GetYear(yrq[YEAR1], yrq[YEAR_MOD]));
						break;
					case '3':
						fullName.Append("Winter ");
						fullName.Append(GetYear(yrq[YEAR1], yrq[YEAR_MOD]) + 1);
						break;
					case '4':
						fullName.Append("Spring ");
						fullName.Append(GetYear(yrq[YEAR1], yrq[YEAR_MOD]) + 1);
						break;
					default:
						throw new InvalidCastException(String.Format("Unrecognized quarter value '{0}' in YRQ '{1}'", yrq[QUARTER], yrq));
				}
			}
			return fullName.ToString();
		}

		#endregion

		#endregion

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <returns>
		/// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
		/// </returns>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(YearQuarter other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}
			if (ReferenceEquals(this, other))
			{
				return true;
			}
			return Equals(other.ID, ID);
		}

		/// <summary>
		/// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
		/// </returns>
		/// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>. </param><filterpriority>2</filterpriority>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}
			if (ReferenceEquals(this, obj))
			{
				return true;
			}
			if (obj.GetType() != typeof(YearQuarter))
			{
				return false;
			}
			return Equals((YearQuarter)obj);
		}

		/// <summary>
		/// Serves as a hash function for a particular type. 
		/// </summary>
		/// <returns>
		/// A hash code for the current <see cref="T:System.Object"/>.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public override int GetHashCode()
		{
			return (ID != null ? ID.GetHashCode() : 0);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static bool operator ==(YearQuarter left, YearQuarter right)
		{
			return Equals(left, right);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static bool operator !=(YearQuarter left, YearQuarter right)
		{
			return !Equals(left, right);
		}

		#region Private members
		private static string MAX_FRIENDLY_NAME = "[Maximum]";
		private static string MIN_FRIENDLY_NAME = "[Minimum]";
		// array indexes for the 4-character YRQ value
		readonly private static short YEAR_MOD = 0;
		readonly private static short YEAR1 = 1;
		readonly private static short YEAR2 = 2;
		readonly private static short QUARTER = 3;

		private string _friendlyName;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="year"></param>
		/// <param name="modifer"></param>
		/// <returns></returns>
		private static int GetYear(char year, char modifer)
		{
			string longYear;
			if (modifer > '9')
			{
				// subtract an ASCII value get a valid year
				longYear = String.Format("2{0:D2}{1}", (modifer - 65) < 0 ? 0 : (modifer - 65), year);
			}
			else
			{
				longYear = String.Format("19{0}{1}", modifer, year);
			}

			return Int32.Parse(longYear);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="yrq"></param>
		/// <returns></returns>
		private static bool IsValid(string yrq)
		{
			if (yrq.Length != 4)
			{
				throw new ArgumentOutOfRangeException("yrq", "Must be a 4-character code");
			}
			return true;
		}

		/// <summary>
		/// Increments the year value of a YearQuarter <see cref="ID"/>
		/// </summary>
		/// <param name="oldYrq"></param>
		/// <param name="newYrq"></param>
		private static void IncrementYear(string oldYrq, ref char[] newYrq)
		{
			int year2 = int.Parse(oldYrq[YEAR2].ToString()) + 1;

			if (year2 == 1)
			{ // starting a new decade
				newYrq[YEAR_MOD] = Convert.ToChar(oldYrq[YEAR_MOD] + 1);	// Increments the ASCII value, which increments the letter
			}
			else
			{
				newYrq[YEAR_MOD] = oldYrq[YEAR_MOD];
			}
			newYrq[YEAR1] = oldYrq[YEAR2];	// shift the previous 2nd year to the new 1st year, and
			newYrq[YEAR2] = Convert.ToChar(year2.ToString("D1"));	// the right-most digit (which is the incremented year)
		}

		/// <summary>
		/// Calculates the century character in a YearQuarter <see cref="ID"/>
		/// </summary>
		/// <param name="century"></param>
		/// <param name="decade"></param>
		/// <returns></returns>
		/// <remarks>
		/// E.g. the 'A' in 'A894'
		/// </remarks>
		private static char GetYearMod(ushort century, ushort decade)
		{
			ushort modChar;
			
			// The year 2000 started using letters for the YRQ ID
			if (century > 19)
			{
				// 65 = 'A'
				modChar = (ushort)(65 + ushort.Parse(string.Concat(century - 20, decade)));	// strip leading 2 and concatenate. e.g. 21 and 5 => 15
			}
			else
			{
				// 48 = '0'
				modChar = (ushort)(48 + decade);
			}
			return Convert.ToChar(modChar);
		}

		#endregion
	}
}
