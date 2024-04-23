using System;
using UnityEngine;

namespace TactileModules.Portraits
{
	public interface IRandomPortraitsAndNames
	{
		Texture2D GetRandomPortrait(int seed);

		string GetRandomName(int seed);
	}
}
