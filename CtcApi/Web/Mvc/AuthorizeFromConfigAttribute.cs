using System.Configuration;
using System.Web;
using System.Web.Mvc;
using CtcApi.Web.Security;

namespace CtcApi.Web.Mvc
{
	/// <summary>
	/// 
	/// </summary>
	public class AuthorizeFromConfigAttribute : AuthorizeAttribute
	{
		private string[] _roles = new string[] { };

		/// <summary>
		/// 
		/// </summary>
		/// <param name="httpContext"></param>
		/// <returns></returns>
		protected override bool AuthorizeCore(HttpContextBase httpContext)
		{
			if (httpContext.User.Identity.IsAuthenticated)
			{
				return ActiveDirectoryRoleProvider.IsUserInRoles(httpContext.User, _roles);
			}
			return false;
		}

		/// <summary>
		/// 
		/// </summary>
		public string RoleKey
		{
			get { return string.Join(",", _roles); }
			set
			{
				string roles = ConfigurationManager.AppSettings[value];
				if (string.IsNullOrWhiteSpace(roles))
				{
					throw new ConfigurationErrorsException(string.Format("AppSetting '{0}' not found.", value));
				}
				Roles = roles;
				_roles = roles.Split(',');
			}
		}
	}
}