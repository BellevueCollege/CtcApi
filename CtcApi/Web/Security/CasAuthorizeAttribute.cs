using System.Web;
using System.Web.Mvc;

namespace CtcApi.Web.Security
{
  /// <summary>
  /// Represents an attribute that is used to restrict access by callers to an action method.
  /// </summary>
  /// <remarks>
  ///   This class saves the <i>ReferralUrlForCas</i> in the <see cref="HttpSessionStateBase">Session State</see>.
  /// </remarks>
	public class CasAuthorizeAttribute : AuthorizeAttribute
	{
    /// <summary>
    /// Determines whether access to the core framework is authorized.
    /// </summary>
    /// <param name="httpContext"></param>
    /// <remarks>
    ///   This method has the side effect of saving the <i>ReferralUrlForCas</i> in the
    ///   <see cref="HttpSessionStateBase">Session State</see>.
    /// </remarks>
    /// <returns></returns>
		protected override bool AuthorizeCore(HttpContextBase httpContext)
		{
		  if (httpContext != null && httpContext.Request != null && httpContext.Request.UrlReferrer != null)
		  {
		    string url = httpContext.Request.UrlReferrer.ToString();
		    if (httpContext.Session != null)
		    {
		      httpContext.Session.Add("ReferralUrlForCas", url);
		    }
		  }

		  return (base.AuthorizeCore(httpContext));
		}
	}
}