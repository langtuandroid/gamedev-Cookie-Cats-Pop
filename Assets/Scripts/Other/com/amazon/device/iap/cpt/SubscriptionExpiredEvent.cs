using System;
using System.Collections.Generic;
using com.amazon.device.iap.cpt.json;

namespace com.amazon.device.iap.cpt
{
	public sealed class SubscriptionExpiredEvent : Jsonable
	{
		public string Sku { get; set; }

		public string ToJson()
		{
			string result;
			try
			{
				Dictionary<string, object> objectDictionary = this.GetObjectDictionary();
				result = Json.Serialize(objectDictionary);
			}
			catch (ApplicationException inner)
			{
				throw new AmazonException("Error encountered while Jsoning", inner);
			}
			return result;
		}

		public override Dictionary<string, object> GetObjectDictionary()
		{
			Dictionary<string, object> result;
			try
			{
				result = new Dictionary<string, object>
				{
					{
						"sku",
						this.Sku
					}
				};
			}
			catch (ApplicationException inner)
			{
				throw new AmazonException("Error encountered while getting object dictionary", inner);
			}
			return result;
		}

		public static SubscriptionExpiredEvent CreateFromDictionary(Dictionary<string, object> jsonMap)
		{
			SubscriptionExpiredEvent result;
			try
			{
				if (jsonMap == null)
				{
					result = null;
				}
				else
				{
					SubscriptionExpiredEvent subscriptionExpiredEvent = new SubscriptionExpiredEvent();
					if (jsonMap.ContainsKey("sku"))
					{
						subscriptionExpiredEvent.Sku = (string)jsonMap["sku"];
					}
					result = subscriptionExpiredEvent;
				}
			}
			catch (ApplicationException inner)
			{
				throw new AmazonException("Error encountered while creating Object from dicionary", inner);
			}
			return result;
		}

		public static SubscriptionExpiredEvent CreateFromJson(string jsonMessage)
		{
			SubscriptionExpiredEvent result;
			try
			{
				Dictionary<string, object> jsonMap = Json.Deserialize(jsonMessage) as Dictionary<string, object>;
				Jsonable.CheckForErrors(jsonMap);
				result = SubscriptionExpiredEvent.CreateFromDictionary(jsonMap);
			}
			catch (ApplicationException inner)
			{
				throw new AmazonException("Error encountered while UnJsoning", inner);
			}
			return result;
		}

		public static Dictionary<string, SubscriptionExpiredEvent> MapFromJson(Dictionary<string, object> jsonMap)
		{
			Dictionary<string, SubscriptionExpiredEvent> dictionary = new Dictionary<string, SubscriptionExpiredEvent>();
			foreach (KeyValuePair<string, object> keyValuePair in jsonMap)
			{
				SubscriptionExpiredEvent value = SubscriptionExpiredEvent.CreateFromDictionary(keyValuePair.Value as Dictionary<string, object>);
				dictionary.Add(keyValuePair.Key, value);
			}
			return dictionary;
		}

		public static List<SubscriptionExpiredEvent> ListFromJson(List<object> array)
		{
			List<SubscriptionExpiredEvent> list = new List<SubscriptionExpiredEvent>();
			foreach (object obj in array)
			{
				list.Add(SubscriptionExpiredEvent.CreateFromDictionary(obj as Dictionary<string, object>));
			}
			return list;
		}
	}
}
