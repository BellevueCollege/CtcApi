using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.UI;
using Common.Logging;
using CtcApi.Extensions;

// from http://blog.stevensanderson.com/2008/10/15/partial-output-caching-in-aspnet-mvc/
// hacked slightly to allow for 0 url parameter pages to be cached

namespace CtcApi.Web.Mvc
{
  /// <summary>
  /// 
  /// </summary>
	public class ActionOutputCacheAttribute : ActionFilterAttribute
	{
    private const int DEFAULT_CACHE_DURATION = 0;

    private readonly ILog _log = LogManager.GetCurrentClassLogger();
    // This hack is optional; 
    private static MethodInfo _switchWriterMethod = typeof(HttpResponse).GetMethod("SwitchWriter", BindingFlags.Instance | BindingFlags.NonPublic);
    private int _cacheDuration;
    private TextWriter _originalWriter;
    private string _cacheKey;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cacheSetting"></param>
    /// <param name="defaultCacheDuration"></param>
    public ActionOutputCacheAttribute(string cacheSetting = "DefaultCacheTime", int defaultCacheDuration = DEFAULT_CACHE_DURATION)
		{
			if (cacheSetting == "DefaultCacheTime")
			{
        _cacheDuration = defaultCacheDuration;
			}
			else
			{
        _cacheDuration = ConfigurationManager.AppSettings[cacheSetting].SafeToInt32(defaultCacheDuration);
			}
			
		}

    /// <summary>
    /// 
    /// </summary>
    /// <param name="filterContext"></param>
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			_cacheKey = ComputeCacheKey(filterContext);
			string cachedOutput = (string)filterContext.HttpContext.Cache[_cacheKey];
			if (cachedOutput != null)
				filterContext.Result = new ContentResult { Content = cachedOutput };
			else
				_originalWriter = (TextWriter)_switchWriterMethod.Invoke(HttpContext.Current.Response, new object[] { new HtmlTextWriter(new StringWriter()) });
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="filterContext"></param>
    public override void OnResultExecuted(ResultExecutedContext filterContext)
		{
			if (_originalWriter != null) // Must complete the caching
			{
				//need the catch/finally in place to prevent the app from halting when the caching
				// mechanism attempts to convert HttpWriter to HtmlTextWriter upon fresh page reads 
				try
				{
					HtmlTextWriter cacheWriter = (HtmlTextWriter)_switchWriterMethod.Invoke(HttpContext.Current.Response, new object[] { _originalWriter });
					string textWritten = ((StringWriter)cacheWriter.InnerWriter).ToString();
					filterContext.HttpContext.Response.Write(textWritten);

					filterContext.HttpContext.Cache.Add(_cacheKey, textWritten, null, DateTime.Now.AddSeconds(_cacheDuration), Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
				}
				catch(Exception ex)
				{
					_log.Info(m => m("Encountered an error while attempting to add to the Cache. This can most likely be safely ignored.\n{0}", ex));
				}
			} 
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="filterContext"></param>
		/// <returns></returns>
    private string ComputeCacheKey(ActionExecutingContext filterContext)
		{
			var keyBuilder = new StringBuilder();
			foreach (var pair in filterContext.RouteData.Values)
				keyBuilder.AppendFormat("rd{0}_{1}_", pair.Key.GetHashCode(), pair.Value.GetHashCode());
			foreach (var pair in filterContext.ActionParameters)
				keyBuilder.AppendFormat("ap{0}_{1}_", pair.Key.GetHashCode(), pair.Value == null ? 0 : pair.Value.GetHashCode()); 
			return keyBuilder.ToString();
		}
	}
}