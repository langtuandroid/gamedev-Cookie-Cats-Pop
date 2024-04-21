using System;
using System.Collections;
using System.Diagnostics;
using Fibers;
using TactileModules.PuzzleGames.GameCore;
using UnityEngine;

namespace TactileModules.GameCore.Audio
{
	public class MusicTrack : INotifiedFlow, IFlow, IFiberRunnable
	{
		public MusicTrack(SoundDefinition soundDefinition, MusicTrackData data)
		{
			this.fadeFiber = new Fiber();
			this.data = data;
			this.soundDefinition = soundDefinition;
		}

		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<MusicTrack> OnTrackExit;



		public void Initialize()
		{
			GameObject gameObject = new GameObject("[MusicTrack]");
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			this.source = gameObject.AddComponent<AudioSource>();
			this.source.loop = this.data.Loop;
			this.StartPlaying();
		}

		public void RegisterMusicTrackListener(MusicTrackListener musicTrackListener)
		{
			this.musicTrackListener = musicTrackListener;
			this.musicTrackListener.OnExit += this.TimedExit;
			this.musicTrackListener.OnExitImmediate += this.ExitImmediate;
		}

		public void TurnMusicOn()
		{
			this.muted = false;
			if (this.source != null)
			{
				this.source.volume = this.soundData.Volume;
			}
		}

		public void TurnMusicOff()
		{
			this.muted = true;
			if (this.source != null)
			{
				this.source.volume = 0f;
			}
		}

		public IEnumerator Run()
		{
			while (this.state != MusicTrack.State.Exit)
			{
				yield return FiberHelper.WaitForFrames(1, (FiberHelper.WaitFlag)0);
				if (this.data.ExitOnDone && !this.IsExiting() && !this.source.isPlaying)
				{
					this.TimedExit();
				}
				else if (this.state == MusicTrack.State.Running && !this.source.isPlaying && this.source.loop)
				{
					this.StartPlaying();
				}
			}
			yield break;
		}

		public void Enter(IFlow previousFlow)
		{
			this.isTopTrack = true;
			if (this.IsExiting())
			{
				this.state = MusicTrack.State.Exit;
				return;
			}
			if (!this.source.isPlaying)
			{
				this.source.UnPause();
			}
			this.state = MusicTrack.State.Running;
			this.fadeFiber.Terminate();
			this.fadeFiber.Start(this.FadeIn(this.soundData.Volume));
		}

		public void Leave(IFlow nextFlow)
		{
			this.isTopTrack = false;
			if (this.IsExiting())
			{
				return;
			}
			if (this.source.isPlaying)
			{
				if (this.data.ExitOnDone)
				{
					this.TimedExit();
				}
				else
				{
					this.fadeFiber.Terminate();
					this.fadeFiber.Start(this.FadeOutAndPause(this.soundData.Volume));
					this.state = MusicTrack.State.Paused;
				}
			}
		}

		private void StartPlaying()
		{
			this.soundData = this.soundDefinition.GetRandomSoundData();
			this.source.volume = ((!this.muted) ? this.soundData.Volume : 0f);
			this.source.clip = this.soundData.Clip;
			this.source.Play();
			this.state = MusicTrack.State.Running;
		}

		private IEnumerator FadeOutAndPause(float volume)
		{
			yield return this.FadeOut(volume);
			this.source.Pause();
			yield break;
		}

		private IEnumerator FadeOut(float volume)
		{
			yield return FiberAnimation.Animate(this.data.FadeOutDuration, delegate(float v)
			{
				if (this.muted)
				{
					this.source.volume = 0f;
				}
				else
				{
					this.source.volume = volume * (1f - v);
				}
			});
			yield break;
		}

		private IEnumerator FadeIn(float volume)
		{
			yield return FiberAnimation.Animate(this.data.FadeInDuration, delegate(float v)
			{
				if (this.muted)
				{
					this.source.volume = 0f;
				}
				else
				{
					this.source.volume = volume * v;
				}
			});
			yield break;
		}

		private IEnumerator FadeOutExit()
		{
			yield return this.FadeOut(this.source.volume);
			this.DestroySource();
			if (this.isTopTrack)
			{
				this.state = MusicTrack.State.Exit;
			}
			yield break;
		}

		private void TimedExit()
		{
			if (this.IsExiting())
			{
				return;
			}
			this.state = MusicTrack.State.IsExiting;
			this.fadeFiber.Terminate();
			this.fadeFiber.Start(this.FadeOutExit());
		}

		private void ExitImmediate()
		{
			this.DestroySource();
			this.state = MusicTrack.State.Exit;
		}

		private bool IsExiting()
		{
			return this.state == MusicTrack.State.Exit || this.state == MusicTrack.State.IsExiting || this.source == null;
		}

		public void OnExit()
		{
			if (this.musicTrackListener != null)
			{
				this.musicTrackListener.OnExit -= this.TimedExit;
				this.musicTrackListener.OnExitImmediate -= this.ExitImmediate;
			}
			if (this.OnTrackExit != null)
			{
				this.OnTrackExit(this);
			}
		}

		private void DestroySource()
		{
			this.fadeFiber.Terminate();
			if (this.source != null)
			{
				this.source.Stop();
				UnityEngine.Object.Destroy(this.source.gameObject);
			}
		}

		private readonly SoundDefinition soundDefinition;

		private readonly MusicTrackData data;

		private readonly Fiber fadeFiber;

		private AudioSource source;

		private MusicTrackListener musicTrackListener;

		private SoundDefinition.SoundData soundData;

		private bool isTopTrack;

		private bool muted;

		private float maxVolume;

		private MusicTrack.State state;

		private enum State
		{
			Running,
			Paused,
			IsExiting,
			Exit
		}
	}
}
