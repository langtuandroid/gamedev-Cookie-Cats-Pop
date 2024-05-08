using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Fibers;
using TactileModules.PuzzleGame.SlidesAndLadders.Data;
using TactileModules.PuzzleGame.SlidesAndLadders.Model;
using TactileModules.PuzzleGame.SlidesAndLadders.UI;
using TactileModules.PuzzleGame.SlidesAndLadders.Views;
using TactileModules.PuzzleGames.GameCore;
using TactileModules.PuzzleGames.Lives;
using TactileModules.SagaCore;
using UnityEngine;

namespace TactileModules.PuzzleGame.SlidesAndLadders.Controllers
{
	public class SlidesAndLaddersMapFlow : MapFlow
	{
		public SlidesAndLaddersMapFlow(MapIdentifier mapIdentifier, MapFacade mapFacade, IFullScreenManager fullScreenManager, IFlowStack flowStack, ISlidesAndLaddersRewards rewards, ILivesManager lives, ISlidesAndLaddersFeatureProgression featureProgression, ISlidesAndLaddersControllerFactory controllerFactory, ISlidesAndLaddersLevelDatabase levelDatabase) : base(mapIdentifier, mapFacade, fullScreenManager, flowStack)
		{
			this.lives = lives;
			this.featureProgression = featureProgression;
			this.controllerFactory = controllerFactory;
			this.levelDatabase = levelDatabase;
			this.rewardController = controllerFactory.CreateRewardController();
		}

		private void HandleFlowStackChanged(IFlow newFlow, IFlow oldFlow)
		{
			if (newFlow == this)
			{
				SlidesAndLaddersPlayFlow slidesAndLaddersPlayFlow = oldFlow as SlidesAndLaddersPlayFlow;
				if (slidesAndLaddersPlayFlow != null)
				{
					if (this.OnGameSessionReceived != null)
					{
						this.OnGameSessionReceived(slidesAndLaddersPlayFlow.PlayedLevelSessionId, slidesAndLaddersPlayFlow.PlayedLevelNumber);
					}
					if (this.OnLevelResultFlowCompleted != null)
					{
						this.OnLevelResultFlowCompleted();
					}
				}
			}
		}

		protected override IEnumerator PostPlayFlowSequence(int nextDotIndexToOpen)
		{
			this.wheelController.RefreshState();
			if (this.featureProgression.HasShownTutorial)
			{
				yield return this.LevelResultFlow();
			}
			else
			{
				SagaMapView sagaMapView = UIViewManager.Instance.FindView<SagaMapView>();
				UIScrollablePanel scrollPanel = sagaMapView.MapStreamer.ScrollPanel;
				SlidesAndLaddersWheelTutorialController slidesAndLaddersWheelTutorialController = this.controllerFactory.CreateTutorialController(this.buttonView.SlidesAndLaddersWheelWidget, scrollPanel, this.buttonView);
				slidesAndLaddersWheelTutorialController.StartTutorial();
			}
			yield break;
		}

		protected override IEnumerator AfterScreenAcquired()
		{
			base.FlowStack.Changed += this.HandleFlowStackChanged;
			this.buttonView = this.SetUpWheelController();
			this.buttonView.ExitClicked += this.HandleExitClicked;
			this.splines = base.MapContentController.ContentRoot.GetComponentsInChildren<SlidesAndLaddersSpline>();
			this.viewExtension = base.MapContentController.MapView.GetComponent<ISlidesAndLaddersMapView>();
			yield break;
		}

		private void HandleExitClicked()
		{
			base.EndThisFlow();
		}

		private SlidesAndLaddersMapButtonView SetUpWheelController()
		{
			UIViewManager.UIViewStateGeneric<SlidesAndLaddersMapButtonView> uiviewStateGeneric = UIViewManager.Instance.ShowView<SlidesAndLaddersMapButtonView>(new object[0]);
			this.wheelController = this.controllerFactory.CreateWheelController(uiviewStateGeneric.View.SlidesAndLaddersWheelWidget);
			SlidesAndLaddersWheelWidgetController slidesAndLaddersWheelWidgetController = this.wheelController;
			slidesAndLaddersWheelWidgetController.OnAnimatingIsDone = (Action<WheelSlot>)Delegate.Combine(slidesAndLaddersWheelWidgetController.OnAnimatingIsDone, new Action<WheelSlot>(this.HandleWheelRotated));
			return uiviewStateGeneric.View;
		}

