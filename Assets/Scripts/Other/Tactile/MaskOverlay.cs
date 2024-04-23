using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using UnityEngine;

namespace Tactile
{
	public class MaskOverlay
	{
		public static MaskOverlay Instance
		{
			get
			{
				if (MaskOverlay.instance == null)
				{
					MaskOverlay.instance = new MaskOverlay();
					MaskOverlay.instance.Initialize();
				}
				return MaskOverlay.instance;
			}
		}

		public bool HasAnyCutouts()
		{
			return this.cutouts != null && this.cutouts.Count > 0;
		}

		public MaskOverlayCutout AddCutout(string layer = null)
		{
			MaskOverlayCutout maskOverlayCutout = new MaskOverlayCutout();
			maskOverlayCutout.changed = new Action(this.MarkDirty);
			maskOverlayCutout.Layer = layer;
			this.cutouts.Add(maskOverlayCutout);
			return maskOverlayCutout;
		}

		public void RemoveAllCutoutsFromLayer(string layer = null)
		{
			for (int i = 0; i < this.cutouts.Count; i++)
			{
				if (this.cutouts[i].Layer == layer)
				{
					this.cutouts[i].changed = null;
					this.cutouts.RemoveAt(i);
					i--;
				}
			}
			this.MarkDirty();
		}

		public void Clear()
		{
			for (int i = 0; i < this.cutouts.Count; i++)
			{
				this.cutouts[i].changed = null;
			}
			this.cutouts.Clear();
			this.defaultIdEnabled = false;
			this.enabledIds.Clear();
			this.MarkDirty();
			this.enabledFiber.Start(this.AnimateEnabledProgress(this.enabledProgress, 0f, 0.5f, AnimationCurve.EaseInOut(0f, 0f, 1f, 1f)));
		}

		public void RemoveCutout(MaskOverlayCutout cutout)
		{
			cutout.changed = null;
			this.cutouts.Remove(cutout);
			this.MarkDirty();
		}

		public float Alpha
		{
			get
			{
				return this.alpha;
			}
			set
			{
				this.alpha = value;
				this.UpdateAlpha();
			}
		}

		public bool Enabled
		{
			get
			{
				return this.defaultIdEnabled || this.enabledIds.Count > 0;
			}
		}

		public float Depth
		{
			get
			{
				return this.overlayCamera.depth;
			}
			set
			{
				this.overlayCamera.depth = value;
			}
		}

		public void Enable(string id = null)
		{
			if (id == null)
			{
				this.defaultIdEnabled = true;
			}
			else
			{
				this.enabledIds.Add(id);
			}
			if (this.Enabled)
			{
				this.overlayRenderer.gameObject.SetActive(true);
				this.enabledFiber.Start(this.AnimateEnabledProgress(this.enabledProgress, 1f, 0.5f, AnimationCurve.EaseInOut(0f, 0f, 1f, 1f)));
			}
			this.MarkDirty();
		}

		public void Disable(string id = null)
		{
			if (id == null)
			{
				this.defaultIdEnabled = false;
			}
			else
			{
				this.enabledIds.Remove(id);
			}
			if (!this.Enabled)
			{
				this.enabledFiber.Start(this.AnimateEnabledProgress(this.enabledProgress, 0f, 0.5f, AnimationCurve.EaseInOut(0f, 0f, 1f, 1f)));
			}
			this.MarkDirty();
		}

		public void Refresh()
		{
			if (!this.Enabled)
			{
				return;
			}
			this.wantsRefresh = 2;
			this.MarkDirty();
		}

		private void UpdateAlpha()
		{
			float num = this.alpha * this.enabledProgress;
			this.renderBufferCamera.backgroundColor = new Color(1f - num, 1f - num, 1f - num, 1f);
			this.MarkDirty();
			bool enabled = num > 0.001f;
			this.renderBufferCamera.enabled = enabled;
			this.overlayCamera.enabled = enabled;
		}

		private IEnumerator AnimateEnabledProgress(float source, float dest, float duration, AnimationCurve curve = null)
		{
			yield return FiberAnimation.Animate(duration, curve, delegate(float p)
			{
				this.enabledProgress = source + (dest - source) * p;
				this.UpdateAlpha();
			}, false);
			if (this.enabledProgress <= 0.001f)
			{
				this.overlayRenderer.gameObject.SetActive(false);
			}
			yield break;
		}

		private void MarkDirty()
		{
			this.renderBufferCameraObject.SetActive(true);
		}

		public void Initialize()
		{
			this.CreateRenderBufferObjects();
			this.CreateOverlayObjects();
			this.Alpha = 0.5f;
			this.enabledProgress = 0f;
			this.overlayRenderer.gameObject.SetActive(false);
		}

