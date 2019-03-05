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
	/// 
	/// </summary>
	[Table("vw_Instruction")]
	internal class InstructionEntity
	{
		[Key]
		public int InstructionID {get;set;}

		public string ClassID {get;set;}
		public string DayID {get;set;}
		public DateTime? StartTime {get;set;}
		public DateTime? EndTime {get;set;}
		public string InstructorSID {get;set;}
		public string InstructorName {get;set;}
		public string Room {get;set;}
		public string SessionSequence{get;set;}
	}
}