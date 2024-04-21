using System;
using UnityEngine;

namespace TactileModules.SagaCore
{
	public static class SagaMapViewFactory
	{
		public static void EnsureViewTemplate()
		{
			if (!UIViewManager.Instance.HasDynamicView(SagaMapViewFactory.VIEW_NAME))
			{
				SagaMapView view = SagaMapViewFactory.CreateViewObject();
				UIViewManager.Instance.AddNewViewToPool(view);
			}
		}

		public static SagaMapView CreateViewObject()
		{
			GameObject gameObject = new GameObject(SagaMapViewFactory.VIEW_NAME);
			SagaMapView sagaMapView = gameObject.AddComponent<SagaMapView>();
			sagaMapView.BlockOtherViews = false;
			sagaMapView.UseDefaultViewAnimator = false;
			Vector2 size = new Vector2(640f, 960f);
			sagaMapView.GetElement().Size = size;
			sagaMapView.MarkOriginalSize();
			GameObject gameObject2 = new GameObject("ScrollPanel");
			gameObject2.transform.parent = gameObject.transform;
			gameObject2.transform.localPosition = Vector3.zero;
			UIElement uielement = gameObject2.AddComponent<UIElement>();
			uielement.Size = sagaMapView.GetElementSize();
			uielement.autoSizing = UIAutoSizing.AllCorners;
			UIScrollablePanel uiscrollablePanel = gameObject2.AddComponent<UIScrollablePanel>();
			uiscrollablePanel.scrollAxis = UIScrollablePanel.ScrollAxis.Vertical;
			uiscrollablePanel.autoContentSizeLimits.width = 640f;
			uiscrollablePanel.startFromTop = false;
			uiscrollablePanel.limitAutoContentSize = true;
			uiscrollablePanel.autoDisableHiddenElements = true;
			gameObject2.GetComponent<BoxCollider>().size = new Vector3(uielement.Size.x, uielement.Size.y, 300f);
			MapStreamer mapStreamer = gameObject2.AddComponent<MapStreamer>();
			mapStreamer.mapSegmentNumberFormat = "D3";
			Rigidbody rigidbody = gameObject2.AddComponent<Rigidbody>();
			rigidbody.isKinematic = true;
			return sagaMapView;
		}

		public static readonly string VIEW_NAME = "SagaMapView";
	}
}
