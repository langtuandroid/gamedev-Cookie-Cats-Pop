using System;
using System.Collections;

namespace TactileModules.Analytics.Tests
{
	[TactileAnalytics.EventAttribute("AnalyticsMockEvent", true)]
	public class EventMock
	{
		public EventMock()
		{
			this.Data = DateTime.Now.ToString();
		}

		public TactileAnalytics.RequiredParam<string> Data { get; set; }

		public Hashtable ToHashtable()
		{
			Type type = base.GetType();
			string value = "AnalyticsMockEvent";
			return new Hashtable
			{
				{
					"eventName",
					value
				},
				{
					"eventTimestamp",
					DateTime.Now.ToString()
				},
				{
					"userId",
					Guid.NewGuid().ToString()
				},
				{
					"sessionId",
					Guid.NewGuid().ToString()
				},
				{
					"platform",
					"Test"
				},
				{
					"versionName",
					"1.0.0"
				},
				{
					"versionCode",
					"999999999999"
				},
				{
					"data",
					this.Data.GetValue()
				}
			};
		}

		public string GetJSON()
		{
			return this.ToHashtable().toJson();
		}
	}
}
