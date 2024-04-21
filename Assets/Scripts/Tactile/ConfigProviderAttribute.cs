using System;
using System.Collections.Generic;
using System.Reflection;

namespace Tactile
{
	[AttributeUsage(AttributeTargets.Class)]
	public class ConfigProviderAttribute : Attribute
	{
		public ConfigProviderAttribute(string key)
		{
			this.Name = key;
		}

		public string Name { get; private set; }

		public static Dictionary<Type, ConfigProviderAttribute> CollectClassesInAssembly()
		{
			Dictionary<Type, ConfigProviderAttribute> dictionary = new Dictionary<Type, ConfigProviderAttribute>();
			Assembly assembly = typeof(ConfigProviderAttribute).Assembly;
			foreach (Type type in assembly.GetTypes())
			{
				ConfigProviderAttribute[] array = (ConfigProviderAttribute[])type.GetCustomAttributes(typeof(ConfigProviderAttribute), false);
				if (array.Length > 0)
				{
					dictionary.Add(type, array[0]);
				}
			}
			return dictionary;
		}
	}
}
