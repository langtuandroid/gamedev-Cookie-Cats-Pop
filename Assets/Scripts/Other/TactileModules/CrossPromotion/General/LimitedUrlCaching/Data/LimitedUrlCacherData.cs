using System;
using System.Collections.Generic;

namespace TactileModules.CrossPromotion.General.LimitedUrlCaching.Data
{
	public class LimitedUrlCacherData
	{
		public LimitedUrlCacherData()
		{
			this.CacheTimeStamps = new Dictionary<string, DateTime>();
		}

		[JsonSerializable("CacheTimeStamps", typeof(DateTime))]
		public Dictionary<string, DateTime> CacheTimeStamps { get; set; }
	}
}
