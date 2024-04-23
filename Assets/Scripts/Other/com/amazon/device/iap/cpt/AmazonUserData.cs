using System;
using System.Collections.Generic;
using com.amazon.device.iap.cpt.json;

namespace com.amazon.device.iap.cpt
{
	public sealed class AmazonUserData : Jsonable
	{
		public string UserId { get; set; }

		public string Marketplace { get; set; }

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
						"userId",
						this.UserId
					},
					{
						"marketplace",
						this.Marketplace
					}
				};
			}
			catch (ApplicationException inner)
			{
				throw new AmazonException("Error encountered while getting object dictionary", inner);
			}
			return result;
		}

		public static AmazonUserData CreateFromDictionary(Dictionary<string, object> jsonMap)
		{
			AmazonUserData result;
			try
			{
				if (jsonMap == null)
				{
					result = null;
				}
				else
				{
					AmazonUserData amazonUserData = new AmazonUserData();
					if (jsonMap.ContainsKey("userId"))
					{
						amazonUserData.UserId = (string)jsonMap["userId"];
					}
					if (jsonMap.ContainsKey("marketplace"))
					{
						amazonUserData.Marketplace = (string)jsonMap["marketplace"];
					}
					result = amazonUserData;
				}
			}
			catch (ApplicationException inner)
			{
				throw new AmazonException("Error encountered while creating Object from dicionary", inner);
			}
			return result;
		}

		public static AmazonUserData CreateFromJson(string jsonMessage)
		{
			AmazonUserData result;
			try
			{
				Dictionary<string, object> jsonMap = Json.Deserialize(jsonMessage) as Dictionary<string, object>;
				Jsonable.CheckForErrors(jsonMap);
				result = AmazonUserData.CreateFromDictionary(jsonMap);
			}
			catch (ApplicationException inner)
			{
				throw new AmazonException("Error encountered while UnJsoning", inner);
			}
			return result;
		}

		public static Dictionary<string, AmazonUserData> MapFromJson(Dictionary<string, object> jsonMap)
		{
			Dictionary<string, AmazonUserData> dictionary = new Dictionary<string, AmazonUserData>();
			foreach (KeyValuePair<string, object> keyValuePair in jsonMap)
			{
				AmazonUserData value = AmazonUserData.CreateFromDictionary(keyValuePair.Value as Dictionary<string, object>);
				dictionary.Add(keyValuePair.Key, value);
			}
			return dictionary;
		}

		public static List<AmazonUserData> ListFromJson(List<object> array)
		{
			List<AmazonUserData> list = new List<AmazonUserData>();
			foreach (object obj in array)
			{
				list.Add(AmazonUserData.CreateFromDictionary(obj as Dictionary<string, object>));
			}
			return list;
		}
	}
}
