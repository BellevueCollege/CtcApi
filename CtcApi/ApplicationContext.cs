using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CtcApi
{
	public class ApplicationContext
	{
		private string _baseDirectory;

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
	}
}