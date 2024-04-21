using System;
using UnityEngine;

[ExecuteInEditMode]
public class UIBitmapFont : UIFont
{
	public float italicFactor
	{
		get
		{
			return (!(this.mReplacement != null)) ? this.mItalicFactor : this.mReplacement.italicFactor;
		}
		set
		{
			if (this.mReplacement == null)
			{
				this.mItalicFactor = value;
			}
		}
	}

	public float defaultSize
	{
		get
		{
			return this.mDefaultSize;
		}
		set
		{
			this.mDefaultSize = value;
		}
	}

	public BMFont bmFont
	{
		get
		{
			return (!(this.mReplacement != null)) ? this.mFont : this.mReplacement.bmFont;
		}
	}

	public int texWidth
	{
		get
		{
			return (!(this.mReplacement != null)) ? ((this.mFont == null) ? 1 : this.mFont.texWidth) : this.mReplacement.texWidth;
		}
	}

	public int texHeight
	{
		get
		{
			return (!(this.mReplacement != null)) ? ((this.mFont == null) ? 1 : this.mFont.texHeight) : this.mReplacement.texHeight;
		}
	}

	public bool upperCaseOnly
	{
		get
		{
			return (!(this.mReplacement != null)) ? this.mUppercaseOnly : this.mReplacement.upperCaseOnly;
		}
		set
		{
			if (this.mReplacement == null)
			{
				this.mUppercaseOnly = value;
			}
		}
	}

	public UIAtlas atlas
	{
		get
		{
			return (!(this.mReplacement != null)) ? this.mAtlas : this.mReplacement.atlas;
		}
		set
		{
			if (this.mReplacement != null)
			{
				this.mReplacement.atlas = value;
			}
			else if (this.mAtlas != value)
			{
				if (value == null)
				{
					if (this.mAtlas != null)
					{
						this.mMat = this.mAtlas.spriteMaterial;
					}
					if (this.sprite != null)
					{
						this.mUVRect = this.uvRect;
					}
				}
				this.mAtlas = value;
				base.MarkAsDirty();
			}
		}
	}

	public float baselineAdjust
	{
		get
		{
			return (!(this.mReplacement != null)) ? this.mBaselineAdjust : this.mReplacement.baselineAdjust;
		}
		set
		{
			if (this.mReplacement != null)
			{
				this.mReplacement.baselineAdjust = value;
			}
			else
			{
				this.mBaselineAdjust = value;
				base.MarkAsDirty();
			}
		}
	}

	public override Material Material
	{
		get
		{
			if (this.mReplacement != null)
			{
				return this.mReplacement.Material;
			}
			return (!(this.mAtlas != null)) ? this.mMat : this.mAtlas.spriteMaterial;
		}
	}

	public void SetMaterial(Material value)
	{
		if (this.mReplacement != null)
		{
			this.mReplacement.SetMaterial(value);
		}
		else if (this.mAtlas == null && this.mMat != value)
		{
			this.mMat = value;
			base.MarkAsDirty();
		}
	}

	public Texture2D texture
	{
		get
		{
			if (this.mReplacement != null)
			{
				return this.mReplacement.texture;
			}
			Material material = this.Material;
			return (!(material != null)) ? null : (material.mainTexture as Texture2D);
		}
	}

	public Rect uvRect
	{
		get
		{
			if (this.mReplacement != null)
			{
				return this.mReplacement.uvRect;
			}
			if (this.mAtlas != null && this.mSprite == null && this.sprite != null)
			{
				Texture texture = this.mAtlas.texture;
				if (texture != null)
				{
					this.mUVRect = this.mSprite.outer;
					if (this.mAtlas.coordinates == UIAtlas.Coordinates.Pixels)
					{
						this.mUVRect = UIMath.ConvertToTexCoords(this.mUVRect, texture.width, texture.height);
					}
					if (this.mSprite.hasPadding)
					{
						Rect rect = this.mUVRect;
						this.mUVRect.xMin = rect.xMin - this.mSprite.paddingLeft * rect.width;
						this.mUVRect.yMin = rect.yMin - this.mSprite.paddingBottom * rect.height;
						this.mUVRect.xMax = rect.xMax + this.mSprite.paddingRight * rect.width;
						this.mUVRect.yMax = rect.yMax + this.mSprite.paddingTop * rect.height;
					}
					if (this.mSprite.hasPadding)
					{
						this.Trim();
					}
				}
			}
			return this.mUVRect;
		}
		set
		{
			if (this.mReplacement != null)
			{
				this.mReplacement.uvRect = value;
			}
			else if (this.sprite == null && this.mUVRect != value)
			{
				this.mUVRect = value;
				base.MarkAsDirty();
			}
		}
	}

