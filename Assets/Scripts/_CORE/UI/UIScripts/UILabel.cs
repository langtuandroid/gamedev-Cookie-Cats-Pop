using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[ExecuteInEditMode]
public class UILabel : UIWidget
{
	protected override void Initialize()
	{
		if (Application.isPlaying && !this.dontLocalize)
		{
			this.mText = UIViewManager.Localize(this.mText);
		}
		this.ApplyStyle();
	}

	protected override void OnDestroy()
	{
		UIWidget.RemoveFromMarkAsChangedInLateUpdate(this);
		Font.textureRebuilt -= this.HandleDynamicTextureRebuilt;
		base.OnDestroy();
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		if (this.mTypingLookahead >= 0 && this.mTypingOnStart)
		{
			this.typingProgress = 0f;
		}
		Font.textureRebuilt += this.HandleDynamicTextureRebuilt;
		this.MarkAsChanged();
		base.UpdateGeometry();
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		Font.textureRebuilt -= this.HandleDynamicTextureRebuilt;
	}

	private void Update()
	{
		if (this.typingProgress < 1f)
		{
			float num = 30f;
			this.typingProgress += num / (float)this.mText.Length * Time.deltaTime;
		}
	}

	private void HandleDynamicTextureRebuilt(Font f)
	{
		UIWidget.MarkAsChangedInLateUpdate(this);
	}

	public UIFont font
	{
		get
		{
			return this.mFont;
		}
		set
		{
			if (this.mFont != value)
			{
				this.mFont = value;
				this.MarkAsChanged();
			}
		}
	}

	private void ApplyStyle()
	{
		if (this.mFont != null && this.mFontStyle != null)
		{
			this.effectStyle = (UILabel.Effect)this.mFontStyle.effect;
			this.applyEffectsToEmbeddedSprites = this.mFontStyle.applyEffectsToEmbeddedSprites;
			this.effectSize = this.mFontStyle.shadowSize;
			this.effectColor = this.mFontStyle.shadowColor;
			this.strokeColor = this.mFontStyle.strokeColor;
			this.strokeSize = this.mFontStyle.strokeSize;
			this.italic = this.mFontStyle.italic;
			this.useGradient = this.mFontStyle.gradient;
			base.Color = this.mFontStyle.startColor;
			if (this.useGradient)
			{
				this.gradientColorEnd = this.mFontStyle.endColor;
			}
			UIFontStyle.Sizing sizing = this.mFontStyle.sizing;
			if (sizing != UIFontStyle.Sizing.WorldSpace)
			{
				if (sizing == UIFontStyle.Sizing.LocalSpace)
				{
					if (this.mFontStyle.fontSize > 0f)
					{
						base.transform.localScale = Vector3.one * this.mFontStyle.fontSize;
					}
				}
			}
			else if (this.mFontStyle.fontSize > 0f)
			{
				if (base.transform.parent != null)
				{
					Vector3 a = new Vector3(1f / base.transform.parent.lossyScale.x, 1f / base.transform.parent.lossyScale.y, 1f);
					base.transform.localScale = a * this.mFontStyle.fontSize;
				}
				else
				{
					base.transform.localScale = Vector3.one * this.mFontStyle.fontSize;
				}
			}
		}
	}

	public UIFontStyle fontStyle
	{
		get
		{
			return this.mFontStyle;
		}
		set
		{
			if (this.mFontStyle != value)
			{
				this.mFontStyle = value;
				this.ApplyStyle();
				this.MarkAsChanged();
			}
		}
	}

	public float typingProgress
	{
		get
		{
			return this.mTypingFillAmount;
		}
		set
		{
			this.mTypingFillAmount = value;
			this.MarkAsChanged();
		}
	}

	public int typingLookahead
	{
		get
		{
			return this.mTypingLookahead;
		}
		set
		{
			this.mTypingLookahead = Mathf.Max(value, -1);
			this.MarkAsChanged();
		}
	}

