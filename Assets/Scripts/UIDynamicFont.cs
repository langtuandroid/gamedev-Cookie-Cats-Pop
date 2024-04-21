using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class UIDynamicFont : UIFont
{
	public override Material Material
	{
		get
		{
			if (!this.IsValid)
			{
				return null;
			}
			if (this.customMaterial == null)
			{
				this.customMaterial = new Material(Shader.Find("Unlit/Font"));
			}
			this.customMaterial.mainTexture = this.unityFont.material.mainTexture;
			return this.customMaterial;
		}
	}

	public override bool IsValid
	{
		get
		{
			return this.unityFont != null;
		}
	}

	protected override float GetLineHeightFromFontSize(float fontSize)
	{
		return (!this.IsValid) ? 0f : ((float)this.unityFont.lineHeight * (fontSize / (float)this.unityFont.fontSize));
	}

	protected override float GetAscentFromFontSize(float fontSize)
	{
		return (!this.IsValid) ? 0f : (fontSize * ((float)this.unityFont.ascent / (float)this.unityFont.lineHeight - this.baselineAdjust));
	}

	public override Vector2 Print(StringBuilder text, float wantedSize, UIFont.PrintOptions options, MeshData meshData)
	{
		string characters;
		if (this.requiresEmojiSafeEncoding)
		{
			characters = text.ToString();
		}
		else
		{
			StringModifier.CopyFromStringBuilder(ref UIDynamicFont.stringBuilderConverterString, text);
			characters = UIDynamicFont.stringBuilderConverterString;
		}
		if (!this.IsValid)
		{
			return Vector2.zero;
		}
		if (meshData == null)
		{
			this.unityFont.RequestCharactersInTexture(characters, 20);
		}
		else
		{
			this.unityFont.RequestCharactersInTexture(characters, this.RequestSize(wantedSize));
		}
		return base.Print(text, wantedSize, options, meshData);
	}

	private int RequestSize(float wantedSize)
	{
		int result = (this.renderSizes.Count <= 0) ? 20 : this.renderSizes[this.renderSizes.Count - 1];
		for (int i = 0; i < this.renderSizes.Count; i++)
		{
			int num = this.renderSizes[i];
			if ((float)num >= wantedSize)
			{
				result = num;
				break;
			}
		}
		return result;
	}

	protected override bool GetGlyphInfo(char ch, out UIFont.GlyphInfo info, float fontSize, bool calcOnly)
	{
		info = default(UIFont.GlyphInfo);
		if (!this.IsValid && !Application.isPlaying)
		{
			return false;
		}
		CharacterInfo characterInfo2;
		bool characterInfo;
		if (calcOnly)
		{
			characterInfo = this.unityFont.GetCharacterInfo(ch, out characterInfo2, 20);
		}
		else
		{
			characterInfo = this.unityFont.GetCharacterInfo(ch, out characterInfo2, this.RequestSize(fontSize));
		}
		float num = fontSize / (float)characterInfo2.size * (1f + this.sizeModifier);
		info.advance = (float)characterInfo2.advance * num;
		info.height = (float)characterInfo2.glyphHeight * num;
		info.width = (float)characterInfo2.glyphWidth * num;
		info.bearing = (float)characterInfo2.bearing * num;
		info.max.x = (float)characterInfo2.maxX * num;
		info.max.y = (float)characterInfo2.maxY * num;
		info.min.x = (float)characterInfo2.minX * num;
		info.min.y = (float)characterInfo2.minY * num;
		info.uvTopLeft = characterInfo2.uvTopLeft;
		info.uvTopRight = characterInfo2.uvTopRight;
		info.uvBottomLeft = characterInfo2.uvBottomLeft;
		info.uvBottomRight = characterInfo2.uvBottomRight;
		return characterInfo;
	}

	private const int measureSize = 20;

	[SerializeField]
	private Font unityFont;

	[SerializeField]
	private float baselineAdjust;

	[SerializeField]
	private float sizeModifier;

	[SerializeField]
	private List<int> renderSizes = new List<int>();

	[SerializeField]
	private bool requiresEmojiSafeEncoding;

	private static string stringBuilderConverterString = new string(' ', 2048);

	private Material customMaterial;
}
