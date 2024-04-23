using System;

namespace TactileModules.RuntimeTools.Random
{
	public interface IRandom
	{
		float GetNextFloat();

		int Range(int minValue, int maxValue);
	}
}
