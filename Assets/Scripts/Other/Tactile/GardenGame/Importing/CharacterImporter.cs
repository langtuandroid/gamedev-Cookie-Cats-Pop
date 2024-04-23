using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tactile.GardenGame.Importing
{
	public class CharacterImporter : MonoBehaviour
	{
		public GameObject rig;

		public List<CharacterImporter.Item> Items = new List<CharacterImporter.Item>();

		[Serializable]
		public class Item
		{
			public Renderer renderer;

			public Material material;
		}
	}
}