	public bool typingOnStart
	{
		get
		{
			return this.mTypingOnStart;
		}
		set
		{
			this.mTypingOnStart = value;
			this.MarkAsChanged();
		}
	}

	public float textHeight
	{
		get
		{
			return this.mTextHeight;
		}
		set
		{
			this.mTextHeight = value;
			this.MarkAsChanged();
		}
	}

	public string text
	{
		get
		{
			return this.mText;
		}
		set
		{
			if (value != null && this.mText != value)
			{
				this.mText = value;
				this.MarkAsChanged();
			}
		}
	}

	public bool fitText
	{
		get
		{
			return this.mFitText;
		}
		set
		{
			this.mFitText = value;
			this.MarkAsChanged();
		}
	}

	public override void MarkAsChanged()
	{
		base.MarkAsChanged();
		if (this.mAutoSize && this.mFont != null)
		{
			float wantedSize = (this.mTextHeight <= 0f) ? 1000f : this.mTextHeight;
			UIFont.PrintOptions printOptions = this.MakeOptions();
			printOptions.rect = new Rect(0f, 0f, 999999f, 99999f);
			printOptions.alignment = base.Pivot.GetAlignment();
			printOptions.vAlignment = base.Pivot.GetVAlignment();
			UILabel.stringBuilder.Length = 0;
			UILabel.stringBuilder.Append(this.mText);
			Vector2 size = this.mFont.Print(UILabel.stringBuilder, wantedSize, printOptions, null);
			if (this.maxSize.x != 0f)
			{
				size.x = Mathf.Clamp(size.x, 0f, this.maxSize.x);
			}
			if (this.maxSize.y != 0f)
			{
				size.y = Mathf.Clamp(size.y, 0f, this.maxSize.y);
			}
			if (base.Element != null)
			{
				base.Element.Size = size;
			}
		}
	}

	public BetterList<char> dynamicText
	{
		get
		{
			return this.mDynamicText;
		}
	}

	public bool autoSize
	{
		get
		{
			return this.mAutoSize;
		}
		set
		{
			this.mAutoSize = value;
			this.MarkAsChanged();
		}
	}

	public Vector2 maxSize
	{
		get
		{
			return this.mMaxSize;
		}
		set
		{
			this.mMaxSize = value;
			this.MarkAsChanged();
		}
	}

	public bool italic
	{
		get
		{
			return this.mItalic;
		}
		set
		{
			if (value != this.mItalic)
			{
				this.mItalic = value;
				this.MarkAsChanged();
			}
		}
	}

	public bool useGradient
	{
		get
		{
			return this.mGradient;
		}
		set
		{
			if (value != this.mGradient)
			{
				this.mGradient = value;
				this.MarkAsChanged();
			}
		}
	}

	public bool supportEncoding
	{
		get
		{
			return this.mEncoding;
		}
		set
		{
			if (this.mEncoding != value)
			{
				this.mEncoding = value;
				this.MarkAsChanged();
			}
		}
	}

	public bool dontLocalize
	{
		get
		{
			return this.mDontLocalize;
		}
		set
		{
			if (this.mDontLocalize != value)
			{
				this.mDontLocalize = value;
				this.MarkAsChanged();
			}
		}
	}

	public float lineWidth
	{
		get
		{
			return base.Size.x;
		}
	}

	public bool multiLine
	{
		get
		{
			return this.mMultiline;
		}
		set
		{
			if (this.mMultiline != value)
			{
				this.mMultiline = value;
				this.MarkAsChanged();
			}
		}
	}

	public UILabel.Effect effectStyle
	{
		get
		{
			return this.mEffectStyle;
		}
		set
		{
			if (this.mEffectStyle != value)
			{
				this.mEffectStyle = value;
				this.MarkAsChanged();
			}
		}
	}

