using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using Tactile;
using TactileModules.GameCore.UI;
using UnityEngine;

namespace TactileModules.GameCore.MenuTutorial
{
	public class RunningTutorial : IRunningTutorial
	{
		public RunningTutorial(IMenuTutorialDefinition definition, IUIController uiController, MenuTutorialMessage messagePrefab)
		{
			this.definition = definition;
			this.uiController = uiController;
			this.messagePrefab = messagePrefab;
			this.fiber = new Fiber(FiberBucket.Manual);
			this.maskOverlay = new MaskOverlay();
			this.maskOverlay.Initialize();
			this.maskOverlay.Alpha = 0.75f;
		}

		public IMenuTutorialDefinition Definition
		{
			get
			{
				return this.definition;
			}
		}

		public void Start()
		{
			this.fiber.Start(this.Logic());
		}

		public void Stop()
		{
			this.fiber.Terminate();
			this.maskOverlay.Destroy();
		}

		public bool Step()
		{
			return this.fiber.Step();
		}

		private IEnumerator Logic()
		{
			for (int i = 0; i < this.Definition.Steps.Count; i++)
			{
				yield return this.RunStep(this.Definition.Steps[i], (i >= this.Definition.Steps.Count - 1) ? null : this.Definition.Steps[i + 1]);
			}
			yield break;
		}

		private IEnumerator WaitForView(string viewName, EnumeratorResult<IUIView> foundView)
		{
			bool correctViewShown = false;
			Action<IUIView> viewCreated = delegate(IUIView view)
			{
				if (view.name.Contains(viewName))
				{
					correctViewShown = true;
					foundView.value = view;
				}
			};
			this.uiController.ViewCreated += viewCreated;
			yield return new Fiber.OnExit(delegate()
			{
				this.uiController.ViewCreated -= viewCreated;
			});
			while (!correctViewShown)
			{
				if (this.uiController.TopView != null && this.uiController.TopView.name.Contains(viewName))
				{
					foundView.value = this.uiController.TopView;
					break;
				}
				yield return null;
			}
			while (!foundView.value.gameObject.activeSelf)
			{
				yield return null;
			}
			this.uiController.ViewCreated -= viewCreated;
			yield break;
		}

		private float GetIntersectionArea(Bounds a, Bounds b)
		{
			float x = a.min.x;
			float x2 = a.max.x;
			float y = a.min.y;
			float y2 = a.max.y;
			float x3 = b.min.x;
			float x4 = b.max.x;
			float y3 = b.min.y;
			float y4 = b.max.y;
			return Mathf.Max(0f, Mathf.Min(x2, x4) - Mathf.Max(x, x3)) * Mathf.Max(0f, Mathf.Min(y2, y4) - Mathf.Max(y, y3));
		}

		private UIButton FindButton(UIButton[] availableButtons, Bounds bounds)
		{
			List<UIButton> list = new List<UIButton>();
			foreach (UIButton uibutton in availableButtons)
			{
				BoxCollider component = uibutton.GetComponent<BoxCollider>();
				if (!(component == null))
				{
					if (component.bounds.Intersects(bounds))
					{
						list.Add(uibutton);
					}
				}
			}
			list.Sort(delegate(UIButton a, UIButton b)
			{
				GameObject gameObject = a.gameObject;
				GameObject gameObject2 = b.gameObject;
				if (gameObject.layer == gameObject2.layer)
				{
					BoxCollider component2 = gameObject.GetComponent<BoxCollider>();
					BoxCollider component3 = gameObject2.GetComponent<BoxCollider>();
					float intersectionArea = this.GetIntersectionArea(bounds, component2.bounds);
					return this.GetIntersectionArea(bounds, component3.bounds).CompareTo(intersectionArea);
				}
				return gameObject2.layer.CompareTo(gameObject.layer);
			});
			return (list.Count <= 0) ? null : list[0];
		}

		private IEnumerator WaitForButtonClick(UIButton button)
		{
			bool isButtonClicked = false;
			Action<UIButton> buttonClicked = delegate(UIButton b)
			{
				isButtonClicked = true;
			};
			button.Clicked += buttonClicked;
			yield return new Fiber.OnExit(delegate()
			{
				button.Clicked -= buttonClicked;
			});
			while (!isButtonClicked)
			{
				yield return null;
			}
			yield break;
		}

		private IEnumerator WaitForClick()
		{
			while (!Input.anyKeyDown)
			{
				yield return null;
			}
			yield return null;
			yield return null;
			yield return null;
			yield break;
		}

