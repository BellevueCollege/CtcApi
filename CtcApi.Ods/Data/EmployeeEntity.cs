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
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ctc.Ods.Data
{
	/// <summary>
	/// Represents the vw_Employee view in ODS
	/// </summary>
	[Table("vw_Employee")]
	internal class EmployeeEntity
	{
		/// <summary>
		/// The employee's unique ID
		/// </summary>
		[Key]
		public string SID{get;set;}

		/// <summary>
		/// The employee's network username
		/// </summary>
		public string ADUserName{get;set;}

		/// <summary>
		/// Name to use in place of <see cref="FirstName"/>
		/// </summary>
		public string AliasName{get;set;}

		/// <summary>
		/// 
		/// </summary>
		public string FirstName{get;set;}

		/// <summary>
		/// 
		/// </summary>
		public string LastName{get;set;}

		/// <summary>
		/// 
		/// </summary>
		public string WorkEmail{get;set;}
	}
}