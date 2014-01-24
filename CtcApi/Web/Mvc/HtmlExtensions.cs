using System;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace CtcApi.Web.Mvc
{
  public static class HtmlExtensions
  {

    #region Html.Include()
    
    const string BEGIN_INCLUDE_TAG = @"<!--#include";
    const string END_INCLUDE_TAG = @"-->";

    /// <summary>
    /// Provides server-side include functionality
    /// </summary>
    /// <param name="html"></param>
    /// <param name="fileAndPath"></param>
    /// <returns>The contents of the file(s) that are requested</returns>
    /// <remarks>
    /// MVC does not support server-side includes on its own, so this method provides that functionality. It will include
    /// the file specified - including any nested server-side includes that are present in that file (and so on, etc.)
    /// </remarks>
    public static IHtmlString Include(this HtmlHelper html, string fileAndPath)
    {

      string fileContents = ProcessIncludeFile(fileAndPath, html.ViewContext.HttpContext.Server);
      return html.Raw(fileContents);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fileAndPath"></param>
    /// <param name="server"></param>
    /// <returns></returns>
    private static string ProcessIncludeFile(string fileAndPath, HttpServerUtilityBase server)
    {
      string fileContents = File.ReadAllText(fileAndPath);

      int position = 0;
      while (position >= 0 && position < fileContents.Length)
      {
        position = fileContents.IndexOf(BEGIN_INCLUDE_TAG, position, StringComparison.Ordinal);
        if (position >= 0)
        {
          int endPosition = fileContents.IndexOf(END_INCLUDE_TAG, position + 1, StringComparison.Ordinal);
          string includeTag = fileContents.Substring(position, endPosition - position + END_INCLUDE_TAG.Length);

          string[] pair = includeTag.Remove(includeTag.Length - END_INCLUDE_TAG.Length).Remove(0, BEGIN_INCLUDE_TAG.Length).Trim().Split('=');
          if (pair.Length >= 2)
          {
            string attribute = pair[0].Trim();
            string file = pair[1].Trim(' ', '"');
            string nextFile;

            if (attribute.ToUpper() == "FILE")
            {
              string fileWithPath = Path.Combine(Path.GetDirectoryName(fileAndPath) ?? string.Empty, file);
              nextFile = ProcessIncludeFile(fileWithPath, server);
            }
            else	// VIRTUAL
            {
              nextFile = ProcessIncludeFile(server.MapPath(file), server);
            }

            if (!string.IsNullOrWhiteSpace(nextFile))
            {
              fileContents = fileContents.Replace(includeTag, nextFile);
            }
          }

          position = endPosition;
        }
      }

      return fileContents;
    }

    #endregion
  }
}
