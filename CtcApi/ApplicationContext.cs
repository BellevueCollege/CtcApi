using System;
using System.DirectoryServices.AccountManagement;
using System.Security.Principal;

namespace CtcApi
{
	public class ApplicationContext
	{
		private string _baseDirectory;
	  private string _user;

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
    /// Gets the <see cref="WindowsIdentity.Name"/> or <see cref="UserPrincipal.UserPrincipalName"/> of the currently executing context.
    /// </summary>
	  public string CurrentUser
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
    public static string GetCurrentUser()
    {
      return GetCurrentUser(null);
    }

	  #region Private members
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
	}
}