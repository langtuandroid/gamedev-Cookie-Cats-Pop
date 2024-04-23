using System;
using System.Collections;

namespace TactileModules.Analytics.EventVerification.Uploading
{
	public interface IUploadRequest
	{
		string GetErrorMessage();

		IUploadResponse GetResponse();

		IEnumerator Run();
	}
}
