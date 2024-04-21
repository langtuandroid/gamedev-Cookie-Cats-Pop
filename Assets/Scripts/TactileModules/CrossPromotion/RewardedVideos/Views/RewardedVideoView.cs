using System;
using System.Collections;
using System.Diagnostics;
using Fibers;
using JetBrains.Annotations;
using TactileModules.CrossPromotion.General.Ads.AdModels;
using TactileModules.NinjaUi.Modifiers;
using TactileModules.NinjaUI.Modifiers;
using TactileModules.TactileLogger;
using UnityEngine;
using UnityEngine.Video;

namespace TactileModules.CrossPromotion.RewardedVideos.Views
{
	public class RewardedVideoView : UIView, IRewardedVideoView, IUIView
	{
		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action ClickedAd;



		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action Closed;



		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action OnVideoEnded;



		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<int> OnVideoFreeze;



		public void Initialize(ICrossPromotionAd crossPromotionAd, ICrossPromotionVideoOverlayView overlayVideoView)
		{
			Log.Info("CrossPromotion", () => "Initializing RewardedVideoView", null);
			this.videoPromotionAd = crossPromotionAd;
			this.overlayView = overlayVideoView;
			this.overlayView.gameObject.SetActive(false);
			this.DisableTargetCamera();
			this.InitializeEndCard();
			this.ShowFullScreenVideo();
		}

		[UsedImplicitly]
		private void ShowEmbeddedVideo()
		{
			Log.Info("CrossPromotion", () => "Showing embedded video", null);
			this.videoType = RewardedVideoView.VideoType.Embedded;
			this.AddVideoPlayerAndAudioSource();
			this.PrepareVideoPlayer();
			this.SetupLayouting();
		}

		[UsedImplicitly]
		private void ShowFullScreenVideo()
		{
			Log.Info("CrossPromotion", () => "Showing native video", null);
			this.videoType = RewardedVideoView.VideoType.Native;
			string videoPath = this.videoPromotionAd.GetVideoPath();
			Handheld.PlayFullScreenMovie(videoPath, Color.black, FullScreenMovieControlMode.Hidden);
			this.blackSprite.gameObject.SetActive(false);
			this.VideoEnded(null);
		}

		private float VideoTimeLeft
		{
			get
			{
				if (this.videoPlayer.frameCount <= 0UL)
				{
					return -1f;
				}
				double time = this.videoPlayer.time;
				float num = this.videoPlayer.frameCount / this.videoPlayer.frameRate;
				return (float)((double)num - time);
			}
		}

		private void DisableTargetCamera()
		{
			this.targetCamera.gameObject.SetActive(false);
		}

		private void InitializeEndCard()
		{
			this.endCard.GetComponent<Collider>().enabled = false;
			this.endCard.SetTexture(this.videoPromotionAd.GetImage());
		}

		private void AddVideoPlayerAndAudioSource()
		{
			this.videoPlayer = base.gameObject.AddComponent<VideoPlayer>();
			this.audioSource = base.gameObject.AddComponent<AudioSource>();
		}

		private void PrepareVideoPlayer()
		{
			this.videoPlayer.isLooping = false;
			this.videoPlayer.playOnAwake = false;
			this.videoPlayer.source = VideoSource.Url;
			this.videoPlayer.url = this.videoPromotionAd.GetVideoPath();
			this.videoPlayer.controlledAudioTrackCount = 1;
			this.videoPlayer.loopPointReached += this.VideoEnded;
			this.videoPlayer.aspectRatio = VideoAspectRatio.Stretch;
			this.videoPlayer.renderMode = VideoRenderMode.CameraNearPlane;
			this.videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
			this.videoPlayer.SetTargetAudioSource(0, this.audioSource);
			this.videoPlayer.targetCamera = this.targetCamera;
			this.videoPlayer.Prepare();
		}

		private void SetupLayouting()
		{
			UICameraWidgetFollower uicameraWidgetFollower = this.endCard.gameObject.AddComponent<UICameraWidgetFollower>();
			uicameraWidgetFollower.TargetCamera = this.targetCamera;
		}

		protected override void ViewDidAppear()
		{
			if (this.videoType != RewardedVideoView.VideoType.Embedded)
			{
				return;
			}
			this.targetCamera.depth = (float)base.gameObject.layer;
			this.videoStarterFiber.Start(this.WaitForPreparedAndStartVideo());
		}

