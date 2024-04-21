using System;
using TactileModules.GameCore.Audio.Assets;
using TactileModules.GameCore.UI;
using TactileModules.PuzzleGames.GameCore;

namespace TactileModules.GameCore.Audio
{
	public class AudioSystemBuilder
	{
		public static AudioSystem Build(IFullScreenManager fullScreenManager, IUIController uiController)
		{
			AssetsModel assets = new AssetsModel();
			MusicTrackStack musicTrackStack = new MusicTrackStack();
			MusicListener musicListener = new MusicListener(musicTrackStack, assets, uiController, fullScreenManager);
			SoundEffectListener soundEffectListener = new SoundEffectListener(uiController, assets);
			AudioStateListener audioStateListener = new AudioStateListener(musicTrackStack, musicListener);
			AudioDatabaseInjector audioDatabaseInjector = new AudioDatabaseInjector(assets);
			if (!audioStateListener.MusicActive)
			{
				musicTrackStack.TurnMusicOff();
				musicListener.TurnMusicOff();
			}
			return new AudioSystem(musicTrackStack, audioStateListener, musicListener, soundEffectListener, audioDatabaseInjector);
		}
	}
}
