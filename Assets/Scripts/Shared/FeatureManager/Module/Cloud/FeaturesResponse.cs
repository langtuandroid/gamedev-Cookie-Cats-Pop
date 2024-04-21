using System;
using System.Collections;
using System.Collections.Generic;
using Cloud;
using TactileModules.FeatureManager.DataClasses;

namespace Shared.FeatureManager.Module.Cloud
{
	public class FeaturesResponse : Response
	{
		public List<FeatureData> UpcomingFeatures
		{
			get
			{
				return this.GetFeatures("upcoming");
			}
		}

		public List<FeatureData> AvailableFeatures
		{
			get
			{
				return this.GetFeatures("available");
			}
		}

		public List<FeatureData> UnavailableFeatures
		{
			get
			{
				return this.GetFeatures("unavailable");
			}
		}

		private List<FeatureData> GetFeatures(string key)
		{
			List<FeatureData> list = new List<FeatureData>();
			ArrayList arrayList = (ArrayList)base.data[key];
			IEnumerator enumerator = arrayList.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					Hashtable table = (Hashtable)obj;
					FeatureData item = JsonSerializer.HashtableToObject<FeatureData>(table);
					list.Add(item);
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			return list;
		}
	}
}
