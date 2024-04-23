using System;
using TactileModules.PuzzleGame.PlayablePostcard.Controllers;

namespace TactileModules.PuzzleGame.PlayablePostcard
{
	public interface IPlayablePostcardControllerFactory
	{
		PlayablePostcardMapFlow CreateAndPushMapFlow();

		PlayablePostcardPlayFlow CreatePlayFlow(LevelProxy levelProxy);

		PlayablePostcardEndedFlow CreateAndPushEndedFlow();
	}
}
