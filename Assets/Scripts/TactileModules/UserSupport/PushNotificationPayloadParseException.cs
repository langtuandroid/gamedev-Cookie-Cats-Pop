using System;

namespace TactileModules.UserSupport
{
	public class PushNotificationPayloadParseException : Exception
	{
		public PushNotificationPayloadParseException(string message) : base(message)
		{
		}
	}
}
