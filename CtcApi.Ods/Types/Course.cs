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
		private ApiSettings _settings;
		private RegexSettings _regexPatterns;
		private IList<CourseDescription> _courseDescriptions;
		private ICourseID _courseId;
		private IList<string> _footnotes;

		/// <summary>
		/// Provides a reference to configuration settings
		/// </summary>
		private ApiSettings Settings
		{
			get
			{
				return _settings ?? (_settings = ConfigurationManager.GetSection(ApiSettings.SectionName) as ApiSettings);
			}
		}

		/// <summary>
		/// Provides a reference to regular expression patterns from configuration settings
		/// </summary>
		/// <seealso cref="ApiSettings"/>
		private RegexSettings Patterns
		{
			get
			{
				return _regexPatterns ?? (_regexPatterns = Settings.RegexPatterns);
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
				return _courseId.ToString();
			}
			internal protected set
			{
        // leverage the logic already coded in CourseID to convert from a string.
        _courseId = Types.CourseID.FromString(value);

				IsCommonCourse = _courseId.IsCommonCourse;
				Subject = _courseId.Subject;
				Number = _courseId.Number;
			}
		}

		/// <summary>
		/// Departmental prefix for the course (e.g. ART)
		/// </summary>
		/// <seealso cref="Number"/>
		[DataMember]
		public string Subject { get; internal protected set; }

		/// <summary>
		/// The (typically numeric) identifier for the course (e.g. 101)
		/// </summary>
		/// <seealso cref="Subject"/>
		[DataMember]
		public string Number { get; internal protected set; }

		/// <summary>
		/// Short title of the course (e.g. Beginning Art)
		/// </summary>
		[DataMember]
		public string Title { get; internal protected set; }

		/// <summary>
		/// One or more long descriptions (defined by <see cref="ICourseDescription.YearQuarterBegin">begin date</see>) for the course
		/// </summary>
		[DataMember]
		public IEnumerable<CourseDescription> Descriptions
		{
			get
			{
				if (_courseDescriptions == null)
				{
					_courseDescriptions = OdsRepository.GetCourseDescriptions(Types.CourseID.FromString(CourseID), YearQuarter.FromString(_YearQuarterBegin),
																																		_CourseDescriptions1, _CourseDescriptions2);
				}
				return _courseDescriptions;
			}
		}

		/// <summary>
		/// Number of credits earned upon completion of the course
		/// </summary>
		[DataMember]
		public Decimal Credits { get; internal protected set; }

		/// <summary>
		/// The <see cref="YearQuarter"/> that the current <see cref="Course"/> becomes active.
		/// </summary>
		public YearQuarter YearQuarterBegin{get; internal protected set;}

		/// <summary>
		/// The <see cref="YearQuarter"/> that the current <see cref="Course"/> records expires.
		/// </summary>
		public YearQuarter YearQuarterEnd{get; internal protected set;}

		/// <summary>
		/// Indicates whether this is a <a href="http://www.sbctc.ctc.edu/college/e_commoncoursenumbering.aspx">Common Course</a>
		/// </summary>
		[DataMember]
		public bool IsCommonCourse { get; internal protected set; }

		/// <summary>
		/// Indicates whether this <see cref="Course"/> can be taken for a variable number of <see cref="Credits"/>.
		/// </summary>
		public bool IsVariableCredits { get; internal protected set; }

		/// <summary>
		/// Provides a collection of Footnotes associated with the current <see cref="Course"/>
		/// </summary>
		public IList<string> Footnotes
		{
			get
			{
				if (_footnotes == null)
				{
					_footnotes = _Footnotes != null ? _Footnotes.ToList() : new List<string>();
				}
				return _footnotes;
			}
			internal protected set {_footnotes = value;}
		}

		#region Internal API properties
		/// <summary>
		/// Allows EF + LINQ to attach course descriptions
		/// </summary>
		internal IEnumerable<CourseDescription1Entity> _CourseDescriptions1 { private get; set; }

		/// <summary>
		/// Allows EF + LINQ to attach course descriptions
		/// </summary>
		internal IEnumerable<CourseDescription2Entity> _CourseDescriptions2 { private get; set; }

		/// <summary>
		/// Allows EF + LINQ to attach the effective year/quarter begin
		/// </summary>
		internal string _YearQuarterBegin
		{
			get {return YearQuarterBegin.ID;}
			set
			{
				YearQuarterBegin = YearQuarter.FromString(string.IsNullOrWhiteSpace(value) ? Settings.YearQuarter.Min : value);
			}
		}

		/// <summary>
		/// Allows EF + LINQ to attach the effective year/quarter end
		/// </summary>
		internal string _YearQuarterEnd
		{
			get {return YearQuarterEnd.ID;}
			set
			{
				YearQuarterEnd = YearQuarter.FromString(string.IsNullOrWhiteSpace(value) ? Settings.YearQuarter.Max : value);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		internal IEnumerable<string> _Footnotes{get;set;}

		#endregion

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
