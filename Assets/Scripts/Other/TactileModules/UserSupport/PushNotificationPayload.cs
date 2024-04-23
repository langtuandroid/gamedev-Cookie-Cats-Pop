using System;
using System.Collections.Generic;

namespace TactileModules.UserSupport
{
	public class PushNotificationPayload
	{
		public string Event { get; set; }

		public int UnreadMessages { get; set; }

		public static PushNotificationPayload DictionaryToPayload(Dictionary<string, object> payloadDictionary)
		{
			PushNotificationPayload pushNotificationPayload = new PushNotificationPayload();
			try
			{
				pushNotificationPayload.Event = (string)payloadDictionary["event"];
				pushNotificationPayload.UnreadMessages = Convert.ToInt32(payloadDictionary["unreadMessages"]);
			}
			catch (Exception ex)
			{
				throw new PushNotificationPayloadParseException("Could not parse payload, fields could not be cast correctly.");
			}
			return pushNotificationPayload;
		}

		private const string EVENT_FIELD = "event";

		private const string UNREAD_MESSAGES_FIELD = "unreadMessages";
	}
}
