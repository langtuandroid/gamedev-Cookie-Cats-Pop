using System;
using System.Collections;
using UnityEngine;

public class TournamentRewardItem : ExtensibleVisual<TournamentRewardItem.IExtension>
{
	public void Init(TournamentPrizeConfig prizeTier)
	{
		string text = prizeTier.RankTo.ToString();
		if (prizeTier.RankTo != prizeTier.RankFrom)
		{
			text = prizeTier.RankFrom.ToString() + "-" + prizeTier.RankTo.ToString();
		}
		this.positionNumber.text = text;
		if (base.Extension != null)
		{
			base.Extension.InitializeRewardVisuals(prizeTier);
		}
	}

	public IEnumerator AnimateGivingReward()
	{
		if (base.Extension != null)
		{
			yield return base.Extension.AnimateGivingReward();
		}
		yield break;
	}

	[SerializeField]
	private UILabel positionNumber;

	public interface IExtension
	{
		void InitializeRewardVisuals(TournamentPrizeConfig prizeTier);

		IEnumerator AnimateGivingReward();
	}
}
