using System;
using System.Collections.Generic;
using UnityEngine;

namespace TactileModules.GameCore.StreamingAssets
{
	public class LowresAssetGeneratorSettings : ScriptableObject
	{
		public string GetGeneratorClass(string group)
		{
			for (int i = 0; i < this.generatorGroups.Count; i++)
			{
				if (this.generatorGroups[i].Group == group)
				{
					return this.generatorGroups[i].GeneratorClass;
				}
			}
			return null;
		}

		public List<LowresAssetGeneratorSettings.GeneratorGroup> generatorGroups = new List<LowresAssetGeneratorSettings.GeneratorGroup>();

		[Serializable]
		public class GeneratorGroup
		{
			public string Group;

			public string GeneratorClass;
		}
	}
}
