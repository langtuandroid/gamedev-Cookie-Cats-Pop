using System;
using System.Collections.Generic;

namespace TactileModules.SagaCore
{
	public static class GameImplementors
	{
		public static void Initialize()
		{
			GameImplementors.registeredImplementors = new List<Type>();
			Type typeFromHandle = typeof(IGameInterface);
			Type[] types = typeFromHandle.Assembly.GetTypes();
			foreach (Type type in types)
			{
				if (typeFromHandle.IsAssignableFrom(type) && !type.IsAbstract)
				{
					GameImplementors.registeredImplementors.Add(type);
				}
			}
		}

		public static Type FindImplementationForType<T>() where T : IGameInterface
		{
			Type typeFromHandle = typeof(T);
			foreach (Type type in GameImplementors.registeredImplementors)
			{
				if (typeFromHandle.IsAssignableFrom(type) && !type.IsAbstract)
				{
					return type;
				}
			}
			return null;
		}

		public static T Create<T>() where T : IGameInterface
		{
			Type type = GameImplementors.FindImplementationForType<T>();
			if (type != null)
			{
				return (T)((object)Activator.CreateInstance(type));
			}
			return default(T);
		}

		private static List<Type> registeredImplementors;
	}
}
