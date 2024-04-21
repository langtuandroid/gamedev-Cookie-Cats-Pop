using System;
using System.Collections;
using Fibers;
using TactileModules.PuzzleGame.PlayablePostcard.Controllers;
using TactileModules.PuzzleGame.PlayablePostcard.Model;
using TactileModules.PuzzleGame.PlayablePostcard.Module.Controllers;
using TactileModules.PuzzleGames.GameCore;

public class PlayablePostcardEndedFlow : IFlow, IFiberRunnable
{
	public PlayablePostcardEndedFlow(PlayablePostcardControllerFactory controllerFactory, PlayablePostcardProgress progress)
	{
		this.controllerFactory = controllerFactory;
		this.progress = progress;
	}

	public IEnumerator Run()
	{
		PhotoBoothController photoBoothControllerResult = this.controllerFactory.CreatePhotoBoothController(this.progress);
		yield return photoBoothControllerResult.ShowResultPostcard();
		yield break;
	}

	public void OnExit()
	{
	}

	private readonly PlayablePostcardControllerFactory controllerFactory;

	private readonly PlayablePostcardProgress progress;
}
