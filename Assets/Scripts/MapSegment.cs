using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using UnityEngine;

[RequireComponent(typeof(UITextureQuad))]
public class MapSegment : UIElement, UIScrollablePanel.IScrollVisibility
{
	public MapSettings MapSettings { get; set; }

	public UITextureQuad textureQuad
	{
		get
		{
			return base.GetComponent<UITextureQuad>();
		}
	}

	public bool ElementIsVisible
	{
		get
		{
			return base.gameObject.activeSelf;
		}
		set
		{
			if (value)
			{
				this.LoadTexture(false);
			}
			else
			{
				this.UnloadSegmentTextures();
			}
			base.gameObject.SetActive(value);
		}
	}

	public MapIdentifier MapIdentifier
	{
		get
		{
			return this.mapIdentifier;
		}
		set
		{
			this.mapIdentifier = value;
		}
	}

	private void OnDestroy()
	{
		if (this.material != null)
		{
			if (Application.isPlaying)
			{
				UnityEngine.Object.Destroy(this.material);
			}
			else
			{
				UnityEngine.Object.DestroyImmediate(this.material);
			}
		}
	}

	protected override void OnDisable()
	{
		this.loadFiber.Terminate();
		base.OnDisable();
	}

	private string GetImagePath(MapSegment.MapSegmentTexture segmentTexture)
	{
		if (segmentTexture == this.bigSegmentTexture)
		{
			return this.imagePath + "big/";
		}
		return this.imagePath + "small/";
	}

	private string GetImageName(MapSegment.MapSegmentTexture segmentTexture)
	{
		if (segmentTexture == this.bigSegmentTexture)
		{
			return this.imageName;
		}
		return this.imageName + "_small";
	}

	private void LoadTexture(bool useBigImage)
	{
		if (MapSegment.staticBlackMaterial == null)
		{
			MapSegment.staticBlackMaterial = new Material(Shader.Find("Unlit/Texture"));
			MapSegment.staticBlackTexture = new Texture2D(1, 1);
			MapSegment.staticBlackTexture.SetPixel(0, 0, Color.grey);
			MapSegment.staticBlackTexture.Apply();
			MapSegment.staticBlackMaterial.mainTexture = MapSegment.staticBlackTexture;
		}
		if (this.textureQuad.Material == null)
		{
			this.textureQuad.Material = MapSegment.staticBlackMaterial;
		}
		this.loadFiber.Start(this.LoadTextureCr());
	}

	private IEnumerator LoadTextureCr()
	{
		yield return this.LoadTextureInternal(this.smallSegmentTexture);
		yield return this.LoadTextureInternal(this.bigSegmentTexture);
		yield break;
	}

	private IEnumerator LoadTextureInternal(MapSegment.MapSegmentTexture segmentTexture)
	{
		yield return this.LoadTextureFromResources(segmentTexture);
		yield return this.LoadTextureFromAssetBundle(segmentTexture);
		yield break;
	}

	private IEnumerator LoadTextureFromResources(MapSegment.MapSegmentTexture segmentTexture)
	{
		if (segmentTexture.Texture == null)
		{
			string path = this.GetImagePath(segmentTexture) + this.GetImageName(segmentTexture);
			segmentTexture.Texture = (Texture2D)Resources.Load(path);
			this.UpdateMaterial(segmentTexture.Texture);
		}
		yield return null;
		yield break;
	}

	private IEnumerator LoadTextureFromAssetBundle(MapSegment.MapSegmentTexture segmentTexture)
	{
		if (segmentTexture.Texture == null || this.MapSettings.allowUpdatingOfResourceMapSegments)
		{
			string imageName = this.GetImageName(segmentTexture);
			Texture2D texture;
			if (segmentTexture == this.bigSegmentTexture)
			{
				EnumeratorResult<Texture2D> result = new EnumeratorResult<Texture2D>();
				yield return this.TryGetBigImageAsset(result, imageName);
				texture = result.value;
			}
			else
			{
				texture = this.LoadTextureFromAssetBundle(this.MapSettings.MapViewSetup.AssetBundle, imageName);
			}
			if (texture != null)
			{
				segmentTexture.Texture = texture;
				this.UpdateMaterial(segmentTexture.Texture);
			}
		}
		yield break;
	}

	private IEnumerator TryGetBigImageAsset(EnumeratorResult<Texture2D> results, string imageName)
	{
		yield return MapAssetBundleManager.Instance.DownloadBigMapSegment(imageName, delegate(object error, AssetBundle assetBundle)
		{
			if (error == null)
			{
				results.value = this.LoadTextureFromAssetBundle(assetBundle, imageName);
				assetBundle.Unload(false);
			}
		});
		yield break;
	}

	private Texture2D TryGetSmallImageAsset(List<Texture2D> mapsegments, string imageName)
	{
		foreach (Texture2D texture2D in mapsegments)
		{
			if (texture2D.name == imageName)
			{
				return texture2D;
			}
		}
		return null;
	}

	private Texture2D LoadTextureFromAssetBundle(AssetBundle assetBundle, string imageName)
	{
		if (assetBundle != null)
		{
			return assetBundle.LoadAsset<Texture2D>(imageName);
		}
		return null;
	}

	public void UnloadSegmentTextures()
	{
		this.textureQuad.Material = null;
		this.UnloadSegmentTexture(this.bigSegmentTexture);
		this.UnloadSegmentTexture(this.smallSegmentTexture);
	}

	private void UnloadSegmentTexture(MapSegment.MapSegmentTexture asset)
	{
		Resources.UnloadAsset(asset.Texture);
		asset.Texture = null;
	}

	private void UpdateMaterial(Texture2D asset)
	{
		if (asset != null)
		{
			if (this.material == null)
			{
				this.material = new Material(Shader.Find("Unlit/Texture"));
			}
			if (this.material != null)
			{
				this.material.mainTexture = asset;
				this.textureQuad.Material = this.material;
				this.material.renderQueue = 1000;
			}
		}
	}

	private Material material;

	private static Material staticBlackMaterial;

	private static Texture2D staticBlackTexture;

	public string imageName;

	public string imagePath;

	[SerializeField]
	private MapIdentifier mapIdentifier;

	private MapSegment.MapSegmentTexture bigSegmentTexture = new MapSegment.MapSegmentTexture();

	private MapSegment.MapSegmentTexture smallSegmentTexture = new MapSegment.MapSegmentTexture();

	private Fiber loadFiber = new Fiber();

	private class MapSegmentTexture
	{
		public Texture2D Texture { get; set; }
	}
}
