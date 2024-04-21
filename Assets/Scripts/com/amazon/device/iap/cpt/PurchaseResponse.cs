using System;
using System.Collections.Generic;
using com.amazon.device.iap.cpt.json;

namespace com.amazon.device.iap.cpt
{
	public sealed class PurchaseResponse : Jsonable
	{
		public string RequestId { get; set; }

		public AmazonUserData AmazonUserData { get; set; }

		public PurchaseReceipt PurchaseReceipt { get; set; }

		public string Status { get; set; }

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
						"requestId",
						this.RequestId
					},
					{
						"amazonUserData",
						(this.AmazonUserData == null) ? null : this.AmazonUserData.GetObjectDictionary()
					},
					{
						"purchaseReceipt",
						(this.PurchaseReceipt == null) ? null : this.PurchaseReceipt.GetObjectDictionary()
					},
					{
						"status",
						this.Status
					}
				};
			}
			catch (ApplicationException inner)
			{
				throw new AmazonException("Error encountered while getting object dictionary", inner);
			}
			return result;
		}

		public static PurchaseResponse CreateFromDictionary(Dictionary<string, object> jsonMap)
		{
			PurchaseResponse result;
			try
			{
				if (jsonMap == null)
				{
					result = null;
				}
				else
				{
					PurchaseResponse purchaseResponse = new PurchaseResponse();
					if (jsonMap.ContainsKey("requestId"))
					{
						purchaseResponse.RequestId = (string)jsonMap["requestId"];
					}
					if (jsonMap.ContainsKey("amazonUserData"))
					{
						purchaseResponse.AmazonUserData = AmazonUserData.CreateFromDictionary(jsonMap["amazonUserData"] as Dictionary<string, object>);
					}
					if (jsonMap.ContainsKey("purchaseReceipt"))
					{
						purchaseResponse.PurchaseReceipt = PurchaseReceipt.CreateFromDictionary(jsonMap["purchaseReceipt"] as Dictionary<string, object>);
					}
					if (jsonMap.ContainsKey("status"))
					{
						purchaseResponse.Status = (string)jsonMap["status"];
					}
					result = purchaseResponse;
				}
			}
			catch (ApplicationException inner)
			{
				throw new AmazonException("Error encountered while creating Object from dicionary", inner);
			}
			return result;
		}

		public static PurchaseResponse CreateFromJson(string jsonMessage)
		{
			PurchaseResponse result;
			try
			{
				Dictionary<string, object> jsonMap = Json.Deserialize(jsonMessage) as Dictionary<string, object>;
				Jsonable.CheckForErrors(jsonMap);
				result = PurchaseResponse.CreateFromDictionary(jsonMap);
			}
			catch (ApplicationException inner)
			{
				throw new AmazonException("Error encountered while UnJsoning", inner);
			}
			return result;
		}

		public static Dictionary<string, PurchaseResponse> MapFromJson(Dictionary<string, object> jsonMap)
		{
			Dictionary<string, PurchaseResponse> dictionary = new Dictionary<string, PurchaseResponse>();
			foreach (KeyValuePair<string, object> keyValuePair in jsonMap)
			{
				PurchaseResponse value = PurchaseResponse.CreateFromDictionary(keyValuePair.Value as Dictionary<string, object>);
				dictionary.Add(keyValuePair.Key, value);
			}
			return dictionary;
		}

		public static List<PurchaseResponse> ListFromJson(List<object> array)
		{
			List<PurchaseResponse> list = new List<PurchaseResponse>();
			foreach (object obj in array)
			{
				list.Add(PurchaseResponse.CreateFromDictionary(obj as Dictionary<string, object>));
			}
			return list;
		}
	}
}
