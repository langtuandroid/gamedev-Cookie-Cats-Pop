using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TactileModules.MapStreaming.Runtime;
using TactileModules.RuntimeTools;
using UnityEngine;

[RequireComponent(typeof(UIScrollablePanel))]
public class MapStreamer : MonoBehaviour
{
    public MapIdentifier MapIdentifier
    {
        get
        {
            return this.mapIdentifier;
        }
    }

    //[DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public event Action<MapDotBase> MapDotClicked;

    public SpawnPool SpawnPool { get; private set; }

    public Transform SpawnedContentRoot { get; private set; }

    public bool DisableSpawning { get; set; }

    public UIScrollablePanel ScrollPanel
    {
        get
        {
            if (this.cachedScrollPanel == null)
            {
                this.cachedScrollPanel = base.GetComponent<UIScrollablePanel>();
            }
            return this.cachedScrollPanel;
        }
    }

    public void Initialize(MapIdentifier pMapIdentifier, MapStreamerCollection pMapStreamerCollection, MapStreamer.IDataProvider pDataProvider = null, Func<MapElementData, bool> pGameSpecificSpawnFilter = null)
    {
        this.gameSpecificSpawnFilter = pGameSpecificSpawnFilter;
        this.mapIdentifier = pMapIdentifier;
        this.mapStreamerCollection = pMapStreamerCollection;
        this.MapSettings = this.mapStreamerCollection.GetMapSettingsFromMapIdentifier(pMapIdentifier);
        MapViewSetup mapViewSetupFromMapIdentifier = this.mapStreamerCollection.GetMapViewSetupFromMapIdentifier(pMapIdentifier);
        this.MapContentPrefab = this.mapStreamerCollection.GetMapRuntimeRoot(pMapIdentifier);
        MapRuntimeRoot component = this.MapContentPrefab.GetComponent<MapRuntimeRoot>();
        foreach (Transform transform in component.prefabsUsedFromLocalAssets)
        {
            this.prefabsByName.Add(transform.name, transform);
        }
        MapStreamerRuntimeSetup mapStreamerRuntimeSetup = this.PrepareRuntimeSetup(this.MapSettings, mapViewSetupFromMapIdentifier, pDataProvider);
        if (this.MapSettings.autogenerateMapsegments)
        {
            this.GenerateMapsegments(mapStreamerRuntimeSetup.totalContentSize);
        }
        IEnumerator enumerator2 = this.MapContentPrefab.transform.GetEnumerator();
        try
        {
            while (enumerator2.MoveNext())
            {
                object obj = enumerator2.Current;
                Transform transform2 = (Transform)obj;
                if (!(transform2 == this.MapContentPrefab.transform))
                {
                    Transform transform3 = UnityEngine.Object.Instantiate<Transform>(transform2);
                    transform3.transform.parent = this.ScrollPanel.ScrollRoot;
                    transform3.transform.localPosition = transform2.transform.localPosition + (Vector3)this.MapContentPrefab.GetElementSize() * 0.5f;
                    transform3.gameObject.SetLayerRecursively(base.gameObject.layer);
                }
            }
        }
        finally
        {
            IDisposable disposable;
            if ((disposable = (enumerator2 as IDisposable)) != null)
            {
                disposable.Dispose();
            }
        }
        this.SpawnedContentRoot = new GameObject("[Spawned Content Root]").transform;
        this.SpawnedContentRoot.transform.parent = this.ScrollPanel.ScrollRoot;
        this.SpawnedContentRoot.transform.localPosition = Vector3.zero;
        this.SpawnedContentRoot.gameObject.SetLayerRecursively(base.gameObject.layer);
        if (mapStreamerRuntimeSetup.spawnDynamicEndPiece)
        {
            this.SpawnEndPiece(mapStreamerRuntimeSetup.endPiecePrefab, mapStreamerRuntimeSetup.endPieceSpawnPosition, this.SpawnedContentRoot);
        }
        this.SpawnPool = PoolManager.Pools.Create("_poolRoot_" + this.mapIdentifier);
        this.SpawnPool.transform.parent = base.transform;
        this.overlapChecker = new Overlap1DChecker<MapElementData>(mapViewSetupFromMapIdentifier.mapElementData, (MapElementData elementData) => new Vector2(elementData.bottomEdge, elementData.topEdge));
        UIScrollablePanel scrollPanel = this.ScrollPanel;
        scrollPanel.OnContentClicked = (Action<GameObject>)Delegate.Combine(scrollPanel.OnContentClicked, new Action<GameObject>(this.HandleScrollPanelClicked));
        this.ScrollPanel.PrepareOptimization();
        this.ScrollPanel.SetTotalContentSize(mapStreamerRuntimeSetup.totalContentSize.x, mapStreamerRuntimeSetup.totalContentSize.y);
        if (this.BackgroundLayer > 0)
        {
            this.viewOwner = base.transform.GetComponentInParent<UIView>();
            GameObject gameObject = new GameObject("BackgroundCamera");
            this.backgroundCamera = gameObject.AddComponent<Camera>();
            FiberCtrl.Pool.Run(this.InitializeBackgroundCameraAndSegments(), false);
        }
    }

