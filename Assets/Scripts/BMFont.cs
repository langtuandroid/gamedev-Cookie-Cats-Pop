using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BMFont
{
	public bool isValid
	{
		get
		{
			return this.mSaved.Count > 0 || this.LegacyCheck();
		}
	}

	public int charSize
	{
		get
		{
			return this.mSize;
		}
		set
		{
			this.mSize = value;
		}
	}

	public int baseOffset
	{
		get
		{
			return this.mBase;
		}
		set
		{
			this.mBase = value;
		}
	}

	public int texWidth
	{
		get
		{
			return this.mWidth;
		}
		set
		{
			this.mWidth = value;
		}
	}

	public int texHeight
	{
		get
		{
			return this.mHeight;
		}
		set
		{
			this.mHeight = value;
		}
	}

	public int glyphCount
	{
		get
		{
			return (!this.isValid) ? 0 : this.mSaved.Count;
		}
	}

	public string spriteName
	{
		get
		{
			return this.mSpriteName;
		}
		set
		{
			this.mSpriteName = value;
		}
	}

	public bool LegacyCheck()
	{
		if (this.mGlyphs != null && this.mGlyphs.Length > 0)
		{
			int i = 0;
			int num = this.mGlyphs.Length;
			while (i < num)
			{
				BMGlyph bmglyph = this.mGlyphs[i];
				if (bmglyph != null)
				{
					bmglyph.index = i;
					this.mSaved.Add(bmglyph);
					this.mDict.Add(i, bmglyph);
				}
				i++;
			}
			this.mGlyphs = null;
			return true;
		}
		return false;
	}

	private int GetArraySize(int index)
	{
		if (index < 256)
		{
			return 256;
		}
		if (index < 65536)
		{
			return 65536;
		}
		if (index < 262144)
		{
			return 262144;
		}
		return 0;
	}

	public BMGlyph GetGlyph(int index, bool createIfMissing)
	{
		BMGlyph bmglyph = null;
		if (this.mDict.Count == 0)
		{
			if (this.mSaved.Count == 0)
			{
				this.LegacyCheck();
			}
			else
			{
				int i = 0;
				int count = this.mSaved.Count;
				while (i < count)
				{
					BMGlyph bmglyph2 = this.mSaved[i];
					this.mDict.Add(bmglyph2.index, bmglyph2);
					i++;
				}
			}
		}
		for (int j = 0; j < this.mSaved.Count; j++)
		{
			if (this.mSaved[j].index == index)
			{
				bmglyph = this.mSaved[j];
				break;
			}
		}
		if (bmglyph == null && createIfMissing)
		{
			bmglyph = new BMGlyph();
			bmglyph.index = index;
			this.mSaved.Add(bmglyph);
			this.mDict.Add(index, bmglyph);
		}
		return bmglyph;
	}

	public BMGlyph GetGlyph(int index)
	{
		return this.GetGlyph(index, false);
	}

	public void Clear()
	{
		this.mGlyphs = null;
		this.mDict.Clear();
		this.mSaved.Clear();
	}

	public void Trim(int xMin, int yMin, int xMax, int yMax)
	{
		if (this.isValid)
		{
			int i = 0;
			int count = this.mSaved.Count;
			while (i < count)
			{
				BMGlyph bmglyph = this.mSaved[i];
				if (bmglyph != null)
				{
					bmglyph.Trim(xMin, yMin, xMax, yMax);
				}
				i++;
			}
		}
	}

	[HideInInspector]
	[SerializeField]
	private BMGlyph[] mGlyphs;

	[HideInInspector]
	[SerializeField]
	private int mSize;

	[HideInInspector]
	[SerializeField]
	private int mBase;

	[HideInInspector]
	[SerializeField]
	private int mWidth;

	[HideInInspector]
	[SerializeField]
	private int mHeight;

	[HideInInspector]
	[SerializeField]
	private string mSpriteName;

	[HideInInspector]
	[SerializeField]
	private List<BMGlyph> mSaved = new List<BMGlyph>();

	private Dictionary<int, BMGlyph> mDict = new Dictionary<int, BMGlyph>();
}
