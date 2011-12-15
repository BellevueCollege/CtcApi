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
using System.Configuration;
using System.Runtime.Serialization;
using Ctc.Ods.Config;
using Ctc.Ods.Data;

namespace Ctc.Ods.Types
{
	/// <summary>
	/// Defines a college course offered to students
	/// </summary>
    [DataContract]
	public class Course : ICourse, IEquatable<Course>
	{
		private RegexSettings _regexPatterns;
		private IList<CourseDescription> _courseDescriptions;
		private string _courseId;

		/// <summary>
		/// Provides a reference to configuration settings
		/// </summary>
		/// <seealso cref="ApiSettings"/>
		private RegexSettings Patterns
		{
			get
			{
				return _regexPatterns ?? (_regexPatterns = (ConfigurationManager.GetSection(ApiSettings.SectionName) as ApiSettings).RegexPatterns);
			}
		}

		#region ICourse Members
		/// <summary>
		/// The unique ID for the course
		/// </summary>
		/// <remarks>
		/// This is a combination of the <see cref="Subject"/> and <see cref="Number"/>
		/// </remarks>
        [DataMember]
		public string CourseID
		{
			get
			{
				return _courseId;
			}
			internal set
			{
				bool isCommonCourse;
				string subject, number;

				_courseId = Utility.ParseCourseID(value, out isCommonCourse, out subject, out number, Patterns.CommonCourseChar);
				
				IsCommonCourse = isCommonCourse;
				Subject = subject;
				Number = number;
			}
		}

		/// <summary>
		/// Departmental prefix for the course (e.g. ART)
		/// </summary>
		/// <seealso cref="Number"/>
        [DataMember]
        public string Subject { get; internal set; }

		/// <summary>
		/// The (typically numeric) identifier for the course (e.g. 101)
		/// </summary>
		/// <seealso cref="Subject"/>
        [DataMember]
        public string Number { get; internal set; }

		/// <summary>
		/// Short title of the course (e.g. Beginning Art)
		/// </summary>
        [DataMember]
        public string Title {get; internal set;}

		/// <summary>
		/// One or more long descriptions (defined by <see cref="ICourseDescription.YearQuarterBegin">begin date</see>) for the course
		/// </summary>
        [DataMember]
        public IEnumerable<CourseDescription> Descriptions {
			get
			{
				if (_courseDescriptions == null)
				{
					_courseDescriptions = OdsRepository.GetCourseDescriptions(Ods.CourseID.FromString(CourseID), YearQuarter.FromString(_YearQuarter),
																																		_CourseDescriptions1, _CourseDescriptions2);
				}
				return _courseDescriptions;
			}
		}

		/// <summary>
		/// Number of credits earned upon completion of the course
		/// </summary>
        [DataMember]
        public Decimal Credits { get; internal set; }

		/// <summary>
		/// Indicates whether this is a <a href="http://www.sbctc.ctc.edu/college/e_commoncoursenumbering.aspx">Common Course</a>
		/// </summary>
        [DataMember]
        public bool IsCommonCourse { get; protected set; }

			/// <summary>
			/// 
			/// </summary>
		public bool IsVariableCredits{get; internal protected set;}

			/// <summary>
		/// Allows EF + LINQ to attach course descriptions
		/// </summary>
		internal IEnumerable<CourseDescription1Entity> _CourseDescriptions1{private get; set; }

		/// <summary>
		/// Allows EF + LINQ to attach course descriptions
		/// </summary>
		internal IEnumerable<CourseDescription2Entity> _CourseDescriptions2{private get; set; }

		/// <summary>
		/// Allows EF + LINQ to attach the year/quarter
		/// </summary>
		internal string _YearQuarter{private get; set;}

		#endregion

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <returns>
		/// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
		/// </returns>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(Course other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}
			if (ReferenceEquals(this, other))
			{
				return true;
			}
			return Equals(other.Subject, Subject) && Equals(other.Number, Number);
		}

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <returns>
		/// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
		/// </returns>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(ICourse other)
		{
			return Equals(other as Course);
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
			if (obj.GetType() != typeof(Course))
			{
				return false;
			}
			return Equals((Course)obj);
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
				int result = (Subject != null ? Subject.GetHashCode() : 0);
				result = (result * 397) ^ (Number != null ? Number.GetHashCode() : 0);
				return result;
			}
		}

		/// <summary>
		/// Operator overload to check equality of two <see cref="Course"/> objects
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static bool operator ==(Course left, Course right)
		{
			return Equals(left, right);
		}

		/// <summary>
		/// Operator overload to check inequality of two <see cref="Course"/> objects
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static bool operator !=(Course left, Course right)
		{
			return !Equals(left, right);
		}
	}
}