    private MapStreamerRuntimeSetup PrepareRuntimeSetup(MapSettings mapSettings, MapViewSetup mapViewSetup, MapStreamer.IDataProvider dataProvider)
    {
        MapStreamerRuntimeSetup mapStreamerRuntimeSetup = new MapStreamerRuntimeSetup();
        mapStreamerRuntimeSetup.spawnDynamicEndPiece = (dataProvider != null);
        if (mapStreamerRuntimeSetup.spawnDynamicEndPiece)
        {
            Vector3 v;
            this.FillDataVisibility(mapViewSetup, dataProvider, out v);
            mapStreamerRuntimeSetup.endPiecePrefab = this.LoadDynamicEndPiecePrefab(mapSettings);
            mapStreamerRuntimeSetup.endPieceSpawnPosition = this.ChooseDynamicEndPiecePosition(mapViewSetup, v, mapStreamerRuntimeSetup.endPiecePrefab.GetComponent<RuntimeEndPiece>());
            mapStreamerRuntimeSetup.totalContentSize = this.ChooseTotalContentSize(mapSettings, mapViewSetup, mapStreamerRuntimeSetup.endPieceSpawnPosition);
        }
        else
        {
            mapStreamerRuntimeSetup.totalContentSize = mapViewSetup.totalContentSize;
        }
        return mapStreamerRuntimeSetup;
    }

    private Vector2 ChooseTotalContentSize(MapSettings mapSettings, MapViewSetup mapViewSetup, Vector2 endPiecePosition)
    {
        return new Vector2(mapViewSetup.totalContentSize.x, endPiecePosition.y + mapSettings.endPieceVisibleHeight);
    }

    private Vector2 ChooseDynamicEndPiecePosition(MapViewSetup mapViewSetup, Vector2 lastAvailableDotPosition, RuntimeEndPiece endPieceSettings)
    {
        Vector2 result = new Vector2(mapViewSetup.totalContentSize.x * 0.5f, lastAvailableDotPosition.y);
        if (lastAvailableDotPosition.y < mapViewSetup.totalContentSize.y)
        {
            float y = endPieceSettings.lowestFullyOpaquePosition.localPosition.y;
            result.y = Mathf.Min(result.y, mapViewSetup.totalContentSize.y - y);
        }
        return result;
    }