		private void HandleWheelRotated(WheelSlot wheelSlot)
		{
			this.HandleWheelSpinEvent(wheelSlot);
			if (wheelSlot.IsReward())
			{
				this.GiveWheelRewards(wheelSlot);
			}
			else
			{
				UICamera.DisableInput();
				this.MoveAvatarOnWheelSpin(wheelSlot);
			}
		}

		private IEnumerator LevelResultFlow()
		{
			yield return this.CheckForLevelRewards();
			yield return this.CheckForSlidesAndLadders();
			yield return this.CheckIfFeatureIsCompleted(null);
			if (this.OnLevelResultFlowCompleted != null)
			{
				this.OnLevelResultFlowCompleted();
			}
			if (this.IsFeatureCompleted())
			{
				yield break;
			}
			this.ResetWheelOnCompleted();
			this.featureProgression.SetResultState(ResultState.None);
			yield break;
		}

		private void HandleValidLevelClicked(int levelId)
		{
			ILevelProxy levelProxy = this.levelDatabase.GetLevelProxy(levelId);
			bool flag = !this.lives.IsOutOfLives() || this.lives.HasUnlimitedLives();
			if (flag && !this.featureProgression.IsReadyToPlayLevel(levelProxy))
			{
				this.wheelController.PlayWheelAvailableEffect();
			}
			else if (flag)
			{
				base.StartFlowForDot(this.CurrentLevelIndex);
			}
		}

		private void ResetWheelOnCompleted()
		{
			if (this.featureProgression.ResultState == ResultState.Completed)
			{
				this.wheelController.Reset();
				this.wheelController.PlayWheelAvailableEffect();
			}
		}

		private IEnumerator CheckForLevelRewards()
		{
			if (this.featureProgression.ResultState == ResultState.Completed)
			{
				yield return this.rewardController.CheckForLevelRewards(this.CurrentLevelIndex);
				base.MapContentController.Refresh();
			}
			yield break;
		}

		private IEnumerator CheckForSlidesAndLadders()
		{
			if (this.featureProgression.ResultState == ResultState.None)
			{
				yield break;
			}
			ILevelProxy current = this.levelDatabase.GetLevelProxy(this.CurrentLevelIndex);
			ILevelProxy nextLevelToPlay = this.levelDatabase.NextLevel(this.levelDatabase.GetLevelProxy(this.CurrentLevelIndex), this.featureProgression.ResultState == ResultState.Completed);
			bool isOnASlideOrLadder = current.Index != nextLevelToPlay.Index;
			if (isOnASlideOrLadder)
			{
				this.HandleSlideOrLadderEvents(current, nextLevelToPlay);
				yield return this.HandleClimbOrSlide(current, nextLevelToPlay);
			}
			yield break;
		}

		private IEnumerator HandleClimbOrSlide(ILevelProxy current, ILevelProxy nextLevelToPlay)
		{
			this.FarthestUnlockedLevelIndex = nextLevelToPlay.Index;
			base.MapContentController.Refresh();
			yield return this.ClimbOrSlideAvatar(current, nextLevelToPlay);
			yield return this.HandleOnClimbOrSlideDone(current.Index > nextLevelToPlay.Index);
			this.CurrentLevelIndex = this.FarthestUnlockedLevelIndex;
			yield return null;
			yield break;
		}

		private IEnumerator HandleOnClimbOrSlideDone(bool isSlide)
		{
			yield return this.CheckForSlideRewards(isSlide);
			if (isSlide)
			{
				this.wheelController.Reset();
				this.wheelController.PlayWheelAvailableEffect();
			}
			yield return FiberHelper.Wait(0.3f, (FiberHelper.WaitFlag)0);
			yield break;
		}

		private IEnumerator CheckForSlideRewards(bool isSlide)
		{
			if (isSlide)
			{
				yield return this.rewardController.AddSlideRewardsToFeatureRewards(base.MapContentController.MapView.MapStreamer, this.splines, this.CurrentLevelIndex, this.FarthestUnlockedLevelIndex, this.levelDatabase.GetLevelProxy(this.levelDatabase.NumberOfLevels - 1).Index);
			}
			yield break;
		}

