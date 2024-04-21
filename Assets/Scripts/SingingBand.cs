using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using UnityEngine;

public class SingingBand : MonoBehaviour
{
	public MultiTrack MultiTrack { get; private set; }

	public List<SingingCharacter> Slots
	{
		get
		{
			if (this.cachedSlots == null)
			{
				this.cachedSlots = new List<SingingCharacter>();
				foreach (Instantiator instantiator in this.slotInstantiators)
				{
					this.Slots.Add(instantiator.GetInstance<SingingCharacter>());
				}
			}
			return this.cachedSlots;
		}
	}

	private void Awake()
	{
		if (base.GetComponent<AudioSource>() == null)
		{
			this.audioSource = base.gameObject.AddComponent<AudioSource>();
			this.audioSource.playOnAwake = false;
		}
	}

	private void Update()
	{
		if (this.delayedRunner != null)
		{
			this.delayedRunner.Step();
		}
	}

	public void ConfigureSingers(bool randomize = false, List<string> singerLineup = null)
	{
		if (singerLineup == null)
		{
			singerLineup = SingletonAsset<SingerDatabase>.Instance.GetDefaultBand();
		}
		List<string> list = new List<string>(singerLineup);
		if (randomize)
		{
			list.Shuffle<string>();
		}
		float num = 0f;
		for (int i = 0; i < 4; i++)
		{
			if (i < list.Count)
			{
				this.Slots[i].SetSinger(list[i]);
				num += this.slotInstantiators[i].GetElementSize().x;
			}
			else
			{
				this.slotInstantiators[i].gameObject.SetActive(false);
			}
		}
		base.GetComponent<UIGridLayout>().numColums = list.Count;
		this.GetElement().SetSizeAndDoLayout(new Vector2(num, this.GetElement().Size.y));
	}

	public void SetMultiTrack(MultiTrack track)
	{
		this.MultiTrack = track;
		VoiceAssigner voiceAssigner = new VoiceAssigner(track);
		List<SingingCharacter> list = new List<SingingCharacter>(this.Slots);
		list.Sort(delegate(SingingCharacter a, SingingCharacter b)
		{
			if (a.SingerInfo == null || b.SingerInfo == null)
			{
				return 0;
			}
			if (a.SingerInfo.preferredVoice > b.SingerInfo.preferredVoice)
			{
				return -1;
			}
			if (a.SingerInfo.preferredVoice < b.SingerInfo.preferredVoice)
			{
				return 1;
			}
			return 0;
		});
		for (int i = 0; i < 4; i++)
		{
			SingingCharacter singingCharacter = list[i];
			if (singingCharacter.SingerInfo != null)
			{
				singingCharacter.SetVoice(track, voiceAssigner.GrabVoice(singingCharacter.SingerInfo.preferredVoice));
			}
		}
		this.CheckForDuplicatesAndMute();
		this.audioSource.clip = track.backgroundAudio;
	}

	private void CheckForDuplicatesAndMute()
	{
		HashSet<AudioClip> hashSet = new HashSet<AudioClip>();
		for (int i = 0; i < 4; i++)
		{
			SingingCharacter singingCharacter = this.Slots[i];
			if (!(singingCharacter == null))
			{
				if (hashSet.Contains(singingCharacter.GetComponent<AudioSource>().clip))
				{
					singingCharacter.gameObject.GetComponent<AudioSource>().volume = 0f;
				}
				hashSet.Add(singingCharacter.GetComponent<AudioSource>().clip);
			}
		}
	}

	public void StartSinging(bool looping)
	{
		for (int i = 0; i < 4; i++)
		{
			SingingCharacter singingCharacter = this.Slots[i];
			if (!(singingCharacter == null))
			{
				if (singingCharacter.gameObject.activeInHierarchy)
				{
					singingCharacter.StartSinging(looping);
				}
			}
		}
		if ((AudioManager.Instance.SoundEffectsActive || AudioManager.Instance.MusicActive) && this.audioSource != null)
		{
			this.audioSource.loop = looping;
			this.audioSource.Play();
		}
		this.SynchronizeTracks();
	}

	public bool AnySinging()
	{
		SingingCharacter[] componentsInChildren = base.GetComponentsInChildren<SingingCharacter>(true);
		foreach (SingingCharacter singingCharacter in componentsInChildren)
		{
			AudioSource component = singingCharacter.GetComponent<AudioSource>();
			if (component != null && component.isPlaying)
			{
				return true;
			}
		}
		return false;
	}

	public void StopSinging()
	{
		for (int i = 0; i < 4; i++)
		{
			this.Slots[i].StopSinging();
		}
		this.audioSource.Stop();
	}

	private void SynchronizeTracks()
	{
		SingingCharacter singingCharacter = this.Slots[0];
		for (int i = 1; i < 4; i++)
		{
			SingingCharacter singingCharacter2 = this.Slots[i];
			if (singingCharacter2 != null)
			{
				singingCharacter2.GetComponent<AudioSource>().timeSamples = singingCharacter.GetComponent<AudioSource>().timeSamples;
			}
		}
		if (this.audioSource != null)
		{
			this.audioSource.timeSamples = singingCharacter.GetComponent<AudioSource>().timeSamples;
		}
	}

	public void PrepareForDrop(float worldSpaceY)
	{
		this.orgLocalPos = new List<Vector3>();
		for (int i = 0; i < 4; i++)
		{
			this.orgLocalPos.Add(this.Slots[i].transform.localPosition);
			Vector3 position = this.Slots[i].transform.position;
			position.y = worldSpaceY;
			this.Slots[i].transform.position = position;
			this.Slots[i].Shadow.Alpha = 0f;
		}
	}

	public IEnumerator AnimateDropFromAbove()
	{
		yield return FiberHelper.RunParallel(new IEnumerator[]
		{
			this.AnimateSingleDrop(0),
			this.AnimateSingleDrop(1),
			this.AnimateSingleDrop(2),
			this.AnimateSingleDrop(3)
		});
		yield break;
	}

	private IEnumerator AnimateSingleDrop(int index)
	{
		SingingCharacter character = this.Slots[index];
		float delay = character.Voice.audioStartTime;
		Vector3 dest = this.orgLocalPos[index];
		Vector3 from = character.transform.localPosition;
		character.dropPivot.localScale = new Vector3(0.8f, 1.2f, 1f);
		character.Shadow.Alpha = 0f;
		yield return FiberHelper.Wait(delay, (FiberHelper.WaitFlag)0);
		character.gameObject.SetActive(true);
		yield return FiberAnimation.Animate(0f, this.dropCurve, delegate(float t)
		{
			character.transform.localPosition = FiberAnimation.LerpNoClamp(from, dest, t);
		}, false);
		character.Shadow.Alpha = 1f;
		yield return FiberAnimation.ScaleTransform(character.dropPivot, Vector3.one, new Vector3(1.5f, 0.7f, 1f), this.landCurve, 0f);
		yield break;
	}

	[SerializeField]
	private AnimationCurve dropCurve;

	[SerializeField]
	private AnimationCurve landCurve;

	[SerializeField]
	private List<Instantiator> slotInstantiators = new List<Instantiator>();

	private List<SingingCharacter> cachedSlots;

	private AudioSource audioSource;

	private FiberRunner delayedRunner = new FiberRunner(FiberBucket.Manual);

	private List<Vector3> orgLocalPos;
}
