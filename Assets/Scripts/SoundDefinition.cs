using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SoundDefinition : ISoundDefinition
{
	public static void SetSoundDisablerFunction(Func<bool> func)
	{
		SoundDefinition.soundDisableFunction = func;
	}

	public static SoundDefinition.PitchMode Mode { get; set; }

	public SoundDefinition.SoundData GetRandomSoundData()
	{
		if (this.Additional == null || this.Additional.Count == 0)
		{
			return this.sound;
		}
		if (this.cachedLottery == null)
		{
			this.cachedLottery = new Lottery<SoundDefinition.SoundData>();
			this.cachedLottery.Add(this.sound.Probability, this.sound);
			foreach (SoundDefinition.SoundData soundData in this.Additional)
			{
				this.cachedLottery.Add(soundData.Probability, soundData);
			}
		}
		return this.cachedLottery.PickRandomItem(false);
	}

	public void PlaySequential(int sequenceStep)
	{
		this.sequentialStep = sequenceStep;
		this.PlaySequential();
	}

	public void PlaySequential()
	{
		this.Play(1f + (float)this.sequentialStep * this.sequentialPitch, false);
		this.sequentialStep++;
	}

	public void ResetSequential()
	{
		this.sequentialStep = 0;
	}

	public void PlaySound()
	{
		this.Play(1f, false);
	}

	public void PlaySoundLooped()
	{
		this.Play(1f, true);
	}

	public AudioSource Play()
	{
		return this.Play(1f, false);
	}

	public AudioSource Play(bool forceLoop)
	{
		return this.Play(1f, forceLoop);
	}

	private float FromTonalPitch(float tonalPitch)
	{
		return Mathf.Pow(1.05946314f, tonalPitch - 1f);
	}

	public AudioSource Play(float pitch, bool forceLoop)
	{
		SoundDefinition.SoundData randomSoundData = this.GetRandomSoundData();
		if (randomSoundData == null || randomSoundData.Clip == null)
		{
			return null;
		}
		if (this.cooldownTime > 0f)
		{
			float time = Time.time;
			float num = time - this.lastPlayedTimestamp;
			if (num >= 0f && num < this.cooldownTime)
			{
				return null;
			}
			this.lastPlayedTimestamp = time;
		}
		AudioSource audioSource;
		if (this.sharedPool)
		{
			audioSource = this.CreateOrGetAudioSource(randomSoundData.Clip);
		}
		else
		{
			GameObject gameObject = new GameObject(randomSoundData.Clip.name);
			audioSource = gameObject.AddComponent<AudioSource>();
		}
		audioSource.clip = randomSoundData.Clip;
		audioSource.volume = randomSoundData.Volume;
		if (randomSoundData != this.sound)
		{
			audioSource.volume *= this.sound.Volume;
		}
		if (this.pitchVariance > 0f)
		{
			pitch *= Mathf.Lerp(1f - this.pitchVariance, 1f + this.pitchVariance, UnityEngine.Random.value);
		}
		if (SoundDefinition.Mode == SoundDefinition.PitchMode.Tonal)
		{
			audioSource.pitch = this.FromTonalPitch(pitch * this.pitchForAllVariations);
		}
		else
		{
			audioSource.pitch = pitch * this.pitchForAllVariations;
		}
		audioSource.loop = (randomSoundData.Loop || forceLoop);
		if (SoundDefinition.soundDisableFunction == null || !SoundDefinition.soundDisableFunction())
		{
			audioSource.Play();
		}
		if (!randomSoundData.Loop && !this.sharedPool)
		{
			UnityEngine.Object.Destroy(audioSource.gameObject, randomSoundData.Clip.length / audioSource.pitch);
		}
		return audioSource;
	}

	public bool ReferencesAudioClip(AudioClip clip)
	{
		if (this.sound.Clip == clip)
		{
			return true;
		}
		foreach (SoundDefinition.SoundData soundData in this.Additional)
		{
			if (soundData.Clip == clip)
			{
				return true;
			}
		}
		return false;
	}

	private AudioSource CreateOrGetAudioSource(AudioClip clip)
	{
		int i = 0;
		while (i < SoundDefinition.pool.Count)
		{
			if (SoundDefinition.pool[i] == null || SoundDefinition.pool[i].clip == null)
			{
				SoundDefinition.pool.RemoveAt(i);
			}
			else
			{
				i++;
			}
		}
		if (SoundDefinition.pool.Count >= 2)
		{
			AudioSource audioSource = SoundDefinition.pool[0];
			float num = 0f;
			foreach (AudioSource audioSource2 in SoundDefinition.pool)
			{
				float num2 = audioSource2.time / audioSource2.clip.length;
				if (num2 > num)
				{
					num = num2;
					audioSource = audioSource2;
				}
			}
			SoundDefinition.pool.Remove(audioSource);
			UnityEngine.Object.Destroy(audioSource.gameObject);
		}
		GameObject gameObject = new GameObject();
		AudioSource audioSource3 = gameObject.AddComponent<AudioSource>();
		UnityEngine.Object.Destroy(audioSource3.gameObject, clip.length);
		SoundDefinition.pool.Add(audioSource3);
		return audioSource3;
	}

	private static Func<bool> soundDisableFunction;

	private static int nextNum;

	private static readonly List<AudioSource> pool = new List<AudioSource>();

	private int sequentialStep;

	private float lastPlayedTimestamp;

	private const int MAX_POOL = 2;

	private Lottery<SoundDefinition.SoundData> cachedLottery;

	public SoundDefinition.SoundData sound;

	public List<SoundDefinition.SoundData> Additional = new List<SoundDefinition.SoundData>();

	public float cooldownTime;

	public float pitchVariance;

	public float pitchForAllVariations = 1f;

	public bool sharedPool;

	public float sequentialPitch;

	[Serializable]
	public class SoundData
	{
		[SerializeField]
		public AudioClip Clip;

		[SerializeField]
		public float Volume = 1f;

		[SerializeField]
		public float Probability = 1f;

		[SerializeField]
		public bool Loop;
	}

	public enum PitchMode
	{
		Multiplier,
		Tonal
	}
}
