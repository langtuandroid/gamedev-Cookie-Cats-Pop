using System;
using System.Collections.Generic;
using System.Linq;

namespace TactileModules.UserSupport.WebViews
{
	public class WebViewLocalResources
	{
		public List<string> Images
		{
			set
			{
				this.images = value;
			}
		}

		public List<string> JavaScriptFiles
		{
			set
			{
				this.javascriptFiles = value;
			}
		}

		public List<string> HTMLFiles
		{
			set
			{
				this.htmlFiles = value;
			}
		}

		public List<string> GetResources()
		{
			return this.images.Concat(this.javascriptFiles).Concat(this.htmlFiles).ToList<string>();
		}

		private List<string> images = new List<string>();

		private List<string> javascriptFiles = new List<string>();

		private List<string> htmlFiles = new List<string>();
	}
}
