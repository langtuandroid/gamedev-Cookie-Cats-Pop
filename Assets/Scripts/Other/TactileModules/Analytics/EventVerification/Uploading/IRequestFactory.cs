using System;
using TactileModules.Analytics.EventVerification.Packaging;

namespace TactileModules.Analytics.EventVerification.Uploading
{
	public interface IRequestFactory
	{
		IUploadRequest CreateRequest(EventCountPackage package);

		void FailoverToNextCollector();
	}
}
