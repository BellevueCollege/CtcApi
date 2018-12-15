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
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.Caching;
using Common.Logging;
using Ctc.Ods.Config;
using Ctc.Ods.Customizations;
using Ctc.Ods.Extensions;
using Ctc.Ods.Types;
using CtcApi;

namespace Ctc.Ods.Data
{
    /// <summary>
    /// Provides acccess to ODS data
    /// </summary>
    public class OdsRepository : IDisposable
    {
        private ILog _log = LogManager.GetLogger<OdsRepository>();

        OdsContext _context;
        private YearQuarter _currentYearQuarter;
        private ApiSettings _settings;
        private string _commonCourseChar;
        private ApplicationContext _appContext;

        private const string CacheType_Absolute = "absolute";
        private const string CacheType_Sliding = "sliding";

        #region Properties
        /// <summary>
        /// Gets a reference to the <see cref="DbContext"/> used by this object
        /// </summary>
        internal OdsContext _DbContext
        {
            get
            {
                if (_context == null)
                {
                    /* Disabled storing OdsContext in cache because:
                     * 1) This post mentions a DataContext object shouldn't be kept around: http://stackoverflow.com/questions/460831/linq-to-sql-return-from-function-as-iqueryablet
                     * 2) It doesn't seem to be giving us that much benefit.
                     * 
                     * - 10/04/2011, shawn.south@bellevuecollege.edu */

                    //Cache cache = _httpContext != null ? _httpContext.Cache : null;

                    //if (cache != null && cache["OdsContext"] != null)
                    //{
                    //  _context = cache["OdsContext"] as OdsContext;
                    //}
                    //else
                    //{
                    _context = new OdsContext();
                    //  if (cache != null)
                    //  {
                    //    cache.Add("OdsContext", _context, null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
                    //  }
                    //}
                }
                return _context;
            }
        }

        private ApiSettings Settings
        {
            get
            {
                _settings = _settings ?? ConfigurationManager.GetSection(ApiSettings.SectionName) as ApiSettings;
                return _settings;
            }
        }
        #endregion

        /// <summary>
        /// Creates a new Repository instance for accessing data from the ODS
        /// </summary>
        /// <seealso cref="ApiSettings"/>
        public OdsRepository(ApplicationContext appContext)
        {
            _appContext = appContext;
            Debug.Print("==> instantiating OdsRepository()...");
            _settings = ConfigurationManager.GetSection(ApiSettings.SectionName) as ApiSettings;
            _commonCourseChar = Settings.RegexPatterns.CommonCourseChar; ;
        }

        /// <summary>
        /// Creates a new Repository instance for accessing data from the ODS
        /// </summary>
        /// <seealso cref="ApiSettings"/>
        public OdsRepository() : this(new ApplicationContext())
        {
        }

        #region YearQuarter
        /// <summary>
        /// Gets the current <see cref="YearQuarter"/> for today's date
        /// </summary>
        /// <seealso cref="ApiSettings.YearQuarter"/>
        public YearQuarter CurrentYearQuarter
        {
            get
            {
                if (_currentYearQuarter == null)
                {
                    _log.Trace("Retrieving current YearQuarter from DB or HttpRuntime.Cache");
                    DateTime curDate = _appContext.CurrentDate ?? DateTime.Now;
                    
                    //abstracted query out to 
                    IQueryable<YearQuarterEntity> quarters = from y in _DbContext.YearQuarters
                                                             where (y.LastClassDay >= curDate.Date)
                                                                  && (y.YearQuarterID != Settings.YearQuarter.Max)
                                                             orderby y.YearQuarterID
                                                             select y;


                    //get quarter using appropriate method for cache type
                    YearQuarterEntity yrq;
                    if (Settings.YearQuarter.CacheType == CacheType_Sliding)
                    {
                        var cacheWindowSliding = TimeSpan.FromMinutes(Settings.YearQuarter.Cache);
                        yrq = quarters.FromCache(cacheWindowSliding).Take(1).Single();
                    } else
                    {
                        var cacheWindow = DateTime.UtcNow.Add(TimeSpan.FromMinutes(Settings.YearQuarter.Cache));
                        yrq = quarters.FromCache(cacheWindow).Take(1).Single();
                    }
                    /*yrq = _DbContext.YearQuarters.FromCache( cacheWindowSliding )
                                                        .Where(quarter => quarter.LastClassDay >= curDate.Date && quarter.YearQuarterID != Settings.YearQuarter.Max)
                                                        .OrderBy(quarter => quarter.YearQuarterID)
                                                        .Take(1).Single();
                     */
                    _currentYearQuarter = YearQuarter.FromString(yrq.YearQuarterID);
                }
                else
                {
                    _log.Debug("Using current YearQuarter already in memory. (NOTE: This is NOT cache - it is a class variable.");
                }
                return _currentYearQuarter;
            }
        }

        /// <summary>
        /// Gets the current registration <see cref="YearQuarter"/>for today's date
        /// </summary>
        /// <returns>The nearest <see cref="YearQuarter"/> that is currently open for registration.</returns>
        /// <seealso cref="GetRegistrationQuarters"/>
        /// <seealso cref="ApiSettings.YearQuarter"/>
        public YearQuarter CurrentRegistrationQuarter
        {
            get
            {
                return GetRegistrationQuarters()[0];
            }
        }

