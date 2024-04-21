using System;
using System.Collections;
using System.Collections.Generic;
using TactileModules.PuzzleCore.LevelPlaying;

public class LevelStartViewController
{
	public LevelStartViewController(IAssetFactory assetFactory, IViewPresenter viewPresenter)
	{
		this.assetFactory = assetFactory;
		this.viewPresenter = viewPresenter;
	}

	public IEnumerator WaitForLevelStartView(LevelProxy level, bool isRetrying, ILevelStartInfo startInfo, ICorePlayFlow corePlayFlow)
	{
		this.startInfo = startInfo;
		this.startInfo.DidStart = false;
		this.level = level;
		this.corePlayFlow = corePlayFlow;
		this.startView = this.assetFactory.CreateLevelStartView();
		this.startView.PlayButtonClicked += this.HandlePlayButtonClicked;
		this.startView.DismissButtonClicked += this.HandleDismissButtonClicked;
		this.startView.Developer_CheatCompleteClicked += this.HandleCheatCompleteClicked;
		this.startView.Initialize(level, isRetrying, corePlayFlow.PlayFlowContext);
		UIViewManager.IUIViewStateGeneric<ILevelStartView> viewState = this.viewPresenter.ShowViewInstance<ILevelStartView>(this.startView, new object[0]);
		yield return corePlayFlow.StartViewInitializedHook.InvokeAll(level, isRetrying);
		yield return viewState.WaitForClose();
		yield break;
	}

	private void HandleCheatCompleteClicked(int numStars)
	{
	}

	private void HandleDismissButtonClicked()
	{
		this.startInfo.DidStart = false;
		this.startView.Close(0);
	}

	private void HandlePlayButtonClicked(List<SelectedBooster> selectedBoosters)
	{
		this.startInfo.SelectedPregameBoosters.Clear();
		this.startInfo.SelectedPregameBoosters.AddRange(selectedBoosters);
		this.startInfo.DidStart = true;
		this.startView.Close(0);
	}

	private readonly IAssetFactory assetFactory;

	private readonly IViewPresenter viewPresenter;

	private ILevelStartInfo startInfo;

	private ILevelStartView startView;

	private LevelProxy level;

	private ICorePlayFlow corePlayFlow;
}
