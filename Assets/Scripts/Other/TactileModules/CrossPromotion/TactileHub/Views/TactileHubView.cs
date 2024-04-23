using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Fibers;
using JetBrains.Annotations;
using TactileModules.CrossPromotion.TactileHub.Models;
using TactileModules.CrossPromotion.TactileHub.ViewComponents;
using UnityEngine;

namespace TactileModules.CrossPromotion.TactileHub.Views
{
	public class TactileHubView : UIView, ITactileHubView, IUIView
	{
		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<IHubGame> OnClick;



		public void Initialize(List<IHubGame> hubGames, UIElement tactileButtonUiElement, IUIViewManager uiViewManager)
		{
			this.tactileButtonUiElement = tactileButtonUiElement;
			this.uiViewManager = uiViewManager;
			this.SetupButtons(hubGames);
			this.SetExistingLabelsFontToDefault();
			this.backgroundAlpha = this.background.Alpha;
			this.title.Alpha = 0f;
			this.background.Alpha = 0f;
			this.animationPivot.gameObject.SetActive(false);
			this.animationFiber.Start(this.BeginIntroAnimation());
		}

		private IEnumerator BeginIntroAnimation()
		{
			yield return null;
			this.animationPivot.gameObject.SetActive(true);
			Vector3 position = this.GetHubButtonPosition();
			yield return this.AnimatePivot(position, this.animationPivot.localPosition, this.AnimationStartScale, this.animationPivot.localScale, 0f, 1f, delegate
			{
			});
			yield break;
		}

		private Vector3 GetHubButtonPosition()
		{
			float viewSizeRatio = this.GetViewSizeRatio();
			Vector3 position = this.tactileButtonUiElement.transform.position;
			Vector3 result = new Vector3(position.x * viewSizeRatio, position.y * viewSizeRatio, position.z);
			return result;
		}

		private float GetViewSizeRatio()
		{
			int layer = base.gameObject.layer;
			UICamera uicamera = this.uiViewManager.FindCameraFromObjectLayer(layer);
			int layer2 = this.tactileButtonUiElement.gameObject.layer;
			UICamera uicamera2 = this.uiViewManager.FindCameraFromObjectLayer(layer2);
			return uicamera.GetComponent<Camera>().orthographicSize / uicamera2.GetComponent<Camera>().orthographicSize;
		}

		private Vector3 AnimationStartScale
		{
			get
			{
				float x = this.tactileButtonUiElement.Size.x;
				float viewSizeRatio = this.GetViewSizeRatio();
				float num = this.uiRoundedRect.Size.x + this.uiRoundedRect.cornerSize * 2f;
				float num2 = x / num * viewSizeRatio;
				return new Vector3(num2, num2, 1f);
			}
		}

		private IEnumerator AnimatePivot(Vector3 startPosition, Vector3 endPosition, Vector3 startScale, Vector3 endScale, float startAlpha, float endAlpha, Action onFinish)
		{
			yield return FiberHelper.RunParallel(new IEnumerator[]
			{
				FiberAnimation.MoveLocalTransform(this.animationPivot, startPosition, endPosition, this.animationCurve, 0f),
				FiberAnimation.ScaleTransform(this.animationPivot, startScale, endScale, this.animationCurve, 0f),
				FiberAnimation.Animate(0f, this.animationCurve, delegate(float t)
				{
					this.background.Alpha = (startAlpha + (endAlpha - startAlpha) * t) * this.backgroundAlpha;
					this.title.Alpha = startAlpha + (endAlpha - startAlpha) * t;
				}, false)
			});
			onFinish();
			yield break;
		}

		private void SetupButtons(List<IHubGame> hubGames)
		{
			using (List<IHubGame>.Enumerator enumerator = hubGames.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					IHubGame hubGame = enumerator.Current;
					TactileHubView _0024this = this;
					TactileHubGameButton tactileHubGameButton = UnityEngine.Object.Instantiate<TactileHubGameButton>(this.hubGameIconPrefab, this.layout.transform, true);
					tactileHubGameButton.Initialize(hubGame);
					tactileHubGameButton.gameObject.SetLayerRecursively(base.gameObject.layer);
					tactileHubGameButton.OnClick += delegate()
					{
						_0024this.OnGameButtonClicked(hubGame);
					};
				}
			}
		}

		private void OnGameButtonClicked(IHubGame hubGame)
		{
			this.OnClick(hubGame);
			base.Close(0);
		}

		private void SetExistingLabelsFontToDefault()
		{
			UIProjectSettings uiprojectSettings = UIProjectSettings.Get();
			UIFont defaultFont = uiprojectSettings.defaultFont;
			UILabel[] componentsInChildren = base.transform.GetComponentsInChildren<UILabel>();
			foreach (UILabel uilabel in componentsInChildren)
			{
				uilabel.font = defaultFont;
			}
		}

		[UsedImplicitly]
		private void CloseView(UIEvent e)
		{
			Vector3 hubButtonPosition = this.GetHubButtonPosition();
			this.animationFiber.Start(this.AnimatePivot(this.animationPivot.localPosition, hubButtonPosition, this.animationPivot.localScale, this.AnimationStartScale, 1f, 0f, delegate
			{
				base.Close(0);
			}));
		}

		[SerializeField]
		private TactileHubGameButton hubGameIconPrefab;

		[SerializeField]
		private UIAdvancedFlowLayout layout;

		[SerializeField]
		private UIRoundedRect uiRoundedRect;

		[SerializeField]
		private Transform animationPivot;

		[SerializeField]
		private UIWidget background;

		[SerializeField]
		private UIWidget title;

		[SerializeField]
		private AnimationCurve animationCurve;

		private readonly Fiber animationFiber = new Fiber();

		private UIElement tactileButtonUiElement;

		private float backgroundAlpha;

		private IUIViewManager uiViewManager;
	}
}
