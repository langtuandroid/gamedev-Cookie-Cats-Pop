using System;
using System.Collections;
using System.Collections.Generic;
using TactileModules.GameCore.Audio.Assets;
using TactileModules.GameCore.UI;
using TactileModules.PuzzleGames.GameCore;

namespace TactileModules.GameCore.Audio
{
	public class MusicListener
	{
		public MusicListener(IMusicTrackStack musicTrackStack, IAssetsModel assets, IUIController uiController, IFullScreenManager fullScreenManager)
		{
			this.musicTrackStack = musicTrackStack;
			this.assets = assets;
			this.uiController = uiController;
			this.fullScreenManager = fullScreenManager;
			this.viewTracks = new Dictionary<IUIView, MusicTrackListener>();
			this.fullScreenTracks = new Dictionary<string, MusicTrackListener>();
			this.HookEvents();
		}

		private void HookEvents()
		{
			this.uiController.ViewCreated += this.ViewCreated;
			this.uiController.ViewWillDisappear += this.ViewDestroyed;
			this.fullScreenManager.PreChange.Register(new Func<ChangeInfo, IEnumerator>(this.OnPreChangeFullScreen));
			this.fullScreenManager.PostChange.Register(new Func<ChangeInfo, IEnumerator>(this.OnPostChangeFullScreen));
		}

		private void ViewCreated(IUIView view)
		{
			ViewMusicTrack component = view.gameObject.GetComponent<ViewMusicTrack>();
			if (component != null)
			{
				MusicTrackListener musicTrackListener = new MusicTrackListener();
				MusicTrack musicTrack = new MusicTrack(component.SoundDefinition, component.Data);
				musicTrack.RegisterMusicTrackListener(musicTrackListener);
				this.musicTrackStack.Push(musicTrack);
				this.viewTracks.Add(view, musicTrackListener);
			}
		}

		private void ViewDestroyed(IUIView view)
		{
			if (this.viewTracks.ContainsKey(view))
			{
				ViewMusicTrack component = view.gameObject.GetComponent<ViewMusicTrack>();
				if (component.MuteScreenTrackOnExit)
				{
					this.MuteCurrentScreenMusic();
				}
				this.viewTracks[view].ExitMusicTrack();
				this.viewTracks.Remove(view);
			}
		}

		private IEnumerator OnPreChangeFullScreen(ChangeInfo changeInfo)
		{
			if (changeInfo.PreviousOwner != null && this.IsFullScreenMusicValid(changeInfo.NextOwner))
			{
				string name = changeInfo.PreviousOwner.GetType().Name;
				if (this.fullScreenTracks.ContainsKey(name))
				{
					this.fullScreenTracks[name].ExitMusicTrack();
					yield break;
				}
			}
			this.AddFullScreenMusicTrack(changeInfo.NextOwner);
			yield break;
		}

		private IEnumerator OnPostChangeFullScreen(ChangeInfo changeInfo)
		{
			if (changeInfo.PreviousOwner != null && this.IsFullScreenMusicValid(changeInfo.NextOwner))
			{
				string previous = changeInfo.PreviousOwner.GetType().Name;
				if (this.fullScreenTracks.ContainsKey(previous))
				{
					this.fullScreenTracks[previous].ExitMusicTrackImmediate();
					this.fullScreenTracks.Remove(previous);
					yield return FiberHelper.WaitForFrames(2, (FiberHelper.WaitFlag)0);
				}
			}
			this.AddFullScreenMusicTrack(changeInfo.NextOwner);
			yield break;
		}

		private bool IsFullScreenMusicValid(IFullScreenOwner fullScreenOwner)
		{
			if (fullScreenOwner != null)
			{
				IMusicTrackComponent musicTrackComponent = this.assets.AudioDatabase.FindFullScreenSoundDefinition(fullScreenOwner);
				if (musicTrackComponent != null && musicTrackComponent.SoundDefinition.sound.Clip != null)
				{
					return true;
				}
			}
			return false;
		}

		private void AddFullScreenMusicTrack(IFullScreenOwner fullScreenOwner)
		{
			if (fullScreenOwner == null || this.fullScreenTracks.ContainsKey(fullScreenOwner.GetType().Name))
			{
				return;
			}
			this.latestFullScreenOwner = fullScreenOwner;
			if (this.muted)
			{
				return;
			}
			IMusicTrackComponent musicTrackComponent = this.assets.AudioDatabase.FindFullScreenSoundDefinition(fullScreenOwner);
			if (musicTrackComponent != null && musicTrackComponent.SoundDefinition.sound.Clip != null)
			{
				MusicTrackListener musicTrackListener = new MusicTrackListener();
				MusicTrack musicTrack = new MusicTrack(musicTrackComponent.SoundDefinition, musicTrackComponent.Data);
				musicTrack.RegisterMusicTrackListener(musicTrackListener);
				this.musicTrackStack.Push(musicTrack);
				this.fullScreenTracks.Add(fullScreenOwner.GetType().Name, musicTrackListener);
			}
		}

		private void MuteCurrentScreenMusic()
		{
			if (this.latestFullScreenOwner != null)
			{
				string name = this.latestFullScreenOwner.GetType().Name;
				if (this.fullScreenTracks.ContainsKey(name))
				{
					this.fullScreenTracks[name].ExitMusicTrackImmediate();
				}
			}
		}

		public void TurnMusicOn()
		{
			this.muted = false;
			if (this.latestFullScreenOwner != null && !this.fullScreenTracks.ContainsKey(this.latestFullScreenOwner.GetType().Name))
			{
				this.AddFullScreenMusicTrack(this.latestFullScreenOwner);
			}
		}

		public void TurnMusicOff()
		{
			this.muted = true;
		}

		private readonly IMusicTrackStack musicTrackStack;

		private readonly IAssetsModel assets;

		private readonly IUIController uiController;

		private readonly IFullScreenManager fullScreenManager;

		private IFullScreenOwner latestFullScreenOwner;

		private Dictionary<IUIView, MusicTrackListener> viewTracks;

		private Dictionary<string, MusicTrackListener> fullScreenTracks;

		private bool muted;
	}
}
