using System;
using System.Collections.Generic;
using com.amazon.device.iap.cpt.json;

namespace com.amazon.device.iap.cpt
{
	public sealed class ResetInput : Jsonable
	{
		public bool Reset { get; set; }

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
						"reset",
						this.Reset
					}
				};
			}
			catch (ApplicationException inner)
			{
				throw new AmazonException("Error encountered while getting object dictionary", inner);
			}
			return result;
		}

		public static ResetInput CreateFromDictionary(Dictionary<string, object> jsonMap)
		{
			ResetInput result;
			try
			{
				if (jsonMap == null)
				{
					result = null;
				}
				else
				{
					ResetInput resetInput = new ResetInput();
					if (jsonMap.ContainsKey("reset"))
					{
						resetInput.Reset = (bool)jsonMap["reset"];
					}
					result = resetInput;
				}
			}
			catch (ApplicationException inner)
			{
				throw new AmazonException("Error encountered while creating Object from dicionary", inner);
			}
			return result;
		}

		public static ResetInput CreateFromJson(string jsonMessage)
		{
			ResetInput result;
			try
			{
				Dictionary<string, object> jsonMap = Json.Deserialize(jsonMessage) as Dictionary<string, object>;
				Jsonable.CheckForErrors(jsonMap);
				result = ResetInput.CreateFromDictionary(jsonMap);
			}
			catch (ApplicationException inner)
			{
				throw new AmazonException("Error encountered while UnJsoning", inner);
			}
			return result;
		}

		public static Dictionary<string, ResetInput> MapFromJson(Dictionary<string, object> jsonMap)
		{
			Dictionary<string, ResetInput> dictionary = new Dictionary<string, ResetInput>();
			foreach (KeyValuePair<string, object> keyValuePair in jsonMap)
			{
				ResetInput value = ResetInput.CreateFromDictionary(keyValuePair.Value as Dictionary<string, object>);
				dictionary.Add(keyValuePair.Key, value);
			}
			return dictionary;
		}

		public static List<ResetInput> ListFromJson(List<object> array)
		{
			List<ResetInput> list = new List<ResetInput>();
			foreach (object obj in array)
			{
				list.Add(ResetInput.CreateFromDictionary(obj as Dictionary<string, object>));
			}
			return list;
		}
	}
}
