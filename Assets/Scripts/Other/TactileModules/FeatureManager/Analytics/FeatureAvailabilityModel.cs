using System;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.TactilePrefs;

namespace TactileModules.FeatureManager.Analytics
{
	public class FeatureAvailabilityModel : IFeatureAvailabilityModel
	{
		public FeatureAvailabilityModel()
		{
			PlayerPrefsSignedString localStorageString = new PlayerPrefsSignedString("FeatureAvailabilityModel", "PersistedState");
			this.storageObject = new LocalStorageJSONObject<FeatureAvailabilityPersistableState>(localStorageString);
			this.persistableState = this.storageObject.Load();
		}

		private bool Exist(string featureInstanceId)
		{
			return this.persistableState.Availability.ContainsKey(featureInstanceId);
		}

		private void Remove(string featureInstanceId)
		{
			this.persistableState.Availability.Remove(featureInstanceId);
			this.Save();
		}

		private bool GetAvailabilityState(string featureInstanceId)
		{
			return this.persistableState.Availability.ContainsKey(featureInstanceId) && this.persistableState.Availability[featureInstanceId];
		}

		private void SetAvailabilityState(string featureInstanceId, bool available)
		{
			if (this.persistableState.Availability.ContainsKey(featureInstanceId))
			{
				this.persistableState.Availability[featureInstanceId] = available;
			}
			else
			{
				this.persistableState.Availability.Add(featureInstanceId, available);
			}
			this.Save();
		}

		private void Save()
		{
			this.storageObject.Save(this.persistableState);
		}

		public void UpdateFeatureAvailabilityAndLogAnalyticsEvent(FeatureData featureData, bool isAvailable, bool areFeatureAssetsAvailable)
		{
			string id = featureData.Id;
			if (this.HasAvailabilityChanged(id, isAvailable))
			{
				if (isAvailable)
				{
				}
				else
				{
					string reason = (!areFeatureAssetsAvailable) ? "assetsNotAvailable" : "other";
				}
				this.SetAvailabilityState(id, isAvailable);
			}
		}

		public void EnsureFeatureIsRemoved(string featureId)
		{
			if (this.Exist(featureId))
			{
				this.Remove(featureId);
			}
		}

		private bool HasAvailabilityChanged(string featureId, bool isAvailable)
		{
			bool flag = !this.Exist(featureId);
			if (flag)
			{
				return true;
			}
			bool availabilityState = this.GetAvailabilityState(featureId);
			return availabilityState != isAvailable;
		}

		private const string PERSISTABLE_STATE_KEY = "PersistedState";

		private const string PERSISTABLE_STATE_DOMAIN = "FeatureAvailabilityModel";

		private readonly LocalStorageJSONObject<FeatureAvailabilityPersistableState> storageObject;

		private readonly FeatureAvailabilityPersistableState persistableState;
	}
}
