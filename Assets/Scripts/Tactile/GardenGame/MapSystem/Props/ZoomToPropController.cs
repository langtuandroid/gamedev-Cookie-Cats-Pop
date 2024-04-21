using System;
using System.Collections;
using Fibers;
using Tactile.GardenGame.MapSystem.Helpers;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem.Props
{
	public class ZoomToPropController
	{
		public ZoomToPropController(MapCamera camera, MapInputArea inputArea, ChoosePropSettings settings)
		{
			this.camera = camera;
			this.inputArea = inputArea;
			this.settings = settings;
			this.zoomFiber = new Fiber(FiberBucket.Manual);
		}

		public void ZoomToProp(MapProp prop)
		{
			this.zoomFiber.Start(this.Zoom(prop));
		}

		private IEnumerator Zoom(MapProp prop)
		{
			this.inputArea.TouchDown += this.InputAreaOnTouchDown;
			yield return new Fiber.OnExit(delegate()
			{
				this.inputArea.TouchDown -= this.InputAreaOnTouchDown;
			});
			Bounds bounds;
			if (!BoundsHelper.TryGetRendererBounds(prop.gameObject, out bounds))
			{
				yield break;
			}
			yield return this.camera.AnimateToBounds(bounds, this.settings.ZoomToPropDuration, this.settings.ZoomToPropCurve, this.settings.MoveToPropCurve);
			yield break;
		}

		private void InputAreaOnTouchDown(MapInputArea.MouseButtonEvent obj)
		{
			this.Stop();
		}

		public void Step()
		{
			this.zoomFiber.Step();
		}

		public void Stop()
		{
			this.zoomFiber.Terminate();
		}

		private readonly MapCamera camera;

		private readonly MapInputArea inputArea;

		private readonly ChoosePropSettings settings;

		private readonly Fiber zoomFiber;
	}
}
