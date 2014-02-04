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
using System.Globalization;
using System.Runtime.Serialization;

namespace Ctc.Ods.Types
{
	/// <summary>
	/// Defines when and where a <see cref="Section"/> is offered
	/// </summary>
	[DataContract]
    public class OfferedItem
	{
		private string _instructorName = string.Empty;

		/// <summary>
		/// 
		/// </summary>
		public OfferedItem()
		{
			ID = Guid.NewGuid().ToString();
		}

		/// <summary>
		/// Unique key needed by LINQ in order to perform queries
		/// </summary>
		/// <remarks>
		/// This value is a random <see cref="Guid"/> and should only be used to
		/// uniquely identify each <see cref="OfferedItem"/> object.
		/// </remarks>
		[Key]
		[DataMember]
        public string ID {get;private set;}

		/// <summary>
		/// The day(s) a <see cref="Section"/> meets (e.g. "TTh")
		/// </summary>
        [DataMember]
        public string Days{get;internal set;}

		/// <summary>
		/// 
		/// </summary>
        [DataMember]
        public DateTime? StartTime{get;internal set;}

		/// <summary>
		/// 
		/// </summary>
        [DataMember]
        public DateTime? EndTime{get;internal set;}

		/// <summary>
		/// 
		/// </summary>
        [DataMember]
        public string InstructorID{get;internal set;}

		/// <summary>
		/// The name of the instructor the current <see cref="OfferedItem"/>
		/// </summary>
		/// <remarks>
		/// If no instructor name is present in the database, this value will be <see cref="String.Empty"/>
		/// </remarks>
        [DataMember]
        public string InstructorName
		{
			get {return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(_instructorName.ToLower());}
			internal set {_instructorName = value ?? string.Empty;}
		}

		/// <summary>
		/// 
		/// </summary>
        [DataMember]
        public string InstructorEmail{get;set;}

		/// <summary>
		/// 
		/// </summary>
        [DataMember]
        public string Room{get;internal set;}

		/// <summary>
		/// Indicates whether this is the primary instructor and/or meeting time
		/// </summary>
        [IgnoreDataMember]
        public bool IsPrimary { get { return SequenceOrder == 0; } }

		/// <summary>
		/// 
		/// </summary>
        [DataMember]
        public int SequenceOrder {get; protected set;}

		#region Internal setters
		// These properties are only used by the Entity Framework to set initial values from the data source
		internal string _SessionSequence
		{
			set
			{
				int sequenceOrder;
				if (int.TryParse(value, out sequenceOrder))
				{
					SequenceOrder = sequenceOrder;
				}
				else
				{
					throw new InvalidCastException(string.Format("Datbase contains invalid SessionSequence: '{0}'", sequenceOrder));
				}
			}
		}

		#endregion
	}
}