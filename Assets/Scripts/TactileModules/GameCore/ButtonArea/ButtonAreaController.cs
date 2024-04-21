using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using TactileModules.GameCore.ButtonArea.Assets;
using TactileModules.GameCore.UI;
using UnityEngine;

namespace TactileModules.GameCore.ButtonArea
{
	public class ButtonAreaController : IButtonAreaController
	{
		public ButtonAreaController(IButtonAreaModel model, IUIController iuiController, IAssetModel assets)
		{
			this.model = model;
			this.iuiController = iuiController;
			this.assets = assets;
			this.HookUIEvents();
			this.HookModelEvents();
			this.animatingButtons = new Dictionary<ButtonAreaButton, ButtonAreaController.AnimatingButton>();
		}

		private void HookUIEvents()
		{
			this.iuiController.ViewCreated += this.OnViewCreated;
			this.iuiController.ViewDestroyed += this.OnViewDestroyed;
		}

		private void OnViewCreated(IUIView view)
		{
			this.PushAllAreasFromGameObject(view.gameObject);
		}

		private void OnViewDestroyed(IUIView view)
		{
			this.PopAllAreasFromGameObject(view.gameObject);
		}

		private void PushAllAreasFromGameObject(GameObject go)
		{
			ButtonArea[] componentsInChildren = go.GetComponentsInChildren<ButtonArea>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				this.model.PushArea(componentsInChildren[i]);
			}
		}

		private void PopAllAreasFromGameObject(GameObject go)
		{
			ButtonArea[] componentsInChildren = go.GetComponentsInChildren<ButtonArea>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				this.model.PopArea(componentsInChildren[i]);
			}
		}

		private void HookModelEvents()
		{
			this.model.ButtonCreated += this.OnButtonCreated;
			this.model.ButtonDestroyed += this.OnButtonDestroyed;
			this.model.ButtonChangedArea += this.OnButtonChangedArea;
		}

		private void OnButtonCreated(ButtonAreaButton button)
		{
			this.animatingButtons.Add(button, new ButtonAreaController.AnimatingButton(button));
		}

		private void OnButtonDestroyed(ButtonAreaButton button)
		{
			this.animatingButtons[button].FadeAndDestroy(delegate
			{
				this.animatingButtons.Remove(button);
			});
		}

		private void OnButtonChangedArea(ButtonAreaButton button, ButtonArea newArea)
		{
			this.animatingButtons[button].MoveToArea(newArea);
		}

		private ButtonAreaController.AnimatingButton FindButtonWithComponent<T>(Predicate<T> predicate) where T : Component
		{
			foreach (KeyValuePair<ButtonAreaButton, ButtonAreaController.AnimatingButton> keyValuePair in this.animatingButtons)
			{
				T componentInChildren = keyValuePair.Key.GetComponentInChildren<T>();
				if (componentInChildren != null && predicate(componentInChildren))
				{
					return keyValuePair.Value;
				}
			}
			return null;
		}

		public IButtonAreaControllerButton<T> GetButton<T>(Predicate<T> predicate) where T : Component
		{
			ButtonAreaController.AnimatingButton animatingButton = this.FindButtonWithComponent<T>(predicate);
			return (animatingButton != null) ? new ButtonAreaController.Button<T>(animatingButton, this) : null;
		}

		private readonly IUIController iuiController;

		private readonly IButtonAreaModel model;

		private readonly IAssetModel assets;

		private Dictionary<ButtonAreaButton, ButtonAreaController.AnimatingButton> animatingButtons;

		private class AnimatingButton
		{
			public AnimatingButton(ButtonAreaButton button)
			{
				this.button = button;
			}

			public ButtonAreaButton Button
			{
				get
				{
					return this.button;
				}
			}

			public void MoveToArea(ButtonArea area)
			{
				this.button.transform.parent = area.transform;
				if (!area.stayAtPosition)
				{
					this.button.transform.localPosition = Vector3.zero;
				}
				this.button.transform.localScale = Vector3.one;
				this.button.gameObject.SetLayerRecursively(area.gameObject.layer);
				if (area.fitButtonSizeToArea)
				{
					area.FitButtonSizeToArea(this.button);
				}
			}

			public void FadeAndDestroy(Action finished)
			{
				finished();
			}

			private IEnumerator FadeAndDestroyAnimation(Action finished)
			{
				yield break;
			}

			private readonly ButtonAreaButton button;

			private Vector3 visiblePosition;

			private Vector3 invisiblePosition;

			private readonly Fiber animationFiber;

			private bool positionSet;
		}

		private class Button<T> : IButtonAreaControllerButton<T> where T : Component
		{
			public Button(ButtonAreaController.AnimatingButton animatingButton, ButtonAreaController buttonAreaController)
			{
				this.animatingButton = animatingButton;
				this.buttonAreaController = buttonAreaController;
			}

			public T Component
			{
				get
				{
					return (this.animatingButton == null) ? ((T)((object)null)) : this.animatingButton.Button.GetComponentInChildren<T>();
				}
			}

			private readonly ButtonAreaController.AnimatingButton animatingButton;

			private readonly ButtonAreaController buttonAreaController;
		}
	}
}
