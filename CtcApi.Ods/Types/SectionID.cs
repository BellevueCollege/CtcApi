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
using System.Runtime.Serialization;

namespace Ctc.Ods.Types
{
  /// <summary>
  /// 
  /// </summary>
  [DataContract]
  public class SectionID : ISectionID, IEquatable<SectionID>, IEquatable<string>
  {
    /// <summary>
    /// Creates a new <see cref="SectionID"/> from an item # and YRQ
    /// </summary>
    /// <param name="itemNumber"></param>
    /// <param name="yrq"></param>
    public SectionID(string itemNumber, string yrq)
    {
      // validation checks
      if (itemNumber.Length != 4)
      {
        throw new ArgumentException("Must be positive number greater than zero (0).", "itemNumber");
      }
      if (yrq.Length != 4)
      {
        throw new FormatException(String.Format("'{0}' is not a valid YearQuarter ID.", yrq));
      }

      ItemNumber = itemNumber;
      YearQuarter = yrq;
    }

    /// <summary>
    /// Converts a string representation of a Section ID to an <see cref="ISectionID"/>
    /// </summary>
    /// <param name="sectionId"></param>
    /// <returns></returns>
    static public ISectionID FromString(string sectionId)
    {
      if (sectionId.Length == 8)
      {
        return new SectionID(sectionId.Substring(0, 4), sectionId.Substring(4));
      }

      throw new FormatException(String.Format("'{0}' is not a valid SectionID", sectionId));
    }

    /// <summary>
    /// A 4-digit number assigned by the <i>Student Management System (SMS)</i>
    /// </summary>
    /// <remarks>
    /// This number in combination with a <see cref="ISectionID.YearQuarter"/> should be a unique
    /// value which represents a specific class.
    /// </remarks>
    /// <seealso cref="ISectionID"/>
    [DataMember]
    public string ItemNumber { get; private set; }

    /// <summary>
    /// A 4-character <i>Year-Quarter (YRQ)</i>
    /// </summary>
    /// <seealso cref="ISectionID"/>
    [DataMember]
    public string YearQuarter { get; private set; }

    #region Overrides of Object
    /// <summary>
    /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
    /// </returns>
    /// <filterpriority>2</filterpriority>
    public override string ToString()
    {
      return String.Concat(ItemNumber, YearQuarter);
    }
    #endregion

    #region Equality members
    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(string other)
    {
      return ToString().Equals(other);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(ISectionID other)
    {
      return Equals(other as SectionID);
    }

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <returns>
    /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
    /// </returns>
    /// <param name="other">An object to compare with this object.</param>
    public bool Equals(SectionID other)
    {
      if (ReferenceEquals(null, other))
      {
        return false;
      }
      if (ReferenceEquals(this, other))
      {
        return true;
      }
      return Equals(other.ItemNumber, ItemNumber) && Equals(other.YearQuarter, YearQuarter);
    }

    /// <summary>
    /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
    /// </summary>
    /// <returns>
    /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
    /// </returns>
    /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>. </param><filterpriority>2</filterpriority>
    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj))
      {
        return false;
      }
      if (ReferenceEquals(this, obj))
      {
        return true;
      }
      Type typ = obj.GetType();
      if (typ != typeof(SectionID))
      {
        if (typ == typeof(string))
        {
          return Equals((string)obj);
        }
        return false;
      }
      return Equals((SectionID)obj);
    }

    /// <summary>
    /// Serves as a hash function for a particular type. 
    /// </summary>
    /// <returns>
    /// A hash code for the current <see cref="T:System.Object"/>.
    /// </returns>
    /// <filterpriority>2</filterpriority>
    public override int GetHashCode()
    {
      unchecked
      {
        return ((ItemNumber != null ? ItemNumber.GetHashCode() : 0) * 397) ^ (YearQuarter != null ? YearQuarter.GetHashCode() : 0);
      }
    }

    ///<summary>
    ///</summary>
    ///<param name="left"></param>
    ///<param name="right"></param>
    ///<returns></returns>
    public static bool operator ==(SectionID left, SectionID right)
    {
      return Equals(left, right);
    }

    ///<summary>
    ///</summary>
    ///<param name="left"></param>
    ///<param name="right"></param>
    ///<returns></returns>
    public static bool operator !=(SectionID left, SectionID right)
    {
      return !Equals(left, right);
    }

    #endregion
  }
}
