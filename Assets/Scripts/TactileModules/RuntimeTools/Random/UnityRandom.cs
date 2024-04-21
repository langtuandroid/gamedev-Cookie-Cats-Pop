using System;
using UnityEngine;

namespace TactileModules.RuntimeTools.Random
{
	public class UnityRandom : IRandom
	{
		public float GetNextFloat()
		{
			return UnityEngine.Random.value;
		}

		public int Range(int minValue, int maxValue)
		{
			return UnityEngine.Random.Range(minValue, maxValue);
		}
	}
}
