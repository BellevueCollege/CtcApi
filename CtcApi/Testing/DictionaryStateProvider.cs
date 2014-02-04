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
using System.Collections.Generic;
using CtcApi.Web;

namespace CtcApi.Testing
{
	public class DictionaryStateProvider : IStateProvider
	{
		private Dictionary<string,object> _collection = new Dictionary<string,object>();

		public object this[string key]
		{
			get
			{
				return _collection.ContainsKey(key) ? _collection[key] : null;
			}
			set {_collection[key] = value;}
		}

		public void Remove(string key)
		{
			_collection.Remove(key);
		}
	}
}