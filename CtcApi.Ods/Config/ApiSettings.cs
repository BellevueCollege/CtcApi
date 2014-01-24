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
using System.Xml.Serialization;
using Ctc.Ods.Types;

namespace Ctc.Ods.Config
{
	/// <summary>
	/// Encapsulates a custom configuration block in the application's .config
	/// </summary>
	/// <remarks>
	/// <para>
	///		<see cref="ApiSettings"/> provides access to a custom configuration node which allows the consuming
	///		application to define configuration settings appropriate to their environment.
	/// </para>
	/// <para>
	///		To use this class; the following must be configured in the .config file for the executing context:
	///		<code lang="xml">
	///			<configuration>
	///			
	///				<configSections>
	///					<section name="ctcOdsApiSettings" type="Ctc.Ods.Config.SettingsConfigHandler, CtcOdsApi" />
	///				</configSections>
	///				
	///				<ctcOdsApiSettings>
	///					<regex CommonCourseChar="&amp;"/>
	///					<yearQuarter minValue="0000" maxValue="Z999" registrationLeadDays="14" cache="30"/>
	///					<waitlist status="W-LISTED"/>
	///					<classFlags online="3" hybrid="8" telecourse="1">
	///						<rule action="exclude" column="SectionStatusID1" position="contains" value="X"/>
	///						<rule action="exclude" column="SectionStatusID1" position="contains" value="Z"/>
	///						<rule action="exclude" column="SectionStatusID2" position="contains" value="A"/>
	///						<rule action="exclude" column="SectionStatusID4" position="contains" value="M"/>
	///						<rule action="exclude" column="SectionStatusID4" position="contains" value="N"/>
	///					</classFlags>
	///				</ctcOdsApiSettings>
	///				
	///			</configuration>
	///		</code>
	/// </para>
	/// <para>
	///		And; then loaded with the following line of code:
	///		<code lang="c#">
	///			ApiSettings settings = (ApiSettings)ConfigurationManager.GetSection(ApiSettings.SectionName);
	///		</code>
	/// </para>
	/// </remarks>
	[XmlType("ctcOdsApiSettings")]
	public class ApiSettings
	{
		/// <summary>
		/// Gets the name of the XML configuration node for this class
		/// </summary>
		/// <exception cref="InvalidOperationException">
		///		Unable to identify the XML <see cref="XmlTypeAttribute.TypeName"/> for this class.
		/// </exception>
		static public string SectionName
		{
			get
			{
				System.Reflection.MemberInfo info = typeof(ApiSettings);
				object[] attributes = info.GetCustomAttributes(typeof(XmlTypeAttribute), false);

				if (attributes != null && attributes.Length > 0 && attributes[0] != null)
				{
// ReSharper disable PossibleNullReferenceException
					return (attributes[0] as XmlTypeAttribute).TypeName;
// ReSharper restore PossibleNullReferenceException
				}
				
				throw new InvalidOperationException("Unable to find an XmlTypeAttribute for ApiSettings");
			}
		}

		/// <summary>
		/// Encapsulates a collection of one or more <see cref="ClassRule"/>s
		/// </summary>
		/// <seealso cref="ClassFlags"/>
		[XmlElement(ElementName = "classFlags")]
		public ClassFlags ClassFlags { get;set;}

		///<summary>
		/// Provides <see cref="ApiSettings"/> related to <see cref="YearQuarter"/> values
		///</summary>
		[XmlElement(ElementName = "yearQuarter")]
		public YearQuarterNode YearQuarter{get;set;}

		/// <summary>
		/// Provides <see cref="ApiSettings"/> related to whether or not a <see cref="Section"/> has a <i>waitlist</i>
		/// </summary>
		/// <remarks>
		/// Once a <see cref="Section"/> has reached its maximum student enrollment for the quarter any additional
		/// students registering will be placed on the waitlist instead.
		/// </remarks>
		[XmlElement(ElementName = "waitlist")]
		public WaitlistNode Waitlist {get;set;}