		public IEnumerator ClimbOrSlideAvatar(ILevelProxy from, ILevelProxy to)
		{
			bool isSlide = to.HumanNumber < from.HumanNumber;
			foreach (SlidesAndLaddersSpline spline in this.splines)
			{
				if ((isSlide && spline.name.Contains(SlidesAndLaddersHelperMethods.GetIdForSlide(from.HumanNumber))) || (!isSlide && spline.name.Contains(SlidesAndLaddersHelperMethods.GetIdForLadder(from.HumanNumber))))
				{
					float durationFactor = spline.Spline.FullLength / 150f;
					this.HandleClimbOrLadderSound(isSlide, durationFactor);
					yield return this.RunAvatarAnimation(isSlide, to, spline, durationFactor);
					base.MapContentController.Refresh();
					yield break;
				}
			}
			yield break;
		}

		private void HandleClimbOrLadderSound(bool isSlide, float durationFactor)
		{
			float num = (!isSlide) ? 3.75f : 2.75f;
			float soundTriggerTimer = durationFactor / num;
			this.PlaySlideOrLadderSound(isSlide, soundTriggerTimer);
		}

		private void PlaySlideOrLadderSound(bool isSlide, float soundTriggerTimer)
		{
			if (isSlide)
			{
				this.viewExtension.PlaySlideSound(soundTriggerTimer);
			}
			else
			{
				this.viewExtension.PlayLadderSound(soundTriggerTimer);
			}
		}

		private IEnumerator RunAvatarAnimation(bool isSlide, ILevelProxy to, SlidesAndLaddersSpline spline, float durationFactor)
		{
			MapAvatare avatar = base.MapContentController.Avatars.GetMeAvatar();
			Vector3 startPosition = avatar.transform.localPosition;
			AnimationCurve animCurve = null;
			UICamera.DisableInput();
			yield return FiberAnimation.Animate(durationFactor, animCurve, delegate(float t)
			{
				avatar.transform.localPosition = this.FollowSpline(t, spline, startPosition);
			}, false);
			UICamera.EnableInput();
			yield break;
		}

		private Vector3 FollowSpline(float t, SlidesAndLaddersSpline spline, Vector3 startPosition)
		{
			float num = 0.03f;
			if (t < num)
			{
				return this.FollowSplineFirstPart(num, t, spline, startPosition);
			}
			float num2 = (t - num) / (1f - num);
			Vector2 vector = spline.Spline.Evaluate(num2 * spline.Spline.FullLength);
			return new Vector3(vector.x, vector.y, 0f) + spline.transform.localPosition;
		}

		private Vector3 FollowSplineFirstPart(float firstPart, float t, SlidesAndLaddersSpline spline, Vector3 startPosition)
		{
			Vector2 vector = spline.Spline.Evaluate(0f * spline.Spline.FullLength);
			Vector3 b = new Vector3(vector.x, vector.y, 0f) + spline.transform.localPosition;
			return Vector3.Lerp(startPosition, b, t / firstPart);
		}

		private void HandleSlideOrLadderEvents(ILevelProxy current, ILevelProxy nextLevelToPlay)
		{
			bool flag = current.Index < nextLevelToPlay.Index;
			int num = Mathf.Abs(current.Index - nextLevelToPlay.Index);
			if (flag && this.OnLaddderUsed != null)
			{
				this.OnLaddderUsed(num);
			}
			else if (this.OnSlideUsed != null)
			{
				this.OnSlideUsed(num);
			}
		}

		private void MoveAvatarOnWheelSpin(WheelSlot wheelSlot)
		{
			this.featureProgression.SetResultState(ResultState.WheelSpin);
			this.moveStepsFiber.Start(this.MoveAvatarOnWheelSpin(wheelSlot.stepsToAdd));
			this.cameraFollowFiber.Start(this.CameraFollow(wheelSlot.stepsToAdd));
		}

		private IEnumerator MoveAvatarOnWheelSpin(int mapSteps)
		{
			ILevelProxy currentLevelProxy = this.levelDatabase.GetLevelProxy(this.CurrentLevelIndex);
			ILevelProxy endProxy = this.NextLevel(currentLevelProxy, mapSteps);
			this.FarthestUnlockedLevelIndex = endProxy.Index;
			yield return FiberHelper.Wait(0.4f, (FiberHelper.WaitFlag)0);
			yield return this.WheelResultFlow();
			yield break;
		}

