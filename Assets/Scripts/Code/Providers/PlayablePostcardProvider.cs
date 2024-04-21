using System;
using TactileModules.PuzzleGame.PlayablePostcard.Model;

namespace Code.Providers
{
	public class PlayablePostcardProvider : IPlayablePostcardProvider
	{
		public void Save()
		{
			PuzzleGame.UserSettings.SaveLocal();
		}
	}
}
