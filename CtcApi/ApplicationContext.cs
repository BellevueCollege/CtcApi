using System;
using System.Configuration;
using System.DirectoryServices.AccountManagement;
using System.Security.Principal;

namespace CtcApi
{
	public class ApplicationContext
	{
	  private const string APPSETTINGS_KEY_CURRENT_DATE = "CurrentDate";

	  private string _baseDirectory;
	  private string _user;
	  private DateTime? _currentDate;

	  /// <summary>
    /// Gets or sets the value to use for <see cref="AppDomain.BaseDirectory"/>
    /// </summary>
    /// <remarks>
    /// The primary use of this property is to override the <see cref="AppDomain.BaseDirectory"/> in situations where the code is
    /// being executed in an <see cref="AppDomain"/> that is not its real-world context. The most common scenario is when running
    /// unit tests.
    /// </remarks>
		public string BaseDirectory
		{
			get
			{
				if (string.IsNullOrWhiteSpace(_baseDirectory))
				{
					_baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
				}
				return _baseDirectory;
			}
			set
			{
				_baseDirectory = value;
			}
		}

    /// <summary>
    /// Gets or sets the "current" date
    /// </summary>
    /// <remarks>
    /// <para>
    ///   This property allows applications to override what the CtcApi believes the current
    ///   date to be. Simply <c>set</c> the property to the date you want before making API calls.
    /// </para>
    /// <para>
    ///   The <c>CurrentDate</c> can be modified in two ways:
    /// </para>
    /// <list type="table">
    ///   <item>
    ///     <term>Programmatically</term>
    ///     <description>
    ///       <example>
    ///         Simply set the this property to the <see cref="DateTime"/> you wish it to be:
    ///         <code>
    ///           ApplicationContext context = new ApplicationContext();
    ///           context.CurrentDate = DateTime.Parse("10/7/2013");
    ///         </code>
    ///       </example>
    ///       <note type="note">
    ///         Setting the <c>CurrentDate</c> programmatically overrides the .config file setting.
    ///       </note>
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <term>In the .config file</term>
    ///     <description>
    ///       <example>
    ///         Specify the a date for the <i>CurrentDate</i> <c>appSettings</c>:
    ///         <code>
    ///           <appSettings>
    ///             <add key="CurrentDate" value="10/7/2013"/>
    ///           </appSettings>
    ///         </code>
    ///       </example>
    ///       <note type="warning">
    ///         If this value is not a valid <see cref="DateTime"/> it will be ignored.
    ///       </note>
    ///     </description>
    ///   </item>
    /// </list>
    /// </remarks>
	  public DateTime? CurrentDate
	  {
	    get
	    {
	      if (!_currentDate.HasValue)
	      {
          _currentDate = GetCurrentDate();
	      }
	      return _currentDate;
	    }
      set
      {
        _currentDate = value.HasValue ? value : GetCurrentDate();
      }
	  }

	  #region Private members

    /// <summary>
    /// Retrieves "current" date from the .config file.
    /// </summary>
    /// <returns>
    ///   The <see cref="DateTime"/> value of the <i>CurrentDate</i> setting if valid,
    ///   otherwise <see cref="DateTime.Now"/>.
    /// </returns>
    private DateTime GetCurrentDate()
    {
      string setting = ConfigurationManager.AppSettings[APPSETTINGS_KEY_CURRENT_DATE];

      DateTime currentDate;
      if (!string.IsNullOrWhiteSpace(setting))
      {
        DateTime dt;
        currentDate = DateTime.TryParse(setting, out dt) ? dt : DateTime.Now;
      }
      else
      {
        currentDate = DateTime.Now;
      }
      return currentDate;
    }

    #region UNDER DEVELOPMENT
	  /// <summary>
	  /// Gets the <see cref="WindowsIdentity.Name"/> or <see cref="UserPrincipal.UserPrincipalName"/> of the currently executing context.
	  /// </summary>
	  private string CurrentUser
	  {
	    get
	    {
	      _user = GetCurrentUser(_user);
	      return _user;
	    }
	  }

	  /// <summary>
	  /// Gets the <see cref="WindowsIdentity.Name"/> or <see cref="UserPrincipal.UserPrincipalName"/> of the currently executing context.
	  /// </summary>
	  /// <returns>
	  /// The <i>username</i> or <i>login</i> of the currently running account.
	  /// </returns>
	  private static string GetCurrentUser()
	  {
	    return GetCurrentUser(null);
	  }

	  /// <summary>
	  /// Gets the <see cref="WindowsIdentity.Name"/> or <see cref="UserPrincipal.UserPrincipalName"/> of the currently executing context.
	  /// </summary>
	  /// <returns>
	  /// The <i>username</i> or <i>login</i> of the currently running account.
	  /// </returns>
	  private static string GetCurrentUser(string user)
	  {
	    const string UNKNOWN = "(UNKNOWN)";

	    if (string.IsNullOrWhiteSpace(user) || UNKNOWN == user)
	    {
	      WindowsIdentity wi = WindowsIdentity.GetCurrent();

	      if (wi != null)
	      {
	        user = wi.Name;
	      }
	      else
	      {
	        // If we don't find a WindowsIdentity, try Active Directory
	        try
	        {
	          user = UserPrincipal.Current.UserPrincipalName;

	          if (string.IsNullOrWhiteSpace(user))
	          {
	            user = UNKNOWN;
	          }
	        }
	        catch (NoMatchingPrincipalException)
	        {
	          // Assuming this Exception simply means the Current principal could not be identified.
	          user = UNKNOWN;
	        }
	      }
	    }

	    return user;
	  }

	  #endregion

	  #endregion
	}
}