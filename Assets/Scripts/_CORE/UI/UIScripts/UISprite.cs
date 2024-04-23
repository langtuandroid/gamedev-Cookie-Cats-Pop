using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class UISprite : UIWidget
{
	public Rect OuterUV
	{
		get
		{
			this.UpdateUVs(false);
			return this.mOuterUV;
		}
	}

	public float PixelSizeFactor
	{
		get
		{
			return this.mPixelSize;
		}
		set
		{
			this.mPixelSize = value;
			this.MarkAsChanged();
		}
	}

	public float CenterFillAmount
	{
		get
		{
			return this.mCenterFillAmount;
		}
		set
		{
			this.mCenterFillAmount = value;
			this.MarkAsChanged();
		}
	}

	public bool Mirrored
	{
		get
		{
			return this.mMirrored;
		}
		set
		{
			this.mMirrored = value;
			this.MarkAsChanged();
		}
	}

	public Rect InnerUV
	{
		get
		{
			this.UpdateUVs(false);
			return this.mInnerUV;
		}
	}

	public bool FillCenter
	{
		get
		{
			return this.mFillCenter;
		}
		set
		{
			if (this.mFillCenter != value)
			{
				this.mFillCenter = value;
				this.MarkAsChanged();
			}
		}
	}

	public bool FillBottom
	{
		get
		{
			return this.mFillBottom;
		}
		set
		{
			if (this.mFillBottom != value)
			{
				this.mFillBottom = value;
				this.MarkAsChanged();
			}
		}
	}

	public bool FillSides
	{
		get
		{
			return this.mFillSides;
		}
		set
		{
			if (this.mFillSides != value)
			{
				this.mFillSides = value;
				this.MarkAsChanged();
			}
		}
	}

	public bool FillTop
	{
		get
		{
			return this.mFillTop;
		}
		set
		{
			if (this.mFillTop != value)
			{
				this.mFillTop = value;
				this.MarkAsChanged();
			}
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

	protected static Material MissingSpriteMaterial
	{
		get
		{
			if (UISprite.staticMissingSpriteMaterial == null)
			{
				UISprite.staticMissingSpriteMaterial = new Material(Shader.Find("VertexLit"));
				UISprite.staticMissingSpriteMaterial.SetColor("_Color", Color.clear);
				UISprite.staticMissingSpriteMaterial.SetColor("_SpecColor", Color.clear);
				UISprite.staticMissingSpriteMaterial.SetColor("_Emission", Color.magenta);
			}
			return UISprite.staticMissingSpriteMaterial;
		}
	}

	public UIAtlas Atlas
	{
		get
		{
			return this.mAtlas;
		}
		set
		{
			if (this.mAtlas != value)
			{
				this.mAtlas = value;
				this.mSpriteSet = false;
				this.mSprite = null;
				if (string.IsNullOrEmpty(this.mSpriteName) && this.mAtlas != null && this.mAtlas.spriteList.Count > 0)
				{
					this.Sprite = this.mAtlas.spriteList[0];
					this.mSpriteName = this.mSprite.name;
				}
				base.MarkMaterialsDirty();
				if (!string.IsNullOrEmpty(this.mSpriteName))
				{
					string spriteName = this.mSpriteName;
					this.mSpriteName = string.Empty;
					this.SpriteName = spriteName;
					this.changeFlags = UIChangeFlags.All;
					this.UpdateUVs(true);
				}
			}
		}
	}

	public string SpriteName
	{
		get
		{
			return this.mSpriteName;
		}
		set
		{
			if (string.IsNullOrEmpty(value))
			{
				if (string.IsNullOrEmpty(this.mSpriteName))
				{
					return;
				}
				this.mSpriteName = string.Empty;
				this.mSprite = null;
				this.changeFlags = UIChangeFlags.All;
			}
			else if (this.mSpriteName != value)
			{
				this.mSpriteName = value;
				this.mSprite = ((!(this.mAtlas != null)) ? null : this.mAtlas.GetSprite(this.mSpriteName));
				this.changeFlags = UIChangeFlags.All;
				if (this.mSprite != null)
				{
					this.UpdateUVs(true);
				}
			}
		}
	}

	public UIAtlas.Sprite Sprite
	{
		get
		{
			if (!this.mSpriteSet)
			{
				this.mSprite = null;
			}
			if (this.mSprite == null && this.mAtlas != null && !string.IsNullOrEmpty(this.mSpriteName))
			{
				this.Sprite = this.mAtlas.GetSprite(this.mSpriteName);
			}
			return this.mSprite;
		}
		set
		{
			this.mSprite = value;
			this.mSpriteSet = true;
		}
	}

	public override Vector2 PivotOffset
	{
		get
		{
			Vector2 zero = Vector2.zero;
			if (this.Sprite != null)
			{
				UIPivot pivot = base.Pivot;
				if (pivot == UIPivot.Top || pivot == UIPivot.Center || pivot == UIPivot.Bottom)
				{
					zero.x = (-1f - this.mSprite.paddingRight + this.mSprite.paddingLeft) * 0.5f;
				}
				else if (pivot == UIPivot.TopRight || pivot == UIPivot.Right || pivot == UIPivot.BottomRight)
				{
					zero.x = -1f - this.mSprite.paddingRight;
				}
				else
				{
					zero.x = this.mSprite.paddingLeft;
				}
				if (pivot == UIPivot.Left || pivot == UIPivot.Center || pivot == UIPivot.Right)
				{
					zero.y = (1f + this.mSprite.paddingBottom - this.mSprite.paddingTop) * 0.5f;
				}
				else if (pivot == UIPivot.BottomLeft || pivot == UIPivot.Bottom || pivot == UIPivot.BottomRight)
				{
					zero.y = 1f + this.mSprite.paddingBottom;
				}
				else
				{
					zero.y = -this.mSprite.paddingTop;
				}
			}
			return zero;
		}
	}

	public int materialIndex
	{
		get
		{
			return this.mMaterialIndex;
		}
		set
		{
			if (this.mMaterialIndex != value)
			{
				this.mMaterialIndex = value;
				this.changeFlags = UIChangeFlags.All;
				base.MarkMaterialsDirty();
				this.UpdateUVs(true);
			}
		}
	}

	protected override Material GetMaterial()
	{
		Material atlasMaterial = this.GetAtlasMaterial();
		if (atlasMaterial != null)
		{
			this.UpdateUVs(true);
		}
		return atlasMaterial;
	}

	public virtual void UpdateUVs(bool force)
	{
		if (this.Sprite != null && (force || this.mInner != this.mSprite.inner || this.mOuter != this.mSprite.outer))
		{
			Material atlasMaterial = this.GetAtlasMaterial();
			if (atlasMaterial != null && atlasMaterial.mainTexture != null)
			{
				Texture mainTexture = atlasMaterial.mainTexture;
				this.mInner = this.mSprite.inner;
				this.mOuter = this.mSprite.outer;
				this.mInnerUV = this.mInner;
				this.mOuterUV = this.mOuter;
				if (this.Atlas.coordinates == UIAtlas.Coordinates.Pixels)
				{
					this.mOuterUV = UIMath.ConvertToTexCoords(this.mOuterUV, mainTexture.width, mainTexture.height);
					this.mInnerUV = UIMath.ConvertToTexCoords(this.mInnerUV, mainTexture.width, mainTexture.height);
				}
			}
		}
	}

	public override void CorrectAspect(AspectCorrection correction)
	{
		Texture mainTexture = base.MainTexture;
		if (this.Sprite == null || mainTexture == null)
		{
			return;
		}
		if (mainTexture != null)
		{
			Rect r = UIMath.ConvertToPixels(this.OuterUV, mainTexture.width, mainTexture.height, true);
			base.Size = UIUtility.CorrectSizeToAspect(base.Size, r.Size().Aspect(), correction);
		}
	}

	private Material GetAtlasMaterial()
	{
		if (this.mAtlas != null && this.mAtlas.Materials != null && this.mAtlas.Materials.Length > 0)
		{
			return this.mAtlas.Materials[this.mMaterialIndex];
		}
		return null;
	}

	protected override void Initialize()
	{
		if (this.mAtlas != null)
		{
			this.UpdateUVs(true);
		}
	}

	public override bool UpdateGeometryColorsOnly(Mesh m, List<Color> buffer)
	{
		buffer.Clear();
		for (int i = 0; i < m.vertexCount; i++)
		{
			buffer.Add(base.Color);
		}
		m.SetColors(buffer);
		return true;
	}

	protected override void OnFill(MeshData data)
	{
		if (this.mSprite == null)
		{
			return;
		}
		Vector2 b = base.Size;
		Vector2 vector = Vector2.zero;
		Vector2 vector2 = Vector2.one;
		if (base.MainTexture != null)
		{
			vector2 = this.mPixelSize * this.Atlas.pixelSize * this.mSprite.pixelSizeFactor * new Vector2((float)base.MainTexture.width, (float)base.MainTexture.height);
			if (this.mKeepAspect && this.OuterUV == this.InnerUV)
			{
				Rect r = UIMath.ConvertToPixels(this.OuterUV, base.MainTexture.width, base.MainTexture.height, true);
				float wantedAspect = r.Size().Aspect() * (float)((!this.mSprite.halfSpriteX) ? 1 : 2) * ((!this.mSprite.halfSpriteY) ? 1f : 0.5f);
				b = UIUtility.CorrectSizeToAspect(base.Size, wantedAspect, AspectCorrection.Fit);
				vector = (base.Size - b) * 0.5f;
			}
		}
		UISprite.SliceGenerator sliceGenerator = new UISprite.SliceGenerator(1, vector2[1], this.mSprite.halfSpriteY, b[1], this);
		UISprite.SliceGenerator sliceGenerator2 = new UISprite.SliceGenerator(0, vector2[0], this.mSprite.halfSpriteX, b[0], this);
		UISprite.Slice slice = new UISprite.Slice
		{
			tc = this.mOuterUV.yMin
		};
		UISprite.Slice slice2 = default(UISprite.Slice);
		for (int i = 0; i < 4; i++)
		{
			if (sliceGenerator.UpdateSlice(i, ref slice2))
			{
				UISprite.Slice slice3 = new UISprite.Slice
				{
					tc = this.mOuterUV.xMin
				};
				UISprite.Slice slice4 = default(UISprite.Slice);
				for (int j = 0; j < 4; j++)
				{
					if (sliceGenerator2.UpdateSlice(j, ref slice4))
					{
						if (!this.IsQuadHidden(slice4, slice2))
						{
							this.AddQuad(data, vector.x + slice3.pos, -vector.y + slice.pos - b.y, vector.x + slice4.pos, -vector.y + slice2.pos - b.y, slice3.tc, slice.tc, slice4.tc, slice2.tc);
						}
						slice3 = slice4;
					}
				}
				slice = slice2;
			}
		}
		data.AddTrisAutomatically();
	}

	private bool IsQuadHidden(UISprite.Slice x, UISprite.Slice y)
	{
		return (x.stage == UISprite.Stage.Center && y.stage == UISprite.Stage.Center && !this.mFillCenter) || (x.stage != UISprite.Stage.Center && !this.mFillSides && y.stage == UISprite.Stage.Center) || (y.stage == UISprite.Stage.First && !this.mFillBottom) || (y.stage == UISprite.Stage.Last && !this.mFillTop);
	}

	private void AddQuad(MeshData data, float left, float top, float right, float bottom, float uvLeft, float uvTop, float uvRight, float uvBottom)
	{
		if (this.mMirrored)
		{
			float num = left;
			left = base.Size.x - right;
			right = base.Size.x - num;
			num = uvLeft;
			uvLeft = uvRight;
			uvRight = num;
		}
		data.verts.Add(new Vector3(right, bottom, 0f));
		data.verts.Add(new Vector3(right, top, 0f));
		data.verts.Add(new Vector3(left, top, 0f));
		data.verts.Add(new Vector3(left, bottom, 0f));
		data.uvs.Add(new Vector2(uvRight, uvBottom));
		data.uvs.Add(new Vector2(uvRight, uvTop));
		data.uvs.Add(new Vector2(uvLeft, uvTop));
		data.uvs.Add(new Vector2(uvLeft, uvBottom));
		data.colors.Add(base.Color);
		data.colors.Add(base.Color);
		data.colors.Add(base.Color);
		data.colors.Add(base.Color);
	}

	//[HideInInspector]
	[SerializeField]
	private UIAtlas mAtlas;

	//[HideInInspector]
	[SerializeField]
	private string mSpriteName;

	//[HideInInspector]
	[SerializeField]
	private bool mFillCenter = true;

	//[HideInInspector]
	[SerializeField]
	private bool mFillBottom = true;

	//[HideInInspector]
	[SerializeField]
	private bool mFillSides = true;

	//[HideInInspector]
	[SerializeField]
	private bool mFillTop = true;

	//[HideInInspector]
	[SerializeField]
	private bool mMirrored;

	//[HideInInspector]
	[SerializeField]
	private bool mKeepAspect;

	//[HideInInspector]
	[SerializeField]
	private float mPixelSize = 1f;

	//[HideInInspector]
	[SerializeField]
	private float mCenterFillAmount = 1f;

	//[HideInInspector]
	[SerializeField]
	private int mMaterialIndex;

	protected UIAtlas.Sprite mSprite;

	protected Rect mOuter;

	protected Rect mOuterUV;

	protected Rect mInner;

	protected Rect mInnerUV;

	private bool mSpriteSet;

	private static Material staticMissingSpriteMaterial;

	private struct SliceGenerator
	{
		public SliceGenerator(int axis, float adjustedTextureSize, bool useHalf, float renderSize, UISprite spr)
		{
			this.axis = axis;
			this.renderSize = renderSize;
			this.useHalf = useHalf;
			this.spr = spr;
			this.stage = UISprite.Stage.First;
			this.firstSegmentSize = (spr.mInnerUV.min[axis] - spr.mOuterUV.min[axis]) * adjustedTextureSize;
			this.lastSegmentSize = (spr.mOuterUV.max[axis] - spr.mInnerUV.max[axis]) * adjustedTextureSize;
			float num = this.firstSegmentSize + ((!useHalf) ? this.lastSegmentSize : this.firstSegmentSize);
			this.sizeFactor = Mathf.Min(renderSize / num, 1f);
			this.slicedFirst = (spr.mOuterUV.min[axis] != spr.mInnerUV.min[axis]);
			this.slicedLast = (spr.mOuterUV.max[axis] != spr.mInnerUV.max[axis]);
		}

		public bool UpdateSlice(int segment, ref UISprite.Slice slice)
		{
			switch (segment)
			{
			case 0:
				if (this.slicedFirst)
				{
					slice.SetSlice(this.firstSegmentSize * this.sizeFactor, this.spr.mInnerUV.min[this.axis], this.stage);
					return true;
				}
				break;
			case 1:
				this.stage = UISprite.Stage.Center;
				if (this.useHalf)
				{
					float num = (!(this.spr.MainTexture != null)) ? 0f : (this.spr.MainTexture.texelSize[this.axis] * 0.5f);
					slice.SetSlice(this.renderSize * 0.5f, this.spr.mOuterUV.max[this.axis] - num, this.stage);
					return true;
				}
				break;
			case 2:
				if (this.useHalf && this.slicedFirst)
				{
					slice.SetSlice(this.renderSize - this.firstSegmentSize * this.sizeFactor, this.spr.mInnerUV.min[this.axis], this.stage);
					this.stage = UISprite.Stage.Last;
					return true;
				}
				if (!this.useHalf && this.slicedLast)
				{
					slice.SetSlice(this.renderSize - this.lastSegmentSize * this.sizeFactor, this.spr.mInnerUV.max[this.axis], this.stage);
					this.stage = UISprite.Stage.Last;
					return true;
				}
				break;
			case 3:
				slice.SetSlice(this.renderSize, (!this.useHalf) ? this.spr.mOuterUV.max[this.axis] : this.spr.mOuterUV.min[this.axis], this.stage);
				return true;
			}
			return false;
		}

		private readonly float firstSegmentSize;

		private readonly float lastSegmentSize;

		private readonly bool slicedFirst;

		private readonly bool slicedLast;

		private readonly float sizeFactor;

		private readonly int axis;

		private readonly bool useHalf;

		private readonly float renderSize;

		private readonly UISprite spr;

		private UISprite.Stage stage;
	}

	private struct Slice
	{
		public void SetSlice(float p, float t, UISprite.Stage s = UISprite.Stage.First)
		{
			this.pos = p;
			this.tc = t;
			this.stage = s;
		}

		public float pos;

		public float tc;

		public UISprite.Stage stage;
	}

	private enum Stage
	{
		First,
		Center,
		Last
	}
}
