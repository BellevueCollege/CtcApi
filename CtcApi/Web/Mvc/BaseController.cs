//Copyright (C) 2012 Bellevue College
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
using System.Reflection;
using System.Web.Mvc;

namespace CtcApi.Web.Mvc
{
	// TODO: make BaseController abstract
	public class BaseController : Controller
	{
		/// <summary>
		/// 
		/// </summary>
		public IStateProvider SessionWrapper {get;set;}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="instance">
		///		The actual <see cref="Assembly"/> which is using this base class.
		///		This refernece allows us to obtain the correct <see cref="Version"/>
		///		information for the initially invoked application.
		/// </param>
		public BaseController(Assembly instance)
		{
			// Set the current version of the application.
			ViewBag.Version = instance.GetName().Version;
		}
	}
}