using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using Tactile;
using TactileModules.Placements;
using TactileModules.PuzzleGame.MainLevels;
using TactileModules.PuzzleGames.GameCore;

namespace TactileModules.SagaCore
{
	public class MainMapFlow : MapFlow, MapPopupManager.IPopupManagerProvider, MapStreamer.IDataProvider, IMainMapFlow, IFullScreenOwner, IMapFlow
	{
		public MainMapFlow(LevelDatabaseCollection levelDatabaseCollection, MapStreamerCollection mapStreamerCollection, CloudClient cloudClient, MapFacade mapFacade, IMainProgression mainProgressionManager, MapPopupManager mapPopupManager, IGameSessionManager gameSessionManager, IFullScreenManager fullScreenManager, IFlowStack flowStack, IPlacementRunner placementRunner, IMainLevelsFlowFactory mainLevelsFlowFactory, IStoryIntroProvider storyIntroProvider) : base("Main", mapFacade, fullScreenManager, flowStack)
		{
			this.levelDatabaseCollection = levelDatabaseCollection;
			this.mapStreamerCollection = mapStreamerCollection;
			this.cloudClient = cloudClient;
			this.mainProgressionManager = mainProgressionManager;
			this.mapPopupManager = mapPopupManager;
			this.gameSessionManager = gameSessionManager;
			mainProgressionManager.DeveloperCheated += this.HandleProgressionCheated;
			this.StartSequenceHooks = new BreakableHookList<MainMapFlow>();
			UserSettingsManager.Instance.SettingsSynced += this.HandleUserSettingsSynced;
			gameSessionManager.NewSessionStarted += this.HandleNewGameSession;
			this.placementRunner = placementRunner;
			this.mainLevelsFlowFactory = mainLevelsFlowFactory;
			this.storyIntroProvider = storyIntroProvider;
		}

		public BreakableHookList<MainMapFlow> StartSequenceHooks { get; private set; }

		MapPopupManager.PopupConfig MapPopupManager.IPopupManagerProvider.PopupConfig
		{
			get
			{
				return ConfigurationManager.Get<MapPopupManager.PopupConfig>();
			}
		}

		private void HandleNewGameSession()
		{
			if (base.IsMapIdle())
			{
				base.RunPostPlayFlowSequence();
			}
		}

		private void HandleUserSettingsSynced(UserSettingsManager obj)
		{
			base.MapContentController.Refresh();
			if (this.needFocusAfterSyncedSettings)
			{
				base.MapContentController.JumpToDot(this.GetFarthestUnlockedDotIndex());
				this.needFocusAfterSyncedSettings = false;
			}
		}

		private void HandleFacebookLogin()
		{
			this.needFocusAfterSyncedSettings = true;
		}

		private void HandleFacebookLogout()
		{
			base.MapContentController.Refresh();
		}

		private void HandleProgressionCheated()
		{
			base.MapContentController.Refresh();
		}

		protected override int GetFarthestUnlockedDotIndex()
		{
			LevelProxy farthestUnlockedLevelProxy = MainProgressionManager.Instance.GetFarthestUnlockedLevelProxy();
			return farthestUnlockedLevelProxy.Index;
		}

		protected override IFlow CreateFlowForDot(int dotIndex)
		{
			if (dotIndex > this.mainProgressionManager.MaxAvailableLevel)
			{
				return null;
			}
			if (this.mainProgressionManager.GetDatabase().GetGateIndex(dotIndex) >= 0)
			{
				return this.mainLevelsFlowFactory.CreateGateFlow();
			}
			if (dotIndex == 0 && this.storyIntroProvider != null)
			{
				return this.storyIntroProvider.CreateStoryFlow();
			}
			LevelProxy level = this.mainProgressionManager.GetDatabase().GetLevel(dotIndex);
			return this.mainLevelsFlowFactory.CreateMainLevelFlow(level);
		}

		protected override IEnumerator AfterScreenAcquired()
		{
			this.levelDatabaseCollection.UpdateLevelDatabasesIfAvailable();
			this.mapStreamerCollection.UpdateLevelDatabasesIfAvailable();
			this.buttonsViewState = UIViewManager.Instance.ShowView<MainMapButtonsView>(new object[0]);
			this.buttonsViewState.View.DidEscape += this.HandleDidExit;
			yield break;
		}

