using System;
using System.Collections;
using System.Collections.Generic;
using TactileModules.PuzzleGame.PlayablePostcard.Model;
using TactileModules.PuzzleGame.PlayablePostcard.Module.Controllers;
using TactileModules.PuzzleGame.PlayablePostcard.Views;
using TactileModules.PuzzleGames.GameCore;
using TactileModules.SagaCore;

namespace TactileModules.PuzzleGame.PlayablePostcard.Controllers
{
	public class PlayablePostcardMapFlow : MapFlow
	{
		public PlayablePostcardMapFlow(PlayablePostcardLevelDatabase levelDatabase, PlayablePostcardProgress progress, MapFacade mapFacade, IFullScreenManager fullScreenManager, IFlowStack flowStack, PlayablePostcardControllerFactory controllerFactory, IUIViewManager viewManager) : base("PlayablePostcard", mapFacade, fullScreenManager, flowStack)
		{
			this.levelDatabase = levelDatabase;
			this.progress = progress;
			this.controllerFactory = controllerFactory;
			this.viewManager = viewManager;
		}

		protected override IEnumerator PostPlayFlowSequence(int nextDotIndexToOpen)
		{
			if (this.progress.HasCompletedPostcard())
			{
				base.EndThisFlow();
				yield break;
			}
			yield return base.AnimateAvatarProgressIfAny();
			base.StartFlowForDot(this.GetFarthestUnlockedDotIndex());
			yield break;
		}

		protected override IEnumerator AfterScreenAcquired()
		{
			UIViewManager.UIViewStateGeneric<PlayablePostcardMapButtonView> uiviewStateGeneric = this.viewManager.ShowView<PlayablePostcardMapButtonView>(new object[0]);
			uiviewStateGeneric.View.OnExitButtonClicked += base.EndThisFlow;
			yield break;
		}

		protected override void AfterScreenLost()
		{
		}

		protected override int GetFarthestUnlockedDotIndex()
		{
			return this.progress.GetFarthestCompletedLevelIndex() + 1;
		}

		protected override IFlow CreateFlowForDot(int dotIndex)
		{
			if (dotIndex == this.GetFarthestUnlockedDotIndex())
			{
				LevelProxy level = this.levelDatabase.GetLevel(dotIndex);
				return this.controllerFactory.CreatePlayFlow(level);
			}
			return null;
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
			return null;
		}

		private readonly PlayablePostcardLevelDatabase levelDatabase;

		private readonly PlayablePostcardProgress progress;

		private readonly PlayablePostcardControllerFactory controllerFactory;

		private readonly IUIViewManager viewManager;

		private bool comingFromLevel;
	}
}
