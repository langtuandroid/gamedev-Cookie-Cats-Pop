using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using Tactile.GardenGame.MapSystem;
using Tactile.GardenGame.Story.Assets;
using Tactile.GardenGame.Story.Dialog;
using Tactile.GardenGame.Story.Monologue;
using Tactile.GardenGame.Story.Views;
using TactileModules.GameCore.StreamingAssets;
using TactileModules.GameCore.UI;
using UnityEngine;

namespace Tactile.GardenGame.Story
{
	public class StoryMapController : IStoryMapController
	{
		public StoryMapController(IUIController uiController, MainMapController mainMapController, IAssetModel assets, PropsManager propsManager, IStoryAudio storyAudio)
		{
			this.uiController = uiController;
			this.mainMapController = mainMapController;
			this.assets = assets;
			this.propsManager = propsManager;
			this.storyAudio = storyAudio;
			this.dialog = new MapScriptDialogPresenter(uiController, assets.DialogOverlayView);
			this.Monologues = new MonologueManager(assets.MonologueDatabase);
			this.CreateBlackBarsView();
		}

		public void Destroy()
		{
			this.dialog.Destroy();
			this.DestroyBlackBarsView();
		}

		public void InstantBlack()
		{
			UIViewManager.Instance.FrontFill = 1f;
		}

		public void RemoveBlack()
		{
			UIViewManager.Instance.FrontFill = 0f;
		}

		public T GetMapComponent<T>(string id) where T : MapComponent
		{
			return this.mainMapController.Map.Entities.GetMapComponent<T>(id);
		}

		public PropsManager PropsManager
		{
			get
			{
				return this.propsManager;
			}
		}

		public IMapScriptDialogPresenter Dialog
		{
			get
			{
				return this.dialog;
			}
		}

		public float Darkness
		{
			get
			{
				return MapLighting.Darkness;
			}
			set
			{
				MapLighting.Darkness = value;
			}
		}

		public void PlayMusic(SoundDefinition soundDefinition)
		{
			this.storyAudio.PlayMusic(soundDefinition);
		}

		public void StopMusic()
		{
			this.storyAudio.StopMusic();
		}

		public void PlaySound(SoundDefinition soundDefinition)
		{
			this.storyAudio.PlaySound(soundDefinition);
		}

		public IEnumerator FadeToBlack()
		{
			this.RemoveBlack();
			yield return UIViewManager.Instance.FadeCameraFrontFill(1f, 0.5f, 0);
			yield break;
		}

		public IEnumerator FadeToMap()
		{
			this.InstantBlack();
			yield return UIViewManager.Instance.FadeCameraFrontFill(0f, 0.5f, 0);
			yield break;
		}

		public IEnumerator ShowFullScreenImage(StreamingAsset image)
		{
			if (!(this.fullscreenImageView != null))
			{
				this.InstantBlack();
				this.mainMapController.Map.Visible = false;
				this.fullscreenImageView = this.uiController.ShowView<StoryImageView>(this.assets.StoryImageView);
			}
			if (this.fullscreenImage != null)
			{
				StoryImageView fullscreenImageViewToFadeIn = this.uiController.ShowView<StoryImageView>(this.assets.StoryImageView);
				fullscreenImageViewToFadeIn.image.SetTexture(image.GetAsset<Texture2D>());
				image.AssetChanged = delegate()
				{
					if (fullscreenImageViewToFadeIn != null)
					{
						fullscreenImageViewToFadeIn.image.SetTexture(image.GetAsset<Texture2D>());
					}
				};
				image.Load();
				fullscreenImageViewToFadeIn.ToggleBackground(false);
				yield return this.FadeImageViewIn(fullscreenImageViewToFadeIn);
				fullscreenImageViewToFadeIn.ToggleBackground(true);
				this.fullscreenImage.Unload();
				this.fullscreenImage = image;
				this.fullscreenImageView.Close(0);
				this.fullscreenImageView = fullscreenImageViewToFadeIn;
			}
			else
			{
				this.fullscreenImage = image;
				image.AssetChanged = delegate()
				{
					if (this.fullscreenImageView != null)
					{
						this.fullscreenImageView.image.SetTexture(this.fullscreenImage.GetAsset<Texture2D>());
					}
				};
				image.Load();
			}
			if (UIViewManager.Instance.FrontFill >= 0.0001f)
			{
				yield return UIViewManager.Instance.FadeCameraFrontFill(0f, 0.5f, 0);
			}
			yield break;
		}