		/// <summary>
		/// Contains regular expression <see cref="ApiSettings"/>
		/// </summary>
		/// <seealso cref="RegexSettings"/>
		[XmlElement(ElementName = "regex")]
		public RegexSettings RegexPatterns { get;set; }

    /// <summary>
    /// Provides rules for displaying a <see cref="Section"/>'s <i>Days</i> information
    /// </summary>
    /// <remarks>
    ///		<para>When no class days are specified for a section, this element
    ///		defines the default value that will be listed on the class schedule (ex: "Arranged").</para>
		///		<code lang="xml">
		///			<configuration>
		///			
		///				<configSections>
		///					<section name="ctcOdsApiSettings" type="Ctc.Ods.Config.SettingsConfigHandler, CtcOdsApi" />
		///				</configSections>
		///				
		///				<ctcOdsApiSettings>
		///					<sectionDaysDefault valueToReplace="ARRANGED" newValue="Arranged"/>
		///				</ctcOdsApiSettings>
		///				
		///			</configuration>
		///		</code>
    /// </remarks>
    [XmlElement(ElementName = "sectionDaysDefault")]
    public DefaultSectionDaysNode SectionDaysDefault { get;set; }
	}

	///<summary>
	/// Provides <see cref="ApiSettings"/> related to <see cref="YearQuarter"/> values
	///</summary>
	[XmlType("yearQuarter")]
	public class YearQuarterNode
	{
		///<summary>
		/// The lowest allowed <see cref="YearQuarter.ID"/> value
		///</summary>
		/// <remarks>
		///		<para>This is not expected to represent a usable <see cref="YearQuarter"/> value, but rather
		///		the smallest value that is programmatically recognized.</para>
		///		<code lang="xml">
		///			<configuration>
		///			
		///				<configSections>
		///					<section name="ctcOdsApiSettings" type="Ctc.Ods.Config.SettingsConfigHandler, CtcOdsApi" />
		///				</configSections>
		///				
		///				<ctcOdsApiSettings>
		///					<yearQuarter minValue="0000"/>
		///				</ctcOdsApiSettings>
		///				
		///			</configuration>
		///		</code>
		/// </remarks>
		/// <seealso cref="YearQuarterNode">&gt;yearQuarter&lt;</seealso>
		/// <seealso cref="Max">&gt;maxValue&lt;</seealso>
		[XmlAttribute("minValue")]
		public string Min{get;set;}

		///<summary>
		/// The largest allowed <see cref="YearQuarter.ID"/> value
		///</summary>
		/// <remarks>
		///		<para>This is not expected to represent a usable <see cref="YearQuarter"/> value, but rather
		///		the largest value that is programmatically recognized.</para>
		///		<code lang="xml">
		///			<configuration>
		///			
		///				<configSections>
		///					<section name="ctcOdsApiSettings" type="Ctc.Ods.Config.SettingsConfigHandler, CtcOdsApi" />
		///				</configSections>
		///				
		///				<ctcOdsApiSettings>
		///					<yearQuarter maxValue="Z999"/>
		///				</ctcOdsApiSettings>
		///				
		///			</configuration>
		///		</code>
		/// </remarks>
		/// <seealso cref="YearQuarterNode">&gt;yearQuarter&lt;</seealso>
		/// <seealso cref="Min">&gt;minValue&lt;</seealso>
		[XmlAttribute("maxValue")]
		public string Max{get;set;}

		/// <summary>
		/// Days in advance of registration to start showing quarter information
		/// </summary>
		/// <remarks>
		///		<para>
		///		The setting below indicates the "current registration quarter" should begin 14 days before
		///		the opening of registration. This allows users to access data before registration actually
		///		opens.
		///		</para>
		///		<code lang="xml">
		///			<configuration>
		///			
		///				<configSections>
		///					<section name="ctcOdsApiSettings" type="Ctc.Ods.Config.SettingsConfigHandler, CtcOdsApi" />
		///				</configSections>
		///				
		///				<ctcOdsApiSettings>
		///					<yearQuarter registrationLeadDays="14"/>
		///				</ctcOdsApiSettings>
		///				
		///			</configuration>
		///		</code>
		/// </remarks>
		/// <seealso cref="YearQuarterNode">&gt;yearQuarter&lt;</seealso>
		[XmlAttribute("registrationLeadDays")]
		public int RegistrationLeadDays{get; set;}


