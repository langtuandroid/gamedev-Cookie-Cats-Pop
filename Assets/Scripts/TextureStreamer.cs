using System;
using System.Collections;
using Fibers;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class TextureStreamer : MonoBehaviour
{
	private void Awake()
	{
		this.fiber.Start(this.Load());
	}

	private void OnDestroy()
	{
		if (this.fiber != null)
		{
			this.fiber.Terminate();
		}
		if (this.usingTextureFromAssetbundle)
		{
			TextureStreamerAssetBundleManager.Instance.ReleaseAssetBundle(this.assetBundleName);
		}
	}

	private void Update()
	{
		this.fiber.Step();
	}

	private IEnumerator Load()
	{
		while (!TextureStreamerAssetBundleManager.Instance.IsAssetbundelsAvailable())
		{
			yield return null;
		}
		yield return TextureStreamerAssetBundleManager.Instance.DownloadAssetBundle(this.assetBundleName, delegate(object error, AssetBundle assetBundle)
		{
			if (assetBundle != null)
			{
				Texture2D texture2D = assetBundle.LoadAsset<Texture2D>(this.assetBundleName);
				if (texture2D != null)
				{
					MeshRenderer component = base.GetComponent<MeshRenderer>();
					component.material.mainTexture = texture2D;
					TextureStreamerAssetBundleManager.Instance.AcquireAssetBundle(this.assetBundleName);
					this.usingTextureFromAssetbundle = true;
				}
			}
		});
		yield break;
	}

	private Fiber fiber = new Fiber(FiberBucket.Manual);

	private bool usingTextureFromAssetbundle;

	[SerializeField]
	[HideInInspector]
	public string assetBundleName;
}
