using System;
using System.Collections.Generic;
using TactileModules.MapFeature;

namespace TactileModules.PuzzleGame.PlayablePostcard.Model
{
	public class PlayablePostcardInstanceCustomData : MapFeatureInstanceCustomData
	{
		public PlayablePostcardInstanceCustomData()
		{
			this.Reset();
		}

		[JsonSerializable("farthestCompleted", null)]
		public int FarthestCompletedLevel { get; set; }

		[JsonSerializable("Postcard", typeof(PostcardItemTypeAndId))]
		public List<PostcardItemTypeAndId> Postcard { get; set; }

		[JsonSerializable("HasShownStartPopup", null)]
		public bool HasShownStartPopup { get; set; }

		public void Reset()
		{
			this.FarthestCompletedLevel = -1;
			this.Postcard = new List<PostcardItemTypeAndId>();
			this.HasShownStartPopup = false;
		}

		public static void TakeState<TFeatureState>(ref TFeatureState toMerge, TFeatureState state) where TFeatureState : PlayablePostcardInstanceCustomData
		{
			toMerge.FarthestCompletedLevel = state.FarthestCompletedLevel;
			toMerge.Postcard = state.Postcard;
			toMerge.HasShownStartPopup = state.HasShownStartPopup;
		}
	}
}
