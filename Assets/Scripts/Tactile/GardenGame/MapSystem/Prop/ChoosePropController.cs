using System;
using System.Collections;
using Fibers;
using Tactile.GardenGame.MapSystem.Props;
using TactileModules.GameCore.UI;
using TactileModules.GardenGame.MapSystem.Assets;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem.Prop
{
	public class ChoosePropController
	{
		public ChoosePropController(IUIController uiController, MapClickableManager mapClickableManager, MainMapUI mapUI, MapCamera camera, PropsManager propsManager, IAssetModel assets)
		{
			this.uiController = uiController;
			this.mapClickableManager = mapClickableManager;
			this.mapUI = mapUI;
			this.camera = camera;
			this.propsManager = propsManager;
			this.assets = assets;
			this.logicFiber = new Fiber(FiberBucket.Update);
			mapClickableManager.Down += this.MapClickableManagerOnDown;
		}

		public void Destroy()
		{
			this.mapClickableManager.Down -= this.MapClickableManagerOnDown;
			this.logicFiber.Terminate();
		}

		private Vector3 GetUICameraWorldPosition(Vector3 worldPosition)
		{
			Vector3 position = this.camera.CachedCamera.WorldToViewportPoint(worldPosition);
			return this.mapUI.ButtonsView.UICamera.ViewportToWorldPoint(position);
		}

		private void MapClickableManagerOnDown(Vector3 clickedWorldPosition, MapClickable mapclickable, MapProp mapprop)
		{
			this.logicFiber.Start(this.MapPropHeldDown(clickedWorldPosition, mapprop));
		}

		private IEnumerator MapPropHeldDown(Vector3 clickedWorldPosition, MapProp mapProp)
		{
			this.mapClickableManager.Down -= this.MapClickableManagerOnDown;
			yield return new Fiber.OnExit(delegate()
			{
				this.mapClickableManager.Down += this.MapClickableManagerOnDown;
			});
			using (ScopedView<ActivateChoosePropIndicatorView> activateIndicatorView = this.uiController.CreateScopedView<ActivateChoosePropIndicatorView>(this.assets.GetActivateChoosePropIndicatorView()))
			{
				ActivateChoosePropIndicatorView view = activateIndicatorView.CreateView();
				view.ShowIndicator(this.GetUICameraWorldPosition(clickedWorldPosition));
				EnumeratorResult<bool> didHoldStill = new EnumeratorResult<bool>();
				yield return this.HoldForDurationWithoutCameraMoving(0.55f, 20f, delegate(float p)
				{
					view.IndicatorProgress = p;
				}, didHoldStill);
				if (!didHoldStill)
				{
					yield break;
				}
			}
			yield return this.mapUI.HideUI();
			yield return this.ChooseProp(mapProp, false);
			yield return this.mapUI.ShowUI();
			yield break;
		}

		private IEnumerator HoldForDurationWithoutCameraMoving(float duration, float movementThreshold, Action<float> onProgress, EnumeratorResult<bool> result)
		{
			result.value = false;
			bool wasUp = false;
			bool secondaryTouchDown = false;
			MapClickableManager.ClickableEvent onUp = delegate(Vector3 p, MapClickable clickable, MapProp prop)
			{
				wasUp = true;
			};
			this.mapClickableManager.Up += onUp;
			Action<MapInputArea.MouseButtonEvent> touchDown = delegate(MapInputArea.MouseButtonEvent e)
			{
				if (e.Index == 0)
				{
					return;
				}
				secondaryTouchDown = true;
			};
			this.mapClickableManager.InputArea.TouchDown += touchDown;
			yield return new Fiber.OnExit(delegate()
			{
				this.mapClickableManager.Up -= onUp;
				this.mapClickableManager.InputArea.TouchDown -= touchDown;
			});
			Vector3 startCameraPosition = this.camera.transform.localPosition;
			float timer = 0f;
			while (timer < duration)
			{
				if (wasUp)
				{
					yield break;
				}
				if (secondaryTouchDown)
				{
					yield break;
				}
				Vector3 currentCameraPosition = this.camera.transform.localPosition;
				float distanceMoved = (currentCameraPosition - startCameraPosition).magnitude;
				if (distanceMoved > movementThreshold)
				{
					yield break;
				}
				onProgress(timer / duration);
				timer += Time.deltaTime;
				yield return null;
			}
			result.value = true;
			onProgress(1f);
			yield break;
		}

		public IEnumerator ChooseProp(MapProp prop, bool forcedFlow)
		{
			ChoosePropFlow choosePropFlow = new ChoosePropFlow(this.uiController, this.mapClickableManager, this.propsManager, this.assets);
			ZoomToPropController zoomToPropController = new ZoomToPropController(this.camera, this.mapClickableManager.InputArea, this.assets.GetChoosePropSettings());
			if (!forcedFlow)
			{
				choosePropFlow.SelectedPropChanged += zoomToPropController.ZoomToProp;
			}
			yield return new Fiber.OnExit(delegate()
			{
				if (!forcedFlow)
				{
					choosePropFlow.SelectedPropChanged -= zoomToPropController.ZoomToProp;
				}
				zoomToPropController.Stop();
			});
			yield return choosePropFlow.Run(prop, forcedFlow, new Action(zoomToPropController.Step));
			yield break;
		}

		private readonly IUIController uiController;

		private readonly MapClickableManager mapClickableManager;

		private readonly MainMapUI mapUI;

		private readonly MapCamera camera;

		private readonly PropsManager propsManager;

		private readonly IAssetModel assets;

		private readonly Fiber logicFiber;
	}
}