		private IEnumerator FadeImageViewIn(StoryImageView fullscreenImageViewToFadeIn)
		{
			float timer = 0f;
			float duration = 0.5f;
			while (timer < duration)
			{
				fullscreenImageViewToFadeIn.image.Alpha = Mathf.Lerp(0f, 1f, timer / duration);
				timer += Mathf.Min(Time.deltaTime, 0.033f);
				yield return null;
			}
			yield break;
		}

		public MapCamera Camera
		{
			get
			{
				return this.mainMapController.Map.Camera;
			}
		}

		public IEnumerator ChooseProp(MapProp prop)
		{
			yield return this.mainMapController.ChoosePropController.ChooseProp(prop, true);
			yield break;
		}

		public IEnumerable<T> IterateObjectsWithComponent<T>()
		{
			return this.mainMapController.Map.Entities.IterateObjectsWithComponent<T>();
		}

		public IEnumerator ShowUI()
		{
			yield return this.mainMapController.UI.ShowUI();
			yield break;
		}

		public IEnumerator HideUI()
		{
			yield return this.mainMapController.UI.HideUI();
			yield break;
		}

		public IEnumerator HideFullScreenImage()
		{
			if (this.fullscreenImageView == null)
			{
				yield break;
			}
			yield return new Fiber.OnExit(delegate()
			{
				if (this.fullscreenImage != null)
				{
					this.fullscreenImage.AssetChanged = null;
					this.fullscreenImage.Unload();
					this.fullscreenImage = null;
				}
				if (this.fullscreenImageView != null && !this.fullscreenImageView.IsClosing)
				{
					this.fullscreenImageView.Close(0);
				}
				this.mainMapController.Map.Visible = true;
				this.RemoveBlack();
			});
			yield return UIViewManager.Instance.FadeCameraFrontFill(1f, 0.5f, 0);
			this.fullscreenImageView.Close(0);
			this.mainMapController.Map.Visible = true;
			while (!this.fullscreenImageView.IsClosing)
			{
				yield return null;
			}
			this.fullscreenImageView = null;
			this.RemoveBlack();
			yield break;
		}

		public IEnumerator ShowFullScreenMessage(string message, float duration)
		{
			this.InstantBlack();
			this.mainMapController.Map.Visible = false;
			StoryMessageView storyMessageView = this.uiController.ShowView<StoryMessageView>(this.assets.StoryMessageView);
			storyMessageView.label.text = message;
			storyMessageView.label.typingOnStart = true;
			bool didSkip = false;
			yield return new Fiber.OnExit(delegate()
			{
				if (!storyMessageView.IsClosing)
				{
					storyMessageView.Close(0);
				}
				this.mainMapController.Map.Visible = true;
				this.RemoveBlack();
			});
			yield return this.uiController.FadeCameraFrontFill(0f, 0.5f);
			while (duration > 0f)
			{
				duration -= Time.deltaTime;
				if (didSkip)
				{
					break;
				}
				yield return null;
			}
			yield return this.uiController.FadeCameraFrontFill(1f, 0.5f);
			storyMessageView.Close(0);
			yield return null;
			this.mainMapController.Map.Visible = true;
			this.RemoveBlack();
			yield break;
		}

		public IEnumerator ShowCelebration(string message)
		{
			StoryCelebrationView storyMessageView = this.uiController.ShowView<StoryCelebrationView>(this.assets.StoryCelebrationView);
			yield return storyMessageView.PlayEffect(message);
			storyMessageView.Close(0);
			yield break;
		}

		public MonologueManager Monologues { get; private set; }

		private void CreateBlackBarsView()
		{
			this.blackBarsView = this.uiController.ShowView<BarsView>(this.assets.BarsView);
		}

		private void DestroyBlackBarsView()
		{
			if (!this.blackBarsView.IsClosing)
			{
				this.blackBarsView.Close(0);
			}
		}

		public IEnumerator ShowBars()
		{
			yield return this.blackBarsView.Show();
			yield break;
		}

		public IEnumerator HideBars()
		{
			yield return this.blackBarsView.Hide();
			yield break;
		}

		private readonly IUIController uiController;

		private readonly MainMapController mainMapController;

		private readonly IAssetModel assets;

		private readonly PropsManager propsManager;

		private readonly IMapScriptDialogPresenter dialog;

		private readonly IStoryAudio storyAudio;

		private StoryImageView fullscreenImageView;

		private StreamingAsset fullscreenImage;

		private BarsView blackBarsView;
	}
}
