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
using System.Configuration;
using System.Xml;
using System.Xml.Serialization;

namespace Ctc.Ods.Config
{
	internal class SettingsConfigHandler : IConfigurationSectionHandler
	{
		/// <summary>
		/// Loads <see cref="ConfigurationSection"/> from .config file into a <see cref="ApiSettings"/> object
		/// </summary>
		/// <param name="parent">Parent object</param>
		/// <param name="configContext">Configuration context object</param>
		/// <param name="section">Settings XML node from the configuration file</param>
		/// <returns>A <see cref="ApiSettings"/> configuration object</returns>
		/// <remarks>
		/// This method is invoked from the .NET configuration system when parsing the .config file.
		/// </remarks>
		/// <seealso cref="ApiSettings"/>
		public object Create(object parent, object configContext, XmlNode section)
		{
			ApiSettings result = null;
			if (section == null)
				return result;
			XmlSerializer ser = new XmlSerializer(typeof(ApiSettings));
			
			using (XmlNodeReader reader = new XmlNodeReader(section))
			{
				result = (ApiSettings)ser.Deserialize(reader);

				return result;
			}
		}
	}
}