    private void FillDataVisibility(MapViewSetup pMapViewSetup, MapStreamer.IDataProvider pDataProvider, out Vector3 lastAvailableDotPosition)
    {
        int count = pMapViewSetup.mapElementData.Count;
        this.elementDataVisibility = new Dictionary<MapElementData, bool>(count);
        lastAvailableDotPosition = Vector3.zero;
        int maxAvailableLevel = pDataProvider.MaxAvailableLevel;
        int num = 0;
        this.TryGetDotPosition(maxAvailableLevel, out lastAvailableDotPosition);
        int i = 0;
        while (i < count)
        {
            MapElementData mapElementData = pMapViewSetup.mapElementData[i];
            Transform transform = (!mapElementData.usingAssetBundle) ? this.GetLocalPrefabFromName(mapElementData.prefabName) : null;
            if (!(transform != null))
            {
                goto IL_AF;
            }
            MapDotBase component = transform.GetComponent<MapDotBase>();
            if (!(component != null))
            {
                goto IL_AF;
            }
            int num2 = num;
            this.elementDataVisibility.Add(mapElementData, num2 <= maxAvailableLevel);
            num++;
            IL_BD:
            i++;
            continue;
            IL_AF:
            this.elementDataVisibility.Add(mapElementData, true);
            goto IL_BD;
        }
    }

    private bool BaseElementFilter(MapElementData pData)
    {
        return this.elementDataVisibility == null || this.elementDataVisibility[pData];
    }

    private GameObject LoadDynamicEndPiecePrefab(MapSettings mapSettings)
    {
        string path = AssetPathUtility.AssetPathToResourcePath(mapSettings.endPiecePrefabPath);
        return Resources.Load<GameObject>(path);
    }

    private void SpawnEndPiece(GameObject prefab, Vector2 position, Transform parent)
    {
        Transform transform = UnityEngine.Object.Instantiate<GameObject>(prefab).transform;
        transform.SetParent(parent);
        transform.localPosition = new Vector3(position.x, position.y, prefab.transform.position.z);
        transform.gameObject.SetLayerRecursively(parent.gameObject.layer);
    }

    public void FocusCameraOnLevel(int index)
    {
        Vector3 vector;
        if (this.TryGetDotPosition(index, out vector))
        {
            Vector3 vector2 = this.ScrollPanel.actualScrollOffset;
            Vector3 v = new Vector3(vector2.x, -vector.y, vector2.z);
            this.ScrollPanel.SetScroll(v);
        }
    }

    public IEnumerator FocusCameraOnLevel(int index, float duration, AnimationCurve curve = null)
    {
        Vector3 pos;
        if (this.TryGetDotPosition(index, out pos))
        {
            Vector3 vector = this.ScrollPanel.actualScrollOffset;
            Vector3 v = new Vector3(vector.x, -pos.y, vector.z);
            this.ScrollPanel.SetScrollAnimated(v, duration, curve);
        }
        yield return this.ScrollPanel.WaitForScrollAnimation();
        yield break;
    }

    public IEnumerator FocusCameraBetweenTwoLevels(int startIndex, int endIndex, float duration, AnimationCurve curve = null)
    {
        Vector3 startPosition;
        Vector3 a;
        if (this.TryGetDotPosition(startIndex, out startPosition) && this.TryGetDotPosition(endIndex, out a))
        {
            Vector3 a2 = a - startPosition;
            Vector3 vector = a - a2 / 2f;
            Vector3 vector2 = this.ScrollPanel.actualScrollOffset;
            Vector3 v = new Vector3(vector2.x, -vector.y, vector2.z);
            this.ScrollPanel.SetScrollAnimated(v, duration, curve);
        }
        yield return this.ScrollPanel.WaitForScrollAnimation();
        yield break;
    }

    private void HandleScrollPanelClicked(GameObject obj)
    {
        MapDotBase component = obj.GetComponent<MapDotBase>();
        if (component != null && this.MapDotClicked != null)
        {
            this.MapDotClicked(component);
        }
    }

    private void OnDestroy()
    {
        this.DespawnAll();
        if (this.BackgroundLayer > 0)
        {
            UIViewManager.Instance.OnScreenChanged -= this.SyncBackgroundCameraToViewLayerCamera;
        }
    }

