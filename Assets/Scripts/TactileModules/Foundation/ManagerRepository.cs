using System;
using System.Collections.Generic;

namespace TactileModules.Foundation
{
	public class ManagerRepository
	{
		public T Register<T>(T si, Type t = null)
		{
			ManagerRepository.managers.Add((t == null) ? si.GetType() : t, si);
			return si;
		}

		public static T Get<T>()
		{
			return (T)((object)ManagerRepository.managers[typeof(T)]);
		}

		public void SetupManagers()
		{
			foreach (KeyValuePair<Type, object> keyValuePair in ManagerRepository.managers)
			{
				if (typeof(IManager).IsAssignableFrom(keyValuePair.Key))
				{
					((IManager)keyValuePair.Value).ManagersConstructed();
				}
			}
		}

		private static readonly Dictionary<Type, object> managers = new Dictionary<Type, object>();
	}
}
