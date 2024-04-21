using System;

namespace TactileModules.Placements
{
	public interface IConfigPropertyGetter<T>
	{
		T Get(string propertyName);
	}
}