	public string spriteName
	{
		get
		{
			return (!(this.mReplacement != null)) ? this.mFont.spriteName : this.mReplacement.spriteName;
		}
		set
		{
			if (this.mReplacement != null)
			{
				this.mReplacement.spriteName = value;
			}
			else if (this.mFont.spriteName != value)
			{
				this.mFont.spriteName = value;
				base.MarkAsDirty();
			}
		}
	}

	public int horizontalSpacing
	{
		get
		{
			return (!(this.mReplacement != null)) ? this.mSpacingX : this.mReplacement.horizontalSpacing;
		}
		set
		{
			if (this.mReplacement != null)
			{
				this.mReplacement.horizontalSpacing = value;
			}
			else if (this.mSpacingX != value)
			{
				this.mSpacingX = value;
				base.MarkAsDirty();
			}
		}
	}

	public int verticalSpacing
	{
		get
		{
			return (!(this.mReplacement != null)) ? this.mSpacingY : this.mReplacement.verticalSpacing;
		}
		set
		{
			if (this.mReplacement != null)
			{
				this.mReplacement.verticalSpacing = value;
			}
			else if (this.mSpacingY != value)
			{
				this.mSpacingY = value;
				base.MarkAsDirty();
			}
		}
	}

	public int horizontalStretching
	{
		get
		{
			return (!(this.mReplacement != null)) ? this.mStretchingX : this.mReplacement.horizontalStretching;
		}
		set
		{
			if (this.mReplacement != null)
			{
				this.mReplacement.horizontalStretching = value;
			}
			else if (this.mStretchingX != value)
			{
				this.mStretchingX = value;
				base.MarkAsDirty();
			}
		}
	}

	public int verticalStretching
	{
		get
		{
			return (!(this.mReplacement != null)) ? this.mStretchingY : this.mReplacement.verticalStretching;
		}
		set
		{
			if (this.mReplacement != null)
			{
				this.mReplacement.verticalStretching = value;
			}
			else if (this.mStretchingY != value)
			{
				this.mStretchingY = value;
				base.MarkAsDirty();
			}
		}
	}

	public float size
	{
		get
		{
			return (!(this.mReplacement != null)) ? ((this.defaultSize <= 0f) ? ((float)this.mFont.charSize) : this.defaultSize) : this.mReplacement.size;
		}
	}

	public float nativeSize
	{
		get
		{
			return (!(this.mReplacement != null)) ? ((float)this.mFont.charSize) : this.mReplacement.size;
		}
	}

	public UIAtlas.Sprite sprite
	{
		get
		{
			if (this.mReplacement != null)
			{
				return this.mReplacement.sprite;
			}
			if (!this.mSpriteSet)
			{
				this.mSprite = null;
			}
			if (this.mSprite == null && this.mAtlas != null && !string.IsNullOrEmpty(this.mFont.spriteName))
			{
				this.mSprite = this.mAtlas.GetSprite(this.mFont.spriteName);
				if (this.mSprite == null)
				{
					this.mSprite = this.mAtlas.GetSprite(base.name);
				}
				this.mSpriteSet = true;
				if (this.mSprite == null)
				{
					this.mFont.spriteName = null;
				}
			}
			return this.mSprite;
		}
	}

	public UIBitmapFont replacement
	{
		get
		{
			return this.mReplacement;
		}
		set
		{
			UIBitmapFont uibitmapFont = value;
			if (uibitmapFont == this)
			{
				uibitmapFont = null;
			}
			if (this.mReplacement != uibitmapFont)
			{
				if (uibitmapFont != null && uibitmapFont.replacement == this)
				{
					uibitmapFont.replacement = null;
				}
				if (this.mReplacement != null)
				{
					base.MarkAsDirty();
				}
				this.mReplacement = uibitmapFont;
				base.MarkAsDirty();
			}
		}
	}