	public bool applyEffectsToEmbeddedSprites
	{
		get
		{
			return this.mApplyEffectsToEmbeddedSprites;
		}
		set
		{
			if (this.mApplyEffectsToEmbeddedSprites != value)
			{
				this.mApplyEffectsToEmbeddedSprites = value;
				this.MarkAsChanged();
			}
		}
	}

	public Color effectColor
	{
		get
		{
			return this.mEffectColor;
		}
		set
		{
			if (this.mEffectColor != value)
			{
				this.mEffectColor = value;
				if (this.mEffectStyle != UILabel.Effect.None)
				{
					this.MarkAsChanged();
				}
			}
		}
	}

	public Color strokeColor
	{
		get
		{
			return this.mStrokeColor;
		}
		set
		{
			if (this.mStrokeColor != value)
			{
				this.mStrokeColor = value;
				if (this.mEffectStyle != UILabel.Effect.None)
				{
					this.MarkAsChanged();
				}
			}
		}
	}

	public Color gradientColorEnd
	{
		get
		{
			return this.mGradientColorEnd;
		}
		set
		{
			this.mGradientColorEnd = value;
			this.MarkAsChanged();
		}
	}

	protected override Material GetMaterial()
	{
		return (!(this.mFont != null)) ? null : this.mFont.Material;
	}

	protected override bool GetMaterials(int index, out Material m)
	{
		if (this.mFont != null)
		{
			return this.mFont.GetMaterials(index, out m);
		}
		m = null;
		return false;
	}

	private void ApplyShadow(MeshData meshData, ref int startIndex, int count, float x, float y, Color c)
	{
		for (int i = 0; i < count; i++)
		{
			int index = startIndex + i;
			Vector3 value = meshData.verts[index];
			value.x += x;
			value.y += y;
			meshData.verts[index] = value;
			Color color = meshData.colors[index];
			Color value2 = c;
			value2.a *= color.a * value2.a;
			meshData.colors[index] = value2;
		}
		startIndex += count;
	}

	private Vector2 MeasurePrint(string text, float wantedSize)
	{
		UILabel.stringBuilder.Length = 0;
		UILabel.stringBuilder.Append(text);
		return this.mFont.Print(UILabel.stringBuilder, wantedSize, this.MakeOptions(), null);
	}

	private UIFont.PrintOptions MakeOptions()
	{
		UIFont.PrintOptions printOptions = UILabel.staticPrintOptions;
		if (this.useGradient)
		{
			printOptions.colorBottom = this.gradientColorEnd;
			printOptions.colorBottom.a = this.gradientColorEnd.a * base.Color.a;
		}
		else
		{
			printOptions.colorBottom = base.Color;
		}
		printOptions.colorTop = base.Color;
		printOptions.rect = this.GetArea();
		printOptions.alignment = base.Pivot.GetAlignment();
		printOptions.vAlignment = base.Pivot.GetVAlignment();
		printOptions.typeWriterLookahead = this.mTypingLookahead;
		printOptions.typeWriterProgress = this.mTypingFillAmount;
		printOptions.useEmbeddedSprites = this.mEncoding;
		printOptions.style = FontStyle.Normal;
		return printOptions;
	}

	private Rect GetArea()
	{
		UIElement element = this.GetElement();
		return (!(element != null)) ? new Rect(0f, 0f, 1000f, 1000f) : new Rect(0f, 0f, element.Size.x, element.Size.y);
	}

	protected override void OnFill(MeshData meshData)
	{
		if (this.mFont == null || !this.mFont.IsValid)
		{
			return;
		}
		if (!this.fitText && this.multiLine)
		{
			this.ResizeLabelToFitTextWithFixedFontSize(meshData);
			return;
		}
		this.FitTextInBounds(meshData);
	}

	private void FitTextInBounds(MeshData meshData)
	{
		UIFont.PrintOptions options = this.MakeOptions();
		float num;
		StringBuilder text;
		this.mFont.FitText(this.text, options, this.mTextHeight, this.multiLine, out num, out text);
		this.mFont.Print(text, num, options, meshData);
		this.ApplyEffects(meshData, num / 100f);
	}