		private void CreateRenderBufferObjects()
		{
			this.renderBuffer = new RenderTexture(256, 256, 16);
			GameObject gameObject = new GameObject();
			gameObject.name = "RenderBufferCamera";
			gameObject.transform.position = Vector3.zero;
			this.renderBufferCamera = gameObject.AddComponent<Camera>();
			this.renderBufferCamera.cullingMask = 0;
			this.renderBufferCamera.targetTexture = this.renderBuffer;
			this.renderBufferCamera.orthographic = true;
			this.renderBufferCamera.orthographicSize = 1f;
			this.renderBufferCamera.nearClipPlane = 1f;
			this.renderBufferCamera.farClipPlane = 2f;
			this.renderBufferCamera.clearFlags = CameraClearFlags.Color;
			this.renderBufferCamera.backgroundColor = new Color(0.5f, 0.5f, 0.5f, 1f);
			this.blobsRenderer = gameObject.AddComponent<MaskOverlayRenderer>();
			this.blobsRenderer.onRender = new Action(this.RenderBlobs);
			MeshBuilder meshBuilder = new MeshBuilder();
			Matrix4x4 identity = Matrix4x4.identity;
			meshBuilder.AddQuad(ref identity, new Vector2(0.5f, 0.5f), new Rect(0f, 0f, 1f, 1f), Color.white, false);
			this.blobMesh = new Mesh();
			meshBuilder.ApplyToMesh(this.blobMesh);
			meshBuilder.Clear();
			meshBuilder.AddInsetQuad(ref identity, new Vector2(0.5f, 0.5f), new Rect(0.5f, 0.5f, 0.001f, 0.001f), Color.white, new Color(1f, 1f, 1f, 0f), 0.95f, false);
			this.blobRectangleMesh = new Mesh();
			meshBuilder.ApplyToMesh(this.blobRectangleMesh);
			this.blobMaterial = MaskOverlayResources.LoadBlobMaterial();
			this.blobMaterial.mainTexture = MaskOverlayResources.LoadBlobTexture_verySmooth();
			this.renderBufferCameraObject = gameObject;
		}

		private void CreateOverlayObjects()
		{
			GameObject gameObject = new GameObject();
			gameObject.name = "OverlayCamera";
			gameObject.transform.position = new Vector3(0f, 0f, -0.05f);
			this.overlayCamera = gameObject.AddComponent<Camera>();
			this.overlayCamera.cullingMask = 0;
			this.overlayCamera.orthographic = true;
			this.overlayCamera.rect = UIViewManager.Instance.GetMainViewZoneRenderRect();
			this.overlayCamera.orthographicSize = 1f;
			this.overlayCamera.nearClipPlane = 1f;
			this.overlayCamera.farClipPlane = 2f;
			this.overlayCamera.clearFlags = CameraClearFlags.Nothing;
			this.overlayCamera.depth = 100f;
			this.overlayRenderer = gameObject.AddComponent<MaskOverlayRenderer>();
			this.overlayRenderer.onRender = new Action(this.RenderOverlay);
			this.overlayMaterial = MaskOverlayResources.LoadOverlayMaterial();
			this.overlayMaterial.mainTexture = this.renderBuffer;
		}

		private void RenderBlobs()
		{
			if (this.wantsRefresh > 0)
			{
				for (int i = 0; i < this.cutouts.Count; i++)
				{
					this.cutouts[i].Refresh();
				}
			}
			this.blobMaterial.SetPass(0);
			foreach (MaskOverlayCutout maskOverlayCutout in this.cutouts)
			{
				Matrix4x4 matrix = Matrix4x4.TRS(new Vector3(maskOverlayCutout.ViewportPosition.x * 2f - 1f, maskOverlayCutout.ViewportPosition.y * 2f - 1f, 1f), Quaternion.identity, new Vector3(maskOverlayCutout.ViewportSize.x, maskOverlayCutout.ViewportSize.y, 1f));
				Graphics.DrawMeshNow((!maskOverlayCutout.Oval) ? this.blobRectangleMesh : this.blobMesh, matrix);
			}
			this.renderBufferCameraObject.SetActive(this.wantsRefresh > 0);
			if (this.wantsRefresh > 0)
			{
				this.wantsRefresh--;
			}
		}

		private void RenderOverlay()
		{
			this.overlayMaterial.SetPass(0);
			Graphics.DrawMeshNow(this.blobMesh, Matrix4x4.TRS(new Vector3(0f, 0f, 1f), Quaternion.identity, new Vector3(this.overlayCamera.aspect * 2f, 2f, 1f)));
		}

		public void Destroy()
		{
			UnityEngine.Object.Destroy(this.blobMaterial);
			this.blobMaterial = null;
			UnityEngine.Object.Destroy(this.overlayMaterial);
			this.overlayMaterial = null;
			UnityEngine.Object.Destroy(this.renderBuffer);
			this.renderBuffer = null;
			UnityEngine.Object.Destroy(this.renderBufferCamera.gameObject);
			this.renderBufferCameraObject = null;
			this.renderBufferCamera = null;
			UnityEngine.Object.Destroy(this.overlayCamera.gameObject);
			this.overlayCamera = null;
			UnityEngine.Object.Destroy(this.blobMesh);
			this.blobMesh = null;
			UnityEngine.Object.Destroy(this.blobRectangleMesh);
			this.blobRectangleMesh = null;
			this.overlayRenderer = null;
			this.blobsRenderer = null;
			this.cutouts.Clear();
			this.alpha = 0f;
			this.enabledProgress = 0f;
			this.enabledFiber.Terminate();
			this.enabledIds.Clear();
			this.defaultIdEnabled = false;
			this.wantsRefresh = 0;
		}

		private static MaskOverlay instance;

		private Material blobMaterial;

		private Material overlayMaterial;

		private GameObject renderBufferCameraObject;

		private RenderTexture renderBuffer;

		private Camera renderBufferCamera;

		private Camera overlayCamera;

		private Mesh blobMesh;

		private Mesh blobRectangleMesh;

		private MaskOverlayRenderer overlayRenderer;

		private MaskOverlayRenderer blobsRenderer;

		private List<MaskOverlayCutout> cutouts = new List<MaskOverlayCutout>();

		private float alpha;

		private float enabledProgress;

		private Fiber enabledFiber = new Fiber();

		private HashSet<string> enabledIds = new HashSet<string>();

		private bool defaultIdEnabled;

		private int wantsRefresh;
	}
}