	private void Trim()
	{
		Texture texture = this.mAtlas.texture;
		if (texture != null && this.mSprite != null)
		{
			Rect rect = UIMath.ConvertToPixels(this.mUVRect, this.texture.width, this.texture.height, true);
			Rect rect2 = (this.mAtlas.coordinates != UIAtlas.Coordinates.TexCoords) ? this.mSprite.outer : UIMath.ConvertToPixels(this.mSprite.outer, texture.width, texture.height, true);
			int xMin = Mathf.RoundToInt(rect2.xMin - rect.xMin);
			int yMin = Mathf.RoundToInt(rect2.yMin - rect.yMin);
			int xMax = Mathf.RoundToInt(rect2.xMax - rect.xMin);
			int yMax = Mathf.RoundToInt(rect2.yMax - rect.yMin);
			this.mFont.Trim(xMin, yMin, xMax, yMax);
		}
	}

	protected override float GetAscentFromFontSize(float fontSize)
	{
		return (float)this.bmFont.baseOffset * fontSize / (float)this.bmFont.charSize;
	}

	protected override float GetLineHeightFromFontSize(float fontSize)
	{
		return fontSize;
	}

	public override bool IsValid
	{
		get
		{
			return this.bmFont != null && this.Material != null;
		}
	}

	protected override bool GetGlyphInfo(char ch, out UIFont.GlyphInfo info, float fontSize, bool calcOnly)
	{
		BMGlyph glyph = this.mFont.GetGlyph((int)ch);
		info = default(UIFont.GlyphInfo);
		if (glyph == null)
		{
			return false;
		}
		float num = fontSize / (float)this.bmFont.charSize;
		info.advance = (float)glyph.advance * num;
		info.bearing = 0f;
		info.height = (float)glyph.height * num;
		info.width = (float)glyph.width * num;
		float num2 = this.uvRect.width / (float)this.mFont.texWidth;
		float num3 = this.mUVRect.height / (float)this.mFont.texHeight;
		Rect rect = default(Rect);
		rect.xMin = this.mUVRect.xMin + num2 * (float)glyph.x;
		rect.yMax = this.mUVRect.yMax - num3 * (float)glyph.y;
		rect.xMax = rect.xMin + num2 * (float)glyph.width;
		rect.yMin = rect.yMax - num3 * (float)glyph.height;
		info.uvBottomLeft = new Vector2(rect.xMin, rect.yMin);
		info.uvBottomRight = new Vector2(rect.xMax, rect.yMin);
		info.uvTopLeft = new Vector2(rect.xMin, rect.yMax);
		info.uvTopRight = new Vector2(rect.xMax, rect.yMax);
		return true;
	}

	[HideInInspector]
	[SerializeField]
	private Material mMat;

	[HideInInspector]
	[SerializeField]
	private Rect mUVRect = new Rect(0f, 0f, 1f, 1f);

	[HideInInspector]
	[SerializeField]
	private BMFont mFont = new BMFont();

	[HideInInspector]
	[SerializeField]
	private int mSpacingX;

	[HideInInspector]
	[SerializeField]
	private int mSpacingY;

	[HideInInspector]
	[SerializeField]
	private int mStretchingX;

	[HideInInspector]
	[SerializeField]
	private int mStretchingY;

	[HideInInspector]
	[SerializeField]
	private UIAtlas mAtlas;

	[HideInInspector]
	[SerializeField]
	private UIBitmapFont mReplacement;

	[HideInInspector]
	[SerializeField]
	private float mItalicFactor = 0.1f;

	[HideInInspector]
	[SerializeField]
	private bool mUppercaseOnly;

	[HideInInspector]
	[SerializeField]
	private float mDefaultSize;

	[HideInInspector]
	[SerializeField]
	private float mBaselineAdjust;

	private UIAtlas.Sprite mSprite;

	private bool mSpriteSet;

	[Flags]
	public enum PrintFlag
	{
		MultiLine = 0,
		Encoding = 1,
		Gradient = 2,
		Italic = 3
	}
}
