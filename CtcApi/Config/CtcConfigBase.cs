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
using System.Configuration;
using System.Xml.Serialization;
using CtcApi.Extensions;

namespace CtcApi.Config
{
	/// <summary>
	/// Base class for CTCAPI <see cref="ConfigurationSection"/>s
	/// </summary>
	/// <typeparam name="T">
	///		<para>The child class which inherits from this class.</para>
	///		<para>
	///		This allows each child to inherit the static <see cref="GetSectionName"/> method,
	///		which returns the <i>name</i> of the child's <see cref="XmlTypeAttribute"/>. This
	///		in turn facilitates deserializing the XML node from the application's .config
	///		file by allowing the calling code to simply tell the <see cref="ConfigurationManager"/>
	///		to load the <see cref="ConfigurationSection"/> with the <i>name</i> retrieved.
	///		</para>
	/// </typeparam>
	public abstract class CtcConfigBase<T> where T : class
	{
		/// <summary>
		/// Gets the name of the XML section (i.e. node) associated with the inheriting child <i>class</i>
		/// </summary>
		/// <returns>
		///		The name of the <see cref="XmlTypeAttribute"/> attached to the child <i>class</i> which
		///		inherits from <see cref="CtcConfigBase{T}"/>
		/// </returns>
		public static string GetSectionName()
		{
			System.Reflection.MemberInfo info = typeof(T);
			object[] attributes = info.GetCustomAttributes(typeof(XmlTypeAttribute), false);

			if (attributes != null && attributes.Length > 0 && attributes[0] != null)
			{
// ReSharper disable PossibleNullReferenceException
				return (attributes[0] as XmlTypeAttribute).TypeName;
// ReSharper restore PossibleNullReferenceException
			}
				
			throw new InvalidOperationException(string.Format("Unable to find an XmlTypeAttribute for '{0}'", typeof(T)));
		}

		/// <summary>
		/// Load settings from the application's .config file.
		/// </summary>
		/// <remarks>
		///		This method loads settings from the XML node which matches the name of the <see cref="XmlTypeAttribute"/>
		///		for the child <i>class</i> which inherits from <see cref="CtcConfigBase{T}"/>
		/// </remarks>
		/// <returns></returns>
		public static T Load()
		{
			return ConfigurationManager.GetSection(GetSectionName()) as T;
		}
	}
}