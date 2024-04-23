using System;

namespace TactileModules.Analytics.EventVerification.Uploading
{
	public interface IUploadResponse
	{
		ReturnCode ResponseCode { get; }
	}
}