		private void HandleDidExit()
		{
			base.FlowStack.Push(new SettingsView.StartScreenFlow());
		}

		protected override void AfterScreenLost()
		{
			if (this.buttonsViewState != null)
			{
				UIViewLayer viewLayerWithView = UIViewManager.Instance.GetViewLayerWithView(this.buttonsViewState.View);
				if (viewLayerWithView != null)
				{
					viewLayerWithView.CloseInstantly();
				}
			}
		}

		protected override IEnumerator PostPlayFlowSequence(int nextDotIndexToOpen)
		{
			if (this.GetFarthestUnlockedDotIndex() <= 1 && this.storyIntroProvider != null)
			{
				yield break;
			}
			yield return this.placementRunner.Run(PlacementIdentifier.PreAnimateAvatar);
			int progressIndex = (!base.MapContentController.Avatars.AnyAvatarsNeedMoving()) ? -1 : this.GetFarthestUnlockedDotIndex();
			yield return this.mapPopupManager.Run(this, base.MapContentController.Avatars.AnyAvatarsNeedMoving(), progressIndex);
			if (this.mapPopupManager.popupResult.flowBroken)
			{
				yield break;
			}
			EnumeratorResult<bool> didBreak = new EnumeratorResult<bool>();
			yield return this.placementRunner.Run(PlacementIdentifier.PostAnimateAvatar, didBreak);
			if (didBreak)
			{
				yield break;
			}
			if (nextDotIndexToOpen >= 0 && this.mapPopupManager.popupResult.shouldShowLevelStartView)
			{
				base.StartFlowForDot(nextDotIndexToOpen);
			}
			if (this.gameSessionManager.HasPendingSessionForRecipient(this))
			{
				this.gameSessionManager.ConsumeSessionForRecipient(this);
				EnumeratorResult<bool> wasFlowBroken = new EnumeratorResult<bool>();
				yield return this.placementRunner.Run(PlacementIdentifier.SessionStart, wasFlowBroken);
			}
			yield break;
		}

		IEnumerator MapPopupManager.IPopupManagerProvider.AnimateProgress()
		{
			yield return base.AnimateAvatarProgressIfAny();
			yield break;
		}

		public override SagaAvatarInfo CreateMeAvatarInfo()
		{
			return new SagaAvatarInfo
			{
				dotIndex = this.GetFarthestUnlockedDotIndex()
			};
		}

		public override Dictionary<CloudUser, SagaAvatarInfo> CreateFriendsAvatarInfos()
		{
			Dictionary<CloudUser, SagaAvatarInfo> dictionary = new Dictionary<CloudUser, SagaAvatarInfo>();
			List<CloudUser> cachedFriends = this.cloudClient.CachedFriends;
			foreach (CloudUser cloudUser in cachedFriends)
			{
				SagaAvatarInfo sagaAvatarInfo = new SagaAvatarInfo();
				MainProgressionManager.PublicState friend = UserSettingsManager.GetFriend<MainProgressionManager.PublicState>(cloudUser);
				if (friend != null)
				{
					sagaAvatarInfo.dotIndex = friend.LatestUnlockedIndex;
					dictionary.Add(cloudUser, sagaAvatarInfo);
				}
			}
			return dictionary;
		}

		protected override MapStreamer.IDataProvider GetMapStreamerDataProvider()
		{
			return this;
		}

		int MapStreamer.IDataProvider.MaxAvailableLevel
		{
			get
			{
				return this.mainProgressionManager.MaxAvailableLevel;
			}
		}

		private readonly LevelDatabaseCollection levelDatabaseCollection;

		private readonly MapStreamerCollection mapStreamerCollection;

		private readonly CloudClient cloudClient;

		private readonly IMainProgression mainProgressionManager;

		private readonly MapPopupManager mapPopupManager;

		private readonly IGameSessionManager gameSessionManager;

		private UIViewManager.UIViewStateGeneric<MainMapButtonsView> buttonsViewState;

		private bool needFocusAfterSyncedSettings;

		private readonly IPlacementRunner placementRunner;

		private readonly IMainLevelsFlowFactory mainLevelsFlowFactory;

		private readonly IStoryIntroProvider storyIntroProvider;
	}
}
