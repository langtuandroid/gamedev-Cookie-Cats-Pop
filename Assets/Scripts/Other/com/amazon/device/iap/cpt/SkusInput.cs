using System;
using System.Collections.Generic;
using System.Linq;
using com.amazon.device.iap.cpt.json;

namespace com.amazon.device.iap.cpt
{
	public sealed class SkusInput : Jsonable
	{
		public List<string> Skus { get; set; }

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
						"skus",
						this.Skus
					}
				};
			}
			catch (ApplicationException inner)
			{
				throw new AmazonException("Error encountered while getting object dictionary", inner);
			}
			return result;
		}

		public static SkusInput CreateFromDictionary(Dictionary<string, object> jsonMap)
		{
			SkusInput result;
			try
			{
				if (jsonMap == null)
				{
					result = null;
				}
				else
				{
					SkusInput skusInput = new SkusInput();
					if (jsonMap.ContainsKey("skus"))
					{
						skusInput.Skus = (from element in (List<object>)jsonMap["skus"]
						select (string)element).ToList<string>();
					}
					result = skusInput;
				}
			}
			catch (ApplicationException inner)
			{
				throw new AmazonException("Error encountered while creating Object from dicionary", inner);
			}
			return result;
		}

		public static SkusInput CreateFromJson(string jsonMessage)
		{
			SkusInput result;
			try
			{
				Dictionary<string, object> jsonMap = Json.Deserialize(jsonMessage) as Dictionary<string, object>;
				Jsonable.CheckForErrors(jsonMap);
				result = SkusInput.CreateFromDictionary(jsonMap);
			}
			catch (ApplicationException inner)
			{
				throw new AmazonException("Error encountered while UnJsoning", inner);
			}
			return result;
		}

		public static Dictionary<string, SkusInput> MapFromJson(Dictionary<string, object> jsonMap)
		{
			Dictionary<string, SkusInput> dictionary = new Dictionary<string, SkusInput>();
			foreach (KeyValuePair<string, object> keyValuePair in jsonMap)
			{
				SkusInput value = SkusInput.CreateFromDictionary(keyValuePair.Value as Dictionary<string, object>);
				dictionary.Add(keyValuePair.Key, value);
			}
			return dictionary;
		}

		public static List<SkusInput> ListFromJson(List<object> array)
		{
			List<SkusInput> list = new List<SkusInput>();
			foreach (object obj in array)
			{
				list.Add(SkusInput.CreateFromDictionary(obj as Dictionary<string, object>));
			}
			return list;
		}
	}
}