		/// <summary>
		/// How long to cache <see cref="YearQuarter"/> data, in minutes
		/// </summary>
		/// <remarks>
		///		<para>
		///		Data from both the <i>YearQuarter</i> and <i>WebRegistrationSetting</i> table will be cached
		///		for this many minutes.
		///		</para>
		///		<note type="important">This caching only applies to queries for <i>only</i> data from these tables.
		///		It is not cached when this data is joined with other data.
		///		</note>
		///		<para>The following configuration example will cache YRQ information for one (1) hour:</para>
		///		<code lang="xml">
		///			<configuration>
		///			
		///				<configSections>
		///					<section name="ctcOdsApiSettings" type="Ctc.Ods.Config.SettingsConfigHandler, CtcOdsApi" />
		///				</configSections>
		///				
		///				<ctcOdsApiSettings>
		///					<yearQuarter cache="60"/>
		///				</ctcOdsApiSettings>
		///				
		///			</configuration>
		///		</code>
		/// </remarks>
		/// <seealso cref="YearQuarterNode">&gt;yearQuarter&lt;</seealso>
		[XmlAttribute("cache")]
		public uint Cache{get;set;}
	}

	/// <summary>
	/// Encapsulates a collection of one or more <see cref="ClassRule"/>s
	/// </summary>
	/// <remarks>
	///		<code lang="xml">
	///			<configuration>
	///			
	///				<configSections>
	///					<section name="ctcOdsApiSettings" type="Ctc.Ods.Config.SettingsConfigHandler, CtcOdsApi" />
	///				</configSections>
	///				
	///				<ctcOdsApiSettings>
	///					<classFlags online="3">
	///						<rule action="exclude" column="SectionStatusID1" position="contains" value="X"/>
	///					</classFlags>
	///				</ctcOdsApiSettings>
	///				
	///			</configuration>
	///		</code>
	/// </remarks>
	[XmlType("classFlags")]
	public class ClassFlags
	{
		/// <summary>
		/// A list of <see cref="ClassRule"/>s, which define which class records to include/exclude
		/// </summary>
		[XmlElement(ElementName = "rule")]
		public List<ClassRule> Rules{get;set;}

		/// <summary>
		/// Value in the <b>ODS</b> which identifies an Online <see cref="ISection"/>
		/// </summary>
		/// <remarks>
		/// The code looks for this value in the <i>SBCTCMisc1</i> column of the <i>vw_Class</i> view.
		/// </remarks>
		[XmlAttribute("online")]
		public string Online{get;set;}

		/// <summary>
		/// Value in the <b>ODS</b> which identifies a Hybrid <see cref="ISection"/>
		/// </summary>
		/// <remarks>
		/// The code looks for this value in the <i>SBCTCMisc1</i> column of the <i>vw_Class</i> view.
		/// </remarks>
		[XmlAttribute("hybrid")]
		public string Hybrid{get;set;}

		/// <summary>
		/// Value in the <b>ODS</b> which identifies a Telecourse <see cref="ISection"/>
		/// </summary>
		/// <remarks>
		/// The code looks for this value in the <i>SBCTCMisc1</i> column of the <i>vw_Class</i> view.
		/// </remarks>
		[XmlAttribute("telecourse")]
		public string Telecourse{get;set;}