		private IEnumerator WheelResultFlow()
		{
			yield return this.MoveSteps(this.levelDatabase.GetLevelProxy(this.CurrentLevelIndex), this.levelDatabase.GetLevelProxy(this.FarthestUnlockedLevelIndex));
			this.CurrentLevelIndex = this.FarthestUnlockedLevelIndex;
			if (SlidesAndLaddersMapFlow._003C_003Ef__mg_0024cache0 == null)
			{
				SlidesAndLaddersMapFlow._003C_003Ef__mg_0024cache0 = new Action(UICamera.EnableInput);
			}
			yield return this.CheckIfFeatureIsCompleted(SlidesAndLaddersMapFlow._003C_003Ef__mg_0024cache0);
			if (this.IsFeatureCompleted())
			{
				yield break;
			}
			this.featureProgression.SetResultState(ResultState.None);
			yield return FiberHelper.Wait(0.4f, (FiberHelper.WaitFlag)0);
			this.HandleValidLevelClicked(this.CurrentLevelIndex);
			UICamera.EnableInput();
			yield break;
		}

		private IEnumerator MoveSteps(ILevelProxy from, ILevelProxy to)
		{
			MapAvatare avatar = base.MapContentController.Avatars.GetMeAvatar();
			yield return avatar.AnimateFramePivotToNewSide(MapAvatare.BackgroundSide.None);
			yield return FiberHelper.Wait(0.2f, (FiberHelper.WaitFlag)0);
			for (int i = from.Index; i < to.Index; i++)
			{
				yield return base.MapContentController.Avatars.MoveAvatar("me", i, i + 1, new SagaAvatarController.AvatarMoveAnimation(this.CustomAvatarAnimation));
			}
			yield return FiberHelper.Wait(0.2f, (FiberHelper.WaitFlag)0);
			yield return avatar.AnimateFramePivotToDefaultSide();
			base.MapContentController.Refresh();
			yield break;
		}

		private IEnumerator CustomAvatarAnimation(MapAvatare avatar, Vector3 fromPos, Vector3 toPos)
		{
			SlidesAndLaddersMapCurves curves = base.MapContentController.MapView.GetComponent<SlidesAndLaddersMapCurves>();
			yield return FiberAnimation.Animate(curves.AvatarMoveCurve.Duration(), delegate(float t)
			{
				Vector3 a = FiberAnimation.LerpNoClamp(fromPos, toPos, t);
				Vector3 b = Vector3.up * 100f * curves.AvatarMoveOffsetCurve.Evaluate(t);
				avatar.transform.localPosition = a + b;
			});
			yield break;
		}

		private IEnumerator CheckIfFeatureIsCompleted(Action OnViewDidAppear = null)
		{
			if (this.featureProgression.IsLevelIndexEndChest(this.CurrentLevelIndex))
			{
				this.featureProgression.CompletedFeature = true;
				UIViewManager.UIViewState vs = this.rewardController.AddFeatureRewardsToInventory();
				if (OnViewDidAppear != null)
				{
					SlidesAndLaddersRewardView slidesAndLaddersRewardView = (SlidesAndLaddersRewardView)vs.View;
					slidesAndLaddersRewardView.OnViewDidAppear = (Action)Delegate.Combine(slidesAndLaddersRewardView.OnViewDidAppear, OnViewDidAppear);
				}
				yield return vs.WaitForClose();
			}
			yield break;
		}

		private bool IsFeatureCompleted()
		{
			if (this.featureProgression.CompletedFeature)
			{
				base.EndThisFlow();
				return true;
			}
			return false;
		}

		private IEnumerator CameraFollow(int mapSteps)
		{
			yield return FiberHelper.Wait(0.4f, (FiberHelper.WaitFlag)0);
			ILevelProxy endProxy = this.NextLevel(this.levelDatabase.GetLevelProxy(this.CurrentLevelIndex), mapSteps);
			yield return base.MapContentController.PanToDot(endProxy.Index, (float)(mapSteps - 1) * 0.35f + 0.9f, null);
			yield break;
		}

		private void HandleWheelSpinEvent(WheelSlot wheelSlot)
		{
			ILevelProxy levelProxy = this.levelDatabase.GetLevelProxy(this.CurrentLevelIndex);
			ILevelProxy levelProxy2 = this.NextLevel(levelProxy, wheelSlot.stepsToAdd);
			if (this.OnWheelSpinResult != null)
			{
				this.OnWheelSpinResult(wheelSlot, this.CurrentLevelIndex, levelProxy2.Index, this.featureProgression.FeatureSpinCount());
			}
		}

