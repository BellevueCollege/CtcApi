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
using System.Configuration;
using System.Linq;
using Ctc.Ods.Config;
using Ctc.Ods.Data;
using Ctc.Ods.Types;

namespace Ctc.Ods.Tests
{
	public static class TestHelper
	{
		/// <summary>
		/// Retrieve section count for the specified facet
		/// </summary>
		/// <param name="facet"></param>
		/// <param name="includeQuarterFilter"></param>
		/// <returns></returns>
		static public int GetSectionCountWithFilter(ISectionFacet facet, bool includeQuarterFilter = true)
		{
			IList<Section> sections = GetSectionsWithFilter(facet, includeQuarterFilter); //.Where(s => s.Yrq.ID == "B122").Select(s => s).ToList();
			return sections.Select(s => s.ID.ToString()).Distinct().Count();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="facet"></param>
		/// <param name="includeQuarterFilter"></param>
		/// <returns></returns>
		static public IList<Section> GetSectionsWithFilter(ISectionFacet facet, bool includeQuarterFilter = true)
		{
			using (OdsRepository repository = new OdsRepository())
			{
				IList<ISectionFacet> facets = GetFacets(facet, includeQuarterFilter);

				return repository.GetSections(facets);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="facet"></param>
		/// <param name="includeQuarterFilter"></param>
		/// <returns></returns>
		static public IList<ISectionFacet> GetFacets(ISectionFacet facet, bool includeQuarterFilter = true)
		{
			IList<ISectionFacet> facets = GetFacets(includeQuarterFilter);
			// additional facet specified by the caller
			facets.Add(facet);

			return facets;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		static public IList<ISectionFacet> GetFacets(bool includeQuarterFilter = true)
		{
			IList<ISectionFacet> facets = new List<ISectionFacet>();
			if (includeQuarterFilter)
			{
				// standard quarter filter
				facets.Add(new RegistrationQuartersFacet(-4));
			}
			return facets;
		}

		/// <summary>
		/// 
		/// </summary>
		static public string MinYrq
		{
			get
			{
				ApiSettings settings = ConfigurationManager.GetSection(ApiSettings.SectionName) as ApiSettings;
				return settings != null ? settings.YearQuarter.Min : "0000";
			}
		}

		/// <summary>
		/// 
		/// </summary>
		static public string MaxYrq
		{
			get
			{
				ApiSettings settings = ConfigurationManager.GetSection(ApiSettings.SectionName) as ApiSettings;
				return settings != null ? settings.YearQuarter.Max : "Z999";
			}
		}

		/// <summary>
		/// Provides college-specific data for testing.
		/// </summary>
		public static class Data
		{
			public static ICourseID CourseIDOfferedEveryQuarter
			{
				get
				{
					string courseID = ConfigurationManager.AppSettings["Testing_CourseIDOfferedEveryQuarter"];
					return CourseID.FromString(courseID);
				}
			}

			static public string NonClassWhereClause
			{
				get
				{
					return ConfigurationManager.AppSettings["Testing_WhereClauseForNonClasses"];
				}
			}

			static public YearQuarter YearQuarterWithSections
			{
				get
				{
					return YearQuarter.FromString(ConfigurationManager.AppSettings["Testing_YearQuarterWithSections"]);
				}
			}

			static public string[] CourseSubjectNotInYRQ
			{
				get
				{
					return ConfigurationManager.AppSettings["Testing_CourseSubjectNotInYRQ"].Split('|');
				}
			}

			static public string CourseSubjectOfferedEveryQuarter
			{
				get
				{
					return ConfigurationManager.AppSettings["Testing_CourseSubjectOfferedEveryQuarter"];
				}
			}

			static public string ShortCourseSubject
			{
				get
				{
					return ConfigurationManager.AppSettings["Testing_ShortCourseSubject"];
				}
			}

			static public string ShortCourseSubjectNumber
			{
				get
				{
					return ConfigurationManager.AppSettings["Testing_ShortCourseSubjectNumber"];
				}
			}

			static public string CommonCourseCharacter
			{
				get
				{
					return ConfigurationManager.AppSettings["Testing_CommonCourseCharacter"];
				}
			}
		}
	}
}
