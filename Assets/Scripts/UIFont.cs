using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public abstract class UIFont : ScriptableObject
{
	public abstract Material Material { get; }

	protected abstract bool GetGlyphInfo(char ch, out UIFont.GlyphInfo info, float fontSize, bool calcOnly);

	protected abstract float GetAscentFromFontSize(float fontSize);

	protected abstract float GetLineHeightFromFontSize(float fontSize);

	public abstract bool IsValid { get; }

	public bool GetMaterials(int index, out Material m)
	{
		if (index != 0)
		{
			if (index == 1)
			{
				if (this.embeddedSprites.Count > 0)
				{
					m = UIProjectSettings.Get().defaultAtlas.material;
					return true;
				}
			}
			m = null;
			return false;
		}
		m = this.Material;
		return true;
	}

	public virtual Vector2 Print(StringBuilder text, float wantedSize, UIFont.PrintOptions options, MeshData meshData)
	{
		if (!this.IsValid)
		{
			return Vector2.zero;
		}
		if (text.Length == 0)
		{
			return Vector2.zero;
		}
		UIFont.PrintState printState = UIFont.printStateStatic;
		printState.options = options;
		printState.size = wantedSize;
		printState.ascent = this.GetAscentFromFontSize(printState.size);
		printState.lineHeight = printState.size;
		printState.meshData = meshData;
		printState.effect.amount = options.typeWriterProgress;
		printState.effect.lookAhead = options.typeWriterLookahead;
		if (printState.effect.amount < 1f)
		{
			printState.effect.amount = Mathf.Lerp((float)(-(float)options.typeWriterLookahead) / (float)text.Length, 1f, printState.effect.amount);
			printState.effect.stopRenderingAtIndex = (float)text.Length * printState.effect.amount - 1f;
		}
		else
		{
			printState.effect.stopRenderingAtIndex = (float)text.Length;
			printState.effect.lookAhead = 0;
		}
		printState.Reset();
		while (printState.index < text.Length)
		{
			char c = text[printState.index];
			if (c == '\n')
			{
				this.LineComplete(printState, true);
			}
			else
			{
				this.PrintGlyph(c, printState);
			}
			printState.index++;
		}
		this.LineComplete(printState, false);
		this.AlignEverythingVertically(printState, printState.PrintedHeight, printState.options.vAlignment);
		return new Vector2(printState.PrintedWidth, printState.PrintedHeight);
	}

	public void FitText(string text, UIFont.PrintOptions options, float maxFontSize, bool wordwrap, out float finalFontSize, out StringBuilder finalText)
	{
		float num = 0f;
		finalText = null;
		maxFontSize = ((maxFontSize <= 0f) ? 1000f : maxFontSize);
		finalFontSize = maxFontSize;
		finalFontSize = Mathf.Min(finalFontSize, options.rect.height);
		float num2 = 0f;
		Vector2 one = Vector2.one;
		if (options.useEmbeddedSprites)
		{
			this.ConvertSpriteEncodingToUnicode(ref text);
		}
		if (wordwrap)
		{
			for (int i = 0; i < 10; i++)
			{
				finalText = this.WordWrapText(text, options.rect.width, finalFontSize);
				Vector2 vector = this.Print(finalText, finalFontSize, options, null);
				one.x = vector.x / options.rect.width;
				one.y = vector.y / options.rect.height;
				float num3;
				if (one.x <= 1f && one.y <= 1f)
				{
					num = Mathf.Max(finalFontSize, num);
					if ((one.x >= 0.95f && one.y >= 0.95f) || num >= maxFontSize)
					{
						break;
					}
					num3 = finalFontSize + Mathf.Abs(finalFontSize - num2) * 0.5f;
				}
				else
				{
					num3 = finalFontSize - Mathf.Abs(finalFontSize - num2) * 0.5f;
				}
				num2 = finalFontSize;
				finalFontSize = num3;
			}
			if (num > 0f)
			{
				finalFontSize = num;
				finalText = this.WordWrapText(text, options.rect.width, finalFontSize);
				Vector2 vector = this.Print(finalText, finalFontSize, options, null);
				one.x = vector.x / options.rect.width;
				one.y = vector.y / options.rect.height;
			}
		}
		else
		{
			UIFont.stringBuilder.Length = 0;
			UIFont.stringBuilder.Append(text);
			finalText = UIFont.stringBuilder;
			Vector2 vector = this.Print(finalText, finalFontSize, options, null);
			one.x = vector.x / options.rect.width;
			one.y = vector.y / options.rect.height;
		}
		float num4 = Mathf.Max(one.x, one.y);
		num4 = Mathf.Max(num4, 1f);
		finalFontSize /= num4;
	}

	public void MarkAsDirty()
	{
		UILabel[] array = UnityEngine.Object.FindObjectsOfType<UILabel>();
		int i = 0;
		int num = array.Length;
		while (i < num)
		{
			UILabel uilabel = array[i];
			if (uilabel.enabled && uilabel.gameObject.activeSelf && uilabel.font == this)
			{
				UIFont font = uilabel.font;
				uilabel.font = null;
				uilabel.font = font;
			}
			i++;
		}
	}

	private void LineComplete(UIFont.PrintState state, bool moveCursor = true)
	{
		this.AlignCurrentLineHorizontally(state, state.printedLastX, state.options.alignment);
		if (moveCursor)
		{
			state.cursorY -= state.lineHeight * (1f + this.lineSpacingModifier);
			state.cursorX = 0f;
			state.printedLastX = 0f;
			if (!state.CalcOnly)
			{
				state.vertIndexForCurrentLine = state.meshData.verts.Count;
			}
		}
	}

	protected bool TryGetGlyphInfo(char ch, out UIFont.GlyphInfo info, float fontSize, bool calcOnly)
	{
		if (ch < '' || ch >= '')
		{
			return this.GetGlyphInfo(ch, out info, fontSize, calcOnly);
		}
		int num = (int)(ch - '');
		if (num < this.embeddedSprites.Count)
		{
			return this.embeddedSprites[num].GetEmbeddedSpriteInfo(out info, fontSize, this);
		}
		return this.GetGlyphInfo('?', out info, fontSize, calcOnly);
	}

	private void PrintGlyph(char ch, UIFont.PrintState state)
	{
		UIFont.GlyphInfo glyphInfo;
		bool flag = this.TryGetGlyphInfo(ch, out glyphInfo, state.size, state.CalcOnly);
		if (flag)
		{
			Vector2 vector = new Vector2(state.cursorX, state.cursorY - state.ascent);
			UIFont.quad[3].x = glyphInfo.max.x + vector.x;
			UIFont.quad[3].y = glyphInfo.min.y + vector.y;
			if (glyphInfo.width > 0f)
			{
				state.printedLastX = UIFont.quad[3].x;
				state.printedTotalX = Mathf.Max(state.printedTotalX, UIFont.quad[3].x);
				state.printedTotalY = UIFont.quad[3].y;
			}
			state.cursorX += glyphInfo.advance;
			if (!state.CalcOnly)
			{
				UIFont.quad[0].x = glyphInfo.min.x + vector.x;
				UIFont.quad[0].y = glyphInfo.min.y + vector.y;
				UIFont.quad[1].x = glyphInfo.min.x + vector.x;
				UIFont.quad[1].y = glyphInfo.max.y + vector.y;
				UIFont.quad[2].x = glyphInfo.max.x + vector.x;
				UIFont.quad[2].y = glyphInfo.max.y + vector.y;
				float num = Mathf.Clamp01(1f - ((float)state.index - state.effect.stopRenderingAtIndex) / (float)(1 + state.effect.lookAhead));
				if (num > 0f)
				{
					MeshData meshData = state.meshData;
					meshData.uvs.Add(glyphInfo.uvBottomLeft);
					meshData.uvs.Add(glyphInfo.uvTopLeft);
					meshData.uvs.Add(glyphInfo.uvTopRight);
					meshData.uvs.Add(glyphInfo.uvBottomRight);
					if (glyphInfo.ignoreColor)
					{
						for (int i = 0; i < 4; i++)
						{
							UIFont.colors[i] = Color.white;
						}
					}
					else
					{
						UIFont.colors[0] = state.options.colorBottom;
						UIFont.colors[1] = state.options.colorTop;
						UIFont.colors[2] = state.options.colorTop;
						UIFont.colors[3] = state.options.colorBottom;
					}
					this.TypeWriterEffect(UIFont.quad, UIFont.colors, num);
					for (int j = 0; j < 4; j++)
					{
						meshData.verts.Add(UIFont.quad[j]);
						meshData.colors.Add(UIFont.colors[j]);
					}
					meshData.AddTrianglesForQuad(glyphInfo.subMesh);
					meshData.numSubmeshes = Mathf.Max(meshData.numSubmeshes, glyphInfo.subMesh + 1);
				}
			}
		}
	}

	public StringBuilder WordWrapText(string text, float areaWidth, float textSize)
	{
		UIFont.result.Length = 0;
		UIFont.word.Length = 0;
		int i = 0;
		float num = 0f;
		float num2 = 0f;
		while (i < text.Length)
		{
			char c = text[i];
			if (c == '\n')
			{
				num = 0f;
				for (int j = 0; j < UIFont.word.Length; j++)
				{
					UIFont.result.Append(UIFont.word[j]);
				}
				UIFont.word.Length = 0;
				num2 = 0f;
				UIFont.result.Append(c);
			}
			else
			{
				if (c != '\r')
				{
					UIFont.word.Append(c);
				}
				if (c == ' ' || c == '\r')
				{
					for (int k = 0; k < UIFont.word.Length; k++)
					{
						UIFont.result.Append(UIFont.word[k]);
					}
					UIFont.word.Remove(0, UIFont.word.Length);
					num2 = 0f;
				}
				if (c == '\r')
				{
					i++;
					continue;
				}
				UIFont.GlyphInfo glyphInfo;
				this.TryGetGlyphInfo(c, out glyphInfo, textSize, true);
				if (num + glyphInfo.max.x > areaWidth)
				{
					UIFont.result.Append('\n');
					num = num2;
				}
				num += glyphInfo.advance;
				num2 += glyphInfo.advance;
			}
			i++;
		}
		for (int l = 0; l < UIFont.word.Length; l++)
		{
			UIFont.result.Append(UIFont.word[l]);
		}
		return UIFont.result;
	}

	private void TypeWriterEffect(Vector3[] quad, Color[] colors, float visibilityFactor)
	{
		float num = (quad[0].x - quad[2].x) * 0.5f * (1f - visibilityFactor);
		float num2 = (quad[0].y - quad[1].y) * 0.5f * (1f - visibilityFactor);
		for (int i = 0; i < 4; i++)
		{
			int num3 = i;
			colors[num3].a = colors[num3].a * visibilityFactor;
		}
		quad[0] += new Vector3(num, num2, 0f);
		quad[1] += new Vector3(num, -num2, 0f);
		quad[2] += new Vector3(-num, -num2, 0f);
		quad[3] += new Vector3(-num, num2, 0f);
	}

	private void ConvertSpriteEncodingToUnicode(ref string text)
	{
		if (this.embeddedSpritesSymbolStringCache == null || this.embeddedSpritesSymbolStringCache.Length != this.embeddedSprites.Count)
		{
			this.embeddedSpritesSymbolStringCache = new string[this.embeddedSprites.Count];
			char c = '';
			for (int i = 0; i < this.embeddedSprites.Count; i++)
			{
				this.embeddedSpritesSymbolStringCache[i] = c.ToString();
				c += '\u0001';
			}
		}
		for (int j = 0; j < this.embeddedSprites.Count; j++)
		{
			UIFont.EmbeddedSprite embeddedSprite = this.embeddedSprites[j];
			if (!string.IsNullOrEmpty(embeddedSprite.encoding))
			{
				if (text.IndexOf(embeddedSprite.encoding) != -1)
				{
					text = text.Replace(embeddedSprite.encoding, this.embeddedSpritesSymbolStringCache[j]);
				}
			}
		}
	}

	private void AlignEverythingVertically(UIFont.PrintState state, float height, VAlignment valign)
	{
		if (valign == VAlignment.Top)
		{
			return;
		}
		if (state.CalcOnly)
		{
			return;
		}
		Vector3 vector = new Vector3(0f, state.options.rect.height - height, 0f);
		if (valign == VAlignment.Center)
		{
			vector *= 0.5f;
		}
		List<Vector3> verts = state.meshData.verts;
		for (int i = 0; i < verts.Count; i++)
		{
			List<Vector3> list;
			int index;
			(list = verts)[index = i] = list[index] - vector;
		}
	}

	private void AlignCurrentLineHorizontally(UIFont.PrintState state, float lineWidth, Alignment alignment)
	{
		if (alignment == Alignment.Left)
		{
			return;
		}
		if (state.CalcOnly)
		{
			return;
		}
		Vector3 vector = new Vector3(state.options.rect.width - lineWidth, 0f, 0f);
		if (alignment == Alignment.Center)
		{
			vector *= 0.5f;
		}
		List<Vector3> verts = state.meshData.verts;
		for (int i = state.vertIndexForCurrentLine; i < verts.Count; i++)
		{
			List<Vector3> list;
			int index;
			(list = verts)[index = i] = list[index] + vector;
		}
	}

	private const char PRIVATE_UNICODE_START = '';

	private const char PRIVATE_UNICODE_END = '';

	public const int MAX_FONT_SIZE = 1000;

	[SerializeField]
	protected List<UIFont.EmbeddedSprite> embeddedSprites;

	[SerializeField]
	private float lineSpacingModifier;

	private static readonly Vector3[] quad = new Vector3[4];

	private static readonly Color[] colors = new Color[4];

	private static readonly StringBuilder stringBuilder = new StringBuilder(1024);

	private static readonly StringBuilder result = new StringBuilder(1024);

	private static readonly StringBuilder word = new StringBuilder(1024);

	private string[] embeddedSpritesSymbolStringCache;

	private static UIFont.PrintState printStateStatic = new UIFont.PrintState();

	public class PrintOptions
	{
		public Rect rect;

		public Color colorTop = Color.white;

		public Color colorBottom = Color.white;

		public FontStyle style;

		public Alignment alignment;

		public VAlignment vAlignment;

		public int typeWriterLookahead = 1;

		public float typeWriterProgress = 1f;

		public bool useEmbeddedSprites;
	}

	[Serializable]
	protected class EmbeddedSprite
	{
		public bool GetEmbeddedSpriteInfo(out UIFont.GlyphInfo g, float fontSize, UIFont font)
		{
			UIAtlas defaultAtlas = UIProjectSettings.Get().defaultAtlas;
			UIAtlas.Sprite sprite = defaultAtlas.GetSprite(this.spriteName);
			Rect rect = default(Rect);
			float num = 1f;
			if (sprite != null)
			{
				num = sprite.outer.width / sprite.outer.height;
				rect = sprite.outer;
			}
			g = default(UIFont.GlyphInfo);
			fontSize *= 1f + this.scaleModifier;
			float ascentFromFontSize = font.GetAscentFromFontSize(fontSize);
			float lineHeightFromFontSize = font.GetLineHeightFromFontSize(fontSize);
			g.height = ascentFromFontSize - (fontSize - lineHeightFromFontSize);
			g.width = g.height * num;
			g.advance = g.width;
			g.min = new Vector2(0f, fontSize - lineHeightFromFontSize);
			g.max = new Vector2(g.width, ascentFromFontSize);
			Rect r = UIMath.ConvertToTexCoords(rect, defaultAtlas.texture.width, defaultAtlas.texture.height);
			g.uvTopLeft = r.TopLeft();
			g.uvTopRight = r.TopRight();
			g.uvBottomLeft = r.BottomLeft();
			g.uvBottomRight = r.BottomRight();
			g.subMesh = 1;
			g.ignoreColor = true;
			return true;
		}

		[UISpriteName]
		public string spriteName;

		public string encoding;

		public float scaleModifier;
	}

	public struct GlyphInfo
	{
		public float advance;

		public float bearing;

		public float width;

		public float height;

		public Vector2 min;

		public Vector2 max;

		public Vector2 uvTopLeft;

		public Vector2 uvTopRight;

		public Vector2 uvBottomLeft;

		public Vector2 uvBottomRight;

		public int subMesh;

		public bool ignoreColor;
	}

	protected class PrintState
	{
		public bool CalcOnly
		{
			get
			{
				return this.meshData == null;
			}
		}

		public float PrintedHeight
		{
			get
			{
				return -(this.cursorY - this.lineHeight);
			}
		}

		public float PrintedWidth
		{
			get
			{
				return this.printedTotalX;
			}
		}

		public void Reset()
		{
			this.index = 0;
			this.cursorX = 0f;
			this.cursorY = 0f;
			this.printedLastX = 0f;
			this.printedTotalX = 0f;
			this.printedTotalY = 0f;
			this.vertIndexForCurrentLine = 0;
		}

		public UIFont.PrintOptions options;

		public float size;

		public float ascent;

		public float lineHeight;

		public float cursorX;

		public float cursorY;

		public float printedLastX;

		public float printedTotalX;

		public float printedTotalY;

		public int index;

		public int vertIndexForCurrentLine;

		public MeshData meshData;

		public UIFont.PrintState.TypeWriterEffect effect;

		public struct TypeWriterEffect
		{
			public int lookAhead;

			public float amount;

			public float stopRenderingAtIndex;
		}
	}
}
