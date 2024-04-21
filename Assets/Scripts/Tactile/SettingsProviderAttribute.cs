using System;
using System.Collections.Generic;
using System.Reflection;

namespace Tactile
{
	[AttributeUsage(AttributeTargets.Class)]
	public class SettingsProviderAttribute : Attribute
	{
		public SettingsProviderAttribute(string key, bool isPublic, params Type[] mergeDependencies)
		{
			foreach (Type c in mergeDependencies)
			{
				if (!typeof(IPersistableState).IsAssignableFrom(c))
				{
					throw new Exception("Merge dependency is not implementing IPersistableState! A merge dependency needs to implement the IPersistableState interface!");
				}
			}
			this.Name = key;
			this.IsPublic = isPublic;
			this.MergeDependencies = new List<Type>(mergeDependencies);
		}

		public string Name { get; private set; }

		public bool IsPublic { get; private set; }

		public List<Type> MergeDependencies { get; private set; }

		public static Dictionary<Type, SettingsProviderAttribute> CollectClassesInAssembly()
		{
			Dictionary<Type, SettingsProviderAttribute> dictionary = new Dictionary<Type, SettingsProviderAttribute>();
			Assembly assembly = typeof(SettingsProviderAttribute).Assembly;
			foreach (Type type in assembly.GetTypes())
			{
				if (typeof(IPersistableState).IsAssignableFrom(type))
				{
					SettingsProviderAttribute[] array = (SettingsProviderAttribute[])type.GetCustomAttributes(typeof(SettingsProviderAttribute), false);
					if (array.Length > 0)
					{
						dictionary.Add(type, array[0]);
					}
				}
			}
			return dictionary;
		}
	}
}