        /// <summary>
        /// Gets a list of <see cref="YearQuarter"/>s, starting with the current registration quarter
        /// </summary>
        /// <param name="count">Total number of <see cref="YearQuarter"/>s to return.</param>
        /// <returns>A list of <see cref="YearQuarter"/> objects, with the first being the current registration quarter.</returns>
        /// <seealso cref="CurrentRegistrationQuarter"/>
        /// <seealso cref="ApiSettings.YearQuarter"/>
        public IList<YearQuarter> GetRegistrationQuarters(int count = 1)
        {
            // LINQ for EF only supports primitive variables
            string maxYrq = Settings.YearQuarter.Max;
            DateTime today = _appContext.CurrentDate ?? DateTime.Now;
            // Registration information should be available *before* registration begins
            // NOTE: we jump ahead n days to simulate date lookup n days prior to the registration date
            DateTime registrationDate = today.Add(new TimeSpan(Settings.YearQuarter.RegistrationLeadDays, 0, 0, 0));
            // Set other boundary of included quarters - last class day of quarter is in future or within last n days
            DateTime quarterSelectEndDate = today.Subtract(new TimeSpan(Settings.YearQuarter.PostLastClassDays, 0, 0, 0));

            IQueryable<YearQuarterEntity> quarters = from y in _DbContext.YearQuarters
                                                     join r in _DbContext.WebRegistrationSettings on y.YearQuarterID equals r.YearQuarterID into y_r
                                                     from r in y_r.DefaultIfEmpty()
                                                     where (r.FirstRegistrationDate != null && r.FirstRegistrationDate <= registrationDate.Date)
                                                          // include the quarter we're currently in, even if registration is no longer open for it
                                                          && (y.LastClassDay >= quarterSelectEndDate.Date)
                                                          && y.YearQuarterID != maxYrq
                                                     orderby y.YearQuarterID descending
                                                     select y;

            _log.Trace("Retrieving registration YearQuarters from DB or HttpRuntime.Cache");
            var cacheWindow = DateTime.UtcNow.Add(TimeSpan.FromMinutes(Settings.YearQuarter.Cache));
            if ( Settings.YearQuarter.CacheType == CacheType_Sliding )
            {
                var cacheWindowSliding = TimeSpan.FromMinutes(Settings.YearQuarter.Cache);
                return quarters.FromCache(cacheWindowSliding)
                                              .Take(count)
                                              .Select(q => new YearQuarter
                                              {
                                                  ID = q.YearQuarterID
                                              }).ToList();
            }

            return quarters.FromCache(cacheWindow)
                                              .Take(count)
                                              .Select(q => new YearQuarter
                                              {
                                                  ID = q.YearQuarterID
                                              }).ToList();
        }

        /// <summary>
        /// Gets a list of future <see cref="YearQuarter"/>s, starting with the current registration quarter.
        /// </summary>
        /// <param name="count">Total number of <see cref="YearQuarter"/>s to return (including the current quarter).</param>
        /// <returns>A list of <see cref="YearQuarter"/> objects, with the first being the current registration quarter.</returns>
        public IList<YearQuarter> GetFutureQuarters(int count = 1)
        {
            string maxYrq = Settings.YearQuarter.Max;
            DateTime today = _appContext.CurrentDate ?? DateTime.Now;
            // Registration information should be available *before* registration begins
            // NOTE: we jump ahead n days to simulate date lookup n days prior to the registration date
            DateTime registrationDate = today.Add(new TimeSpan(Settings.YearQuarter.RegistrationLeadDays, 0, 0, 0));
            
            IQueryable<YearQuarterEntity> quarters = from y in _DbContext.YearQuarters
                                                     join r in _DbContext.WebRegistrationSettings on y.YearQuarterID equals r.YearQuarterID into y_r
                                                     from r in y_r.DefaultIfEmpty()
                                                     where (r.FirstRegistrationDate != null && r.FirstRegistrationDate >= registrationDate.Date
                                                          // include the quarter we're currently in, even if registration is no longer open for it
                                                          || y.LastClassDay >= today.Date)
                                                          && y.YearQuarterID != maxYrq
                                                     orderby y.YearQuarterID ascending
                                                     select y;

            _log.Trace("Retrieving future YearQuarters from DB or HttpRuntime.Cache");
            var cacheWindow = DateTime.UtcNow.Add(TimeSpan.FromMinutes(Settings.YearQuarter.Cache));
            if ( Settings.YearQuarter.CacheType == CacheType_Sliding )
            {
                var cacheWindowSliding = TimeSpan.FromMinutes(Settings.YearQuarter.Cache);
                return quarters.FromCache(cacheWindowSliding)
                                  .Take(count)
                                  .Select(q => new YearQuarter
                                  {
                                      ID = q.YearQuarterID
                                  }).ToList();
            }

            return quarters.FromCache(cacheWindow)
                                .Take(count)
                                .Select(q => new YearQuarter
                                {
                                    ID = q.YearQuarterID
                                }).ToList();
        }

        #endregion

        #region Course
        /// <summary>
        /// Retrieves all <see cref="Course"/>s from the ODS
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        ///		<example>
        ///			<code type="cs">
        /// using (OdsRepository repository = new OdsRepository())
        ///	{
        ///		IEnumerable&lt;Course&gt; courses = repository.GetCourses();
        /// }
        ///			</code>
        ///		</example>
        /// </remarks>
        public IList<Course> GetCourses(IList<ISectionFacet> facetOptions = null)
        {
            IQueryable<Course> courses;

            if (facetOptions != null)
            {
                string yrqId = CurrentYearQuarter.ID;
                SectionFilters filters = new SectionFilters(this) { facetOptions };

                courses = _DbContext.Courses.Join(_DbContext.Sections.CompoundWhere(filters.FilterArray),
                                                                                    course => course.CourseID,
                                                                                    section => section.CourseID,
                                                                                    (course, section) => new { course, section })
                                                                        .Where(h => (h.course.YearQuarterEnd ?? Settings.YearQuarter.Max).CompareTo(yrqId) >= 0)
                                                                        .OrderBy(h => h.course.CourseID.Replace(_commonCourseChar, " "))
                                                                        .Distinct()
                                                                        .Select(h => new Course
                                                                        {
                                                                            CourseID = h.course.CourseID,
                                                                            Credits = h.course.Credits,
                                                                            Title = h.course.Title2 ?? h.course.Title1,
                                                                            _CourseDescriptions1 = _DbContext.CourseDescriptions1.Where(d => d.CourseID == h.course.CourseID),
                                                                            _CourseDescriptions2 = _DbContext.CourseDescriptions2.Where(d => d.CourseID == h.course.CourseID),
                                                                            _YearQuarterBegin = h.course.YearQuarterBegin,
                                                                            _YearQuarterEnd = h.course.YearQuarterEnd,
                                                                            IsVariableCredits = (h.course.VariableCredits ?? false),
                                                                            _Footnotes = _DbContext.Footnote.Where(f => h.course.FootnoteID1 == f.FootnoteId || h.course.FootnoteID2 == f.FootnoteId)
                                                                                                                                        .Select(f => f.FootnoteText)
                                                                        });
            }
            else
            {
                courses = GetAllCourses();
            }

            return courses.OrderBy(c => c.CourseID.Replace(_commonCourseChar, " ")).ThenBy(c => c._YearQuarterBegin).ToList();
        }

