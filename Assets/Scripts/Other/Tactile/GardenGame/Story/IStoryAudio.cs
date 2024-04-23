using System;

namespace Tactile.GardenGame.Story
{
	public interface IStoryAudio
	{
		void PlayMusic(SoundDefinition soundDefinition);

		void StopMusic();

		void PlaySound(SoundDefinition soundDefinition);
	}
}
