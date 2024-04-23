using System;
using UnityEngine;

namespace TactileModules.GameCore.Inventory
{
	public class InventoryCollectTarget : MonoBehaviour
	{
		public InventoryItem TypeToCollect
		{
			get
			{
				return this.typeToCollect;
			}
			set
			{
				this.typeToCollect = value;
			}
		}

		[SerializeField]
		private InventoryItem typeToCollect;
	}
}
