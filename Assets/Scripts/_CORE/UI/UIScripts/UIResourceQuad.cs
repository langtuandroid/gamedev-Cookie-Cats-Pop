using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class UIResourceQuad : UIWidget
{
	public bool KeepAspect
	{
		get
		{
			return this.keepAspect;
		}
		set
		{
			if (this.keepAspect != value)
			{
				this.keepAspect = value;
				this.MarkAsChanged();
			}
		}
	}

	public bool MirrorX
	{
		get
		{
			return this.mMirrorX;
		}
		set
		{
			this.mMirrorX = value;
			this.changeFlags |= UIChangeFlags.All;
		}
	}

	public bool MirrorY
	{
		get
		{
			return this.mMirrorY;
		}
		set
		{
			this.mMirrorY = value;
			this.changeFlags |= UIChangeFlags.All;
		}
	}

	public string TextureResource
	{
		get
		{
			return this.textureResourcePath;
		}
		set
		{
			if (this.textureResourcePath != value)
			{
				this.UnloadTexture();
				this.textureResourcePath = value;
				this.LoadTexture();
			}
		}
	}

	public override void CorrectAspect(AspectCorrection correction)
	{
		Texture mainTexture = base.MainTexture;
		if (mainTexture != null)
		{
			base.Size = UIUtility.CorrectSizeToAspect(base.Size, (float)mainTexture.width / (float)mainTexture.height, correction);
		}
	}

	protected override void OnDestroy()
	{
		this.UnloadTexture();
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
		base.OnDestroy();
	}

	protected override void Initialize()
	{
		this.LoadTexture();
	}

	public Shader Shader
	{
		get
		{
			return this.shader;
		}
		set
		{
			if (this.shader != value)
			{
				this.shader = value;
				this.PrepareMaterial();
			}
		}
	}

	private void PrepareMaterial()
	{
		if (this.shader == null)
		{
			this.shader = Shader.Find(UIProjectSettings.DEFAULT_SHADER);
		}
		if (this.material == null)
		{
			this.material = new Material(this.shader);
		}
		else
		{
			this.material.shader = this.shader;
		}
	}

	protected override Material GetMaterial()
	{
		return this.material;
	}

	public void LoadTexture()
	{
		if (this.textureResourcePath == null)
		{
			return;
		}
		this.PrepareMaterial();
		if (this.material == null)
		{
			return;
		}
		this.asset = (Texture2D)Resources.Load(this.textureResourcePath);
		this.material.mainTexture = this.asset;
		if (UIResourceQuad.refCounts.ContainsKey(this.textureResourcePath))
		{
			Dictionary<string, int> dictionary;
			string key;
			(dictionary = UIResourceQuad.refCounts)[key = this.textureResourcePath] = dictionary[key] + 1;
		}
		else
		{
			UIResourceQuad.refCounts[this.textureResourcePath] = 1;
		}
	}

	public void UnloadTexture()
	{
		if (this.asset == null)
		{
			return;
		}
		if (UIResourceQuad.refCounts.ContainsKey(this.textureResourcePath))
		{
			Dictionary<string, int> dictionary;
			string key;
			(dictionary = UIResourceQuad.refCounts)[key = this.textureResourcePath] = dictionary[key] - 1;
		}
		else
		{
			UIResourceQuad.refCounts[this.textureResourcePath] = 0;
		}
		if (UIResourceQuad.refCounts[this.textureResourcePath] == 0)
		{
			Resources.UnloadAsset(this.asset);
		}
		this.asset = null;
	}

	protected override void OnFill(MeshData meshData)
	{
		this.OnFillNormal(meshData.verts, meshData.uvs, meshData.colors);
	}

	public void OnFillNormal(List<Vector3> verts, List<Vector2> uvs, List<Color> cols)
	{
		Vector2 vector = base.Size;
		Vector2 zero = Vector2.zero;
		if (this.keepAspect)
		{
			Texture mainTexture = base.MainTexture;
			if (mainTexture != null)
			{
				Rect r = new Rect(0f, 0f, (float)mainTexture.width, (float)mainTexture.height);
				vector = UIUtility.CorrectSizeToAspect(base.Size, r.Size().Aspect(), AspectCorrection.Fit);
				zero.x = (base.Size.x - vector.x) * 0.5f;
				zero.y = (base.Size.y - vector.y) * 0.5f;
			}
		}
		verts.Add(new Vector3(vector.x + zero.x, -zero.y, 0f));
		verts.Add(new Vector3(vector.x + zero.x, -vector.y - zero.y, 0f));
		verts.Add(new Vector3(zero.x, -vector.y - zero.y, 0f));
		verts.Add(new Vector3(zero.x, -zero.y, 0f));
		if (this.MirrorX && this.MirrorY)
		{
			uvs.Add(Vector2.one);
			uvs.Add(new Vector2(0f, 1f));
			uvs.Add(Vector2.zero);
			uvs.Add(new Vector2(1f, 0f));
		}
		else if (this.MirrorX)
		{
			uvs.Add(new Vector2(0f, 1f));
			uvs.Add(Vector2.zero);
			uvs.Add(new Vector2(1f, 0f));
			uvs.Add(Vector2.one);
		}
		else if (this.mMirrorY)
		{
			uvs.Add(new Vector2(1f, 0f));
			uvs.Add(Vector2.one);
			uvs.Add(new Vector2(0f, 1f));
			uvs.Add(Vector2.zero);
		}
		else
		{
			uvs.Add(Vector2.one);
			uvs.Add(new Vector2(1f, 0f));
			uvs.Add(Vector2.zero);
			uvs.Add(new Vector2(0f, 1f));
		}
		cols.Add(base.Color);
		cols.Add(base.Color);
		cols.Add(base.Color);
		cols.Add(base.Color);
	}

	private static Dictionary<string, int> refCounts = new Dictionary<string, int>();

	private Material material;

	private Texture2D asset;

	[SerializeField]
	private string textureResourcePath;

	[SerializeField]
	private Shader shader;

	[HideInInspector]
	[SerializeField]
	private bool keepAspect;

	[HideInInspector]
	[SerializeField]
	private bool mMirrorX;

	[HideInInspector]
	[SerializeField]
	private bool mMirrorY;
}