        ///<summary>
        /// Retrieves the specified <see cref="Course"/>s from the ODS
        ///</summary>
        ///<param name="subject"></param>
        ///<param name="facetOptions"></param>
        ///<returns></returns>
        public IList<Course> GetCourses(string subject, IList<ISectionFacet> facetOptions = null)
        {
            IQueryable<Course> courses;

            if (facetOptions != null)
            {
                string yrqId = CurrentYearQuarter.ID;
                SectionFilters filters = new SectionFilters(this) { facetOptions };

                courses = _DbContext.Courses.Join(_DbContext.Sections.CompoundWhere(filters.FilterArray),
                                                                                course => course.CourseID,
                                                section => section.CourseID,
                                                (course, section) => new { course, section })
                                                                    .Where(h => h.course.CourseID.Substring(0, 5).Trim().ToUpper() == subject.ToUpper())
                                                                    .Where(h => (h.course.YearQuarterEnd ?? Settings.YearQuarter.Max).CompareTo(yrqId) >= 0)
                                                                    .Distinct()
                                                                    .Select(h => new Course
                                                                    {
                                                                        CourseID = h.course.CourseID,
                                                                        Credits = h.course.Credits,
                                                                        Title = h.course.Title2 ?? h.course.Title1,
                                                                        _CourseDescriptions1 = _DbContext.CourseDescriptions1.Where(d => d.CourseID == h.course.CourseID),
                                                                        _CourseDescriptions2 = _DbContext.CourseDescriptions2.Where(d => d.CourseID == h.course.CourseID),
                                                                        _YearQuarterBegin = h.course.YearQuarterBegin,
                                                                        _YearQuarterEnd = h.course.YearQuarterEnd,
                                                                        IsVariableCredits = h.course.VariableCredits ?? false,
                                                                        _Footnotes = _DbContext.Footnote.Where(f => h.course.FootnoteID1 == f.FootnoteId || h.course.FootnoteID2 == f.FootnoteId)
                                                                                                                                        .Select(f => f.FootnoteText)
                                                                    });
            }
            else
            {
                courses = GetAllCourses(subjects: subject);
            }

            return courses.OrderBy(c => c.CourseID.Replace(_commonCourseChar, " ")).ThenBy(c => c._YearQuarterBegin).ToList();
        }

        /// <summary>
        /// Retrieves the specified <see cref="Course"/>s from the ODS
        /// </summary>
        /// <param name="subjects"></param>
        /// <param name="facetOptions"></param>
        /// <returns></returns>
        public IList<Course> GetCourses(IList<string> subjects, IList<ISectionFacet> facetOptions = null)
        {
            if (subjects.Count == 1)
            {
                return GetCourses(subjects[0], facetOptions);
            }

            IQueryable<Course> courses;

            if (facetOptions != null)
            {
                string yrqId = CurrentYearQuarter.ID;
                SectionFilters filters = new SectionFilters(this) { facetOptions };

                courses = _DbContext.Courses.Join(_DbContext.Sections.CompoundWhere(filters.FilterArray),
                                                                                course => course.CourseID,
                                                section => section.CourseID,
                                                (course, section) => new { course, section })
                                                                    .Where(h => subjects.Select(s => s.ToUpper()).Contains(h.course.CourseID.Substring(0, 5).Trim().ToUpper()))
                                                                    .Where(h => (h.course.YearQuarterEnd ?? Settings.YearQuarter.Max).CompareTo(yrqId) >= 0)
                                                                    .Distinct()
                                                                    .Select(h => new Course
                                                                    {
                                                                        CourseID = h.course.CourseID,
                                                                        Credits = h.course.Credits,
                                                                        Title = h.course.Title2 ?? h.course.Title1,
                                                                        _CourseDescriptions1 = _DbContext.CourseDescriptions1.Where(d => d.CourseID == h.course.CourseID),
                                                                        _CourseDescriptions2 = _DbContext.CourseDescriptions2.Where(d => d.CourseID == h.course.CourseID),
                                                                        _YearQuarterBegin = h.course.YearQuarterBegin,
                                                                        _YearQuarterEnd = h.course.YearQuarterEnd,
                                                                        IsVariableCredits = h.course.VariableCredits ?? false,
                                                                        _Footnotes = _DbContext.Footnote.Where(f => h.course.FootnoteID1 == f.FootnoteId || h.course.FootnoteID2 == f.FootnoteId)
                                                                                                                                        .Select(f => f.FootnoteText)
                                                                    });
            }
            else
            {
                courses = GetAllCourses(subjects: subjects.ToArray());
            }

            return courses.OrderBy(c => c.CourseID.Replace(_commonCourseChar, " ")).ThenBy(c => c._YearQuarterBegin).ToList();
        }

        /// <summary>
        /// Retrieves the specified <see cref="Course"/>s from the ODS
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="facetOptions"></param>
        /// <returns>
        ///		<note type="note">
        ///		This method may return more than one <see cref="Course"/> record.
        ///		</note>
        /// </returns>
        /// <remarks>
        ///		<example>
        ///			<code type="cs">
        ///		using (OdsRepository repository = new OdsRepository())
        ///		{
        ///			IList&lt;Course&gt; courses = repository.GetCourses(CourseID.FromString("ART 101"));
        ///		}
        ///			</code>
        ///		</example>
        /// </remarks>
        public IList<Course> GetCourses(ICourseID courseId, IList<ISectionFacet> facetOptions = null)
        {
            IQueryable<Course> courses;

            if (facetOptions != null)
            {
                SectionFilters filters = new SectionFilters(this) { facetOptions };
                // LINQ to Entities only supports simple data types
                string yrqId = CurrentYearQuarter.ID;
                string realSubject = (courseId.IsCommonCourse ? string.Concat(courseId.Subject, Settings.RegexPatterns.CommonCourseChar) : courseId.Subject).ToUpper();

                courses = _DbContext.Courses.Join(_DbContext.Sections.CompoundWhere(filters.FilterArray),
                                                  course => course.CourseID,
                                                  section => section.CourseID,
                                                  (course, section) => new { course, section })
                        .Where(
                                h =>
                                h.course.CourseID.Substring(0, 5).Trim().ToUpper() == realSubject
                                && h.course.CourseID.EndsWith(courseId.Number))
                        .Where(h => (h.course.YearQuarterEnd ?? Settings.YearQuarter.Max).CompareTo(yrqId) >= 0)
                        .Distinct()
                        .Select(h => new Course
                        {
                            CourseID = h.course.CourseID,
                            Credits = h.course.Credits,
                            Title = h.course.Title2 ?? h.course.Title1,
                            _CourseDescriptions1 = _DbContext.CourseDescriptions1.Where(d => d.CourseID == h.course.CourseID),
                            _CourseDescriptions2 = _DbContext.CourseDescriptions2.Where(d => d.CourseID == h.course.CourseID),
                            _YearQuarterBegin = h.course.YearQuarterBegin,
                            _YearQuarterEnd = h.course.YearQuarterEnd,
                            IsVariableCredits = h.course.VariableCredits ?? false,
                            _Footnotes = _DbContext.Footnote.Where(f => h.course.FootnoteID1 == f.FootnoteId || h.course.FootnoteID2 == f.FootnoteId)
                                                                                                                  .Select(f => f.FootnoteText)
                        });
            }
            else
            {
                courses = GetAllCourses(courseId: courseId);
            }

            return courses.OrderBy(c => c.CourseID.Replace(_commonCourseChar, " ")).ThenBy(c => c._YearQuarterBegin).ToList();
        }

