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
using System.Linq;
using System.Runtime.Serialization;
using Ctc.Ods.Config;
using Ctc.Ods.Data;

namespace Ctc.Ods.Types
{
	/// <summary>
	/// Defines an instance of a college class (i.e. <see cref="Course"/>)
	/// </summary>
	/// <remarks>
	/// A <see cref="Section"/> provides a rich, heirarchical represenation of the data found in
	/// the <i>vw_Class</i> view of the ODS. This allows the .NET developer to work with a logical
	/// object rather than being limited by the schematic designs of the HP3000 data source.
	/// 
	/// To obtain one or more <see cref="Section"/> objects use the <see cref="OdsRepository"/>
	/// </remarks>
	/// <seealso cref="OdsRepository"/>
	[DataContract]
	public class Section : ISection
	{
		readonly private ApiSettings _settings = ConfigurationManager.GetSection(ApiSettings.SectionName) as ApiSettings;
		private string _onlineFlag;
		private string _hybridFlag;
		private string _telecourseFlag;
		private string _continuousEnrollmentFlag;

		private string _courseTitle;
		private string _classID;
		private ICourseID _courseId;
		private IList<CourseDescription> _courseDescriptions;
		private IList<string> _footnotes;
		private RegexSettings _validationPatterns;

		#region Public members

		/// <summary>
		/// Gets the unique identifier for the <see cref="Section"/>
		/// </summary>
		/// <seealso cref="ISectionID"/>
		/// <seealso cref="SectionID"/>
		public ISectionID ID { get; protected internal set; }

		/// <summary>
		/// 
		/// </summary>
		[DataMember]
		public string CourseSubject { get; protected internal set; }

		/// <summary>
		/// 
		/// </summary>
		[DataMember]
		public string CourseNumber { get; protected internal set; }

