using System;
using System.Collections;
using System.Collections.Generic;
using Tactile;
using TactileModules.FeatureManager;
using TactileModules.PuzzleCore.LevelPlaying;
using TactileModules.PuzzleGame.HotStreak;
using TactileModules.PuzzleGames.EndlessChallenge;
using UnityEngine;

public static class FreebieLogic
{
	public static IEnumerator ResolveFreeGift(bool wasBought, LevelSession levelSession, GameView gameView)
	{
		UICamera.DisableInput();
		int numFreebieUsed = Mathf.Max(UserSettingsManager.Instance.GetSettings<GameStatsManager.PersistableState>().NumFreebieUsed, 0);
		ExperienceConfig config = ConfigurationManager.Get<ExperienceConfig>();
		List<LevelVisuals.FreebieEntry> freebies = null;
		if (levelSession.Level.LevelAsset is EndlessLevel)
		{
			freebies = SingletonAsset<EndlessChallengeSetup>.Instance.endlessChallengeFreebeeBoosters;
		}
		else
		{
			freebies = ((levelSession.Level.LevelDifficulty != LevelDifficulty.Hard) ? SingletonAsset<LevelVisuals>.Instance.freebeeBoosters : SingletonAsset<LevelVisuals>.Instance.hardModeFreebeeBoosters);
		}
		BoosterLogic chosenFreebie;
		if (config.FixedFirstFreebies && numFreebieUsed < freebies.Count)
		{
			chosenFreebie = freebies[numFreebieUsed].logicPrefab;
		}
		else
		{
			Lottery<BoosterLogic> lottery = new Lottery<BoosterLogic>();
			foreach (LevelVisuals.FreebieEntry freebieEntry in freebies)
			{
				bool flag = false;
				foreach (SelectedBooster selectedBooster in levelSession.PregameBoosters)
				{
					CPBoosterMetaData metaData = InventoryManager.Instance.GetMetaData<CPBoosterMetaData>(selectedBooster.id);
					if (metaData.LogicPrefab.GetType().IsAssignableFrom(freebieEntry.logicPrefab.GetType()))
					{
						flag = true;
						break;
					}
				}
				HotStreakManager featureHandler = FeatureManager.GetFeatureHandler<HotStreakManager>();
				List<ItemAmount> currentTierBonus = featureHandler.CurrentTierBonus;
				foreach (ItemAmount itemAmount in currentTierBonus)
				{
					CPBoosterMetaData metaData2 = InventoryManager.Instance.GetMetaData<CPBoosterMetaData>(itemAmount.ItemId);
					if (metaData2.LogicPrefab.GetType().IsAssignableFrom(freebieEntry.logicPrefab.GetType()))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					lottery.Add(freebieEntry.chance, freebieEntry.logicPrefab);
				}
			}
			chosenFreebie = lottery.PickRandomItem(false);
		}
		GameEventManager.Instance.Emit(41, chosenFreebie, (!wasBought) ? 1 : 0);
		yield return BoosterLogic.ResolveBooster(chosenFreebie, levelSession, gameView);
		UserSettingsManager.Instance.GetSettings<GameStatsManager.PersistableState>().NumFreebieUsed = numFreebieUsed + 1;
		UICamera.EnableInput();
		yield break;
	}
}
