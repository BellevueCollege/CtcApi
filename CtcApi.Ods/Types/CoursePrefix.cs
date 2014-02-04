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
using Ctc.Ods.Config;
using System.Runtime.Serialization;

namespace Ctc.Ods.Types
{
	/// <summary>
	/// Defines a subject of study to which <see cref="Course"/>s belong
	/// </summary>
    [DataContract]
    public class CoursePrefix : ICoursePrefix, IEquatable<CoursePrefix>
	{
		private RegexSettings _validationPatterns;

		#region Properties
		/// <summary>
		/// Provides a reference to configuration settings
		/// </summary>
		private RegexSettings Patterns
		{
			get
			{
// ReSharper disable PossibleNullReferenceException
				return _validationPatterns ?? (_validationPatterns = (ConfigurationManager.GetSection(ApiSettings.SectionName) as ApiSettings).RegexPatterns);
// ReSharper restore PossibleNullReferenceException
			}
		}
		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <seealso cref="Subject"/>
		internal string _Subject
		{
			set
			{
				string subject = value.Trim();

				// remove trailing Common Course Numbering marker if present
				Subject = subject.EndsWith(Patterns.CommonCourseChar) ? subject.Substring(0, subject.Length - 1) : subject;
			}
		}

		/// <summary>
		/// The five-character abbreviation which identifies a course of study (e.g. ENGL)
		/// </summary>
		/// <seealso cref="Title"/>
		[DataMember]
        public string Subject {get; private set;}

		/// <summary>
		/// The full name of a course of study (e.g. English)
		/// </summary>
		/// <seealso cref="Subject"/>
        [DataMember]
        public string Title {get;internal protected set;}

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <returns>
		/// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
		/// </returns>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(CoursePrefix other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}
			if (ReferenceEquals(this, other))
			{
				return true;
			}
			return Equals(other.Subject, Subject) && Equals(other.Title, Title);
		}

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <returns>
		/// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
		/// </returns>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(ICoursePrefix other)
		{
			return Equals(other as CoursePrefix);
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
			if (obj.GetType() != typeof(CoursePrefix))
			{
				return false;
			}
			return Equals((CoursePrefix)obj);
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
				return ((Subject != null ? Subject.GetHashCode() : 0) * 397) ^ (Title != null ? Title.GetHashCode() : 0);
			}
		}

		///<summary>
		///</summary>
		///<param name="left"></param>
		///<param name="right"></param>
		///<returns></returns>
		public static bool operator ==(CoursePrefix left, CoursePrefix right)
		{
			return Equals(left, right);
		}

		///<summary>
		///</summary>
		///<param name="left"></param>
		///<param name="right"></param>
		///<returns></returns>
		public static bool operator !=(CoursePrefix left, CoursePrefix right)
		{
			return !Equals(left, right);
		}
	}
}