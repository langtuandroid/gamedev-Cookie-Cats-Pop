using System;
using System.Collections;
using TactileModules.FeatureManager;
using TactileModules.FeatureManager.DataClasses;

public class OneLifeChallengePopups
{
	public class OneLifeChallengeStartPopup : MapPopupManager.IMapPopup
	{
		public OneLifeChallengeStartPopup(OneLifeChallengeManager manager, FeatureManager featureManager)
		{
			this.manager = manager;
			this.featureManager = featureManager;
			MapPopupManager.Instance.RegisterPopupObject(this);
		}

		private bool ShouldShowPopup()
		{
			return this.manager.Config.FeatureEnabled && FeatureManager.Instance.CanActivateFeature(this.manager) && PuzzleGame.PlayerState.FarthestUnlockedLevelHumanNumber >= this.manager.Config.LevelRequired;
		}

		private IEnumerator ShowPopup(FeatureData featureData)
		{
			FeatureManager.Instance.ActivateFeature(this.manager, featureData);
			yield return this.manager.provider.ShowEventStartView();
			yield break;
		}

		public void TryShowPopup(int unlockedLevelIndex, MapPopupManager.PopupFlow popupFlow)
		{
			if (!this.manager.Config.FeatureEnabled)
			{
				return;
			}
			if (this.ShouldShowPopup())
			{
				FeatureData feature = this.featureManager.GetFeature(this.manager);
				popupFlow.AddPopup(this.ShowPopup(feature));
			}
		}

		private readonly OneLifeChallengeManager manager;

		private readonly FeatureManager featureManager;
	}

	public class OneLifeChallengeEndPopup : MapPopupManager.IMapPopup
	{
		public OneLifeChallengeEndPopup(OneLifeChallengeManager manager)
		{
			this.manager = manager;
			MapPopupManager.Instance.RegisterPopupObject(this);
		}

		private bool ShouldEnd()
		{
			return FeatureManager.Instance.ShouldDeactivateFeature(this.manager);
		}

		private bool ShouldShowPopup()
		{
			return this.ShouldEnd() && !this.manager.GetActivatedFeature().GetCustomInstanceData<OneLifeChallengeInstanceCustomData, FeatureMetaData, FeatureTypeCustomData>(this.manager).RewardClaimed;
		}

		private bool ShouldRunSilentAction()
		{
			return this.ShouldEnd() && this.manager.GetActivatedFeature().GetCustomInstanceData<OneLifeChallengeInstanceCustomData, FeatureMetaData, FeatureTypeCustomData>(this.manager).RewardClaimed;
		}

		private IEnumerator ShowPopup()
		{
			FeatureManager.Instance.DeactivateFeature(this.manager);
			yield return this.manager.provider.ShowEventEndedView();
			yield break;
		}

		public void TryShowPopup(int unlockedLevelIndex, MapPopupManager.PopupFlow popupFlow)
		{
			if (!this.manager.Config.FeatureEnabled)
			{
				return;
			}
			if (this.ShouldShowPopup())
			{
				popupFlow.AddPopup(this.ShowPopup());
			}
			else if (this.ShouldRunSilentAction())
			{
				popupFlow.AddSilentAction(delegate
				{
					FeatureManager.Instance.DeactivateFeature(this.manager);
				});
			}
		}

		private readonly OneLifeChallengeManager manager;
	}

	public class OneLifeChallengeSessionStartPopup : MapPopupManager.IMapPopup
	{
		public OneLifeChallengeSessionStartPopup(OneLifeChallengeManager manager)
		{
			this.manager = manager;
			MapPopupManager.Instance.RegisterPopupObject(this);
		}

		private bool ShouldShowPopup()
		{
			return this.manager.Config.FeatureEnabled && this.manager.HasActiveFeature() && FeatureManager.Instance.GetStabilizedTimeLeftToFeatureDurationEnd(this.manager) > 0 && !FeatureManager.Instance.ShouldDeactivateFeature(this.manager);
		}

		private IEnumerator ShowPopup()
		{
			yield return this.manager.provider.ShowEventStartSessionView();
			yield break;
		}

		public void TryShowPopup(int unlockedLevelIndex, MapPopupManager.PopupFlow popupFlow)
		{
			if (!this.manager.Config.FeatureEnabled)
			{
				return;
			}
			if (this.ShouldShowPopup())
			{
				popupFlow.AddPopup(this.ShowPopup());
			}
		}

		private readonly OneLifeChallengeManager manager;
	}
}