    public void DespawnAll()
    {
        foreach (KeyValuePair<MapElementData, MapElementSpawner> keyValuePair in this.spawnedObjects)
        {
            keyValuePair.Value.Despawn();
        }
        this.spawnedObjects.Clear();
    }

    public void ForceRespawn()
    {
        this.DespawnAll();
        this.overlapChecker.Clear();
        this.SpawnOrDespawnElements();
    }

    private void Update()
    {
        this.SpawnOrDespawnElements();
    }

    public bool TryGetDotPosition(int dotIndex, out Vector3 position)
    {
        MapViewSetup mapViewSetup = this.MapSettings.MapViewSetup;
        if (dotIndex < 0 || dotIndex >= mapViewSetup.dotPositions.Count)
        {
            dotIndex = Mathf.Clamp(dotIndex, 0, mapViewSetup.dotPositions.Count - 1);
        }
        position = mapViewSetup.dotPositions[dotIndex];
        return true;
    }

    private void SpawnOrDespawnElements()
    {
        if (this.DisableSpawning)
        {
            return;
        }
        float num = this.GetElementSize().y * 0.5f;
        float y = -this.ScrollPanel.ScrollRoot.localPosition.y + num + this.preloadMargin;
        float x = -this.ScrollPanel.ScrollRoot.localPosition.y - num - this.preloadMargin;
        while (this.overlapChecker.Check(new Vector2(x, y), delegate (MapElementData elementData, bool isInside)
        {
            bool flag = this.spawnedObjects.ContainsKey(elementData);
            if (!isInside && flag)
            {
                this.DespawnElement(elementData);
            }
            else if (isInside && !flag)
            {
                this.SpawnElement(elementData);
            }
        }))
        {
        }
    }

    public Transform GetLocalPrefabFromName(string prefabName)
    {
        if (!this.prefabsByName.ContainsKey(prefabName))
        {
            return null;
        }
        return this.prefabsByName[prefabName];
    }

    private void SpawnElement(MapElementData e)
    {
        bool flag = this.BaseElementFilter(e);
        if (this.gameSpecificSpawnFilter != null)
        {
            flag = (flag && this.gameSpecificSpawnFilter(e));
        }
        if (flag)
        {
            MapElementSpawner mapElementSpawner = new MapElementSpawner();
            mapElementSpawner.Spawn(this, e);
            this.spawnedObjects.Add(e, mapElementSpawner);
        }
    }

    private void DespawnElement(MapElementData e)
    {
        this.spawnedObjects[e].Despawn();
        this.spawnedObjects.Remove(e);
    }

    private IEnumerator InitializeBackgroundCameraAndSegments()
    {
        yield return null;
        if (base.transform == null)
        {
            yield break;
        }
        MapSegment[] mapSegments = base.GetComponentsInChildren<MapSegment>(true);
        foreach (MapSegment mapSegment in mapSegments)
        {
            mapSegment.gameObject.SetLayerRecursively(this.BackgroundLayer);
        }
        UIViewLayer viewLayer = UIViewManager.Instance.GetViewLayerWithView(this.viewOwner);
        this.backgroundCamera.transform.parent = viewLayer.ViewCamera.transform;
        this.backgroundCamera.transform.localPosition = Vector3.zero;
        this.SyncBackgroundCameraToViewLayerCamera();
        UIViewManager.Instance.OnScreenChanged += this.SyncBackgroundCameraToViewLayerCamera;
        yield break;
    }

    private void SyncBackgroundCameraToViewLayerCamera()
    {
        UIViewLayer viewLayerWithView = UIViewManager.Instance.GetViewLayerWithView(this.viewOwner);
        if (viewLayerWithView == null)
        {
            return;
        }
        Camera viewCamera = viewLayerWithView.ViewCamera;
        if (viewCamera == null)
        {
            return;
        }
        this.backgroundCamera.CopyFrom(viewCamera);
        this.backgroundCamera.cullingMask = 1 << this.BackgroundLayer;
        this.backgroundCamera.depth = (float)this.BackgroundLayer;
    }

