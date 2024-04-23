using System;

namespace TactileModules.Analytics
{
	public enum ReturnCode
	{
		NoError,
		MissingBody = -1,
		InvalidAppId = -2,
		MissingHeaders = -3,
		InvalidSecretId = -4,
		InvalidSignature = -5,
		RetryLater = -1000,
		Overloaded = -1001,
		ClientConnectionError = -10000,
		UnexpectedResponse = -20000
	}
}
