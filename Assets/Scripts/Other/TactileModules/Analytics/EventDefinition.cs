using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace TactileModules.Analytics
{
	public class EventDefinition
	{
		public EventDefinition()
		{
			this.EventParams = new List<EventParamDefinition>();
		}

		public List<EventParamDefinition> EventParams { get; set; }

		public string SchemaHash { get; set; }

		public TactileAnalytics.EventAttribute EventAttribute { get; set; }

		public void UpdateSchemaHash()
		{
			string eventSchemaHashBasis = this.GetEventSchemaHashBasis();
			MD5CryptoServiceProvider md5CryptoServiceProvider = new MD5CryptoServiceProvider();
			byte[] array = md5CryptoServiceProvider.ComputeHash(Encoding.UTF8.GetBytes(eventSchemaHashBasis));
			StringBuilder stringBuilder = new StringBuilder(array.Length * 2);
			foreach (byte b in array)
			{
				stringBuilder.Append(b.ToString("x2"));
			}
			this.SchemaHash = stringBuilder.ToString();
		}

		public string GetEventSchemaHashBasis()
		{
			return EventDefinition.CalculateEventSchemaHashBasis(this.EventAttribute.EventName, this.EventParams);
		}

		private static string CalculateEventSchemaHashBasis(string eventName, List<EventParamDefinition> eventParams)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(eventName);
			foreach (string value in TactileAnalytics.GetValidDefaultEventParamNames())
			{
				stringBuilder.Append(",");
				stringBuilder.Append(value);
			}
			foreach (EventParamDefinition eventParamDefinition in eventParams)
			{
				stringBuilder.Append(",");
				stringBuilder.Append(eventParamDefinition.MemberName.ToLower());
				stringBuilder.Append("=");
				stringBuilder.Append(eventParamDefinition.ParamName);
			}
			return stringBuilder.ToString();
		}
	}
}
