using System;

namespace TactileModules.CrossPromotion.General.LimitedUrlCaching
{
	public interface ILimitedUrlCacherRetriever
	{
		ILimitedUrlCacher GetLimitedUrlCacher(string url);
	}
}
