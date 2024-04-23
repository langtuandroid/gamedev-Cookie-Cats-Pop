using System;
using System.Collections.Generic;
using com.amazon.device.iap.cpt.json;

namespace com.amazon.device.iap.cpt
{
	public sealed class NotifyFulfillmentInput : Jsonable
	{
		public string ReceiptId { get; set; }

		public string FulfillmentResult { get; set; }

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
						"receiptId",
						this.ReceiptId
					},
					{
						"fulfillmentResult",
						this.FulfillmentResult
					}
				};
			}
			catch (ApplicationException inner)
			{
				throw new AmazonException("Error encountered while getting object dictionary", inner);
			}
			return result;
		}

		public static NotifyFulfillmentInput CreateFromDictionary(Dictionary<string, object> jsonMap)
		{
			NotifyFulfillmentInput result;
			try
			{
				if (jsonMap == null)
				{
					result = null;
				}
				else
				{
					NotifyFulfillmentInput notifyFulfillmentInput = new NotifyFulfillmentInput();
					if (jsonMap.ContainsKey("receiptId"))
					{
						notifyFulfillmentInput.ReceiptId = (string)jsonMap["receiptId"];
					}
					if (jsonMap.ContainsKey("fulfillmentResult"))
					{
						notifyFulfillmentInput.FulfillmentResult = (string)jsonMap["fulfillmentResult"];
					}
					result = notifyFulfillmentInput;
				}
			}
			catch (ApplicationException inner)
			{
				throw new AmazonException("Error encountered while creating Object from dicionary", inner);
			}
			return result;
		}

		public static NotifyFulfillmentInput CreateFromJson(string jsonMessage)
		{
			NotifyFulfillmentInput result;
			try
			{
				Dictionary<string, object> jsonMap = Json.Deserialize(jsonMessage) as Dictionary<string, object>;
				Jsonable.CheckForErrors(jsonMap);
				result = NotifyFulfillmentInput.CreateFromDictionary(jsonMap);
			}
			catch (ApplicationException inner)
			{
				throw new AmazonException("Error encountered while UnJsoning", inner);
			}
			return result;
		}

		public static Dictionary<string, NotifyFulfillmentInput> MapFromJson(Dictionary<string, object> jsonMap)
		{
			Dictionary<string, NotifyFulfillmentInput> dictionary = new Dictionary<string, NotifyFulfillmentInput>();
			foreach (KeyValuePair<string, object> keyValuePair in jsonMap)
			{
				NotifyFulfillmentInput value = NotifyFulfillmentInput.CreateFromDictionary(keyValuePair.Value as Dictionary<string, object>);
				dictionary.Add(keyValuePair.Key, value);
			}
			return dictionary;
		}

		public static List<NotifyFulfillmentInput> ListFromJson(List<object> array)
		{
			List<NotifyFulfillmentInput> list = new List<NotifyFulfillmentInput>();
			foreach (object obj in array)
			{
				list.Add(NotifyFulfillmentInput.CreateFromDictionary(obj as Dictionary<string, object>));
			}
			return list;
		}
	}
}
