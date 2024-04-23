using System;
using System.Collections;
using Cloud;
using UnityEngine;

namespace TactileModules.TactileCloud
{
	public interface ICloudResponse
	{
		Hashtable data { get; set; }

		DateTime utcReceived { get; set; }

		string Body { get; }

		bool Success { get; }

		ReturnCode ReturnCode { get; }

		string ErrorInfo { get; }

		bool PasswordInvalid { get; }

		bool IsNetworkError { get; }

		bool IsRecoverableError { get; }

		void FillResponse(WWW request);
	}
}
