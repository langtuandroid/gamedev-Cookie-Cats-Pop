using System;
using System.Collections.Generic;
using UnityEngine;

[SingletonAssetPath("Assets/[ModuleAssets]/Resources/Tournaments/TournamentSetup.asset")]
public class TournamentSetup : SingletonAsset<TournamentSetup>
{
	public TournamentSetup.RankSetup GetRankSetup(TournamentRank rank)
	{
		foreach (TournamentSetup.RankSetup rankSetup in this.ranks)
		{
			if (rankSetup.tournamentRank == rank)
			{
				return rankSetup;
			}
		}
		return this.ranks[0];
	}

	public string GetRankAsString(TournamentRank rank)
	{
		return this.GetRankSetup(rank).displayName;
	}

	public TournamentSetup.TutorialStep GetDebriefingMessage(int prizeTier)
	{
		return (prizeTier < 0 || prizeTier >= this.debriefingMessages.Count) ? null : this.debriefingMessages[prizeTier];
	}

	public Texture2D GetRandomPortrait(int hash)
	{
		if (this.portraitTextures == null || this.portraitTextures.Count == 0)
		{
			return null;
		}
		int value = hash % this.portraitTextures.Count;
		return this.portraitTextures[Mathf.Abs(value)];
	}

	public List<TournamentSetup.TutorialStep> tutorialSteps;

	public List<TournamentSetup.RankSetup> ranks;

	public List<TournamentSetup.TutorialStep> debriefingMessages;

	public List<Texture2D> portraitTextures;

	[Serializable]
	public class TutorialStep
	{
		public string LocalizedMessage
		{
			get
			{
				return L.Get(this.message);
			}
		}

		public string message;
	}

	[Serializable]
	public class RankSetup
	{
		public string displayName;

		public TournamentRank tournamentRank;

		public string bigIconResourcePath;

		public InventoryItem ticketItem;

		public UIFontStyle fontStyle;

		[UISpriteName]
		public string iconSpriteName;
	}
}
