using System;
using System.Reflection;
using TactileModules.PuzzleGames.Configuration;

namespace TactileModules.Placements
{
	public class ConfigPropertyGetter<TConfig, TValue> : IConfigPropertyGetter<TValue>
	{
		public ConfigPropertyGetter(IConfigGetter<TConfig> configGetter)
		{
			this.configGetter = configGetter;
		}

		public TValue Get(string propertyName)
		{
			TConfig tconfig = this.configGetter.Get();
			Type typeFromHandle = typeof(TConfig);
			PropertyInfo property = typeFromHandle.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			return (TValue)((object)property.GetValue(tconfig, null));
		}

		private readonly IConfigGetter<TConfig> configGetter;
	}
}
