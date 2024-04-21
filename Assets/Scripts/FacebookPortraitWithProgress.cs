using System;
using System.Collections;
using Fibers;
using UnityEngine;

public class FacebookPortraitWithProgress : MonoBehaviour
{
	private void Awake()
	{
		this.defaultTexture = (this.portrait.MainTexture as Texture2D);
		this.CreateMaterialDuplicate();
	}

	private void CreateMaterialDuplicate()
	{
		if (this.materialDuplicateCreated)
		{
			return;
		}
		this.materialDuplicateCreated = true;
		this.material = new Material(this.portrait.renderer.sharedMaterial);
		Material material = this.material;
		material.name += " Cloned";
		this.portrait.renderer.sharedMaterial = this.material;
	}

	private void SetTexture(Texture2D texture)
	{
		this.CreateMaterialDuplicate();
		this.material.mainTexture = texture;
	}

	private void OnEnable()
	{
		this.progress.gameObject.SetActive(false);
	}

	private void OnDestroy()
	{
		UnityEngine.Object.Destroy(this.material);
		if (this.fiber != null)
		{
			this.fiber.Terminate();
		}
	}

	public void Reset()
	{
		if (this.fiber != null)
		{
			this.fiber.Terminate();
		}
		this.progress.gameObject.SetActive(false);
		this.SetTexture(this.defaultTexture);
	}

	public void Load(FacebookClient facebookClient, string facebookId, string url = null)
	{
		this.lastLoadFacebookId = facebookId;
		if (this.fiber != null)
		{
			this.fiber.Terminate();
		}
		this.fiber = new Fiber();
		this.fiber.Start(this.LoadCr(facebookClient, facebookId, url));
	}

	private IEnumerator LoadCr(FacebookClient facebookClient, string facebookId, string url = null)
	{
		this.progress.gameObject.SetActive(true);
		if (!string.IsNullOrEmpty(facebookId))
		{
			IEnumerator e = this.LoadImageFromFacebookIdCr(facebookClient, facebookId, delegate(object err)
			{
			});
			while (e.MoveNext())
			{
				object obj = e.Current;
				yield return obj;
			}
		}
		if (!string.IsNullOrEmpty(url))
		{
			IEnumerator e2 = this.LoadImageFromFacebookUrlCr(facebookClient, url, delegate(object err)
			{
			});
			while (e2.MoveNext())
			{
				object obj2 = e2.Current;
				yield return obj2;
			}
		}
		if (this.progress != null)
		{
			this.progress.gameObject.SetActive(false);
		}
		yield break;
	}

	private IEnumerator LoadImageFromFacebookIdCr(FacebookClient facebookClient, string facebookId, Action<object> callback)
	{
		IEnumerator e = facebookClient.LoadPortrait(facebookId, delegate(object error, Texture2D tex)
		{
			if (tex != null)
			{
				if (facebookId == this.lastLoadFacebookId)
				{
					this.SetTexture(tex);
					this.portrait.Color = Color.white;
				}
			}
			else
			{
				string arg = "Failed to load facebook portrait for " + facebookId;
				if (error != null)
				{
					arg = arg + ". Error: " + error;
				}
			}
			callback(error);
		});
		while (e.MoveNext())
		{
			object obj = e.Current;
			yield return obj;
		}
		yield break;
	}

	private IEnumerator LoadImageFromFacebookUrlCr(FacebookClient facebookClient, string url, Action<object> callback)
	{
		IEnumerator e = facebookClient.DoImageRequest(url, delegate(object error, Texture2D tex)
		{
			if (tex != null)
			{
				this.SetTexture(tex);
				this.portrait.Color = Color.white;
			}
			else
			{
				string arg = "Failed to load facebook portrait.";
				if (error != null)
				{
					arg = arg + ". Error: " + error;
				}
			}
			callback(error);
		});
		while (e.MoveNext())
		{
			object obj = e.Current;
			yield return obj;
		}
		yield break;
	}

	public UITextureQuad portrait;

	public StepRotator progress;

	private string lastLoadFacebookId;

	private Fiber fiber;

	public Material material;

	private Texture2D defaultTexture;

	private bool materialDuplicateCreated;
}
