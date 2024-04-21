using System;
using System.Collections.Generic;
using System.Reflection;

namespace TactileModules.Analytics
{
	public class EventDefinitions
	{
		public EventDefinitions()
		{
			this.BuildEventDefinitions();
		}

		public EventDefinition GetEventDefinition(Type eventType)
		{
			return this.eventDefinitions[eventType];
		}

		private void BuildEventDefinitions()
		{
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				foreach (Type type in assembly.GetTypes())
				{
					if (type.GetCustomAttributes(typeof(TactileAnalytics.EventAttribute), true).Length > 0)
					{
						this.eventDefinitions[type] = this.GenerateEventDefinition(type);
					}
				}
			}
		}

		private static TactileAnalytics.EventAttribute GetEventAttribute(Type eventType)
		{
			object[] customAttributes = eventType.GetCustomAttributes(typeof(TactileAnalytics.EventAttribute), true);
			if (customAttributes.Length == 0)
			{
				throw new Exception("Missing EventAttribute on event object. EventType=" + eventType.ToString());
			}
			if (customAttributes.Length > 1)
			{
				throw new Exception("More than 1 EventAttribute on event object. EventType=" + eventType.ToString());
			}
			return (TactileAnalytics.EventAttribute)customAttributes[0];
		}

		private EventDefinition GenerateEventDefinition(Type eventType)
		{
			EventDefinition eventDefinition = new EventDefinition();
			eventDefinition.EventAttribute = EventDefinitions.GetEventAttribute(eventType);
			foreach (MemberInfo memberInfo in MemberInfoUtils.GetAllProperties(eventType))
			{
				if (MemberInfoUtils.IsGenericType(memberInfo))
				{
					Type genericTypeDefinition = MemberInfoUtils.GetGenericTypeDefinition(memberInfo);
					if (genericTypeDefinition == typeof(TactileAnalytics.RequiredParam<>) || genericTypeDefinition == typeof(TactileAnalytics.OptionalParam<>))
					{
						eventDefinition.EventParams.Add(new EventParamDefinition
						{
							MemberInfo = memberInfo,
							MemberName = memberInfo.Name,
							Required = (genericTypeDefinition == typeof(TactileAnalytics.RequiredParam<>)),
							ValueType = MemberInfoUtils.GetGenericArguments(memberInfo)[0]
						});
					}
				}
			}
			eventDefinition.EventParams.Sort((EventParamDefinition a, EventParamDefinition b) => string.Compare(a.MemberName, b.MemberName, true));
			EventDefinitions.EventParamNameGenerator eventParamNameGenerator = new EventDefinitions.EventParamNameGenerator();
			foreach (EventParamDefinition eventParamDefinition in eventDefinition.EventParams)
			{
				eventParamDefinition.ParamName = eventParamNameGenerator.GetParamName(eventParamDefinition);
				if (eventParamDefinition.ParamName == null)
				{
					throw new Exception(string.Concat(new string[]
					{
						"Invalid event parameter value type. memberName=",
						eventParamDefinition.MemberName,
						", valueType=",
						eventParamDefinition.ValueType.ToString(),
						", EventType=",
						eventType.ToString(),
						", EventName=",
						eventDefinition.EventAttribute.EventName
					}));
				}
				if (TactileAnalytics.GetValidEventParamNames().IndexOf(eventParamDefinition.ParamName) == -1)
				{
					throw new Exception(string.Concat(new string[]
					{
						"Invalid event parameter name generated. We will need to add more parameters of this type to the BigQuery database. memberName=",
						eventParamDefinition.MemberName,
						", paramName=",
						eventParamDefinition.ParamName,
						", valueType=",
						eventParamDefinition.ValueType.ToString(),
						", EventType=",
						eventType.ToString(),
						", EventName=",
						eventDefinition.EventAttribute.EventName
					}));
				}
			}
			eventDefinition.UpdateSchemaHash();
			return eventDefinition;
		}

		private Dictionary<Type, EventDefinition> eventDefinitions = new Dictionary<Type, EventDefinition>();

		private class EventParamNameGenerator
		{
			public string GetParamName(EventParamDefinition eventParam)
			{
				string result = null;
				Type valueType = eventParam.ValueType;
				if (this.paramCount.ContainsKey(valueType))
				{
					result = this.paramPrefix[valueType] + this.paramCount[valueType].ToString();
					Dictionary<Type, int> dictionary;
					Type key;
					(dictionary = this.paramCount)[key = valueType] = dictionary[key] + 1;
				}
				return result;
			}

			private Dictionary<Type, int> paramCount = new Dictionary<Type, int>
			{
				{
					typeof(int),
					1
				},
				{
					typeof(double),
					1
				},
				{
					typeof(bool),
					1
				},
				{
					typeof(string),
					1
				},
				{
					typeof(DateTime),
					1
				}
			};

			private Dictionary<Type, string> paramPrefix = new Dictionary<Type, string>
			{
				{
					typeof(int),
					"i_param"
				},
				{
					typeof(double),
					"f_param"
				},
				{
					typeof(bool),
					"b_param"
				},
				{
					typeof(string),
					"s_param"
				},
				{
					typeof(DateTime),
					"ts_param"
				}
			};
		}
	}
}
