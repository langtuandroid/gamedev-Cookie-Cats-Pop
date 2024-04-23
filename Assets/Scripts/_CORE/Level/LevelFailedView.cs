using System;
using JetBrains.Annotations;
using TactileModules.Foundation;
using TactileModules.PuzzleGame.PiggyBank;
using TactileModules.PuzzleGame.PiggyBank.Controllers;
using TactileModules.PuzzleGame.PiggyBank.UI;
using TactileModules.PuzzleGames.GameCore;
using UnityEngine;

public class LevelFailedView : UIView
{
	protected override void ViewLoad(object[] parameters)
	{
		AudioManager.Instance.SetMusic(null, true);
		SingletonAsset<SoundDatabase>.Instance.levelFailed.Play();
		PiggyBankSystem piggyBankSystem = ManagerRepository.Get<PiggyBankSystem>();
		if (piggyBankSystem != null)
		{
			this.piggyBankStateController = piggyBankSystem.ControllerFactory.CreateStateController();
			if (this.IsPiggyBankActive())
			{
				this.piggyBankCollector.GetInstance<PiggyBankCollector>().Activate();
			}
		}
	}

	private bool IsPiggyBankActive()
	{
		return this.piggyBankStateController != null && this.piggyBankStateController.IsActive();
	}

	[UsedImplicitly]
	protected void DismissClicked(UIEvent e)
	{
		base.Close(PostLevelPlayedAction.Exit);
	}

	[UsedImplicitly]
	protected void RetryButtonClicked(UIEvent e)
	{
		base.Close(PostLevelPlayedAction.Retry);
	}

	[SerializeField]
	private UIInstantiator piggyBankCollector;

	private PiggyBankStateController piggyBankStateController;
}
