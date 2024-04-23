using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class UITextureQuad : UIWidget
{
	public UITextureQuad.TileMode Tiling
	{
		get
		{
			return this.tiling;
		}
		set
		{
			this.tiling = value;
			this.changeFlags = UIChangeFlags.All;
		}
	}

	public bool KeepAspect
	{
		get
		{
			return this.mKeepAspect;
		}
		set
		{
			if (this.mKeepAspect != value)
			{
				this.mKeepAspect = value;
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

	public Vector2 repeatSize
	{
		get
		{
			return this.mRepeatSize;
		}
		set
		{
			this.mRepeatSize = value;
			this.changeFlags |= UIChangeFlags.All;
		}
	}

	public Vector2 repeatOffset
	{
		get
		{
			return this.mRepeatOffset;
		}
		set
		{
			this.mRepeatOffset = value;
			this.changeFlags |= UIChangeFlags.All;
		}
	}

	protected override void Initialize()
	{
		if (base.renderer.sharedMaterial == null)
		{
			this.customMaterial = new Material(Shader.Find("Unlit/Transparent Colored"));
			this.customMaterial.mainTexture = Texture2D.whiteTexture;
			base.renderer.sharedMaterial = this.customMaterial;
		}
		base.Initialize();
	}

	public override bool RendererInspectorsHidden
	{
		get
		{
			return false;
		}
	}

	protected override void OnFill(MeshData meshData)
	{
		if (this.tiling == UITextureQuad.TileMode.Tile)
		{
			this.OnFillTiled(meshData.verts, meshData.uvs, meshData.colors);
		}
		else
		{
			this.OnFillNormal(meshData.verts, meshData.uvs, meshData.colors);
		}
	}

	public override void CorrectAspect(AspectCorrection correction)
	{
		Texture mainTexture = base.GetComponent<Renderer>().sharedMaterial.mainTexture;
		if (mainTexture != null)
		{
			Vector2 v = new Vector2((float)mainTexture.width, (float)mainTexture.height);
			base.Size = UIUtility.CorrectSizeToAspect(base.Size, v.Aspect(), correction);
		}
	}

	protected override void OnDestroy()
	{
		this.DestroyCustomMaterial(this.customMaterial);
		base.OnDestroy();
	}

	private void DestroyCustomMaterial(Material material)
	{
		if (material != null)
		{
			if (Application.isPlaying)
			{
				UnityEngine.Object.Destroy(material);
			}
			else
			{
				UnityEngine.Object.DestroyImmediate(material);
			}
		}
	}

	protected override Material GetMaterial()
	{
		return base.renderer.sharedMaterial;
	}

	public Material Material
	{
		get
		{
			return base.renderer.sharedMaterial;
		}
		set
		{
			base.renderer.sharedMaterial = value;
		}
	}

	public void SetTexture(Texture2D tex)
	{
		this.DestroyCustomMaterial(this.customMaterial);
		Material sharedMaterial = base.GetComponent<Renderer>().sharedMaterial;
		if (sharedMaterial != null)
		{
			this.customMaterial = new Material(sharedMaterial);
		}
		else
		{
			this.customMaterial = new Material(Shader.Find("Unlit/Transparent Colored"));
		}
		this.customMaterial.mainTexture = tex;
		this.Material = this.customMaterial;
		if (this.KeepAspect)
		{
			this.MarkAsChanged();
		}
	}

	public void OnFillNormal(List<Vector3> verts, List<Vector2> uvs, List<Color> cols)
	{
		Vector2 vector = base.Size;
		Vector2 zero = Vector2.zero;
		if (this.mKeepAspect)
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

	public void OnFillTiled(List<Vector3> verts, List<Vector2> uvs, List<Color> cols)
	{
		verts.Add(new Vector3(base.Size.x, 0f, 0f));
		verts.Add(new Vector3(base.Size.x, -base.Size.y, 0f));
		verts.Add(new Vector3(0f, -base.Size.y, 0f));
		verts.Add(new Vector3(0f, 0f, 0f));
		float x = base.Size.x / this.repeatSize.x;
		float y = base.Size.y / this.repeatSize.y;
		uvs.Add(new Vector2(x, y) + this.repeatOffset);
		uvs.Add(new Vector2(x, 0f) + this.repeatOffset);
		uvs.Add(Vector2.zero + this.repeatOffset);
		uvs.Add(new Vector2(0f, y) + this.repeatOffset);
		cols.Add(base.Color);
		cols.Add(base.Color);
		cols.Add(base.Color);
		cols.Add(base.Color);
	}

	[SerializeField]
	[HideInInspector]
	private UITextureQuad.TileMode tiling;

	[HideInInspector]
	[SerializeField]
	private Vector2 mRepeatSize = Vector2.one;

	[HideInInspector]
	[SerializeField]
	private Vector2 mRepeatOffset;

	[HideInInspector]
	[SerializeField]
	private bool mKeepAspect;

	[HideInInspector]
	[SerializeField]
	private bool mMirrorX;

	[HideInInspector]
	[SerializeField]
	private bool mMirrorY;

	private Material customMaterial;

	public enum TileMode
	{
		None,
		Tile
	}
}
