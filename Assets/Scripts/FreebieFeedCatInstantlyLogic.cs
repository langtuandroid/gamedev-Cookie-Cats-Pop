using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreebieFeedCatInstantlyLogic : BoosterLogic
{
	protected override IEnumerator Logic(LevelSession session)
	{
		List<MatchFlag> activePowers = new List<MatchFlag>(session.AdjustedEnabledPowerupColors);
		activePowers.Shuffle<MatchFlag>();
		GamePowers.Power chosenPower = session.Powers[activePowers[0]];
		this.title.text = L.Get(SingletonAsset<LevelVisuals>.Instance.FreebieInstafeedText);
		if (FreebieFeedCatInstantlyLogic.shopItemMap.ContainsKey(chosenPower.Color))
		{
			ShopItemIdentifier item = FreebieFeedCatInstantlyLogic.shopItemMap[chosenPower.Color];
			this.icon.SpriteName = ShopManager.Instance.GetMetaData<CPShopItemMetaData>(item).ImageSpriteName;
		}
		yield return FiberHelper.Wait(2f, (FiberHelper.WaitFlag)0);
		chosenPower.Activate();
		yield break;
	}

	[SerializeField]
	private UILabel title;

	[SerializeField]
	private UISprite icon;

	private static readonly Dictionary<MatchFlag, ShopItemIdentifier> shopItemMap = new Dictionary<MatchFlag, ShopItemIdentifier>
	{
		{
			"Blue",
			"ShopItemFillBlue"
		},
		{
			"Green",
			"ShopItemFillGreen"
		},
		{
			"Red",
			"ShopItemFillRed"
		},
		{
			"Yellow",
			"ShopItemFillYellow"
		}
	};
}
