using System;
using System.Collections;
using Fibers;
using TactileModules.FeatureManager;
using TactileModules.PuzzleGame.SlidesAndLadders.Model;
using TactileModules.PuzzleGame.SlidesAndLadders.UI;
using UnityEngine;

namespace TactileModules.PuzzleGame.SlidesAndLadders.Views
{
	public class SlidesAndLaddersMapView : MapViewBase
	{
		public bool ViewAppeared
		{
			get
			{
				return this.viewAppeared;
			}
		}

		public SlidesAndLaddersSpline[] Splines
		{
			get
			{
				return this.splines;
			}
		}

		public AnimationCurve SlideCurve
		{
			get
			{
				return this.slideCurve;
			}
		}

		public AnimationCurve LadderCurve
		{
			get
			{
				return this.ladderCurve;
			}
		}

		public AnimationCurve AvatarMoveCurve
		{
			get
			{
				return this.avatarMoveCurve;
			}
		}

		public AnimationCurve AvatarMoveOffsetCurve
		{
			get
			{
				return this.avatarMoveOffsetCurve;
			}
		}

		private SlidesAndLaddersHandler SlidesAndLaddersHandler
		{
			get
			{
				if (this.handler == null)
				{
					this.handler = TactileModules.FeatureManager.FeatureManager.GetFeatureHandler<SlidesAndLaddersHandler>();
				}
				return this.handler;
			}
		}

		protected override LevelProxy FarthestUnlockedLevel
		{
			get
			{
				return (LevelProxy)this.LevelDatabase.GetLevelProxy(this.SlidesAndLaddersHandler.InstanceCustomData.FarthestUnlockedLevelIndex);
			}
		}

		protected override LevelProxy FarthestCompletedLevel
		{
			get
			{
				return this.FarthestUnlockedLevel.PreviousLevel;
			}
		}

		protected string LevelType
		{
			get
			{
				return "slides-and-ladders";
			}
		}

		protected override MapStreamerCollection MapStreamerCollection
		{
			get
			{
				return this.SlidesAndLaddersHandler.MapStreamerCollection;
			}
		}

		protected ISlidesAndLaddersLevelDatabase LevelDatabase
		{
			get
			{
				return this.SlidesAndLaddersHandler.LevelDatabase;
			}
		}

		protected override void ViewLoad(object[] parameters)
		{
			base.ViewLoad(parameters);
			this.viewAppeared = false;
		}

		protected override void ViewDidAppear()
		{
			this.extension = base.GetComponent<ISlidesAndLaddersMapView>();
			this.viewAppeared = true;
			this.splines = this.mapStreamer.ScrollPanel.ScrollRoot.GetComponentsInChildren<SlidesAndLaddersSpline>();
			this.mapStreamer.DisableSpawning = this.disableMapSpawning;
		}

		protected override IEnumerator StartupSequence()
		{
			yield break;
		}

		protected override bool LastPlayedLevelBelongsToCurrentMap()
		{
			return base.lastLevelPlayedType == this.LevelType;
		}

		protected override LevelDatabaseCollection LevelDatabaseCollection
		{
			get
			{
				return this.SlidesAndLaddersHandler.MapViewProvider.LevelDatabaseCollection;
			}
		}

		public void UpdateMapDotsToLevel(int levelIndex)
		{
			SlidesAndLaddersLevelDot[] componentsInChildren = base.transform.GetComponentsInChildren<SlidesAndLaddersLevelDot>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (componentsInChildren[i].LevelId <= levelIndex)
				{
					componentsInChildren[i].Initialize();
				}
			}
		}

		public void PlaySlideSound(float soundTriggerTimer)
		{
			this.slideOrLadderSoundFiber.Start(this.PlaySlideOrLadderSound(true, soundTriggerTimer));
		}

		public void PlayLadderSound(float soundTriggerTimer)
		{
			this.slideOrLadderSoundFiber.Start(this.PlaySlideOrLadderSound(false, soundTriggerTimer));
		}

		private IEnumerator PlaySlideOrLadderSound(bool isSlide, float waitTimer)
		{
			if (this.extension != null)
			{
				yield return FiberHelper.Wait(waitTimer, (FiberHelper.WaitFlag)0);
				if (isSlide)
				{
					this.extension.PlaySlideSound(0f);
				}
				else
				{
					this.extension.PlayLadderSound(0f);
				}
			}
			yield break;
		}

		protected override void HandleScrollPanelClicked(MapDotBase mapDot)
		{
			this.OnScrollPanelClicked(mapDot);
		}

		public void UpdateMapDotsAndAvatar()
		{
		}

		protected override MapAvatarController.IDataRetriever GetMapAvatarDataRetriever()
		{
			return new MapAvatarControllerDataRetriever(this.mapStreamer, () => this.FarthestUnlockedLevel.Index);
		}

		protected override void OnAndroidBackButton()
		{
			this.SwitchFromMapViewToMainMap();
		}

		public void SwitchFromMapViewToMainMap()
		{
			if (this.extension != null)
			{
				this.extension.FadeAndSwitchToMainMapView();
			}
		}

		protected override UIViewManager.UIViewState ShowMapButtonView()
		{
			return UIViewManager.Instance.ShowView<SlidesAndLaddersMapButtonView>(new object[0]);
		}

		protected override void SubscribeToFriendsAndSettingsSynced(Action callback)
		{
			this.SlidesAndLaddersHandler.MapViewProvider.SubscribeToFriendsAndSettingsSynced(callback);
		}

		protected override void UnsubscribeToFriendsAndSettingsSynced(Action callback)
		{
			this.SlidesAndLaddersHandler.MapViewProvider.UnsubscribeToFriendsAndSettingsSynced(callback);
		}

		protected override void SubscribeToVIPStateChange(Action<bool> callback)
		{
			this.SlidesAndLaddersHandler.MapViewProvider.SubscribeToVIPStateChange(callback);
		}

		protected override void UnsubscribeToVIPStateChange(Action<bool> callback)
		{
			this.SlidesAndLaddersHandler.MapViewProvider.UnsubscribeToVIPStateChange(callback);
		}

		protected override UIViewManager.UIViewState ShowNoMoreLivesView()
		{
			if (this.extension != null)
			{
				return this.extension.ShowNoMoreLivesView();
			}
			return null;
		}

		public void HandleOutOfLivesPopup()
		{
			FiberCtrl.Pool.Run(base.HandleOutOfLives(), false);
		}

		[SerializeField]
		private bool disableMapSpawning;

		[SerializeField]
		private AnimationCurve ladderCurve;

		[SerializeField]
		private AnimationCurve slideCurve;

		[SerializeField]
		private AnimationCurve avatarMoveCurve;

		[SerializeField]
		private AnimationCurve avatarMoveOffsetCurve;

		public Action<MapDotBase> OnScrollPanelClicked;

		private readonly Fiber slideOrLadderSoundFiber = new Fiber();

		private SlidesAndLaddersHandler handler;

		private ISlidesAndLaddersMapView extension;

		private bool viewAppeared;

		private MapDotBase lastMapDot;

		private SlidesAndLaddersSpline[] splines;

		private LevelDatabaseCollection levelDatabaseCollection;
	}
}
