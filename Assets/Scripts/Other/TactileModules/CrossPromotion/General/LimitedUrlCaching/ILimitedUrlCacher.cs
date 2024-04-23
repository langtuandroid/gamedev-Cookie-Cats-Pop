using System;

namespace TactileModules.CrossPromotion.General.LimitedUrlCaching
{
	public interface ILimitedUrlCacher
	{
		void EnsureAssetIsCached(string url);

		bool IsCached(string url);

		string GetCachePath(string url);
	}
}