		protected override void ViewWillDisappear()
		{
			this.videoStarterFiber.Terminate();
			this.overlayView.Close(0);
			this.EnableInput();
		}

		private void Update()
		{
			this.currentVideoDuration += Time.deltaTime * 1000f;
			if ((this.videoPlayer != null && this.videoPlayer.isPlaying) || !this.endCard.gameObject.activeSelf)
			{
				this.EndVideoIfExceededDuration();
			}
			this.EnsureVideoNotPaused();
			if (this.shouldVideoPlay)
			{
				this.overlayView.SetTimeLabel(this.VideoTimeLeft);
			}
		}

		private void EnsureVideoNotPaused()
		{
			if (!this.shouldVideoPlay || this.videoPlayer.isPlaying)
			{
				return;
			}
			this.videoPlayer.Play();
		}

		private void EndVideoIfExceededDuration()
		{
			int duration = this.videoPromotionAd.GetVideoCreative().Duration;
			if (this.currentVideoDuration > (float)(duration + 10000))
			{
				this.VideoDidFreeze();
			}
		}

		private void VideoDidFreeze()
		{
			int obj = -1;
			if (this.videoPlayer != null)
			{
				obj = (int)this.videoPlayer.frame;
				this.videoPlayer.Stop();
			}
			this.OnVideoFreeze(obj);
			this.VideoEnded(this.videoPlayer);
		}

		private IEnumerator WaitForPreparedAndStartVideo()
		{
			while (this.videoPlayer != null && !this.videoPlayer.isPrepared)
			{
				yield return null;
			}
			this.StartVideo();
			yield break;
		}

		private void StartVideo()
		{
			this.overlayView.gameObject.SetActive(true);
			this.overlaySizeModifier = this.endCard.gameObject.AddComponent<UISizeModifier>();
			this.overlaySizeModifier.SizeModifier = this.frameSizeModifier;
			this.overlaySizeModifier.TargetToScale = this.overlayView.GetFrame();
			this.overlayElementFollower = this.endCard.gameObject.AddComponent<UIElementTransformFollower>();
			this.overlayElementFollower.Target = this.overlayView.GetFrame().transform;
			this.blackSprite.gameObject.SetActive(false);
			this.targetCamera.gameObject.SetActive(true);
			this.videoPlayer.Play();
			this.shouldVideoPlay = true;
			this.DisableInput();
		}

		private void VideoEnded(VideoPlayer source)
		{
			this.shouldVideoPlay = false;
			this.targetCamera.gameObject.SetActive(false);
			this.endCard.GetComponent<Collider>().enabled = true;
			UnityEngine.Object.Destroy(this.overlaySizeModifier);
			UnityEngine.Object.Destroy(this.overlayElementFollower);
			this.overlayView.Close(0);
			this.EnableInput();
			this.OnVideoEnded();
		}

		private void DisableInput()
		{
			if (this.inputDisabled)
			{
				return;
			}
			UICamera.DisableInput();
			this.inputDisabled = true;
		}

		private void EnableInput()
		{
			if (!this.inputDisabled)
			{
				return;
			}
			UICamera.EnableInput();
			this.inputDisabled = false;
		}

		[UsedImplicitly]
		private void OnEndScreenCloseClicked(UIEvent uiEvent)
		{
			this.Closed();
		}

		[UsedImplicitly]
		private void OnEndScreenDownloadClicked(UIEvent uiEvent)
		{
			this.ClickedAd();
		}

		[Header("Cross Promotion Video View")]
		[SerializeField]
		private Camera targetCamera;

		[Header("Object references")]
		[SerializeField]
		private UITextureQuad endCard;

		[SerializeField]
		private UISprite blackSprite;

		[Header("Configuration")]
		[SerializeField]
		private Vector2 frameSizeModifier;

		private const int VIDEO_TIMEOUT_DURATION = 10000;

		private VideoPlayer videoPlayer;

		private AudioSource audioSource;

		private float currentVideoDuration;

		private readonly Fiber videoStarterFiber = new Fiber();

		private UISizeModifier overlaySizeModifier;

		private UIElementTransformFollower overlayElementFollower;

		private bool inputDisabled;

		private bool shouldVideoPlay;

		private RewardedVideoView.VideoType videoType;

		private ICrossPromotionAd videoPromotionAd;

		private ICrossPromotionVideoOverlayView overlayView;

		private enum VideoType
		{
			Embedded,
			Native
		}
	}
}
