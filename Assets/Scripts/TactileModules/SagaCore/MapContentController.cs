using System;
using System.Collections;
using System.Diagnostics;
using TactileModules.Foundation;
using UnityEngine;

namespace TactileModules.SagaCore
{
	public class MapContentController : SagaAvatarController.IDotPositions
	{
		public MapContentController(MapIdentifier mapId, CloudClient cloudClient, VipManager vipManager, FacebookClient facebookClient, MapFacade mapFacade)
		{
			this.MapIdentifier = mapId;
			this.Avatars = new SagaAvatarController(cloudClient, vipManager, facebookClient, mapFacade);
		}

		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<MapDotBase> DotClicked;



		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action Refreshed;



		public MapIdentifier MapIdentifier { get; private set; }

		public SagaMapView MapView { get; private set; }

		public SagaAvatarController Avatars { get; private set; }

		public void CreateViews(SagaAvatarController.IAvatarInfoProvider avatarInfoProvider, MapStreamer.IDataProvider mapStreamerDataProvider = null)
		{
			SagaMapView original = Resources.Load<SagaMapView>("Map/" + this.MapIdentifier + "MapSagaView");
			this.MapView = UnityEngine.Object.Instantiate<SagaMapView>(original);
			this.mapViewHandle = UIViewManager.Instance.ShowViewInstance<SagaMapView>(this.MapView, new object[0]);
			this.MapView.MapStreamer.Initialize(this.MapIdentifier, ManagerRepository.Get<MapStreamerCollection>(), mapStreamerDataProvider, null);
			this.MapView.MapStreamer.MapDotClicked += this.MapDotClicked;
			Rect visualBounds = new Rect(Vector2.zero, this.MapView.MapStreamer.ScrollPanel.TotalContentSize);
			this.Avatars.AttachToDotProvider(this, avatarInfoProvider, -30f, visualBounds);
			this.MapView.MapStreamer.FocusCameraOnLevel(this.currentFocusedDotIndex);
			PlayerAvatarMapLocation componentInChildren = this.MapView.GetComponentInChildren<PlayerAvatarMapLocation>();
			if (componentInChildren != null)
			{
				componentInChildren.Initialize(this.Avatars, this.MapView.MapStreamer.ScrollPanel);
			}
		}

		private void MapDotClicked(MapDotBase obj)
		{
			this.DotClicked(obj);
		}

		public void DestroyViews()
		{
			this.Avatars.DetachFromDotProvider();
			UIViewLayer viewLayerWithView = UIViewManager.Instance.GetViewLayerWithView(this.MapView);
			if (viewLayerWithView != null)
			{
				viewLayerWithView.CloseInstantly();
			}
			this.MapView = null;
		}

		public void Refresh()
		{
			if (this.MapView != null)
			{
				this.MapView.MapStreamer.ForceRespawn();
				this.Avatars.Refresh();
			}
		}

		public void JumpToDot(int dotIndex)
		{
			this.currentFocusedDotIndex = dotIndex;
			if (this.MapView != null)
			{
				this.MapView.MapStreamer.FocusCameraOnLevel(this.currentFocusedDotIndex);
			}
		}

		public void JumpToPosition(Vector2 pos)
		{
			this.MapView.MapStreamer.ScrollPanel.SetScroll(pos);
		}

		public IEnumerator PanToDot(int dotIndex, float speed = 1f, AnimationCurve curve = null)
		{
			this.currentFocusedDotIndex = dotIndex;
			if (this.MapView != null)
			{
				yield return this.MapView.MapStreamer.FocusCameraOnLevel(this.currentFocusedDotIndex, speed, curve);
			}
			yield break;
		}

		public Transform ContentRoot
		{
			get
			{
				return (!(this.MapView != null)) ? null : this.MapView.MapStreamer.ScrollPanel.ScrollRoot;
			}
		}

		public bool TryGetDotPosition(int dotIndex, out Vector3 posInMapSpace)
		{
			return this.MapView.MapStreamer.TryGetDotPosition(dotIndex, out posInMapSpace);
		}

		private UIViewManager.UIViewState mapViewHandle;

		private int currentFocusedDotIndex;
	}
}
