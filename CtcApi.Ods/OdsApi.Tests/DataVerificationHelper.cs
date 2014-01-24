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
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Ctc.Ods.Types;
using CtcApi;

namespace Ctc.Ods.Tests
{

	class DataVerificationHelper : IDisposable
	{
		// constants
		/// <summary>
		/// Filters out non-classes
		/// </summary>
		private readonly string _sectionFilter =  string.Format(" {0} ", TestHelper.Data.NonClassWhereClause);

		private string _yrqFilter;

		private DbProviderFactory _provider;
		private DbConnection _conn;
		private int _allSectionsCount = -1;

		public string CurrentYrq{get; private set;}

		/// <summary>
		/// 
		/// </summary>
		public int AllSectionsCount
		{
			get
			{
				if (_allSectionsCount < 0)
				{
					_allSectionsCount = GetSectionCount("not ClassID is null");
				}
				return _allSectionsCount;
			}
		}

    /// <summary>
    /// 
    /// </summary>
		public DataVerificationHelper() : this(new ApplicationContext())
		{
		}

		/// <summary>
		/// 
		/// </summary>
		public DataVerificationHelper(ApplicationContext appContext)
		{
			_provider = DbProviderFactories.GetFactory("System.Data.SqlClient");
			_conn = _provider.CreateConnection();
			_conn.ConnectionString = ConfigurationManager.ConnectionStrings["OdsContext"].ConnectionString;

			DateTime today = appContext.CurrentDate ?? DateTime.Now;
			CurrentYrq = GetYearQuarterID(string.Format("LastClassDay >= cast('{0}' as datetime)", today));

			_yrqFilter = String.Format(" and YearQuarterID in (SELECT TOP (4) y.[YearQuarterID] FROM [vw_YearQuarter] AS y LEFT OUTER JOIN [vw_WebRegistrationSetting] AS r ON y.[YearQuarterID] = r.[YearQuarterID] WHERE (((r.[FirstRegistrationDate] IS NOT NULL AND r.[FirstRegistrationDate] <= cast('{0}' as smalldatetime)) OR y.[FirstClassDay] <= cast('{1}' as smalldatetime)) AND y.[YearQuarterID] <> 'Z999') ORDER BY y.[YearQuarterID] DESC) ",
			                           today.AddDays(14).ToShortDateString(), today.ToShortDateString());
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="whereClause"></param>
		/// <returns></returns>
		public string GetYearQuarterID(string whereClause)
		{
			string sql = string.Format("select top 1 YearQuarterID from vw_YearQuarter where YearQuarterID <> 'Z999' and {0} order by YearQuarterID", whereClause);
			try
			{
				if (_conn.State != ConnectionState.Open) {
					_conn.Open();
				}

				using (DbCommand cmd = _conn.CreateCommand())
				{
					cmd.CommandText = sql;

					return cmd.ExecuteScalar().ToString();
				}
			}
			catch (SqlException ex)
			{
				throw new DataException(String.Format("An error occurred while attempting execute the following SQL: \"{0}\"", sql), ex);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="whereClause"></param>
		/// <returns></returns>
		public string GetRandomClassID(string whereClause)
		{
			string sql = string.Format("select top 1 ClassID from vw_Class where {0} and YearQuarterID >= '{1}' order by newid()", whereClause, CurrentYrq);
			try
			{
				if (_conn.State != ConnectionState.Open) {
					_conn.Open();
				}

				using (DbCommand cmd = _conn.CreateCommand())
				{
					cmd.CommandText = sql;

					// NOTE: A NullReferenceException here most likely means an empty recordset was returned.
					return cmd.ExecuteScalar().ToString();
				}
			}
			catch (SqlException ex)
			{
				throw new DataException(String.Format("An error occurred while attempting execute the following SQL: \"{0}\"", sql), ex);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="whereClause"></param>
		/// <returns></returns>
		public int GetCourseCount(string whereClause)
		{
			string yrqFilter = string.Format(" and isnull(EffectiveYearQuarterEnd, 'Z999') >= '{0}' ", CurrentYrq);
			string sql = string.Format("select count(c.CourseID) FROM (select DISTINCT replace(CourseID,'&', ' ') AS CourseID, EffectiveYearQuarterBegin, EffectiveYearQuarterEnd from vw_Course WHERE {0} {1}) c", whereClause, yrqFilter);

			try
			{
				if (_conn.State != ConnectionState.Open) {
					_conn.Open();
				}

				using (DbCommand cmd = _conn.CreateCommand())
				{
					cmd.CommandText = sql;

					Debug.Print("==> EXECUTING SQL QUERY: {0}", sql);
					return int.Parse(cmd.ExecuteScalar().ToString());
				}
			}
			catch (SqlException ex)
			{
				throw new DataException(String.Format("An error occurred while attempting execute the following SQL: \"{0}\"", sql), ex);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="whereClause"></param>
		/// <returns></returns>
		public int GetCourseIDCountForSections(string whereClause)
		{
			string courseYrqFilter = string.Format(" isnull(EffectiveYearQuarterEnd, 'Z999') >= '{0}' ", CurrentYrq);
			string sql = string.Format("select count(distinct replace(CourseID,'&', ' ')) from vw_Class where {0} and CourseID in (select CourseID from vw_Course where {3}) {1} {2}",
																whereClause, _sectionFilter, _yrqFilter, courseYrqFilter);
			try
			{
				if (_conn.State != ConnectionState.Open) {
					_conn.Open();
				}

				using (DbCommand cmd = _conn.CreateCommand())
				{
					cmd.CommandText = sql;

					return int.Parse(cmd.ExecuteScalar().ToString());
				}
			}
			catch (SqlException ex)
			{
				throw new DataException(String.Format("An error occurred while attempting execute the following SQL: \"{0}\"", sql), ex);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="whereClause"></param>
		/// <returns></returns>
		public int GetSectionCount(string whereClause)
		{
      string sql = String.Format("SELECT count(DISTINCT ClassID) FROM vw_Class where {0} {1}", whereClause, _sectionFilter);

      // Only include the default YRQ filter if the caller hasn't explicitly defined a direct YRQ where clause
      Regex explicitWhere = new Regex(@"(^|\s+)YearQuarterID\s+(in\s+\(|[=><])\s*('|select)", RegexOptions.IgnoreCase);
      if (!explicitWhere.IsMatch(sql))
		  {
		    sql = string.Format("{0} {1}", sql, _yrqFilter);
		  }
			Debug.Print("Executing >> {0}", sql);

			try
			{
				if (_conn.State != ConnectionState.Open) {
					_conn.Open();
				}

				using (DbCommand cmd = _conn.CreateCommand())
				{
					cmd.CommandText = sql;

					return int.Parse(cmd.ExecuteScalar().ToString());
				}
			}
			catch (SqlException ex)
			{
				throw new DataException(String.Format("An error occurred while attempting execute the following SQL: \"{0}\"", sql), ex);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="whereClause"></param>
		/// <param name="applySectionFilter"></param>
		/// <returns></returns>
		public int GetCourseSubjectCountForSections(string whereClause, bool applySectionFilter = false)
		{
			string sql = String.Format("SELECT count(DISTINCT REPLACE(p.[CoursePrefixID], '&', '') + p.Title) FROM vw_CoursePrefix AS p where p.[CoursePrefixID] in (select LEFT(CourseID, 5) from vw_Class where {0} {1})",
																	whereClause, applySectionFilter ? _sectionFilter : string.Empty);

			return int.Parse(ExecuteScalar(sql));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="whereClause"></param>
		/// <param name="applySectionFilter"></param>
		/// <returns></returns>
		public string GetRandomCourseSubject(string whereClause, bool applySectionFilter = false)
		{
			string sql = String.Format("SELECT TOP 1 REPLACE(p.[CoursePrefixID], '&', '') FROM vw_CoursePrefix AS p where p.[CoursePrefixID] in (select LEFT(CourseID, 5) from vw_Class where {0} {1})",
																	whereClause, applySectionFilter ? _sectionFilter : string.Empty);

			return ExecuteScalar(String.Concat(sql, " ORDER BY NEWID()"));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="whereClause"></param>
		/// <returns></returns>
		public string GetRandomFootnoteID(string whereClause = null)
		{
			if (!string.IsNullOrEmpty(whereClause))
			{
				whereClause = string.Concat("where ", whereClause);
			}
			return ExecuteScalar(String.Format("select top 1 FootnoteID from vw_Footnote {0} order by NEWID()", whereClause));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="instructorID"></param>
		/// <returns></returns>
		public string GetInstructorName(string instructorID)
		{
			return ExecuteScalar(String.Format("select ISNULL(e.AliasName, e.FirstName) +' '+ e.LastName AS FullName FROM vw_Employee e WHERE e.[SID] = '{0}'", instructorID));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="queryString"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public IList<ISectionID> GetSectionIDListFromQuery(string queryString, int count)
		{
			IList<ISectionID> ids = new List<ISectionID>(count);

			string sql = string.Format("select top {0} ClassID from vw_Class where CourseTitle like '%{1}%' {2} {3}", count, queryString, _yrqFilter, _sectionFilter);
			try
			{
				if (_conn.State != ConnectionState.Open) {
					_conn.Open();
				}

				using (DbCommand cmd = _conn.CreateCommand())
				{
					cmd.CommandText = sql;

					cmd.ExecuteScalar().ToString();
					using (DbDataReader rs = cmd.ExecuteReader())
					{
						while (rs.Read())
						{
							ids.Add(SectionID.FromString(rs["ClassID"].ToString().Trim()));
						}
					}

					return ids;
				}
			}
			catch (SqlException ex)
			{
				throw new DataException(String.Format("An error occurred while attempting execute the following SQL: \"{0}\"", sql), ex);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="queryString"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public IList<ICourseID> GetCourseIDListFromQuery(string queryString, int count, string whereClause = null)
		{
			IList<ICourseID> ids = new List<ICourseID>(count);

			string sql = string.Format("select top {0} CourseID from (select distinct CourseID from vw_Class where CourseTitle like '%{1}%' {4} {2} {3}) a", count, queryString, _yrqFilter, _sectionFilter, whereClause ?? string.Empty);
			try
			{
				if (_conn.State != ConnectionState.Open) {
					_conn.Open();
				}

				using (DbCommand cmd = _conn.CreateCommand())
				{
					cmd.CommandText = sql;

					cmd.ExecuteScalar().ToString();
					using (DbDataReader rs = cmd.ExecuteReader())
					{
						while (rs.Read())
						{
							ids.Add(CourseID.FromString(rs["CourseID"].ToString().Trim()));
						}
					}

					return ids;
				}
			}
			catch (SqlException ex)
			{
				throw new DataException(String.Format("An error occurred while attempting execute the following SQL: \"{0}\"", sql), ex);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sql"></param>
		/// <returns></returns>
		public DbDataReader ExecuteReader(string sql)
		{
			string fullSql = string.Format("{0} {1} {2}", sql, _sectionFilter, _yrqFilter);
			try
			{
				if (_conn.State != ConnectionState.Open) {
					_conn.Open();
				}

				using (DbCommand cmd = _conn.CreateCommand())
				{
					cmd.CommandText = fullSql;
					return cmd.ExecuteReader();
				}
			}
			catch (SqlException ex)
			{
				throw new DataException(String.Format("An error occurred while attempting execute the following SQL: \"{0}\"", sql), ex);
			}
		}

		#region Helper methods
		private string ExecuteScalar(string sql)
		{
			try
			{
				if (_conn.State != ConnectionState.Open) {
					_conn.Open();
				}

				using (DbCommand cmd = _conn.CreateCommand())
				{
					cmd.CommandText = sql;

					return cmd.ExecuteScalar().ToString();
				}
			}
			catch (SqlException ex)
			{
				throw new DataException(String.Format("An error occurred while attempting execute the following SQL: \"{0}\"", sql), ex);
			}
		}

		#endregion

		#region Implementation of Dispose
		/// <summary>
		/// 
		/// </summary>
		public void Dispose()
		{
			GC.SuppressFinalize(this);
			Dispose(true);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="disposing"></param>
		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_conn != null && _conn.State != ConnectionState.Closed)
				{
					_conn.Close();
				}
			}
		}

		#endregion
	}
}
