using System;
using System.Collections.Generic;
using System.Reflection;
using TactileModules.Analytics.Interfaces;

namespace TactileModules.Analytics
{
	public class DecoratorRegistry
	{
		public void RegisterDecorator(IEventDecorator decorator)
		{
			Type type = decorator.GetType();
			Type type2 = null;
			foreach (Type type3 in type.GetInterfaces())
			{
				if (type3.IsGenericType && type3.GetGenericTypeDefinition() == typeof(IEventDecorator<>))
				{
					Type[] genericArguments = type3.GetGenericArguments();
					if (genericArguments.Length == 1)
					{
						type2 = genericArguments[0];
						break;
					}
				}
			}
			if (type2 != null)
			{
				if (!this.eventTypesAndInvokeInfos.ContainsKey(type2))
				{
					this.eventTypesAndInvokeInfos[type2] = new List<DecoratorRegistry.InvokeInfo>();
				}
				this.eventTypesAndInvokeInfos[type2].Add(new DecoratorRegistry.InvokeInfo
				{
					instance = decorator,
					methodInfo = type.GetMethod("Decorate")
				});
				return;
			}
			throw new Exception("Unable to register event decorator.");
		}

		public void InvokeDecorators(object eventObject)
		{
			if (this.eventTypesAndInvokeInfos.Count == 0)
			{
				return;
			}
			object[] parameters = new object[]
			{
				eventObject
			};
			foreach (KeyValuePair<Type, List<DecoratorRegistry.InvokeInfo>> keyValuePair in this.eventTypesAndInvokeInfos)
			{
				Type key = keyValuePair.Key;
				bool flag = key.IsAssignableFrom(eventObject.GetType());
				if (flag)
				{
					List<DecoratorRegistry.InvokeInfo> value = keyValuePair.Value;
					foreach (DecoratorRegistry.InvokeInfo invokeInfo in value)
					{
						invokeInfo.methodInfo.Invoke(invokeInfo.instance, parameters);
					}
				}
			}
		}

		private readonly Dictionary<Type, List<DecoratorRegistry.InvokeInfo>> eventTypesAndInvokeInfos = new Dictionary<Type, List<DecoratorRegistry.InvokeInfo>>();

		private class InvokeInfo
		{
			public IEventDecorator instance;

			public MethodInfo methodInfo;
		}
	}
}