    /// <summary>
    /// Value in the <b>ODS</b> which identifies a Continuous Enrollment <see cref="ISection"/>
    /// </summary>
    /// <remarks>
    /// The code looks for this value in the <i>ContinuousSequentialIndicator</i> column of the <i>vw_Class</i> view.
    /// </remarks>
    [XmlAttribute("continuousEnrollment")]
    public string ContinuousEnrollment { get; set; }

    /// <summary>
    /// Value in the <b>ODS</b> which identifies a Continuous Enrollment <see cref="ISection"/>
    /// </summary>
    /// <remarks>
    /// The code looks for this value in the <i>ContinuousSequentialIndicator</i> column of the <i>vw_Class</i> view.
    /// </remarks>
    [XmlAttribute("lateStartDaysCount")]
    public ushort LateStartDaysCount { get; set; }
	}

	/// <summary>
	/// Represents a filter rule for retrieving class data from ODS
	/// </summary>
	/// <remarks>
	///		<code lang="xml">
	///			<configuration>
	///			
	///				<configSections>
	///					<section name="ctcOdsApiSettings" type="Ctc.Ods.Config.SettingsConfigHandler, CtcOdsApi" />
	///				</configSections>
	///				
	///				<ctcOdsApiSettings>
	///					<classFlags>
	///						<rule action="exclude" column="SectionStatusID1" position="contains" value="X"/>
	///					</classFlags>
	///				</ctcOdsApiSettings>
	///				
	///			</configuration>
	///		</code>
	/// </remarks>
	/// <seealso cref="ClassFlags"/>
	/// <seealso cref="ApiSettings"/>
	[XmlType("rule")]
	public class ClassRule
	{
		/// <summary>
		/// Defines whether to include or exclude class data that matches the associated rule
		/// </summary>
		/// <seealso cref="ClassRuleAction"/>
		/// <seealso cref="ClassFlags"/>
		/// <seealso cref="ApiSettings"/>
		[XmlAttribute("action")]
		public ClassRuleAction Action{get;set;}

		/// <summary>
		/// The name of the column to query in the vw_Class table
		/// </summary>
		/// <seealso cref="ClassFlags"/>
		/// <seealso cref="ApiSettings"/>
		[XmlAttribute("column")]
		public string Column{get;set;}

		/// <summary>
		/// Where to look for the specified <see cref="Value"/> in the specified <see cref="Column"/>
		/// </summary>
		/// <seealso cref="ClassRulePosition"/>
		/// <seealso cref="ClassFlags"/>
		/// <seealso cref="ApiSettings"/>
		[XmlAttribute("position")]
		public ClassRulePosition Position{get;set;}

		/// <summary>
		/// The value to look in the specified <see cref="Column"/> for
		/// </summary>
		/// <seealso cref="ClassFlags"/>
		/// <seealso cref="ApiSettings"/>
		[XmlAttribute("value")]
		public string Value{get;set;}
	}

	///<summary>
	///</summary>
	[XmlType("waitlist")]
	public class WaitlistNode
	{
		///<summary>
		///</summary>
		[XmlAttribute("status")]
		public string Status{get;set;}
	}

	/// <summary>
	/// Contains regular expression <see cref="ApiSettings"/>
	/// </summary>
	/// <remarks>
	///		<code lang="xml">
	///			<configuration>
	///			
	///				<configSections>
	///					<section name="ctcOdsApiSettings" type="Ctc.Ods.Config.SettingsConfigHandler, CtcOdsApi" />
	///				</configSections>
	///				
	///				<ctcOdsApiSettings>
	///					<regex CommonCourseChar="&amp;"/>
	///				</ctcOdsApiSettings>
	///				
	///			</configuration>
	///		</code>
	/// </remarks>
	/// <seealso cref="ApiSettings"/>
	[XmlType("regex")]
	public class RegexSettings
	{
		/// <summary>
		/// The character which identifies a <see cref="Course"/> as being the same across schools
		/// </summary>
		[XmlAttribute("CommonCourseChar")]
		public string CommonCourseChar {get;set;}
	}

