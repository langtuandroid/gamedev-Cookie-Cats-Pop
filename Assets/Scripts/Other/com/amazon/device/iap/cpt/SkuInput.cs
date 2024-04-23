using System;
using System.Collections.Generic;
using com.amazon.device.iap.cpt.json;

namespace com.amazon.device.iap.cpt
{
	public sealed class SkuInput : Jsonable
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

		public static SkuInput CreateFromDictionary(Dictionary<string, object> jsonMap)
		{
			SkuInput result;
			try
			{
				if (jsonMap == null)
				{
					result = null;
				}
				else
				{
					SkuInput skuInput = new SkuInput();
					if (jsonMap.ContainsKey("sku"))
					{
						skuInput.Sku = (string)jsonMap["sku"];
					}
					result = skuInput;
				}
			}
			catch (ApplicationException inner)
			{
				throw new AmazonException("Error encountered while creating Object from dicionary", inner);
			}
			return result;
		}

		public static SkuInput CreateFromJson(string jsonMessage)
		{
			SkuInput result;
			try
			{
				Dictionary<string, object> jsonMap = Json.Deserialize(jsonMessage) as Dictionary<string, object>;
				Jsonable.CheckForErrors(jsonMap);
				result = SkuInput.CreateFromDictionary(jsonMap);
			}
			catch (ApplicationException inner)
			{
				throw new AmazonException("Error encountered while UnJsoning", inner);
			}
			return result;
		}

		public static Dictionary<string, SkuInput> MapFromJson(Dictionary<string, object> jsonMap)
		{
			Dictionary<string, SkuInput> dictionary = new Dictionary<string, SkuInput>();
			foreach (KeyValuePair<string, object> keyValuePair in jsonMap)
			{
				SkuInput value = SkuInput.CreateFromDictionary(keyValuePair.Value as Dictionary<string, object>);
				dictionary.Add(keyValuePair.Key, value);
			}
			return dictionary;
		}

		public static List<SkuInput> ListFromJson(List<object> array)
		{
			List<SkuInput> list = new List<SkuInput>();
			foreach (object obj in array)
			{
				list.Add(SkuInput.CreateFromDictionary(obj as Dictionary<string, object>));
			}
			return list;
		}
	}
}