	private void ResizeLabelToFitTextWithFixedFontSize(MeshData meshData)
	{
		float textHeight = this.textHeight;
		float x = base.Size.x;
		UIFont.PrintOptions options = this.MakeOptions();
		UILabel.stringBuilder.Length = 0;
		UILabel.stringBuilder.Append(this.text);
		this.mFont.Print(UILabel.stringBuilder, textHeight, options, null);
		StringBuilder text = this.mFont.WordWrapText(this.text, x, textHeight);
		Vector2 vector = this.mFont.Print(text, textHeight, options, meshData);
		this.ApplyEffects(meshData, textHeight / 100f);
		Vector2 b = new Vector2(base.Size.x, base.Size.y);
		base.Size = new Vector2(base.Size.x, vector.y);
		Vector3 a = base.Size - b;
		base.transform.localPosition -= a / 2f;
	}

	protected void ApplyEffects(MeshData meshData, float pixel)
	{
		if (this.effectStyle != UILabel.Effect.None)
		{
			Vector3 localScale = base.transform.localScale;
			if (localScale.x == 0f || localScale.y == 0f)
			{
				return;
			}
			UILabel.orgIndexCounts.Clear();
			for (int i = 0; i < meshData.numSubmeshes; i++)
			{
				UILabel.orgIndexCounts.Add(meshData.GetIndices(i).Count);
			}
			int count = meshData.verts.Count;
			bool flag = this.effectStyle == UILabel.Effect.Shadow || this.effectStyle == UILabel.Effect.ShadowAndOutline;
			bool flag2 = this.effectStyle == UILabel.Effect.Outline || this.effectStyle == UILabel.Effect.ShadowAndOutline;
			int num = ((!flag) ? 0 : 1) + ((!flag2) ? 0 : 8);
			for (int j = 0; j < num; j++)
			{
				int count2 = meshData.verts.Count;
				for (int k = 0; k < count; k++)
				{
					meshData.verts.Add(meshData.verts[k]);
					meshData.colors.Add(meshData.colors[k]);
					meshData.uvs.Add(meshData.uvs[k]);
				}
				for (int l = 0; l < (this.applyEffectsToEmbeddedSprites ? meshData.numSubmeshes : 1); l++)
				{
					List<int> indices = meshData.GetIndices(l);
					for (int m = 0; m < UILabel.orgIndexCounts[l]; m++)
					{
						indices.Add(indices[m] + count2);
					}
				}
			}
			if (!this.applyEffectsToEmbeddedSprites)
			{
				for (int n = 1; n < meshData.numSubmeshes; n++)
				{
					List<int> indices2 = meshData.GetIndices(n);
					for (int num2 = 0; num2 < indices2.Count; num2++)
					{
						List<int> list;
						int index;
						(list = indices2)[index = num2] = list[index] + count * num;
					}
				}
			}
			int num3 = 0;
			if (flag)
			{
				Vector2 vector = this.effectSize * pixel;
				this.ApplyShadow(meshData, ref num3, count, vector.x, -vector.y, this.mEffectColor);
			}
			if (flag2)
			{
				Vector2 vector2 = this.strokeSize * pixel;
				this.ApplyShadow(meshData, ref num3, count, vector2.x, -vector2.y, this.mStrokeColor);
				this.ApplyShadow(meshData, ref num3, count, -vector2.x, -vector2.y, this.mStrokeColor);
				this.ApplyShadow(meshData, ref num3, count, vector2.x, vector2.y, this.mStrokeColor);
				this.ApplyShadow(meshData, ref num3, count, -vector2.x, vector2.y, this.mStrokeColor);
				this.ApplyShadow(meshData, ref num3, count, -vector2.x, 0f, this.mStrokeColor);
				this.ApplyShadow(meshData, ref num3, count, vector2.x, 0f, this.mStrokeColor);
				this.ApplyShadow(meshData, ref num3, count, 0f, -vector2.y, this.mStrokeColor);
				this.ApplyShadow(meshData, ref num3, count, 0f, vector2.y, this.mStrokeColor);
			}
		}
	}