        /// <summary>
        /// Retrieves specified course description from database
        /// </summary>
        /// <param name="courseID"></param>
        /// <param name="yrq"></param>
        /// <returns></returns>
        public IList<CourseDescription> GetCourseDescription(ICourseID courseID, YearQuarter yrq = null)
        {
            if (yrq == null) { yrq = CurrentYearQuarter; }
            return GetCourseDescriptions(courseID, yrq, _DbContext.CourseDescriptions1.Select(d => d), _DbContext.CourseDescriptions2.Select(d => d));
        }

        #region Private members
        /// <summary>
        /// Returns all <see cref="Course"/>s without <see cref="Section"/> filtering
        /// </summary>
        /// <param name="yrq">Only retrieve <see cref="Course"/>s that are active for the specified <see cref="YearQuarter"/></param>
        /// <param name="courseId">Only retrieve <see cref="Course"/>s for the specified <see cref="ICourseID"/></param>
        /// <param name="subjects">Only retrieve <see cref="Course"/>s with the specified subject prefix(es).</param>
        /// <remarks>
        ///		If any GetCourses method was called without specifying any <see cref="ISectionFacet"/>s, this method
        ///		will retrieve all current and future <see cref="Course"/>s after applying the remaining filter requests (e.g.
        ///		<paramref name="courseId"/>, <paramref name="subjects"/>, etc.)
        /// </remarks>
        /// <returns></returns>
        protected IQueryable<Course> GetAllCourses(YearQuarter yrq = null, ICourseID courseId = null, params string[] subjects)
        {
            IQueryable<CourseEntity> courseData = _DbContext.Courses.Select(c => c);

            string yrqId = (yrq != null) ? yrq.ID : CurrentYearQuarter.ID;

            // filter by active-in-current-or-future YRQ
            courseData = courseData.Where(c => (c.YearQuarterEnd ?? Settings.YearQuarter.Max).CompareTo(yrqId) >= 0);

            // filter by course
            if (courseId != null)
            {
                // LINQ-to-Entities only support simple data types
                string realSubject = (courseId.IsCommonCourse ? string.Concat(courseId.Subject, Settings.RegexPatterns.CommonCourseChar) : courseId.Subject).ToUpper();

                courseData = courseData.Where(c => c.CourseID.Substring(0, 5).Trim().ToUpper() == realSubject && c.CourseID.EndsWith(courseId.Number));
            }

            // filter by subject(s)
            if (subjects != null && subjects.Count() > 0)
            {
                if (subjects.Count() == 1)
                {
                    string subject = subjects[0].ToUpper(); // LINQ-to-Entities doesn't support indexed array values
                    courseData = courseData.Where(c => c.CourseID.Substring(0, 5).Trim().ToUpper() == subject);
                }
                else
                {
                    courseData = courseData.Where(c => subjects.Select(s => s.ToUpper()).Contains(c.CourseID.Substring(0, 5).Trim().ToUpper()));
                }
            }

            return courseData.Distinct().Select(c => new Course
            {
                CourseID = c.CourseID,
                Credits = c.Credits,
                Title = c.Title2 ?? c.Title1,
                _CourseDescriptions1 = _DbContext.CourseDescriptions1.Where(d => d.CourseID == c.CourseID),
                _CourseDescriptions2 = _DbContext.CourseDescriptions2.Where(d => d.CourseID == c.CourseID),
                _YearQuarterBegin = c.YearQuarterBegin,
                _YearQuarterEnd = c.YearQuarterEnd,
                IsVariableCredits = c.VariableCredits ?? false,
                _Footnotes = _DbContext.Footnote.Where(f => c.FootnoteID1 == f.FootnoteId || c.FootnoteID2 == f.FootnoteId)
                                                                                                                        .Select(f => f.FootnoteText)
            });
        }

        #endregion

        #endregion

        #region GetSections members
        /// <summary>
        /// Retrieves an <see cref="IList"/> of <see cref="Section"/>s
        /// </summary>
        /// <param name="facetOptions"></param>
        /// <returns></returns>
        public IList<Section> GetSections(IList<ISectionFacet> facetOptions = null)
        {
            SectionFilters filters = new SectionFilters(this);

            if (facetOptions != null)
            {
                filters.Add(facetOptions);
            }

            IQueryable<Section> sections = GetSections(filters);

            Debug.Print("==> Created [{0}] Sections.", sections.Count());
            return sections.ToList();
        }

