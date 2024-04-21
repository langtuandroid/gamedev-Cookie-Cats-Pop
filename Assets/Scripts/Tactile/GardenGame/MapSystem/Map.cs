using System;
using System.Collections.Generic;
using Fibers;
using Tactile.GardenGame.MapSystem.Navigation;
using Tactile.GardenGame.MapSystem.Tiles;
using TactileModules.GardenGame.MapSystem.Assets;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem
{
	public class Map
	{
		public Map(PropsManager propsManager, Func<Camera> cameraFunction, IAssetModel assets)
		{
			this.PropsManager = propsManager;
			this.assets = assets;
			this.cameraFunction = cameraFunction;
			this.Areas = new MapAreas(19);
			MapLighting.Initialize();
			MapHeightMap.Initialize(this.Areas);
			this.Entities = new MapInformation(this.Areas);
			this.tileRenderer = new MapTileRenderer(assets.GetMapTileDatabase(), new Vector2(1024f, 1024f), this.Entities.GroundBounds.min, null);
			this.SetupCamera(this.Entities.FindObjectWithComponent<MapCamera>(), this.tileRenderer);
			this.ClickableManager = new MapClickableManager(this.InputArea, this.Areas, this.Camera);
			this.InitializeMapComponents();
			this.Camera.CameraRendered += this.CameraOnCameraRendered;
			this.PropsManager.PropStateChanged += this.ApplySavedState;
			this.ApplySavedState();
			this.Navigation = new MapNavigationMesh();
			this.Navigation.Initialize();
			this.Navigation.UpdateMesh(this.Areas, this.Entities.GroundBounds);
			this.cameraController = new MapCameraController(this.InputArea, this.Camera);
		}

		public PropsManager PropsManager { get; private set; }

		public MapCamera Camera { get; private set; }

		public MapInputArea InputArea { get; private set; }

		public MapAreas Areas { get; set; }

		public MapInformation Entities { get; private set; }

		public MapNavigationMesh Navigation { get; private set; }

		public MapClickableManager ClickableManager { get; private set; }

		public Camera GUICamera
		{
			get
			{
				return this.cameraFunction();
			}
		}

		private void CameraOnCameraRendered(Camera camera)
		{
			MapLighting.UpdateLighting(camera, GardenGameSetup.Get.lightingRenderTexture, GardenGameSetup.Get.lightingLayer);
			MapHeightMap.UpdateHeightmap(camera, GardenGameSetup.Get.heightMapRenderTexture);
		}

		public void Destroy()
		{
			this.PropsManager.PropStateChanged -= this.ApplySavedState;
			this.Camera.Destroy();
			this.Areas.Destroy();
			this.tileRenderer.Destroy();
			UnityEngine.Object.Destroy(this.InputArea.gameObject);
			this.InputArea = null;
			this.cameraController.Destroy();
		}

		private void InitializeMapComponents()
		{
			List<MapComponent> list = new List<MapComponent>(this.Areas.IterateComponents<MapComponent>());
			list.Sort((MapComponent a, MapComponent b) => a.IntializationOrder.CompareTo(b.IntializationOrder));
			foreach (MapComponent mapComponent in list)
			{
				mapComponent.Initialize(this);
			}
		}

		private void SetupCamera(MapCamera mapCamera, MapTileRenderer tileRenderer)
		{
			this.Camera = mapCamera;
			this.Camera.Initialize();
			MapTileCamera mapTileCamera = this.Camera.gameObject.AddComponent<MapTileCamera>();
			mapTileCamera.Initialize(this.Camera.CachedCamera, tileRenderer);
			UICamera.SortCamerasByDepth();
			this.InputArea = this.CreateInputArea(this.Camera.CachedCamera);
		}

		private MapInputArea CreateInputArea(Camera unityCamera)
		{
			GameObject gameObject = new GameObject("InputArea");
			BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
			boxCollider.size = this.Entities.GroundBounds.size + new Vector3(0f, 0f, 2f);
			Vector3 center = this.Entities.GroundBounds.center;
			center.z = 0f;
			gameObject.transform.position = center + new Vector3(0f, 0f, unityCamera.transform.position.z + unityCamera.nearClipPlane + boxCollider.size.z);
			gameObject.gameObject.SetLayerRecursively(19);
			return gameObject.AddComponent<MapInputArea>();
		}

		public void ApplySavedState()
		{
			foreach (MapProp mapProp in this.Entities.IterateObjectsWithComponent<MapProp>())
			{
				MapObjectID component = mapProp.gameObject.GetComponent<MapObjectID>();
				if (component != null)
				{
					int propSkin = this.PropsManager.GetPropSkin(component.Id);
					if (propSkin != -1)
					{
						mapProp.SetCurrentVariation(propSkin);
					}
					else
					{
						MapProp.Variation defaultVariation = mapProp.DefaultVariation;
						if (defaultVariation != null)
						{
							mapProp.SetCurrentVariation(mapProp.DefaultVariation.ID);
						}
					}
				}
			}
		}

		public bool Visible
		{
			get
			{
				return this.Areas.Visible;
			}
			set
			{
				this.Areas.Visible = value;
				this.tileRenderer.Visible = value;
			}
		}

		public bool InteractionEnabled
		{
			get
			{
				return this.InputArea.enabled;
			}
			set
			{
				this.InputArea.enabled = value;
				this.ClickableManager.Enabled = value;
				this.cameraController.Enabled = value;
			}
		}

		private const int MAP_LAYER = 19;

		private FiberRunner runFiber;

		private MapTileRenderer tileRenderer;

		private Fiber moveFiber;

		private readonly IAssetModel assets;

		private readonly Func<Camera> cameraFunction;

		private MapCameraController cameraController;
	}
}
