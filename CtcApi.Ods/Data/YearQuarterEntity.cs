﻿//Copyright (C) 2011 Bellevue College and Peninsula College
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
	[Table("vw_YearQuarter")]
	internal class YearQuarterEntity
	{
		/// <summary>
		/// 
		/// </summary>
		[Key]
		public string YearQuarterID{get;set;}

		/// <summary>
		/// 
		/// </summary>
		public DateTime FirstClassDay{get;set;}

		/// <summary>
		/// 
		/// </summary>
		public DateTime LastClassDay{get;set;}
	}
}