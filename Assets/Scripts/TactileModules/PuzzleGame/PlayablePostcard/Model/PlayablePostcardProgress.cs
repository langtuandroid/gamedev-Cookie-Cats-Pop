using System;
using System.Collections.Generic;

namespace TactileModules.PuzzleGame.PlayablePostcard.Model
{
	public class PlayablePostcardProgress
	{
		public PlayablePostcardProgress(PlayablePostcardInstanceCustomData instanceData)
		{
			this.instanceData = instanceData;
		}

		public PostcardItemType GetCurrentItemType()
		{
			if (this.instanceData.Postcard.Count == 0)
			{
				return PostcardItemType.Background;
			}
			if (!this.ContainsItemType(PostcardItemType.Character))
			{
				return PostcardItemType.Character;
			}
			if (!this.ContainsItemType(PostcardItemType.Costume))
			{
				return PostcardItemType.Costume;
			}
			if (!this.ContainsItemType(PostcardItemType.Prop))
			{
				return PostcardItemType.Prop;
			}
			if (!this.ContainsItemType(PostcardItemType.Text))
			{
				return PostcardItemType.Text;
			}
			return PostcardItemType.Background;
		}

		public int GetFarthestCompletedLevelIndex()
		{
			return this.instanceData.FarthestCompletedLevel;
		}

		public bool ShouldCreatePostcard()
		{
			return this.instanceData.FarthestCompletedLevel + 1 > this.instanceData.Postcard.Count<PostcardItemTypeAndId>();
		}

		public bool HasCompletedPostcard()
		{
			return this.instanceData.Postcard.Count<PostcardItemTypeAndId>() == 5;
		}

		private bool ContainsItemType(PostcardItemType itemType)
		{
			return this.instanceData.Postcard.Exists((PostcardItemTypeAndId x) => x.Type == itemType);
		}

		public void CompletedLevel()
		{
			this.instanceData.FarthestCompletedLevel++;
		}

		public void UpdatePersistedPostcard(PostcardItemType itemType, string id)
		{
			this.instanceData.Postcard.Add(new PostcardItemTypeAndId
			{
				Type = itemType,
				Id = id
			});
		}

		public List<PostcardItemTypeAndId> GetPersistedPostcard()
		{
			List<PostcardItemTypeAndId> list = new List<PostcardItemTypeAndId>();
			list.AddRange(this.instanceData.Postcard.ToArray());
			return list;
		}

		private readonly PlayablePostcardInstanceCustomData instanceData;

		private const int MAX_LEVELS = 5;
	}
}
