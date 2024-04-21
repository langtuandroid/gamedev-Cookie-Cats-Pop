using System;
using System.Collections.Generic;
using com.amazon.device.iap.cpt.json;

namespace com.amazon.device.iap.cpt
{
	public sealed class GetPurchaseUpdatesResponse : Jsonable
	{
		public string RequestId { get; set; }

		public AmazonUserData AmazonUserData { get; set; }

		public List<PurchaseReceipt> Receipts { get; set; }

		public string Status { get; set; }

		public bool HasMore { get; set; }

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
						"receipts",
						(this.Receipts == null) ? null : Jsonable.unrollObjectIntoList<PurchaseReceipt>(this.Receipts)
					},
					{
						"status",
						this.Status
					},
					{
						"hasMore",
						this.HasMore
					}
				};
			}
			catch (ApplicationException inner)
			{
				throw new AmazonException("Error encountered while getting object dictionary", inner);
			}
			return result;
		}

		public static GetPurchaseUpdatesResponse CreateFromDictionary(Dictionary<string, object> jsonMap)
		{
			GetPurchaseUpdatesResponse result;
			try
			{
				if (jsonMap == null)
				{
					result = null;
				}
				else
				{
					GetPurchaseUpdatesResponse getPurchaseUpdatesResponse = new GetPurchaseUpdatesResponse();
					if (jsonMap.ContainsKey("requestId"))
					{
						getPurchaseUpdatesResponse.RequestId = (string)jsonMap["requestId"];
					}
					if (jsonMap.ContainsKey("amazonUserData"))
					{
						getPurchaseUpdatesResponse.AmazonUserData = AmazonUserData.CreateFromDictionary(jsonMap["amazonUserData"] as Dictionary<string, object>);
					}
					if (jsonMap.ContainsKey("receipts"))
					{
						getPurchaseUpdatesResponse.Receipts = PurchaseReceipt.ListFromJson(jsonMap["receipts"] as List<object>);
					}
					if (jsonMap.ContainsKey("status"))
					{
						getPurchaseUpdatesResponse.Status = (string)jsonMap["status"];
					}
					if (jsonMap.ContainsKey("hasMore"))
					{
						getPurchaseUpdatesResponse.HasMore = (bool)jsonMap["hasMore"];
					}
					result = getPurchaseUpdatesResponse;
				}
			}
			catch (ApplicationException inner)
			{
				throw new AmazonException("Error encountered while creating Object from dicionary", inner);
			}
			return result;
		}

		public static GetPurchaseUpdatesResponse CreateFromJson(string jsonMessage)
		{
			GetPurchaseUpdatesResponse result;
			try
			{
				Dictionary<string, object> jsonMap = Json.Deserialize(jsonMessage) as Dictionary<string, object>;
				Jsonable.CheckForErrors(jsonMap);
				result = GetPurchaseUpdatesResponse.CreateFromDictionary(jsonMap);
			}
			catch (ApplicationException inner)
			{
				throw new AmazonException("Error encountered while UnJsoning", inner);
			}
			return result;
		}

		public static Dictionary<string, GetPurchaseUpdatesResponse> MapFromJson(Dictionary<string, object> jsonMap)
		{
			Dictionary<string, GetPurchaseUpdatesResponse> dictionary = new Dictionary<string, GetPurchaseUpdatesResponse>();
			foreach (KeyValuePair<string, object> keyValuePair in jsonMap)
			{
				GetPurchaseUpdatesResponse value = GetPurchaseUpdatesResponse.CreateFromDictionary(keyValuePair.Value as Dictionary<string, object>);
				dictionary.Add(keyValuePair.Key, value);
			}
			return dictionary;
		}

		public static List<GetPurchaseUpdatesResponse> ListFromJson(List<object> array)
		{
			List<GetPurchaseUpdatesResponse> list = new List<GetPurchaseUpdatesResponse>();
			foreach (object obj in array)
			{
				list.Add(GetPurchaseUpdatesResponse.CreateFromDictionary(obj as Dictionary<string, object>));
			}
			return list;
		}
	}
}