        /// <summary>
        /// Retrieves an <see cref="IList"/> of <see cref="Section"/>s
        /// </summary>
        /// <param name="yrq"></param>
        /// <param name="facetOptions"></param>
        /// <returns></returns>
        public IList<Section> GetSections(YearQuarter yrq, IList<ISectionFacet> facetOptions = null)
        {
            SectionFilters filters = new SectionFilters(this);

            string yrqId = yrq.ID;
            filters.Add(s => s.YearQuarterID == yrqId);

            if (facetOptions != null)
            {
                filters.Add(facetOptions);
            }

            IQueryable<Section> sections = GetSections(filters);

            Debug.Print("==> Created [{0}] Sections.", sections.Count());
            return sections.ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="yrq"></param>
        /// <param name="facetOptions"></param>
        /// <returns></returns>
        public IList<Section> GetSections(ICourseID courseId, YearQuarter yrq = null, IList<ISectionFacet> facetOptions = null)
        {
            SectionFilters filters = new SectionFilters(this);

            // apply ancillary filters
            if (yrq != null)
            {
                string yrqId = yrq.ID;
                filters.Add(s => s.YearQuarterID == yrqId);
            }
            if (facetOptions != null)
            {
                filters.Add(facetOptions);
            }

            // apply the primary filter
            filters.Add(s => s.CourseID.StartsWith(courseId.Subject) && s.CourseID.EndsWith(courseId.Number));

            IQueryable<Section> sections = GetSections(filters);

            return sections.ToList();
        }

        /// <summary>
        /// Retrieve all Sections for a specified <see cref="Course.Subject"/>
        /// </summary>
        /// <param name="subject">The <see cref="Course"/> <see cref="Course.Subject"/> which you want to retrieve all Sections for.</param>
        /// <param name="yrq"><b>(OPTIONAL)</b> Filter results so they only include Sections in this YearQuarter</param>
        /// <param name="facetOptions"></param>
        /// <returns></returns>
        public IList<Section> GetSections(string subject, YearQuarter yrq = null, IList<ISectionFacet> facetOptions = null)
        {
            SectionFilters filters = new SectionFilters(this);

            // apply ancillary filters
            if (yrq != null)
            {
                string yrqId = yrq.ID;
                filters.Add(s => s.YearQuarterID == yrqId);
            }
            filters.Add(facetOptions);

            // apply the primary filter
            filters.Add(s => s.CourseID.Substring(0, 5).Trim().ToUpper() == subject.ToUpper());

            IQueryable<Section> sections = GetSections(filters);
            return sections.ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subjects"></param>
        /// <param name="yrq"></param>
        /// <param name="facetOptions"></param>
        /// <returns></returns>
        public IList<Section> GetSections(IList<string> subjects, YearQuarter yrq = null, IList<ISectionFacet> facetOptions = null)
        {
            if (subjects.Count == 1)
            {
                return GetSections(subjects.First(), yrq, facetOptions);
            }

            string[] prefixes = subjects.ToArray();

            SectionFilters filters = new SectionFilters(this);
            filters.Add(s => prefixes.Select(p => p.ToUpper()).Contains(s.CourseID.Substring(0, 5).Trim().ToUpper()));
            if (yrq != null)
            {
                string yrqId = yrq.ID;
                filters.Add(s => s.YearQuarterID == yrqId);
            }
            filters.Add(facetOptions);

            IQueryable<Section> sections = GetSections(filters);
            return sections.ToList();
        }

        /// <summary>
        /// Retrieve all Sections specified in the provided list of <see cref="SectionID"/>s
        /// </summary>
        /// <param name="sectionIds"></param>
        /// <param name="facetOptions"></param>
        /// <returns></returns>
        public IList<Section> GetSections(IList<ISectionID> sectionIds, IList<ISectionFacet> facetOptions = null)
        {
            string[] ids = sectionIds.Select(s => s.ToString()).ToArray();

            SectionFilters filters = new SectionFilters(this)
                                         {
                                                 s => ids.Contains(s.ClassID.Trim()),
                                                                        facetOptions
                                         };

            IQueryable<Section> sections = GetSections(filters);
            return sections.ToList();
        }

        /// <summary>
        /// Retrieve all Sections specified in the provided list of <see cref="CourseID"/>s
        /// </summary>
        /// <param name="courseIds"></param>
        /// <param name="yrq"></param>
        /// <param name="facetOptions"></param>
        /// <returns></returns>
        public IList<Section> GetSections(IList<ICourseID> courseIds, YearQuarter yrq = null, IList<ISectionFacet> facetOptions = null)
        {
            // removing all whitespace
            string[] ids = courseIds.Select(c => string.Concat(c.Subject, c.Number)).ToArray();

            SectionFilters filters = new SectionFilters(this);

            if (yrq != null)
            {
                string yrqId = yrq.ID;
                filters.Add(s => s.YearQuarterID == yrqId);
            }
            // compare with whitespace removed
            filters.Add(s => ids.Contains(string.Concat(s.CourseID.Substring(0, 5).Trim(), s.CourseID.Substring(5).Trim())));
            // add remaining facets
            filters.Add(facetOptions);

            IQueryable<Section> sections = GetSections(filters);
            return sections.ToList();
        }

        #region Private members
        /// <summary>
        /// Converts a collection of <see cref="SectionEntity"/>s into a collection of <see cref="Section"/> objects
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        /// <remarks>
        /// Performing this convertion allows us to construct a richer, multi-level <see cref="Section"/> object unconstrained
        /// by database-imposed relationship structures.
        /// </remarks>
        private IQueryable<Section> GetSections(SectionFilters filters)
        {
            // TODO: move this master query into a Mapper class.

            // NOTE: Linq to Entities can't handle values more complex than simple data types
            // TODO: encapsulate config settings in a handler class
            string emailDomain = ConfigurationManager.AppSettings["EmailDomain"];
            DefaultSectionDaysNode defaultDaysValue = Settings.SectionDaysDefault;
            string waitlistStatus = Settings.Waitlist.Status;

            // construct the Section object we will pass back to the caller
            IQueryable<Section> sections = _DbContext.Set<SectionEntity>().CompoundWhere(filters.FilterArray)
                    .GroupJoin(_DbContext.Footnote, s => s.FootnoteID1, f => f.FootnoteId, (sectionData, temp) => new { sectionData, temp })
                    .SelectMany(t => t.temp.DefaultIfEmpty(), (t, foot1) => new { t.sectionData, Footnote1 = foot1.FootnoteText })
                    .GroupJoin(_DbContext.Footnote, s => s.sectionData.FootnoteID2, f => f.FootnoteId, (joinedData, temp) => new { joinedData, temp })
                    .SelectMany(t => t.temp.DefaultIfEmpty(), (t, foot2) => new { t.joinedData, Footnote2 = foot2.FootnoteText })
                    .OrderBy(s => s.joinedData.sectionData.YearQuarterID)   // apply a default sort order (expected to be the most common desired)
                    .ThenBy(s => s.joinedData.sectionData.CourseID.Replace(_commonCourseChar, " "))
                    .Select(section => new Section
                    {
                        ClassID = section.joinedData.sectionData.ClassID,
                        CourseID = section.joinedData.sectionData.CourseID,
                        // use CourseTitle2 from the Course table, otherwise fall back to CourseTitle, then the course title from the Section (i.e. Class) table
                        // NOTE: This is Bellevue College logic. If different logic is desired, we should move this to the Section class
                        _CourseTitle = _DbContext.Courses.Where(c => c.CourseID == section.joinedData.sectionData.CourseID && c.YearQuarterEnd.CompareTo(section.joinedData.sectionData.YearQuarterID) >= 0)
                                                                                                                                     .Select(c => c.Title2 ?? c.Title1).DefaultIfEmpty(section.joinedData.sectionData.CourseTitle).FirstOrDefault(),
                        Credits = section.joinedData.sectionData.Credits,
                        SectionCode = section.joinedData.sectionData.Section,
                        _YearQuarterID = section.joinedData.sectionData.YearQuarterID,
                        StartDate = section.joinedData.sectionData.StartDate,
                        EndDate = section.joinedData.sectionData.EndDate,
                        // count up how many students are waitlisted for each section
                        WaitlistCount = _DbContext.WaitListCounts.Where(w => w.Status == waitlistStatus && w.ClassID == section.joinedData.sectionData.ClassID).Count(),
                        // Construct the collection of instructor/day/time/location information...
                        Offered = _DbContext.InstructionDetails.Where(i => i.ClassID == section.joinedData.sectionData.ClassID)
                                                                                                                                                 .OrderBy(i => i.SessionSequence)
                                                                                                                                .Select(i => new OfferedItem
                                                                                                                                {
                                                                                                                                    // get the Day text (e.g. MW, TTh)
                                                                                                                                    Days = _DbContext.Days.Where(d => d.DayID == i.DayID)
                                                                                                                .Select(d => d.Title == defaultDaysValue.ValueToFind ? defaultDaysValue.NewValue : d.Title)
                                                                                                                .DefaultIfEmpty(defaultDaysValue.NewValue)
                                                                                                                                                                                                       .FirstOrDefault(),
                                                                                                                                    StartTime = i.StartTime,
                                                                                                                                    EndTime = i.EndTime,
                                                                                                                                    InstructorID = i.InstructorSID,
                                                                                                                                    InstructorName = _DbContext.Employees.Where(e => e.SID == i.InstructorSID)
                                                                                                                                                                                                                                                             // string.Format() not supported by EF, but string.Concat() is
                                                                                                                                                                                                                                                             .Select(e => String.Concat(e.AliasName ?? e.FirstName, " ", e.LastName))
                                                                                                                                                                                                                                                             .DefaultIfEmpty(string.Empty)
                                                                                                                                                                                                                                                             .FirstOrDefault(),
                                                                                                                                    InstructorEmail = _DbContext.Employees.Where(e => e.SID == i.InstructorSID)
                                                                                                                                                                                                                                       .Select(e => e.WorkEmail ?? String.Concat(e.ADUserName, "@", emailDomain))
                                                                                                                                                                                                                                       .DefaultIfEmpty(String.Empty)
                                                                                                                                                                                                                                       .FirstOrDefault(),
                                                                                                                                    Room = i.Room,
                                                                                                                                    _SessionSequence = i.SessionSequence ?? "00"
                                                                                                                                }),
                        IsLinked = section.joinedData.sectionData.ItemYRQLink != null && section.joinedData.sectionData.ItemYRQLink != section.joinedData.sectionData.ItemNumber,
                        LinkedTo = section.joinedData.sectionData.ItemYRQLink,
                        _Footnote1 = section.joinedData.Footnote1 ?? string.Empty,
                        _Footnote2 = section.Footnote2 ?? string.Empty,
                        //_CourseFootnotes = _DbContext.Footnote.Where(f => _DbContext.Courses.Where(c => c.CourseID == section.joinedData.sectionData.CourseID
                        //                                                                              && c.YearQuarterEnd.CompareTo(section.joinedData.sectionData.YearQuarterID) >= 0)
                        //                                                                    .Select(c => c.FootnoteID1).Contains(f.FootnoteId)
                        //                                                  //                  ||
                        //                                                  //_DbContext.Courses.Where(c => c.CourseID == section.joinedData.sectionData.CourseID
                        //                                                  //                            && c.YearQuarterEnd.CompareTo(section.joinedData.sectionData.YearQuarterID) >= 0)
                        //                                                  //                  .Select(c => c.FootnoteID2).Contains(f.FootnoteId)
                        //                                      ).Select(f => f.FootnoteText),
                        _CourseDescriptions1 = _DbContext.CourseDescriptions1.Where(d => d.CourseID == section.joinedData.sectionData.CourseID),
                        _CourseDescriptions2 = _DbContext.CourseDescriptions2.Where(d => d.CourseID == section.joinedData.sectionData.CourseID),
                        _SBCTCMisc1 = section.joinedData.sectionData.SBCTCMisc1,
                        _ContinuousSequentialIndicator = section.joinedData.sectionData.ContinuousSequentialIndicator,
                        // As per SBCTC policy (http://www.sbctc.ctc.edu/general/policymanual/_a-policymanual-ch5Append.aspx), the default
                        // "last registration date" is the "last instructional day of the course" - 2/24/2012, shawn.south@bellevuecollege.edu
                        // TODO: Use LastClassDay of quarter if EndDate is null
                        _LastRegistrationDate = section.joinedData.sectionData.LastRegistrationDate ?? section.joinedData.sectionData.EndDate,
                        IsVariableCredits = section.joinedData.sectionData.VariableCredits ?? false,
                        IsDifferentStartDate = section.joinedData.sectionData.StartDate.HasValue && section.joinedData.sectionData.StartDate !=
                                                                _DbContext.YearQuarters.Where(y => y.YearQuarterID == section.joinedData.sectionData.YearQuarterID)
                                                                        .Select(y => y.FirstClassDay)
                                                                                                    .DefaultIfEmpty(DateTime.MaxValue)
                                                                        .FirstOrDefault(),
                        IsDifferentEndDate = section.joinedData.sectionData.EndDate.HasValue && section.joinedData.sectionData.EndDate !=
                                                                       _DbContext.YearQuarters.Where(y => y.YearQuarterID == section.joinedData.sectionData.YearQuarterID)
                                                                               .Select(y => y.LastClassDay)
                                                                                                                    .DefaultIfEmpty(DateTime.MaxValue)
                                                                               .FirstOrDefault(),

                    });
            Debug.Print("==> Created [{0}] Sections.  {1}", sections.Count(), DateTime.Now);
            Debug.Flush();

            return sections;
        }
        #endregion

        #endregion

        #region GetCourseSubjects members
        /// <summary>
        /// Returns an unfiltered list of all Course Subjects from the CoursePrefix table
        /// </summary>
        /// <remarks>
        /// This list is filtered to only include <see cref="Course"/> subjects which only have active courses
        /// in the current and/or future <see cref="YearQuarter"/>s.
        /// </remarks>
        /// <returns>List of CoursePrefix including Title and Subject</returns>
        public IList<CoursePrefix> GetCourseSubjects()
        {
            string ccChar = Settings.RegexPatterns.CommonCourseChar;

            // TODO: Filter by current or future course active (in CourseDescriptions) instead of Sections' YRQ
            IQueryable<CoursePrefix> subjects;
            subjects = (from prefix in _DbContext.CoursePrefixes
                        join c in _DbContext.Sections on prefix.CoursePrefixID equals c.CourseID.Substring(0, 5)    // left 5 characters
                        where c.YearQuarterID.CompareTo(CurrentYearQuarter.ID) >= 0
                        select new CoursePrefix
                        {
                            _Subject = prefix.CoursePrefixID.Replace(ccChar, ""),   // ignore Common Course denominator at this level
                            Title = prefix.Title
                        }
                       );

            return subjects.Distinct().OrderBy(p => p.Title).ToList();
        }

        /// <summary>
        /// Returns filtered list of all Course Subjects by the first character in Title
        /// </summary>
        /// <param name="firstChar">The first character in Subject to search by</param>
        /// <remarks>
        /// This list is filtered to only include <see cref="Course"/> subjects which only have active courses
        /// in the current and/or future <see cref="YearQuarter"/>s.
        /// </remarks>
        /// <returns>Filtered list of CourseSubjects by the supplied char</returns>
        public IList<CoursePrefix> GetCourseSubjects(char firstChar)
        {
            string character = firstChar.ToString(); // Hack for LINQ 'where' that only accepts string
            string ccChar = Settings.RegexPatterns.CommonCourseChar;

            IQueryable<CoursePrefix> subjects;

            subjects = (from prefix in _DbContext.CoursePrefixes
                        join c in _DbContext.Sections on prefix.CoursePrefixID equals c.CourseID.Substring(0, 5) // left 5 characters
                        where c.YearQuarterID.CompareTo(CurrentYearQuarter.ID) >= 0 && prefix.CoursePrefixID.StartsWith(character)
                        select new CoursePrefix
                        {
                            _Subject = prefix.CoursePrefixID.Replace(ccChar, ""),   // ignore Common Course denominator at this level
                            Title = prefix.Title
                        }
                       );

            return subjects.Distinct().OrderBy(p => p.Title).ToList();
        }

        ///	<summary>
        /// Returns a list of Course Subjects which have active <see cref="Section"/>s for
        /// the <see cref="YearQuarter"/> specified
        ///	</summary>
        ///	<param name="yrq"></param>
        ///<param name="facetOptions"></param>
        ///<returns></returns>
        ///	<exception cref="NotImplementedException"></exception>
        public IList<CoursePrefix> GetCourseSubjects(YearQuarter yrq, IList<ISectionFacet> facetOptions = null)
        {
            string ccChar = Settings.RegexPatterns.CommonCourseChar;

            SectionFilters filters = new SectionFilters(this);
            filters.Add(s => s.YearQuarterID == yrq.ID);

            if (facetOptions != null && facetOptions.Count > 0)
            {
                filters.Add(facetOptions);
            }

            IQueryable<CoursePrefix> subjects = _DbContext.CoursePrefixes.Join(_DbContext.Sections.CompoundWhere(filters.FilterArray),
                                                                               p => p.CoursePrefixID,
                                                                               s => s.CourseID.Substring(0, 5),
                                                                               (p, s) => new { p, s })
                    .Where(h => h.s.YearQuarterID == yrq.ID)
                    .Select(h => new CoursePrefix
                    {
                        _Subject = h.p.CoursePrefixID.Replace(ccChar, ""),  // ignore Common Course denominator at this level
                        Title = h.p.Title
                    }
                    );
            return subjects.Distinct().OrderBy(p => p.Title).ToList();
        }

        #endregion

        /// <summary>
        /// Retrieves the number of <see cref="Section"/>s the specified <see cref="Course"/> in the given <see cref="YearQuarter"/>
        /// </summary>
        /// <param name="courseID"></param>
        /// <param name="yrq"></param>
        /// <param name="facets"></param>
        /// <returns></returns>
        public int SectionCountForCourse(ICourseID courseID, YearQuarter yrq, IList<ISectionFacet> facets = null)
        {
            SectionFilters filters = new SectionFilters(this);

            string yrqID = yrq.ID;
            filters.Add(s => s.YearQuarterID == yrqID);

            if (facets != null)
            {
                filters.Add(facets);
            }

            string subject = courseID.IsCommonCourse ? string.Concat(courseID.Subject, Settings.RegexPatterns.CommonCourseChar) : courseID.Subject;

            filters.Add(s => s.CourseID.Substring(0, 5).Trim().ToUpper() == subject.ToUpper() && s.CourseID.EndsWith(courseID.Number));

            int sectionCount = _DbContext.Sections.CompoundWhere(filters.FilterArray).Count();

            return sectionCount;
        }

        /// <summary>
        /// Retrieves the number of <see cref="Section"/>s the specified <see cref="Course"/> subject in the given <see cref="YearQuarter"/>
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="yrq"></param>
        /// <param name="facets"></param>
        /// <returns></returns>
        public int SectionCountForCourse(string subject, YearQuarter yrq, IList<ISectionFacet> facets = null)
        {
            SectionFilters filters = new SectionFilters(this);

            string yrqID = yrq.ID;
            filters.Add(s => s.YearQuarterID == yrqID);

            if (facets != null)
            {
                filters.Add(facets);
            }

            filters.Add(s => s.CourseID.Substring(0, 5).Trim().ToUpper() == subject.ToUpper());

            int sectionCount = _DbContext.Sections.CompoundWhere(filters.FilterArray).Count();

            return sectionCount;
        }

        #region Implementation of Dispose
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_context != null)
                {
                    if (_appContext != null)
                    {
                        // The calling app is most likely caching a _context object, so don't dispose it
                        // (even if we're wrong, nulling the variable should trigger garbage collection)
                        _context = null;
                    }
                    else
                    {
                        _context.Dispose();
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// Uses the configured <see cref="ICourseDescriptionStrategy"/> to determine the description(s) for the specified course
        /// </summary>
        /// <param name="courseID">The ID of the course we want description(s) for</param>
        /// <param name="yrq">
        ///		The <see cref="YearQuarter"/> we want the description for. This will be the first <see cref="CourseDescription"/> in the
        ///		returned list. Any additional <see cref="CourseDescription"/>s in the list will be planned future updates.</param>
        /// <param name="descriptionCollections">
        ///		The collections of <see cref="CourseDescription"/> objects to use in our logic. At this time, the business logic expects
        ///		two (2) collections to be passed.
        /// </param>
        /// <returns>A list of <see cref="CourseDescription"/>s, with the currently active one at index 0, and in order of implementation.</returns>
        /// <exception cref="ApplicationException">
        ///		The configured <see cref="Customizations.Types.CourseDescriptionStrategy"/> was unable to be loaded.
        /// </exception>
        static internal IList<CourseDescription> GetCourseDescriptions(ICourseID courseID, YearQuarter yrq, params IEnumerable<CourseDescriptionEntityBase>[] descriptionCollections)
        {
            string assemblyName = Utility.AssemblyName;
            string descriptionStrategyTypeName = Customizations.Types.CourseDescriptionStrategy;

            try
            {
                ICourseDescriptionStrategy descriptionStrategy;
                descriptionStrategy = AppDomain.CurrentDomain.CreateInstanceAndUnwrap(assemblyName,
                                                                                      descriptionStrategyTypeName,  // the fully-qualified name of the class to instantiate
                                                                                      true, // ignore case
                                                                                      BindingFlags.Default,
                                                                                      null, // use default Binder
                                                                                      descriptionCollections,   // arguments passed to the constructor
                                                                                      null, // use default CultureInfo
                                                                                      null  // no client ActivationAttributes
                                            ) as ICourseDescriptionStrategy;

                IList<CourseDescription> courseDescriptions;
                if (descriptionStrategy != null)
                {
                    courseDescriptions = descriptionStrategy.GetDescriptions(courseID, yrq);
                }
                else
                {
                    ICourseDescriptionStrategy defaultStrategy = new DefaultCourseDescriptionStrategy(descriptionCollections[0], descriptionCollections[1]);
                    courseDescriptions = defaultStrategy.GetDescriptions(courseID, yrq);
                }

                return courseDescriptions;
            }
            catch (TypeLoadException ex)
            {
                throw new ApplicationException(String.Format("Failed to load custom CourseDescriptionStrategy '{0}' from '{1}'", descriptionStrategyTypeName, assemblyName), ex);
            }
        }

        /// <summary>
        /// Encapsulates a collection of <see cref="SectionEntity"/> LINQ query expressions
        /// </summary>
        /// <remarks>
        /// This object is intended to be used within and by the members of <see cref="OdsRepository"/> class.
        /// </remarks>
        /// <seealso cref="OdsRepository"/>
        private class SectionFilters : List<Expression<Func<SectionEntity, bool>>>
        {
            readonly private OdsRepository _repository;

            /// <summary>
            /// Creates a new collection of <see cref="SectionEntity"/> query expressions
            /// </summary>
            /// <param name="repository">Provides access to the current <see cref="OdsRepository"/> instance.</param>
            /// <remarks>
            /// This constructor automtically adds various default expression filters that apply to all Section records retrieved.
            /// </remarks>
            public SectionFilters(OdsRepository repository)
            {
                _repository = repository;

                // --- Default filters that get applied to ALL Section retrievals ---
                // (retrieved from the .config file)
                foreach (ClassRule rule in _repository.Settings.ClassFlags.Rules)
                {
                    if (rule.Action == ClassRuleAction.Exclude)
                    {
                        string ruleValue = rule.Value;
                        switch (rule.Column.ToUpper())
                        {
                            case "SECTIONSTATUSID1":
                                // TODO: account for other types of comparisons (right now only "contains" is supported - 6/21/2011, shawn.south@bellevuecollege.edu)
                                Add(s => s.SectionStatusID1 == null || !(s.SectionStatusID1.ToUpper().Contains(ruleValue)));
                                break;
                            case "SECTIONSTATUSID2":
                                Add(s => s.SectionStatusID2 == null || !(s.SectionStatusID2.ToUpper().Contains(ruleValue)));
                                break;
                            case "SECTIONSTATUSID4":
                                Add(s => s.SectionStatusID4 == null || !(s.SectionStatusID4.ToUpper().Contains(ruleValue)));
                                break;
                        }
                    }
                }
            }

            /// <summary>
            /// Gets the collection of expression filters as an array
            /// </summary>
            /// <remarks>
            /// This value can then be passed to other methods which take a parameter array of expressions.
            ///		<example>
            ///			<code>
            /// SectionFilters filters = new SectionFilters();
            /// 
            /// filters.Add(s => s.YearQuarterID.CompareTo("B014") > 0);
            /// filters.Add(s => !String.IsNullOrWhitespace(s.DayID));
            /// 
            /// var sections = _context.Sections.CompoundWhere(filters.FilterArray);
            ///			</code>
            ///		</example>
            /// </remarks>
            /// <seealso cref="LinqExtensions.CompoundWhere{T}"/>
            public Expression<Func<SectionEntity, bool>>[] FilterArray
            {
                get { return this.ToArray(); }
            }

            /// <summary>
            /// Adds a collection of <see cref="ISectionFacet"/> objects' expressions
            /// </summary>
            /// <param name="facets"></param>
            public void Add(IEnumerable<ISectionFacet> facets)
            {
                if (facets != null && facets.Count() > 0)
                {
                    foreach (ISectionFacet facet in facets)
                    {
                        Add(facet.GetFilter<SectionEntity>(_repository._DbContext));
                    }
                }
            }
        }
    }
}
