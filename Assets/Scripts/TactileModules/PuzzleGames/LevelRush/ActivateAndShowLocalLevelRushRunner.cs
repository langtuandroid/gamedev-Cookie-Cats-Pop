using System;
using System.Collections;
using Fibers;
using UnityEngine;

namespace TactileModules.PuzzleGames.LevelRush
{
	public class ActivateAndShowLocalLevelRushRunner : IFiberRunnable
	{
		public ActivateAndShowLocalLevelRushRunner(ILevelRushActivation levelRushActivation, IMainMapPlugin mainMapPlugin, IAssetModel assetModel)
		{
			this.levelRushActivation = levelRushActivation;
			this.mainMapPlugin = mainMapPlugin;
			this.assetModel = assetModel;
		}

		public IEnumerator Run()
		{
			this.levelRushActivation.ActivateLocalLevelRush();
			yield return this.ShowLevelRushStartedView(false);
			yield return this.mainMapPlugin.DropPresents();
			yield break;
		}

		private IEnumerator ShowLevelRushStartedView(bool isReminder)
		{
			LevelRushStartView view = UnityEngine.Object.Instantiate<LevelRushStartView>(this.assetModel.LevelRushStartView);
			UIViewManager.IUIViewStateGeneric<LevelRushStartView> vs = UIViewManager.Instance.ShowViewInstance<LevelRushStartView>(view, new object[]
			{
				isReminder
			});
			yield return vs.WaitForClose();
			yield break;
		}

		public void OnExit()
		{
		}

		private readonly ILevelRushActivation levelRushActivation;

		private readonly IMainMapPlugin mainMapPlugin;

		private readonly IAssetModel assetModel;
	}
}