		private void GiveWheelRewards(WheelSlot wheelSlot)
		{
			this.featureProgression.SetResultState(ResultState.None);
			this.rewardController.AddWheelRewardToInventory(wheelSlot);
		}

		private ILevelProxy NextLevel(ILevelProxy current, int steps)
		{
			if (current.Index == this.levelDatabase.NumberOfLevels - 1)
			{
				return current;
			}
			ILevelProxy levelProxy = current;
			for (int i = 0; i < steps; i++)
			{
				levelProxy = levelProxy.NextLevel;
				if (levelProxy.Index == this.levelDatabase.NumberOfLevels - 1)
				{
					return levelProxy;
				}
			}
			return levelProxy;
		}

		protected override void AfterScreenLost()
		{
			base.FlowStack.Changed -= this.HandleFlowStackChanged;
		}

		protected override int GetFarthestUnlockedDotIndex()
		{
			return this.FarthestUnlockedLevelIndex;
		}

		protected override IFlow CreateFlowForDot(int dotIndex)
		{
			ILevelProxy levelProxy = this.levelDatabase.GetLevelProxy(dotIndex);
			if (this.featureProgression.IsReadyToPlayLevel(levelProxy))
			{
				return this.controllerFactory.CreatePlayFlow((LevelProxy)levelProxy);
			}
			return null;
		}

		public override SagaAvatarInfo CreateMeAvatarInfo()
		{
			return new SagaAvatarInfo
			{
				dotIndex = this.FarthestUnlockedLevelIndex
			};
		}

		public override Dictionary<CloudUser, SagaAvatarInfo> CreateFriendsAvatarInfos()
		{
			return null;
		}

		private int CurrentLevelIndex
		{
			get
			{
				return this.featureProgression.CurrentLevelIndex;
			}
			set
			{
				this.featureProgression.CurrentLevelIndex = value;
			}
		}

		private int FarthestUnlockedLevelIndex
		{
			get
			{
				return this.featureProgression.FarthestUnlockedLevelIndex;
			}
			set
			{
				this.featureProgression.FarthestUnlockedLevelIndex = value;
			}
		}

		private const float WAIT_TO_MOVE = 0.4f;

		private const float WAIT_TO_SHOW_LEVELSTART = 0.4f;

		private const float MAP_STEPS_DURATION = 0.35f;

		private const float LAST_MAP_STEP_DURATION = 0.9f;

		public SlidesAndLaddersMapFlow.OnLadderUsedEvent OnLaddderUsed;

		private readonly ILivesManager lives;

		private readonly ISlidesAndLaddersFeatureProgression featureProgression;

		private readonly ISlidesAndLaddersControllerFactory controllerFactory;

		private readonly ISlidesAndLaddersLevelDatabase levelDatabase;

		private ISlidesAndLaddersMapView viewExtension;

		private SlidesAndLaddersMapButtonView buttonView;

		private readonly Fiber moveStepsFiber = new Fiber();

		private readonly Fiber cameraFollowFiber = new Fiber();

		private readonly SlidesAndLaddersRewardController rewardController;

		private SlidesAndLaddersWheelWidgetController wheelController;

		private SlidesAndLaddersSpline[] splines;

		public SlidesAndLaddersMapFlow.OnSlideUsedEvent OnSlideUsed;

		public SlidesAndLaddersMapFlow.OnWheelSpinResultEvent OnWheelSpinResult;

		public SlidesAndLaddersMapFlow.OnLevelResultCompletedEvent OnLevelResultFlowCompleted;

		public SlidesAndLaddersMapFlow.OnGameSessionReceivedEvent OnGameSessionReceived;

		[CompilerGenerated]
		private static Action _003C_003Ef__mg_0024cache0;

		public delegate void OnLadderUsedEvent(int mapStepsMovedUp);

		public delegate void OnSlideUsedEvent(int mapStepsMovedDown);

		public delegate void OnWheelSpinResultEvent(WheelSlot result, int levelIndexBeforeSpin, int levelIndexBeforeSpinning, int featureSpinCount);

		public delegate void OnLevelResultCompletedEvent();

		public delegate void OnGameSessionReceivedEvent(string sessionId, int levelNumber);
	}
}