		/// <summary>
		/// 
		/// </summary>
		[DataMember]
		public IList<CourseDescription> CourseDescriptions
		{
			get
			{
				if (_courseDescriptions == null)
				{
					_courseDescriptions = OdsRepository.GetCourseDescriptions(Types.CourseID.FromString(CourseID), Yrq,
																																		_CourseDescriptions1, _CourseDescriptions2);
				}
				return _courseDescriptions;
			}
			protected internal set { _courseDescriptions = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		[DataMember]
		public YearQuarter Yrq { get; protected internal set; }

		/// <summary>
		/// 
		/// </summary>
		[DataMember]
		public DateTime? StartDate { get; protected internal set; }

		/// <summary>
		/// 
		/// </summary>
		[DataMember]
		public DateTime? EndDate { get; protected internal set; }

		/// <summary>
		/// 
		/// </summary>
		public bool IsOnline { get; protected internal set; }

		/// <summary>
		/// 
		/// </summary>
		[DataMember]
		public bool IsOnCampus { get; protected internal set; }

		/// <summary>
		/// 
		/// </summary>
		[DataMember]
		public bool IsTelecourse { get; protected internal set; }

		/// <summary>
		/// 
		/// </summary>
		[DataMember]
		public bool IsHybrid { get; protected internal set; }

		/// <summary>
		/// 
		/// </summary>
		[DataMember]
		public bool IsContinuousEnrollment { get; protected internal set; }

		/// <summary>
		/// 
		/// </summary>
		[DataMember]
		public bool IsVariableCredits { get; protected internal set; }

		/// <summary>
		/// 
		/// </summary>
		[DataMember]
		public bool IsDifferentStartDate { get; protected internal set; }

		/// <summary>
		/// 
		/// </summary>
		public bool IsLinked { get; protected internal set; }

		/// <summary>
		/// An <see cref="SectionID.ItemNumber"/> identifying another <see cref="Section"/> that this <see cref="Section"/> is linked to
		/// </summary>
		/// <remarks>
		/// <para>Only classes in the same <see cref="YearQuarter"/> can be linked.</para>
		/// <para>If the current <see cref="Section"/> is not linked to another, then its own <see cref="SectionID.ItemNumber"/> may appear here.</para>
		/// </remarks>
		public string LinkedTo { get; protected internal set; }


        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public bool IsDifferentEndDate { get; protected internal set; }

		#region Properties mapped to data columns

		#region Internal values set only by the API
		/// <summary>
		/// 
		/// </summary>
		internal string _YearQuarterID
		{
			set { Yrq = YearQuarter.FromString(value); }
		}

		/// <summary>
		/// 
		/// </summary>
		internal IEnumerable<CourseDescription1Entity> _CourseDescriptions1 { private get; set; }

		/// <summary>
		/// 
		/// </summary>
		internal IEnumerable<CourseDescription2Entity> _CourseDescriptions2 { private get; set; }

		/// <summary>
		/// 
		/// </summary>
		internal string _Footnote1 { get; set; }

		/// <summary>
		/// 
		/// </summary>
		internal string _Footnote2 { get; set; }

		/// <summary>
		/// 
		/// </summary>
		internal string _SBCTCMisc1
		{
			set
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					IsOnCampus = true;
					IsOnline = IsHybrid = IsTelecourse = false;
				}
				else
				{
					IsOnline = value.StartsWith(_onlineFlag);
					IsHybrid = value.StartsWith(_hybridFlag);
					IsTelecourse = value.StartsWith(_telecourseFlag);
					IsOnCampus = !IsOnline && !IsHybrid && !IsTelecourse;
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		internal string _ContinuousSequentialIndicator
		{
			set
			{
				if (!string.IsNullOrWhiteSpace(value) && value.StartsWith(_continuousEnrollmentFlag))
				{
					IsContinuousEnrollment = true;
				}
			}
		}

		/// <summary>
		/// Data retrieval stub for <see cref="LastRegistrationDate"/>
		/// </summary>
		/// <seealso cref="LastRegistrationDate"/>
		internal DateTime? _LastRegistrationDate
		{
			set
			{
				LastRegistrationDate = value.HasValue ? value.Value : DateTime.MinValue;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		internal string _CourseTitle { private get; set; }

		/// <summary>
		/// 
		/// </summary>
		internal IEnumerable<string> _CourseFootnotes { get; set; }

		#endregion

		/// <summary>
		/// 
		/// </summary>
		[DataMember]
		public string SectionCode { get; protected internal set; }

		/// <summary>
		/// 
		/// </summary>
		[DataMember]
		public IEnumerable<string> Footnotes
		{
			get
			{
				if (_footnotes == null)
				{
					_footnotes = new List<string>(2);
				}
				if (_footnotes.Count <= 0)
				{
					foreach (string footnote in new[] { _Footnote1, _Footnote2 })
					{
						if (!string.IsNullOrWhiteSpace(footnote))
						{
							_footnotes.Add(footnote);
						}
					}
				}
				return _footnotes;
			}
			protected set { _footnotes = value.ToList(); }
		}

		/// <summary>
		/// 
		/// </summary>
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
        CourseSubject = _courseId.Subject;
        CourseNumber = _courseId.Number;
      }
    }

		/// <summary>
		/// 
		/// </summary>
		[DataMember]
		public bool IsCommonCourse { get; protected internal set; }

		/// <summary>
		/// 
		/// </summary>
		[DataMember]
		public string CourseTitle
		{
			get
			{
				// unless title has been set by a superclass, get it from the data context
				_courseTitle = _courseTitle ?? _CourseTitle;
				return _courseTitle;
			}
			protected set { _courseTitle = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		[DataMember]
		public Decimal Credits { get; protected internal set; }

		/// <summary>
		/// 
		/// </summary>
		[DataMember]
		public IEnumerable<OfferedItem> Offered { get; protected internal set; }

		/// <summary>
		/// Number of students on the waitlist
		/// </summary>
		/// <remarks>
		/// A non-zero value is also an indication that the <see cref="Section"/> is <i>closed</i>.
		/// </remarks>
		[DataMember]
		public int WaitlistCount { get; protected internal set; }
		#endregion

		#endregion

		#region Constructors
		/// <summary>
		/// 
		/// </summary>
		public Section()
		{
			_onlineFlag = _settings.ClassFlags.Online ?? string.Empty;
			_hybridFlag = _settings.ClassFlags.Hybrid ?? string.Empty;
			_telecourseFlag = _settings.ClassFlags.Telecourse ?? string.Empty;
			_continuousEnrollmentFlag = _settings.ClassFlags.ContinuousEnrollment ?? string.Empty;
		}

		/// <summary>
		/// 
		/// </summary>
		public Section(string sectionId)
			: this()
		{
			ID = SectionID.FromString(sectionId);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sectionId"></param>
		public Section(ISectionID sectionId)
			: this()
		{
			ID = sectionId;
		}
		#endregion

		#region Private members

		/// <summary>
		/// Internal unique ID field - needed by Entity Framework
		/// </summary>
		internal string ClassID
		{
			get { return _classID; }
			set
			{
				_classID = value;
				ID = SectionID.FromString(_classID);
			}
		}

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

		/// <summary>
		/// The latest date to register for the <see cref="Section"/>
		/// </summary>
		/// <remarks>
		///		<note type="note">
		///			<para>
		///			Per SBCTC policy, this value defaults to the "the last instructional day of the course" (e.g. <see cref="EndDate"/>)
		///			</para>
		///			- <a href="http://www.sbctc.ctc.edu/general/policymanual/_a-policymanual-ch5Append.aspx">http://www.sbctc.ctc.edu/general/policymanual/_a-policymanual-ch5Append.aspx</a>
		///		</note>
		/// </remarks>
		/// <seealso cref="IsContinuousEnrollment"/>
		/// <seealso cref="StartDate"/>
		/// <seealso cref="EndDate"/>
		public DateTime LastRegistrationDate{get;set;}

		#endregion

		#region Equality methods
		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <returns>
		/// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
		/// </returns>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(ISection other)
		{
			return Equals((Section)other);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool Equals(Section other)
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
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
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
			if (obj.GetType() != typeof(Section))
			{
				return false;
			}
			return Equals((Section)obj);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return (_classID != null ? _classID.GetHashCode() : 0);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static bool operator ==(Section left, Section right)
		{
			return Equals(left, right);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static bool operator !=(Section left, Section right)
		{
			return !Equals(left, right);
		}
		#endregion

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
			return ID.ToString();
		}
		#endregion
	}
}