    private void GenerateMapsegments(Vector2 pTotalContentSize)
    {
        Vector2 vector = pTotalContentSize;
        int num = (int)vector.x;
        int num2 = Mathf.FloorToInt(vector.y / (float)num) + 1;
        string text = this.MapSettings.autogeneratedImageFolder;
        int num3 = text.IndexOf("Resources/");
        if (num3 > 0)
        {
            text = text.Substring(num3 + 10);
        }
        int num4 = this.MapSettings.startAtMapIndex + num2;
        for (int i = this.MapSettings.startAtMapIndex; i < num4; i++)
        {
            int num5 = i - this.MapSettings.startAtMapIndex;
            Vector3 center = new Vector3((float)num * 0.5f, (float)num * 0.5f + (float)(num5 * num) - (float)i * 0.05f, (float)i * 0.01f);
            string text2 = this.MapSettings.imageFileBaseName + "_" + i.ToString(this.mapSegmentNumberFormat);
            this.GenerateMapSegment(text2, num, center, text, Vector2.zero);
            if (this.MapSettings.hasSideTiles)
            {
                this.GenerateMapSegment(text2 + "_L0", num / 2, center, text, new Vector2(-1.5f, 0.5f));
                this.GenerateMapSegment(text2 + "_L1", num / 2, center, text, new Vector2(-1.5f, -0.5f));
                this.GenerateMapSegment(text2 + "_R0", num / 2, center, text, new Vector2(1.5f, 0.5f));
                this.GenerateMapSegment(text2 + "_R1", num / 2, center, text, new Vector2(1.5f, -0.5f));
            }
        }
    }

    private void GenerateMapSegment(string imageName, int mapSegmentSize, Vector3 center, string pathInResources, Vector2 offset)
    {
        GameObject gameObject = new GameObject("Autogenerated_" + imageName);
        MapSegment mapSegment = gameObject.AddComponent<MapSegment>();
        gameObject.transform.parent = this.ScrollPanel.ScrollRoot;
        mapSegment.Size = Vector2.one * (float)mapSegmentSize;
        mapSegment.MapIdentifier = this.mapIdentifier;
        mapSegment.MapSettings = this.mapStreamerCollection.GetMapSettingsFromMapIdentifier(this.mapIdentifier);
        gameObject.transform.localPosition = center + (Vector3)offset * (float)mapSegmentSize;
        mapSegment.imagePath = pathInResources;
        mapSegment.imageName = imageName;
        gameObject.gameObject.SetLayerRecursively(base.gameObject.layer);
    }

    public void AddSpawnFilter(Func<MapElementData, bool> filter)
    {
        this.gameSpecificSpawnFilter = (Func<MapElementData, bool>)Delegate.Combine(this.gameSpecificSpawnFilter, filter);
    }

    public void RemoveAllSpawnFilters()
    {
        this.gameSpecificSpawnFilter = null;
    }

    [SerializeField]
    private MapIdentifier mapIdentifier;

    private MapSettings MapSettings;

    private MapStreamerCollection mapStreamerCollection;

    private MapRuntimeRoot MapContentPrefab;

    public float preloadMargin;

    public int BackgroundLayer;

    public string mapSegmentNumberFormat = "D3";

    private Func<MapElementData, bool> gameSpecificSpawnFilter;

    private Dictionary<string, Transform> prefabsByName = new Dictionary<string, Transform>();

    private Dictionary<MapElementData, MapElementSpawner> spawnedObjects = new Dictionary<MapElementData, MapElementSpawner>();

    private Dictionary<MapElementData, bool> elementDataVisibility;

    private Overlap1DChecker<MapElementData> overlapChecker;

    private UIScrollablePanel cachedScrollPanel;

    private Camera backgroundCamera;

    private UIView viewOwner;

    public interface IDataProvider
    {
        int MaxAvailableLevel { get; }
    }
}
