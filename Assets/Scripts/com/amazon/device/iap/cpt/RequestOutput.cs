using System;
using System.Collections.Generic;
using com.amazon.device.iap.cpt.json;

namespace com.amazon.device.iap.cpt
{
	public sealed class RequestOutput : Jsonable
	{
		public string RequestId { get; set; }

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
					}
				};
			}
			catch (ApplicationException inner)
			{
				throw new AmazonException("Error encountered while getting object dictionary", inner);
			}
			return result;
		}

		public static RequestOutput CreateFromDictionary(Dictionary<string, object> jsonMap)
		{
			RequestOutput result;
			try
			{
				if (jsonMap == null)
				{
					result = null;
				}
				else
				{
					RequestOutput requestOutput = new RequestOutput();
					if (jsonMap.ContainsKey("requestId"))
					{
						requestOutput.RequestId = (string)jsonMap["requestId"];
					}
					result = requestOutput;
				}
			}
			catch (ApplicationException inner)
			{
				throw new AmazonException("Error encountered while creating Object from dicionary", inner);
			}
			return result;
		}

		public static RequestOutput CreateFromJson(string jsonMessage)
		{
			RequestOutput result;
			try
			{
				Dictionary<string, object> jsonMap = Json.Deserialize(jsonMessage) as Dictionary<string, object>;
				Jsonable.CheckForErrors(jsonMap);
				result = RequestOutput.CreateFromDictionary(jsonMap);
			}
			catch (ApplicationException inner)
			{
				throw new AmazonException("Error encountered while UnJsoning", inner);
			}
			return result;
		}

		public static Dictionary<string, RequestOutput> MapFromJson(Dictionary<string, object> jsonMap)
		{
			Dictionary<string, RequestOutput> dictionary = new Dictionary<string, RequestOutput>();
			foreach (KeyValuePair<string, object> keyValuePair in jsonMap)
			{
				RequestOutput value = RequestOutput.CreateFromDictionary(keyValuePair.Value as Dictionary<string, object>);
				dictionary.Add(keyValuePair.Key, value);
			}
			return dictionary;
		}

		public static List<RequestOutput> ListFromJson(List<object> array)
		{
			List<RequestOutput> list = new List<RequestOutput>();
			foreach (object obj in array)
			{
				list.Add(RequestOutput.CreateFromDictionary(obj as Dictionary<string, object>));
			}
			return list;
		}
	}
}
