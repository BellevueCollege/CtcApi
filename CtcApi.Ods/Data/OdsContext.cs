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
using System.Data.Entity;
using System.Diagnostics;
using Ctc.Ods.Extensions;
using Ctc.Ods.Types;

namespace Ctc.Ods.Data
{
	internal class OdsContext : DbContext
	{
		// NOTE: Entity Framework (or at least this syntax) only works with classes - not interfaces
		public DbSet<CourseEntity> Courses { get; set; }
		public DbSet<SectionEntity> Sections { get; set; }
		public DbSet<CourseDescription1Entity> CourseDescriptions1 { get; set; }
		public DbSet<CourseDescription2Entity> CourseDescriptions2 { get; set; }
		public DbSet<Footnote> Footnote { get; set; }
		public DbSet<CoursePrefixEntity> CoursePrefixes { get; set; }
		/// <summary>
		/// 
		/// </summary>
		/// <remarks>NOTE: This data is cached via <see cref="QueryResultCacheExtension"/></remarks>
		public DbSet<DayEntity> Days { get; set; }
		/// <summary>
		/// 
		/// </summary>
		/// <remarks>NOTE: This data is cached via <see cref="QueryResultCacheExtension"/></remarks>
		public DbSet<YearQuarterEntity> YearQuarters{get;set;}
		public DbSet<InstructionEntity> InstructionDetails{get;set;}
		public DbSet<WebRegistrationSettingEntity> WebRegistrationSettings{get;set;}
		public DbSet<WaitlistEntity> WaitListCounts{get;set;}
		public DbSet<InstructorEntity> Instructors{get;set;}
		public DbSet<EmployeeEntity> Employees{get;set;}

		public OdsContext()
		{
			Debug.Print("==> instantiating OdsContext()...");
		}

		/// <summary>
		/// This method is called when the model for a derived context has been initialized, but
		///             before the model has been locked down and used to initialize the context.  The default
		///             implementation of this method does nothing, but it can be overridden in a derived class
		///             such that the model can be further configured before it is locked down.
		/// </summary>
		/// <remarks>
		/// Typically, this method is called only once when the first instance of a derived context
		///             is created.  The model for that context is then cached and is for all further instances of
		///             the context in the app domain.  This caching can be disabled by setting the ModelCaching
		///             property on the given ModelBuidler, but note that this can seriously degrade performance.
		///             More control over caching is provided through use of the DbModelBuilder and DbContextFactory
		///             classes directly.
		/// </remarks>
		/// <param name="modelBuilder">The builder that defines the model for the context being created.</param>
		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			DefineCourseMappings(modelBuilder);
			DefineFootnoteMappings(modelBuilder);
			DefineCourseDescriptionMappings(modelBuilder);
		}

		/// <summary>
		/// Define database views and keys for the CourseDescription objects
		/// </summary>
		/// <param name="modelBuilder"></param>
		/// <seealso cref="CourseDescription1Entity"/>
		/// <seealso cref="CourseDescription2Entity"/>
		/// <seealso cref="CourseDescriptionEntityBase"/>
		private void DefineCourseDescriptionMappings(DbModelBuilder modelBuilder)
		{
			/*****************************************************************************************
			 * These inheritence relationships were created following the examples in this blog post:
			 * http://weblogs.asp.net/manavi/archive/2011/01/03/inheritance-mapping-strategies-with-entity-framework-code-first-ctp5-part-3-table-per-concrete-type-tpc-and-choosing-strategy-guidelines.aspx
			 *****************************************************************************************/
			modelBuilder.Entity<CourseDescription2Entity>().Map(m => {
			                                                    	m.MapInheritedProperties();
			                                                    	m.ToTable("vw_CourseDescription2");
			                                                    });

			modelBuilder.Entity<CourseDescription1Entity>().Map(m => {
			                                                    	m.MapInheritedProperties();
			                                                    	m.ToTable("vw_CourseDescription");
			                                                    });
		}

		/// <summary>
		/// Define entity mappings for the <see cref="Course"/> class
		/// </summary>
		/// <param name="modelBuilder">The builder that defines the model for the context being created.</param>
		private void DefineCourseMappings(DbModelBuilder modelBuilder)
		{
			// identify the view to map to
			modelBuilder.Entity<CourseEntity>().ToTable("vw_Course");
			modelBuilder.Entity<CourseEntity>().HasKey(c => c.UniqueId);

			modelBuilder.Entity<CourseEntity>().Property(c => c.CourseID).IsRequired();

			// Map columns that have a different name from their associated properties
			modelBuilder.Entity<CourseEntity>().Property(c => c.UniqueId).HasColumnName("CourseKey");
			modelBuilder.Entity<CourseEntity>().Property(c => c.Title1).HasColumnName("CourseTitle");
			modelBuilder.Entity<CourseEntity>().Property(c => c.Title2).HasColumnName("CourseTitle2");
			modelBuilder.Entity<CourseEntity>().Property(c => c.YearQuarterBegin).HasColumnName("EffectiveYearQuarterBegin");
			modelBuilder.Entity<CourseEntity>().Property(c => c.YearQuarterEnd).HasColumnName("EffectiveYearQuarterEnd");
		}


		/// <summary>
		/// Define entity mappings for the <see cref="Footnote"/> class
		/// </summary>
		/// <param name="modelBuilder"></param>
		private void DefineFootnoteMappings(DbModelBuilder modelBuilder)
		{
			// identify the view to map to
			modelBuilder.Entity<Footnote>().ToTable("vw_Footnote");
			modelBuilder.Entity<Footnote>().HasKey(c => c.FootnoteId);

			// Map columns that have a diff name from their assoc. properties
			// TODO: Should we rename all Key names to "UniqueId" to clear confusion for API users? Is there meta we can add otherwise?
			//modelBuilder.Entity<Footnote>().Property(c => c.UniqueID).HasColumnName("FootnoteID");
		}
	}
}
