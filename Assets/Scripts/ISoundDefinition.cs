using System;

public interface ISoundDefinition
{
	SoundDefinition.SoundData GetRandomSoundData();

	void PlaySequential(int sequenceStep);

	void PlaySequential();

	void PlaySound();

	void PlaySoundLooped();
}
