using System;
using UnityEngine;

namespace Tactile.GardenGame.Importing
{
	public class ModelImporterSettings : ScriptableObject
	{
		public Material GetMaterialFromName(string name)
		{
			string text = name.ToLowerInvariant();
			for (int i = 0; i < this.materials.Length; i++)
			{
				if (text.Contains(this.materials[i].Name.ToLowerInvariant()))
				{
					return this.materials[i].material;
				}
			}
			return this.defaultMaterial;
		}

		public Material defaultMaterial;

		public ModelImporterSettings.CharacterMaterial[] materials;

		[Serializable]
		public class CharacterMaterial
		{
			public string Name;

			public Material material;
		}
	}
}
