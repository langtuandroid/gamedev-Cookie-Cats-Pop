using System;
using System.Collections.Generic;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.TactilePrefs;

namespace TactileModules.FeatureManager.Analytics
{
	public class FeatureReceivedEventLoggingStateHandler : IFeatureReceivedEventLoggingStateHandler
	{
		public FeatureReceivedEventLoggingStateHandler(ILocalStorageObject<FeatureReceivedEventLoggingState> localStorageObject)
		{
			this.localStorageObject = localStorageObject;
			this.featureReceivedEventLoggingState = localStorageObject.Load();
		}

		public bool CanLogReceivedEvent(FeatureData featureData)
		{
			bool flag = this.featureReceivedEventLoggingState.LoggedFeatures.Contains(featureData.Id);
			if (flag)
			{
				return false;
			}
			this.featureReceivedEventLoggingState.LoggedFeatures.Add(featureData.Id);
			this.SaveStorage();
			return true;
		}

		public List<string> GetDisappearedFeatures(List<FeatureData> featureDatas)
		{
			List<string> list = new List<string>();
			using (List<string>.Enumerator enumerator = this.featureReceivedEventLoggingState.LoggedFeatures.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					string loggedFeature = enumerator.Current;
					if (featureDatas.Find((FeatureData data) => data.Id == loggedFeature) == null)
					{
						list.Add(loggedFeature);
					}
				}
			}
			foreach (string item in list)
			{
				this.featureReceivedEventLoggingState.LoggedFeatures.Remove(item);
			}
			return list;
		}

		private void SaveStorage()
		{
			this.EnsureStateIsWithinAllowedLimit();
			this.localStorageObject.Save(this.featureReceivedEventLoggingState);
		}

		private void EnsureStateIsWithinAllowedLimit()
		{
			while (this.featureReceivedEventLoggingState.LoggedFeatures.Count > 100)
			{
				this.featureReceivedEventLoggingState.LoggedFeatures.RemoveAt(0);
			}
		}

		private readonly ILocalStorageObject<FeatureReceivedEventLoggingState> localStorageObject;

		private readonly FeatureReceivedEventLoggingState featureReceivedEventLoggingState;
	}
}
