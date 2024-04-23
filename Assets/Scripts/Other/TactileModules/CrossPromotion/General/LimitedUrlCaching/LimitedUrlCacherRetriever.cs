using System;

namespace TactileModules.CrossPromotion.General.LimitedUrlCaching
{
	public class LimitedUrlCacherRetriever : ILimitedUrlCacherRetriever
	{
		public LimitedUrlCacherRetriever(ILimitedUrlCacher textureCacher, ILimitedUrlCacher videoCacher)
		{
			this.textureCacher = textureCacher;
			this.videoCacher = videoCacher;
		}

		public ILimitedUrlCacher GetLimitedUrlCacher(string url)
		{
			if (this.IsVideo(url))
			{
				return this.videoCacher;
			}
			return this.textureCacher;
		}

		private bool IsVideo(string url)
		{
			return url.EndsWith(".mp4");
		}

		private readonly ILimitedUrlCacher textureCacher;

		private readonly ILimitedUrlCacher videoCacher;
	}
}
