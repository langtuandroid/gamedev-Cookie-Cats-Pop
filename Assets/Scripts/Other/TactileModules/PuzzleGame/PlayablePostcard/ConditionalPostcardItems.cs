using System;
using System.Collections.Generic;
using TactileModules.PuzzleGame.PlayablePostcard.Model;
using UnityEngine;

namespace TactileModules.PuzzleGame.PlayablePostcard
{
	public class ConditionalPostcardItems : MonoBehaviour
	{
		public bool ExcludeOthers
		{
			get
			{
				return this.excludeOthers;
			}
			set
			{
				this.excludeOthers = value;
			}
		}

		public bool Match(List<PostcardItemTypeAndId> postcardData)
		{
			foreach (ConditionalPostcardItems.ItemTypeAndIds itemTypeAndIds in this.conditions)
			{
				PostcardItemTypeAndId itemTypeAndId = this.GetItemTypeAndId(postcardData, itemTypeAndIds.type);
				if (itemTypeAndId == null)
				{
					return false;
				}
				if (!itemTypeAndIds.ids.Exists((string x) => x == itemTypeAndId.Id))
				{
					return false;
				}
			}
			return true;
		}

		private PostcardItemTypeAndId GetItemTypeAndId(List<PostcardItemTypeAndId> postcardData, PostcardItemType itemType)
		{
			return postcardData.Find((PostcardItemTypeAndId x) => x.Type == itemType);
		}

		[SerializeField]
		[HideInInspector]
		public List<ConditionalPostcardItems.ItemTypeAndIds> conditions;

		[SerializeField]
		[HideInInspector]
		public bool excludeOthers;

		[Serializable]
		public class ItemTypeAndIds
		{
			[SerializeField]
			[HideInInspector]
			public PostcardItemType type;

			[SerializeField]
			[HideInInspector]
			public List<string> ids = new List<string>();
		}
	}
}
