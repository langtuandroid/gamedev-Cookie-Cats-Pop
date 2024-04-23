using System;
using TactileModules.Analytics.EventVerification.Storage.Date;

namespace TactileModules.Analytics.EventVerification.Storage
{
	public class CacheControl : ICacheControl
	{
		public CacheControl(IStorageDate storageDate, int maxAgeInDays)
		{
			this.storageDate = storageDate;
		}

		public bool IsExpired(string storedDate)
		{
			string dateA = this.storageDate.Today();
			int num = StorageDate.DaysBetween(dateA, storedDate);
			return num > this.maxAgeInDays;
		}

		private int maxAgeInDays = 30;

		private readonly IStorageDate storageDate;
	}
}
