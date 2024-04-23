using System;
using System.Collections;
using System.Collections.Generic;
using Tactile;
using TactileModules.Foundation;
using TactileModules.PuzzleCore.LevelPlaying;

public class BoosterSuggester
{
	public BoosterSuggester(IPlayFlowEvents playLevelFacade)
	{
		playLevelFacade.PlayFlowCreated += this.OnPlayFlowCreated;
	}

	private void OnPlayFlowCreated(ICorePlayFlow obj)
	{
		obj.LevelEndedHook.Register(new Func<ILevelAttempt, IEnumerator>(this.HandleLevelEnded));
	}

	private IEnumerator HandleLevelEnded(ILevelAttempt attempt)
	{
		if (attempt.DidPlayAndFail)
		{
			LevelSession levelSession = ((PlayLevel)attempt.GameImplementation).Session;
			if (BoosterSuggester.ShouldShowBoosterSuggestion(levelSession))
			{
				List<InventoryItem> availableBoosters = BoosterSuggester.GetAvailableBoostersForCurrentLevel(levelSession);
				UIViewManager.Instance.ShowView<BoosterSuggestionView>(new object[]
				{
					levelSession,
					availableBoosters
				});
				yield return UIViewManager.Instance.WaitForFadeDown();
				BoosterSuggestionView suggestionView = UIViewManager.Instance.FindView<BoosterSuggestionView>();
				while (suggestionView.ClosingResult == null)
				{
					yield return null;
				}
				List<InventoryItem> itemSuggestions = suggestionView.ClosingResult as List<InventoryItem>;
				if (itemSuggestions != null)
				{
					ManagerRepository.Get<BoosterManager>().AutoSelectedBoosters = itemSuggestions;
				}
			}
		}
		yield break;
	}

	private static bool ShouldShowBoosterSuggestion(LevelSession levelSession)
	{
		return ConfigurationManager.Get<BoosterConfig>().ShowSuggestionOnFail && levelSession.GetContinuousFailsForThisLevel() >= ConfigurationManager.Get<BoosterConfig>().SuggestionNeedToFailXTimes && BoosterSuggester.GetAvailableBoostersForCurrentLevel(levelSession).Count != 0;
	}

	private static List<InventoryItem> GetAvailableBoostersForCurrentLevel(LevelSession levelSession)
	{
		List<InventoryItem> list = new List<InventoryItem>();
		if (InventoryManager.Instance.GetAmount("BoosterSuperQueue") > 0)
		{
			list.Add("BoosterSuperQueue");
		}
		if (InventoryManager.Instance.GetAmount("BoosterSuperAim") > 0)
		{
			list.Add("BoosterSuperAim");
		}
		bool flag = LevelUtils.PieceWithTypeExist<MinusPiece>(levelSession.Level.LevelAsset as LevelAsset, null);
		bool flag2 = LevelUtils.PieceWithTypeExist<DeathPiece>(levelSession.Level.LevelAsset as LevelAsset, null);
		if ((flag || flag2) && InventoryManager.Instance.GetAmount("BoosterShield") > 0)
		{
			list.Add("BoosterShield");
		}
		return list;
	}
}