    /// <summary>
    /// Used to find a particular value that is used as a default in ODS, and replace it with
    /// a new value. Sections with no scheduled days might default to "ARRANGED," but in many
    /// situations you might want to replace this value with something more application specific
    /// </summary>
    [XmlType("sectionDaysDefault")]
    public class DefaultSectionDaysNode
    {
        /// <summary>
        /// The old "default" in the ODS to find
        /// </summary>
        /// <remarks>
				///		<code lang="xml">
				///			<configuration>
				///			
				///				<configSections>
				///					<section name="ctcOdsApiSettings" type="Ctc.Ods.Config.SettingsConfigHandler, CtcOdsApi" />
				///				</configSections>
				///				
				///				<ctcOdsApiSettings>
				///					<sectionDaysDefault valueToReplace="ARRANGED" newValue="Arranged"/>
				///				</ctcOdsApiSettings>
				///				
				///			</configuration>
				///		</code>
        /// </remarks>
        /// <seealso cref="NewValue">&gt;newValue&lt;</seealso>
        [XmlAttribute("valueToReplace")]
        public string ValueToFind { get; set; }

        /// <summary>
        /// The new default value
        /// </summary>
        /// <remarks>
				///		<code lang="xml">
				///			<configuration>
				///			
				///				<configSections>
				///					<section name="ctcOdsApiSettings" type="Ctc.Ods.Config.SettingsConfigHandler, CtcOdsApi" />
				///				</configSections>
				///				
				///				<ctcOdsApiSettings>
				///					<sectionDaysDefault valueToReplace="ARRANGED" newValue="Arranged"/>
				///				</ctcOdsApiSettings>
				///				
				///			</configuration>
				///		</code>
        /// </remarks>
        /// <seealso cref="ValueToFind">&gt;valueToReplace&lt;</seealso>
        [XmlAttribute("newValue")]
        public string NewValue { get; set; }
    }

    /// <summary>
    /// Sets up the definition of a "late start" section.
    /// </summary>
    [XmlType("lateStartDefinition")]
    public class LateStartDefinitionNode
    {
        /// <summary>
        /// Defines how many days after the start of a quarter a class needs to start in order to be called a "Late Start" class.
        /// </summary>
				/// <remarks>
				///		<code lang="xml">
				///			<configuration>
				///			
				///				<configSections>
				///					<section name="ctcOdsApiSettings" type="Ctc.Ods.Config.SettingsConfigHandler, CtcOdsApi" />
				///				</configSections>
				///				
				///				<ctcOdsApiSettings>
				///					<lateStartDefinition daysCount="20"/>
				///				</ctcOdsApiSettings>
				///				
				///			</configuration>
				///		</code>
				/// </remarks>
        [XmlAttribute("daysCount")]
        public ushort LateStartDays { get; set; }
    }

	#region Enumerations
	/// <summary>
	/// Defines whether to include or exclude class data that matches the associated rule
	/// </summary>
	/// <seealso cref="ClassRule"/>
	/// <seealso cref="ClassFlags"/>
	/// <seealso cref="ApiSettings"/>
	public enum ClassRuleAction
	{
		/// <summary>
		/// Include class data that matches this rule
		/// </summary>
		[XmlEnum("include")]
		Include = 0x0,

		/// <summary>
		/// Exclude class data that matches this rule
		/// </summary>
		[XmlEnum("exclude")]
		Exclude = 0x2
	}

	/// <summary>
	/// Where to look for the specified <see cref="ClassRule.Value"/> in the specified <see cref="ClassRule.Column"/>
	/// </summary>
	/// <seealso cref="ClassRule"/>
	/// <seealso cref="ClassFlags"/>
	/// <seealso cref="ApiSettings"/>
	public enum ClassRulePosition
	{
		/// <summary>
		/// The <see cref="ClassRule.Value"/> can be anywhere in the specified <see cref="ClassRule.Column"/>
		/// </summary>
		[XmlEnum("contains")]
		Contains = 0x0
	}

	#endregion
}