	public override bool UpdateGeometryColorsOnly(Mesh m, List<Color> buffer)
	{
		bool flag = this.effectStyle == UILabel.Effect.Shadow || this.effectStyle == UILabel.Effect.ShadowAndOutline;
		bool flag2 = this.effectStyle == UILabel.Effect.Outline || this.effectStyle == UILabel.Effect.ShadowAndOutline;
		int vertexCount = m.vertexCount;
		int num = vertexCount / (1 + ((!flag) ? 0 : 1) + ((!flag2) ? 0 : 8));
		if (flag)
		{
			Color item = this.mEffectColor;
			item.a *= base.Color.a;
			for (int i = 0; i < num; i++)
			{
				buffer.Add(item);
			}
		}
		if (flag2)
		{
			Color item2 = this.mStrokeColor;
			item2.a *= base.Color.a;
			int num2 = num * 8;
			for (int j = 0; j < num2; j++)
			{
				buffer.Add(item2);
			}
		}
		if (!this.useGradient)
		{
			for (int k = 0; k < num; k++)
			{
				buffer.Add(base.Color);
			}
		}
		else
		{
			Color gradientColorEnd = this.gradientColorEnd;
			gradientColorEnd.a *= base.Color.a;
			for (int l = 0; l < num; l += 4)
			{
				buffer.Add(gradientColorEnd);
				buffer.Add(base.Color);
				buffer.Add(base.Color);
				buffer.Add(gradientColorEnd);
			}
		}
		m.SetColors(buffer);
		return true;
	}

	[HideInInspector]
	[SerializeField]
	private UIFont mFont;

	[HideInInspector]
	[SerializeField]
	private UIFontStyle mFontStyle;

	[HideInInspector]
	[SerializeField]
	private string mText = string.Empty;

	[HideInInspector]
	[SerializeField]
	private float mTextHeight;

	[HideInInspector]
	[SerializeField]
	private bool mEncoding = true;

	[HideInInspector]
	[SerializeField]
	private bool mMultiline;

	[HideInInspector]
	[SerializeField]
	private UILabel.Effect mEffectStyle;

	[HideInInspector]
	[SerializeField]
	private bool mApplyEffectsToEmbeddedSprites = true;

	[HideInInspector]
	[SerializeField]
	private Color mEffectColor = Color.black;

	[HideInInspector]
	[SerializeField]
	private Color mStrokeColor = Color.black;

	[HideInInspector]
	[SerializeField]
	private bool mItalic;

	[HideInInspector]
	[SerializeField]
	private bool mGradient;

	[HideInInspector]
	[SerializeField]
	private bool mAutoSize;

	[HideInInspector]
	[SerializeField]
	private Vector2 mMaxSize;

	[HideInInspector]
	[SerializeField]
	private Color mGradientColorEnd = Color.gray;

	[HideInInspector]
	[SerializeField]
	private bool mDontLocalize;

	[HideInInspector]
	[SerializeField]
	private float mTypingFillAmount = 1f;

	[HideInInspector]
	[SerializeField]
	private int mTypingLookahead = -1;

	[HideInInspector]
	[SerializeField]
	private bool mTypingOnStart;

	[HideInInspector]
	[SerializeField]
	private bool mFitText = true;

	public Vector2 effectSize = new Vector2(1f, 1f);

	public Vector2 strokeSize = new Vector2(1f, 1f);

	private BetterList<char> mDynamicText = new BetterList<char>();

	private static readonly StringBuilder stringBuilder = new StringBuilder(1024);

	private static readonly UIFont.PrintOptions staticPrintOptions = new UIFont.PrintOptions();

	private static readonly List<int> orgIndexCounts = new List<int>(1);

	public enum Effect
	{
		None,
		Shadow,
		Outline,
		ShadowAndOutline
	}
}
