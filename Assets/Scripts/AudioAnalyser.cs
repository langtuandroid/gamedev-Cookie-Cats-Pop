using System;
using UnityEngine;

public class AudioAnalyser : MonoBehaviour
{
	public AudioSource AudioSource
	{
		get
		{
			if (this.cachedAudioSource == null)
			{
				this.cachedAudioSource = base.GetComponent<AudioSource>();
			}
			return this.cachedAudioSource;
		}
	}

	private void Awake()
	{
		if (base.GetComponent<AudioSource>() == null)
		{
			AudioSource audioSource = base.gameObject.AddComponent<AudioSource>();
			audioSource.playOnAwake = false;
		}
		this.noSoundPhase = UnityEngine.Random.value * 3.14159274f * 2f;
	}

	public void StartSinging(bool looping)
	{
		this.isPlaying = true;
		this.SingingStarted();
		this.cachedAudioSource.loop = looping;
		if (this.IsAudioEnabled)
		{
			this.cachedAudioSource.Play();
		}
		this.timeStarted = Time.timeSinceLevelLoad;
	}

	public void StopSinging()
	{
		this.isPlaying = false;
		this.cachedAudioSource.Stop();
		this.SingingStopped();
	}

	protected virtual void SingingStarted()
	{
	}

	protected virtual void SingingStopped()
	{
	}

	protected virtual void SingingUpdated(float rawInputLevel)
	{
	}

	protected virtual void Update()
	{
		this.UpdateAudioAnalysis();
	}

	protected virtual bool IsAudioEnabled
	{
		get
		{
			return true;
		}
	}

	private float GetLevelUsingRMS()
	{
		this.AudioSource.GetOutputData(this.soundData, 0);
		float num = 0f;
		foreach (float num2 in this.soundData)
		{
			num += num2 * num2;
		}
		num /= (float)this.soundData.Length;
		return Mathf.Clamp01(Mathf.Sqrt(num * 50f));
	}

	private float GetLevelUsingSpectrum()
	{
		this.AudioSource.GetSpectrumData(this.fftData, 0, FFTWindow.Rectangular);
		float num = 0f;
		int num2 = 0;
		for (int i = this.lowFFTBand; i < this.highFFTBand; i++)
		{
			num += this.fftData[i];
			num2++;
		}
		num /= (float)num2;
		return Mathf.Clamp01(num * 10f);
	}

	private float SinePart(float phase, float freq)
	{
		return Mathf.Sin(Time.timeSinceLevelLoad * freq + this.noSoundPhase + phase);
	}

	private void UpdateAudioAnalysis()
	{
		if (!this.AudioSource.isPlaying && this.IsAudioEnabled)
		{
			if (this.isPlaying)
			{
				this.StopSinging();
			}
			return;
		}
		if (!Mathf.Approximately(this.AudioSource.pitch, 1f))
		{
			this.AudioSource.timeSamples = Mathf.FloorToInt((Time.timeSinceLevelLoad - this.timeStarted) * (float)this.AudioSource.clip.frequency);
		}
		float num;
		if (this.IsAudioEnabled)
		{
			num = this.GetLevelUsingSpectrum();
		}
		else
		{
			num = (this.SinePart(0f, 7f) + this.SinePart(1f, 13f) * 2f + this.SinePart(2f, 20f)) / 5f;
		}
		this.SingingUpdated(num);
		global::Debug.DrawLine(base.gameObject.transform.position, base.gameObject.transform.position + Vector3.left * 50f);
		global::Debug.DrawLine(base.gameObject.transform.position, base.gameObject.transform.position + Vector3.left * 50f * num, Color.red);
	}

	public int lowFFTBand;

	public int highFFTBand;

	private float[] soundData = new float[256];

	private float[] fftData = new float[64];

	private float smoothedRMS;

	private bool isPlaying;

	private float timeUntilIntensityCheck;

	private float timeStarted;

	private AudioSource cachedAudioSource;

	private float noSoundPhase;

	public class SmoothingValue
	{
		public SmoothingValue(float responsiveness, float adaptiveResponse, float coolDownThreshold, float coolDownTime)
		{
			this.responsiveness = responsiveness;
			this.coolDownThreshold = coolDownThreshold;
			this.coolDownTime = coolDownTime;
			this.adaptiveResponse = adaptiveResponse;
		}

		public float Value { get; private set; }

		public void Feed(float v)
		{
			if (this.timer > 0f)
			{
				this.timer -= Time.deltaTime;
				return;
			}
			if ((v >= this.coolDownThreshold && this.Value < this.coolDownThreshold) || (v <= this.coolDownThreshold && this.Value > this.coolDownThreshold))
			{
				this.timer = this.coolDownTime;
			}
			float num = this.adaptiveResponse * Mathf.Abs(v - this.Value);
			this.Value += (v - this.Value) * Mathf.Clamp01(Time.deltaTime * (this.responsiveness + num));
		}

		public float responsiveness = 1f;

		public float coolDownThreshold;

		public float coolDownTime;

		public float adaptiveResponse;

		private float timer;
	}
}
