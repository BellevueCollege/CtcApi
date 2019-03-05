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
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ctc.Ods.Data
{
	/// <summary>
	/// Represents the vw_Class view in ODS
	/// </summary>
	/// <seealso cref="OdsContext"/>
	[Table("vw_Class")]
	public class SectionEntity
	{
		/// <summary>
		/// 
		/// </summary>
		[Key]
		public string ClassID { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string Section { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string YearQuarterID { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string ItemNumber { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string ItemYRQLink { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string ClusterItemNumber { get; set; }

		///<summary>
		/// How many students the <see cref="Section"/> can accomodate
		///</summary>
		public int? ClassCapacity { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public int? ClusterCapacity { get; set; }

		///<summary>
		/// Number of students currently enrolled.
		/// TODO: verify StudentsEnrolled has accurate data
		///</summary>
		public int? StudentsEnrolled { get; set; }

		#region Course info
		/// <summary>
		/// 
		/// </summary>
		[Required]
		public string CourseID { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string CourseTitle { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public Decimal Credits { get; set; }

		#endregion

		#region Instructor info
		/// <summary>
		/// 
		/// </summary>
		public string InstructorSID { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string InstructorName { get; set; }

		#endregion

		#region When & where
		/// <summary>
		/// 
		/// </summary>
		public string DayID { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public DateTime? StartDate { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public DateTime? EndDate { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public DateTime? StartTime { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public DateTime? EndTime { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string Room { get; set; }

		/// <summary>
		/// The latest date to register for the <see cref="Section"/>, if different from the reqular schedule
		/// </summary>
		/// <seealso cref="ContinuousSequentialIndicator"/>
		public DateTime? LastRegistrationDate { get; set; }

		#endregion

		#region Flags
		/// <summary>
		/// Contains flags which indicate which type of <see cref="Section"/> this is
		/// </summary>
		public string SBCTCMisc1 { get; set; }

		/// <summary>
		/// Contains flags to indicate the <see cref="Section"/>'s enrollment type
		/// </summary>
		public string ContinuousSequentialIndicator { get; set; }

		/// <summary>
		/// Contains flags to indicate the <see cref="Section"/>'s credit type
		/// </summary>
		public bool? VariableCredits { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string SectionStatusID1 { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string SectionStatusID2 { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string SectionStatusID4 { get; set; }

		#endregion

		#region Footnotes
		/// <summary>
		/// 
		/// </summary>
		public string FootnoteID1 { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string FootnoteID2 { get; set; }

		#endregion
	}
}
