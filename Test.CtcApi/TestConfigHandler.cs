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

namespace Test.CtcApi
{
	internal class TestConfigHandler : IConfigurationSectionHandler
	{
		public object Create(object parent, object configContext, XmlNode section)
		{
			TestCtcConfigBase.TestConfig result = null;
			if (section == null)
				return result;
			XmlSerializer ser = new XmlSerializer(typeof(TestCtcConfigBase.TestConfig));
			
			using (XmlNodeReader reader = new XmlNodeReader(section))
			{
				result = (TestCtcConfigBase.TestConfig)ser.Deserialize(reader);

				return result;
			}
		}
	}
}
