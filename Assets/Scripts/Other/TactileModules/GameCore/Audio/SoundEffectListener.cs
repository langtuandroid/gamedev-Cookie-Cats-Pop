using System;
using TactileModules.GameCore.Audio.Assets;
using TactileModules.GameCore.UI;

namespace TactileModules.GameCore.Audio
{
	public class SoundEffectListener
	{
		public SoundEffectListener(IUIController uiController, IAssetsModel assets)
		{
			this.assets = assets;
			uiController.ViewCreated += this.ViewCreated;
			uiController.ViewWillDisappear += this.ViewDestroyed;
		}

		private void ViewCreated(IUIView view)
		{
			this.HandleButtonEvents(view);
			OverrideViewCreatedSound component = view.gameObject.GetComponent<OverrideViewCreatedSound>();
			if (component != null)
			{
				component.SoundDefinition.PlaySound();
				return;
			}
			if (this.HasLayerAnimation(view) || this.HasViewAnimators(view))
			{
				this.assets.AudioDatabase.ViewSlideIn.PlaySound();
			}
		}

		private void ViewDestroyed(IUIView view)
		{
			OverrideViewDestroyedSound component = view.gameObject.GetComponent<OverrideViewDestroyedSound>();
			if (component != null)
			{
				component.SoundDefinition.PlaySound();
				return;
			}
			if (this.HasLayerAnimation(view) || this.HasViewAnimators(view))
			{
				this.assets.AudioDatabase.ViewSlideOut.PlaySound();
			}
		}

		private bool HasLayerAnimation(IUIView view)
		{
			return view.GetViewLayerAnimation() != null;
		}

		private bool HasViewAnimators(IUIView view)
		{
			UIViewAnimator[] componentsInChildren = view.gameObject.GetComponentsInChildren<UIViewAnimator>();
			return componentsInChildren != null && componentsInChildren.Length > 0;
		}

		private void HandleButtonEvents(IUIView view)
		{
			UIButton[] componentsInChildren = view.gameObject.GetComponentsInChildren<UIButton>();
			foreach (UIButton uibutton in componentsInChildren)
			{
				uibutton.Clicked += this.PlayButtonClick;
			}
		}

		private void PlayButtonClick(UIButton button)
		{
			OverrideButtonClickedSound component = button.GetComponent<OverrideButtonClickedSound>();
			if (component != null)
			{
				component.SoundDefinition.PlaySound();
				return;
			}
			this.assets.AudioDatabase.DefaultButtonClick.PlaySound();
		}

		private readonly IAssetsModel assets;
	}
}