		private IEnumerator RunStep(MenuTutorialStep step, MenuTutorialStep nextStep)
		{
			EnumeratorResult<IUIView> foundView = new EnumeratorResult<IUIView>();
			yield return this.WaitForView(step.ViewName, foundView);
			UICamera viewCamera = this.uiController.GetCameraFromView(foundView.value);
			bool didBlockOtherCameras = viewCamera.BlockOtherCameras;
			viewCamera.BlockOtherCameras = true;
			UIButton[] allButtons = foundView.value.gameObject.GetComponentsInChildren<UIButton>();
			for (int i = 0; i < allButtons.Length; i++)
			{
				allButtons[i].enabled = false;
			}
			while (this.uiController.AnyViewsAnimating)
			{
				yield return null;
			}
			this.maskOverlay.Enable(null);
			GameObject uiViewPrefabGo = new GameObject("StepView");
			uiViewPrefabGo.AddComponent<UIElement>().Size = foundView.value.OriginalSize;
			UIView uiViewPrefab = uiViewPrefabGo.AddComponent<UIView>();
			uiViewPrefab.BlockOtherViews = false;
			UIElement highlightElement = new GameObject("highlight").AddComponent<UIElement>();
			highlightElement.autoSizing = (UIAutoSizing)step.Highlight.layouting;
			highlightElement.transform.position = step.Highlight.position;
			highlightElement.Size = step.Highlight.size;
			highlightElement.transform.parent = uiViewPrefabGo.transform;
			UIElement messageElement = new GameObject("message").AddComponent<UIElement>();
			messageElement.autoSizing = step.MessageLayouting;
			messageElement.transform.position = step.MessagePosition;
			messageElement.Size = step.MessageSize;
			messageElement.transform.parent = uiViewPrefabGo.transform;
			uiViewPrefab.GetComponent<UIElement>().SetSizeAndDoLayout(foundView.value.GetElementSize());
			UIButton buttonToClick = null;
			if (step.DimissType == MenuTutorialStep.DimissTypes.Button)
			{
				buttonToClick = this.FindButton(allButtons, new Bounds(highlightElement.transform.position, new Vector3(highlightElement.Size.x, highlightElement.Size.y, 10000f)));
				if (buttonToClick == null)
				{
				}
			}
			for (int j = 0; j < allButtons.Length; j++)
			{
				allButtons[j].enabled = (buttonToClick == allButtons[j]);
			}
			this.maskOverlay.Depth = viewCamera.cachedCamera.depth + 0.05f;
			MaskOverlayCutout cutout = this.maskOverlay.AddCutout(null);
			cutout.Camera = viewCamera.cachedCamera;
			cutout.WorldPosition = highlightElement.transform.position;
			cutout.WorldSize = highlightElement.Size;
			cutout.Oval = step.Highlight.oval;
			UIView overlayView = this.uiController.ShowView<UIView>(uiViewPrefab);
			UnityEngine.Object.Destroy(uiViewPrefabGo);
			MenuTutorialMessage message = UnityEngine.Object.Instantiate<MenuTutorialMessage>(this.messagePrefab);
			message.transform.position = messageElement.transform.position;
			message.gameObject.SetLayerRecursively(overlayView.gameObject.layer);
			message.GetComponent<UIElement>().SetSizeAndDoLayout(messageElement.Size);
			message.Message = L.Get(step.Message);
			yield return new Fiber.OnExit(delegate()
			{
				if (nextStep == null || !foundView.value.name.Contains(nextStep.ViewName))
				{
					for (int k = 0; k < allButtons.Length; k++)
					{
						allButtons[k].enabled = true;
					}
				}
				viewCamera.BlockOtherCameras = didBlockOtherCameras;
				overlayView.Close(0);
				this.maskOverlay.RemoveCutout(cutout);
				this.maskOverlay.Disable(null);
				UnityEngine.Object.Destroy(message.gameObject);
			});
			if (buttonToClick != null)
			{
				yield return this.WaitForButtonClick(buttonToClick);
			}
			else
			{
				yield return this.WaitForClick();
			}
			yield break;
		}

		private readonly IMenuTutorialDefinition definition;

		private readonly IUIController uiController;

		private readonly MaskOverlay maskOverlay;

		private readonly MenuTutorialMessage messagePrefab;

		private readonly Fiber fiber;
	}
}
