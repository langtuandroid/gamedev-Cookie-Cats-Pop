using System;

namespace Cloud
{
	public enum ReturnCode
	{
		NoErrorAlreadyLatestVersion = 1,
		NoError = 0,
		UnexpectedError = -1,
		UnexpectedMongoDbError = -2,
		InvalidDeviceInContext = -3,
		InvalidUserInContext = -4,
		MissingDeviceInContext = -5,
		MissingUserInContext = -6,
		InvalidOrMissingDataInBody = -7,
		NotLatestVersion = -8,
		ValidationError = -9,
		AuthenticationFailed = -10,
		AuthenticationFailedInvalidNonce = -11,
		AuthenticatedDataNotAllowed = -12,
		InvalidFacebookAccessToken = -13,
		ExternalConnectionError = -14,
		TimedOut = -15,
		NotFound = -404,
		DevelopmentNotImplemented = -1000,
		ClientConnectionError = -10000
	}
}
