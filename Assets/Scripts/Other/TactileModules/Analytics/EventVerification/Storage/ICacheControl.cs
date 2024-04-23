using System;

namespace TactileModules.Analytics.EventVerification.Storage
{
	public interface ICacheControl
	{
		bool IsExpired(string storedDate);
	}
}
