using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Common.Logging;

namespace CtcApi.Extensions
{
	public static class ByteExtensions
	{
		private static ILog _log = LogManager.GetCurrentClassLogger();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="data"></param>
		/// <param name="filename"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public static bool ToFile(this byte[] data, string filename, int length)
		{
			try
			{
				if (File.Exists(filename))
				{
					File.Delete(filename);
				}
				using (FileStream fs = File.Create(filename, length, FileOptions.None))
				{
					fs.Write(data, 0, length);
					fs.Flush();
					fs.Close();

					return true;
				}
			}
			catch (Exception ex)
			{
				_log.Error(m => m("Unable to write file '{0}'...\n{1}", filename, ex));
			}
			return false;
		}
	}
}