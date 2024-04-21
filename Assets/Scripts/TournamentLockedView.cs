using System;

public class TournamentLockedView : UIView
{
	public void SetLockedRank(TournamentRank rankToUnlockNext)
	{
		this.rankToUnlockNext = rankToUnlockNext;
	}

	protected override void ViewWillAppear()
	{
		int num = TournamentManager.Instance.LevelNrRequiredForTournament(this.rankToUnlockNext);
		TournamentSetup.RankSetup rankSetup = SingletonAsset<TournamentSetup>.Instance.GetRankSetup(this.rankToUnlockNext);
		this.rankName.text = L.Get(rankSetup.displayName);
		if (rankSetup.fontStyle != null)
		{
			this.rankName.fontStyle = rankSetup.fontStyle;
		}
		this.unlockLabel.text = string.Format(L.Get("Until level {0}"), num);
		this.description.text = string.Format(L.Get("Compete in {0} tournaments and win fabulous prizes!"), L.Get(rankSetup.displayName));
		this.trophy.TextureResource = rankSetup.bigIconResourcePath;
	}

	private void DismissClicked(UIEvent e)
	{
		base.Close(0);
	}

	public UILabel rankName;

	public UILabel description;

	public UILabel unlockLabel;

	public UIResourceQuad trophy;

	private TournamentRank rankToUnlockNext = TournamentRank.Bronze;
}
