using System;
using System.Collections.Generic;
using System.Linq;
using com.amazon.device.iap.cpt.json;

namespace com.amazon.device.iap.cpt
{
	public sealed class GetProductDataResponse : Jsonable
	{
		public string RequestId { get; set; }

		public Dictionary<string, ProductData> ProductDataMap { get; set; }

		public List<string> UnavailableSkus { get; set; }

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
						"productDataMap",
						(this.ProductDataMap == null) ? null : Jsonable.unrollObjectIntoMap<ProductData>(this.ProductDataMap)
					},
					{
						"unavailableSkus",
						this.UnavailableSkus
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

		public static GetProductDataResponse CreateFromDictionary(Dictionary<string, object> jsonMap)
		{
			GetProductDataResponse result;
			try
			{
				if (jsonMap == null)
				{
					result = null;
				}
				else
				{
					GetProductDataResponse getProductDataResponse = new GetProductDataResponse();
					if (jsonMap.ContainsKey("requestId"))
					{
						getProductDataResponse.RequestId = (string)jsonMap["requestId"];
					}
					if (jsonMap.ContainsKey("productDataMap"))
					{
						getProductDataResponse.ProductDataMap = ProductData.MapFromJson(jsonMap["productDataMap"] as Dictionary<string, object>);
					}
					if (jsonMap.ContainsKey("unavailableSkus"))
					{
						getProductDataResponse.UnavailableSkus = (from element in (List<object>)jsonMap["unavailableSkus"]
						select (string)element).ToList<string>();
					}
					if (jsonMap.ContainsKey("status"))
					{
						getProductDataResponse.Status = (string)jsonMap["status"];
					}
					result = getProductDataResponse;
				}
			}
			catch (ApplicationException inner)
			{
				throw new AmazonException("Error encountered while creating Object from dicionary", inner);
			}
			return result;
		}

		public static GetProductDataResponse CreateFromJson(string jsonMessage)
		{
			GetProductDataResponse result;
			try
			{
				Dictionary<string, object> jsonMap = Json.Deserialize(jsonMessage) as Dictionary<string, object>;
				Jsonable.CheckForErrors(jsonMap);
				result = GetProductDataResponse.CreateFromDictionary(jsonMap);
			}
			catch (ApplicationException inner)
			{
				throw new AmazonException("Error encountered while UnJsoning", inner);
			}
			return result;
		}

		public static Dictionary<string, GetProductDataResponse> MapFromJson(Dictionary<string, object> jsonMap)
		{
			Dictionary<string, GetProductDataResponse> dictionary = new Dictionary<string, GetProductDataResponse>();
			foreach (KeyValuePair<string, object> keyValuePair in jsonMap)
			{
				GetProductDataResponse value = GetProductDataResponse.CreateFromDictionary(keyValuePair.Value as Dictionary<string, object>);
				dictionary.Add(keyValuePair.Key, value);
			}
			return dictionary;
		}

		public static List<GetProductDataResponse> ListFromJson(List<object> array)
		{
			List<GetProductDataResponse> list = new List<GetProductDataResponse>();
			foreach (object obj in array)
			{
				list.Add(GetProductDataResponse.CreateFromDictionary(obj as Dictionary<string, object>));
			}
			return list;
		}
	}
}
