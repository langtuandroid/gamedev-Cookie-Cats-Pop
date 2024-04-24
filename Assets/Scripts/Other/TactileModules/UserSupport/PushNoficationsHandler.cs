using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace TactileModules.UserSupport
{
	public class PushNoficationsHandler : IPushNotificationHandler
	{
		
		public event Action<PushNotificationPayload> NotificationReceived;
		
		private void OnNotificationReceived(OSNotification notification)
		{
			this.PrintNotification(notification.payload);
			PushNotificationPayload pushNotificationPayload = this.ParsePayload(notification);
			if (this.IsRelevant(pushNotificationPayload))
			{
				this.NotificationReceived(pushNotificationPayload);
			}
		}

		private PushNotificationPayload ParsePayload(OSNotification notification)
		{
			PushNotificationPayload result = new PushNotificationPayload();
			try
			{
				result = PushNotificationPayload.DictionaryToPayload(notification.payload.additionalData);
			}
			catch (PushNotificationPayloadParseException ex)
			{
			}
			return result;
		}

		protected bool IsRelevant(PushNotificationPayload payload)
		{
			return !string.IsNullOrEmpty(payload.Event) && payload.Event.Contains("newSupportMessage");
		}

		private void PrintNotification(OSNotificationPayload payload)
		{
		}

		private string PrintDictionary(Dictionary<string, object> dict)
		{
			Dictionary<string, object>.KeyCollection keys = dict.Keys;
			string text = string.Empty;
			foreach (string text2 in keys)
			{
				string text3 = text;
				text = string.Concat(new object[]
				{
					text3,
					text2,
					":",
					dict[text2],
					","
				});
			}
			return text;
		}

	}
}
