using System;
using System.Collections.Generic;
using com.amazon.device.iap.cpt.json;

namespace com.amazon.device.iap.cpt
{
	public sealed class PurchaseReceipt : Jsonable
	{
		public string ReceiptId { get; set; }

		public long CancelDate { get; set; }

		public long PurchaseDate { get; set; }

		public string Sku { get; set; }

		public string ProductType { get; set; }

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
						"cancelDate",
						this.CancelDate
					},
					{
						"purchaseDate",
						this.PurchaseDate
					},
					{
						"sku",
						this.Sku
					},
					{
						"productType",
						this.ProductType
					}
				};
			}
			catch (ApplicationException inner)
			{
				throw new AmazonException("Error encountered while getting object dictionary", inner);
			}
			return result;
		}

		public static PurchaseReceipt CreateFromDictionary(Dictionary<string, object> jsonMap)
		{
			PurchaseReceipt result;
			try
			{
				if (jsonMap == null)
				{
					result = null;
				}
				else
				{
					PurchaseReceipt purchaseReceipt = new PurchaseReceipt();
					if (jsonMap.ContainsKey("receiptId"))
					{
						purchaseReceipt.ReceiptId = (string)jsonMap["receiptId"];
					}
					if (jsonMap.ContainsKey("cancelDate"))
					{
						purchaseReceipt.CancelDate = (long)jsonMap["cancelDate"];
					}
					if (jsonMap.ContainsKey("purchaseDate"))
					{
						purchaseReceipt.PurchaseDate = (long)jsonMap["purchaseDate"];
					}
					if (jsonMap.ContainsKey("sku"))
					{
						purchaseReceipt.Sku = (string)jsonMap["sku"];
					}
					if (jsonMap.ContainsKey("productType"))
					{
						purchaseReceipt.ProductType = (string)jsonMap["productType"];
					}
					result = purchaseReceipt;
				}
			}
			catch (ApplicationException inner)
			{
				throw new AmazonException("Error encountered while creating Object from dicionary", inner);
			}
			return result;
		}

		public override string ToString()
		{
			return string.Format("<PurchaseReceipt> sku: {0} productType: {1} purchaseDate: {2} cancelDate: {3} receiptId: {4}", new object[]
			{
				this.Sku,
				this.ProductType,
				this.PurchaseDate,
				this.CancelDate,
				this.ReceiptId
			});
		}

		public static PurchaseReceipt CreateFromJson(string jsonMessage)
		{
			PurchaseReceipt result;
			try
			{
				Dictionary<string, object> jsonMap = Json.Deserialize(jsonMessage) as Dictionary<string, object>;
				Jsonable.CheckForErrors(jsonMap);
				result = PurchaseReceipt.CreateFromDictionary(jsonMap);
			}
			catch (ApplicationException inner)
			{
				throw new AmazonException("Error encountered while UnJsoning", inner);
			}
			return result;
		}

		public static Dictionary<string, PurchaseReceipt> MapFromJson(Dictionary<string, object> jsonMap)
		{
			Dictionary<string, PurchaseReceipt> dictionary = new Dictionary<string, PurchaseReceipt>();
			foreach (KeyValuePair<string, object> keyValuePair in jsonMap)
			{
				PurchaseReceipt value = PurchaseReceipt.CreateFromDictionary(keyValuePair.Value as Dictionary<string, object>);
				dictionary.Add(keyValuePair.Key, value);
			}
			return dictionary;
		}

		public static List<PurchaseReceipt> ListFromJson(List<object> array)
		{
			List<PurchaseReceipt> list = new List<PurchaseReceipt>();
			foreach (object obj in array)
			{
				list.Add(PurchaseReceipt.CreateFromDictionary(obj as Dictionary<string, object>));
			}
			return list;
		}
	}
}
