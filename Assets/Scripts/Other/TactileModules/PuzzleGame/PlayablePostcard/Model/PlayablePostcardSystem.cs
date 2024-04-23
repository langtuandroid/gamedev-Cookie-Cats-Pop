using System;

namespace TactileModules.PuzzleGame.PlayablePostcard.Model
{
	public class PlayablePostcardSystem
	{
		public PlayablePostcardSystem(PlayablePostcardHandler handler, PlayablePostcardActivation postcardActivation, IPlayablePostcardProvider provider)
		{
			this.Handler = handler;
			this.PostcardActivation = postcardActivation;
			this.Provider = provider;
		}

		public PlayablePostcardHandler Handler { get; private set; }

		public PlayablePostcardActivation PostcardActivation { get; private set; }

		public IPlayablePostcardProvider Provider { get; private set; }
	}
}
