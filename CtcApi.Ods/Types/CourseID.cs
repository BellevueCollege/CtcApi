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
using System.Text.RegularExpressions;
using Ctc.Ods.Config;

namespace Ctc.Ods.Types
{
	/// <summary>
	/// Representa the unique ID for a <see cref="Course"/>
	/// </summary>
	/// <remarks>
	/// This is a combination of the <see cref="Subject"/> and <see cref="Number"/>
	/// </remarks>
	public class CourseID : ICourseID, IEquatable<CourseID>
	{
		private static string _commonCourseChar;

		/// <summary>
		/// 
		/// </summary>
		protected static string CommonCourseChar
		{
			get
			{
				if (string.IsNullOrEmpty(_commonCourseChar))
				{
					ApiSettings settings = Utility.GetApiSettings();
					_commonCourseChar = settings != null ? settings.RegexPatterns.CommonCourseChar : "&";
				}
				return _commonCourseChar;
			}
		}

		/// <summary>
		/// Departmental prefix for the course (e.g. ART)
		/// </summary>
		/// <seealso cref="Number"/>
		/// <seealso cref="IsCommonCourse"/>
		public string Subject{get;set;}

		/// <summary>
		/// The (typically numeric) identifier for the course (e.g. 101)
		/// </summary>
		/// <seealso cref="Subject"/>
		public string Number{get;set;}

		/// <summary>
		/// Indicates whether this CourseID represents a <a href="http://www.sbctc.ctc.edu/college/e_commoncoursenumbering.aspx">Common Course</a>
		/// </summary>
		/// <seealso cref="ICourseID.Subject"/>
		public bool IsCommonCourse{get;set;}

		#region Constructors
	  /// <summary>
	  /// 
	  /// </summary>
	  /// <param name="subject"></param>
	  /// <param name="number"></param>
	  /// <param name="isCommonCourse"></param>
	  public CourseID(string subject, string number, bool isCommonCourse = false)
		{
			subject = subject.Trim();
			number = number.Trim();

			IsCommonCourse = isCommonCourse || subject.EndsWith(CommonCourseChar);
			Subject = IsCommonCourse ? subject.Replace(CommonCourseChar, string.Empty) : subject;
			Number = number;
		}

		#region Factory methods
		/// <summary>
		/// 
		/// </summary>
		/// <param name="courseId"></param>
		/// <returns></returns>
		public static CourseID FromString(string courseId)
		{
		  bool isCommonCourse = false;
      courseId = courseId.ToUpper();

      // handle Course IDs w/ separating whitespace (e.g. user-/developer-entered)
      if (Regex.IsMatch(courseId, string.Format(@"[\w\{0}]+[\s\t]\w+", CommonCourseChar)))
      {
        // the following regex doesn't handle CommonCourseChar, so if present set out flag and strip it out
        if (courseId.Contains(CommonCourseChar))
        {
          isCommonCourse = true;
          courseId = courseId.Replace(CommonCourseChar, "");
        }

        // contains whitespace in the middle or the course subject
        string[] parts = Regex.Split(courseId, @"(?<=\p{L}[\p{Z}\t]?)(?=\p{N})"); // magic regex - handles e.g. "C S C 112", "P E 108"

        // use values w/o leading/trailing whitespace
        return new CourseID(parts[0].Trim(), parts[1].Trim(), isCommonCourse);
      }

      // handle Course IDs that may not have whitespace - assuming fixed length (e.g. from HP)
      return new CourseID(courseId.Substring(0, 5), courseId.Length > 5 ? courseId.Substring(5) : String.Empty, isCommonCourse);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="subject"></param>
		/// <param name="number"></param>
		/// <returns></returns>
		static public CourseID FromString(string subject, string number)
		{
			return new CourseID(subject, number);
		}

		#endregion

		#endregion

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="ICourseID"/>.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"/> that represents the current <see cref="ICourseID"/>.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public override string ToString()
		{
			return String.Concat(Subject, " ", Number);
		}

		#region Equality members
		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <returns>
		/// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
		/// </returns>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(ICourseID other)
		{
			return Equals(other as CourseID);
		}

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <returns>
		/// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
		/// </returns>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(CourseID other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}
			if (ReferenceEquals(this, other))
			{
				return true;
			}
			return Equals(other.Subject, Subject) && Equals(other.Number, Number) && Equals(other.IsCommonCourse, IsCommonCourse);
		}

		/// <summary>
		/// Indicates whether the current object is equal to a <see cref="string"/> representing the same value.
		/// </summary>
		/// <returns>
		/// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
		/// </returns>
		/// <param name="other">A string to compare with this object.</param>
		public bool Equals(string other)
		{
			return Equals(FromString(other));
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
			if (obj.GetType() != typeof(CourseID))
			{
				return false;
			}
			return Equals((CourseID)obj);
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
			unchecked
			{
				return ((Subject != null ? Subject.GetHashCode() : 0) * 397) ^ (Number != null ? Number.GetHashCode() : 0);
			}
		}

		///<summary>
		///</summary>
		///<param name="left"></param>
		///<param name="right"></param>
		///<returns></returns>
		public static bool operator ==(CourseID left, CourseID right)
		{
			return Equals(left, right);
		}

		///<summary>
		///</summary>
		///<param name="left"></param>
		///<param name="right"></param>
		///<returns></returns>
		public static bool operator !=(CourseID left, CourseID right)
		{
			return !Equals(left, right);
		}

		#endregion
	}
}