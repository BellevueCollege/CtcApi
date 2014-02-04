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
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq.Expressions;
using Ctc.Ods.Data;
using Ctc.Ods.Types;

namespace Ctc.Ods.Customizations
{
	/// <summary>
	/// 
	/// </summary>
	internal interface IOdsDataMapper<T> where T : IRichDataObject
	{
		///<summary>
		/// 
		///</summary>
		///<param name="db"></param>
		///<returns></returns>
		IList<T> GetData(DbContext db);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="facet"></param>
		void AddFacet(IDataFacet facet);
	}

	/// <summary>
	/// 
	/// </summary>
	public class DefaultSectionMapper : IOdsDataMapper<Section>
	{
		// TODO: look at IMSInterchange mapper
		// how can we define expected methods, but not expose them externally??

		///<summary>
		/// 
		///</summary>
		///<param name="db"></param>
		///<returns></returns>
		internal IList<Section> GetData(OdsContext db)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="facet"></param>
		internal void AddFacet(ISectionFacet facet)
		{
			throw new NotImplementedException();
		}

		///<summary>
		/// 
		///</summary>
		///<param name="db"></param>
		///<returns></returns>
		public IList<Section> GetData(DbContext db)
		{
			if (db is OdsContext)
			{
				return GetData(db as OdsContext);
			}
			throw new ArgumentException("Invalid DbContext object.", "db");
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="facet"></param>
		public void AddFacet(IDataFacet facet)
		{
			if (facet is ISectionFacet)
			{
				AddFacet(facet as ISectionFacet);
			}
			throw new ArgumentException("Data Mapper requires an ISectionFacet object.", "facet");
		}
	}
}