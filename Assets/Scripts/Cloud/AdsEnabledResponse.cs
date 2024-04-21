using System;
using System.Collections;
using System.Collections.Generic;

namespace Cloud
{
	public class AdsEnabledResponse : Response
	{
		public List<string> InterstitialProviders
		{
			get
			{
				ArrayList array = (ArrayList)base.data["interstitials"];
				return JsonSerializer.ArrayListToGenericList<string>(array);
			}
		}

		public List<string> VideoProviders
		{
			get
			{
				ArrayList array = (ArrayList)base.data["videos"];
				return JsonSerializer.ArrayListToGenericList<string>(array);
			}
		}
	}
}
