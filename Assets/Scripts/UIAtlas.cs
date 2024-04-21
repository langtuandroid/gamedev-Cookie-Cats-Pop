using System;
using System.Collections.Generic;
using UnityEngine;

public class UIAtlas : MonoBehaviour
{
	public Material material
	{
		get
		{
			return (this.materials == null || this.materials.Length <= 0) ? null : this.materials[0];
		}
		set
		{
			if (this.materials == null || this.materials.Length == 0)
			{
				this.materials = new Material[1];
			}
			this.materials[0] = value;
		}
	}

	public Material[] Materials
	{
		get
		{
			return this.materials;
		}
		set
		{
			this.materials = value;
		}
	}

	public Material spriteMaterial
	{
		get
		{
			return (!(this.mReplacement != null)) ? this.material : this.mReplacement.spriteMaterial;
		}
		set
		{
			if (this.mReplacement != null)
			{
				this.mReplacement.spriteMaterial = value;
			}
			else if (this.material == null)
			{
				this.mPMA = 0;
				this.material = value;
			}
			else
			{
				this.MarkAsDirty();
				this.mPMA = -1;
				this.material = value;
				this.MarkAsDirty();
			}
		}
	}

	public bool premultipliedAlpha
	{
		get
		{
			if (this.mReplacement != null)
			{
				return this.mReplacement.premultipliedAlpha;
			}
			if (this.mPMA == -1)
			{
				Material spriteMaterial = this.spriteMaterial;
				this.mPMA = ((!(spriteMaterial != null) || !(spriteMaterial.shader != null) || !spriteMaterial.shader.name.Contains("Premultiplied")) ? 0 : 1);
			}
			return this.mPMA == 1;
		}
	}

	public List<UIAtlas.Sprite> spriteList
	{
		get
		{
			return (!(this.mReplacement != null)) ? this.sprites : this.mReplacement.spriteList;
		}
		set
		{
			if (this.mReplacement != null)
			{
				this.mReplacement.spriteList = value;
			}
			else
			{
				this.sprites = value;
			}
		}
	}

	public List<string> pathList
	{
		get
		{
			return (!(this.mReplacement != null)) ? this.paths : this.mReplacement.pathList;
		}
		set
		{
			if (this.mReplacement != null)
			{
				this.mReplacement.paths = value;
			}
			else
			{
				this.paths = value;
			}
		}
	}

	public Texture texture
	{
		get
		{
			return (!(this.mReplacement != null)) ? ((!(this.material != null)) ? null : this.material.mainTexture) : this.mReplacement.texture;
		}
	}

	public UIAtlas.Coordinates coordinates
	{
		get
		{
			return (!(this.mReplacement != null)) ? this.mCoordinates : this.mReplacement.coordinates;
		}
		set
		{
			if (this.mReplacement != null)
			{
				this.mReplacement.coordinates = value;
			}
			else if (this.mCoordinates != value)
			{
				if (this.material == null || this.material.mainTexture == null)
				{
					return;
				}
				this.mCoordinates = value;
				Texture mainTexture = this.material.mainTexture;
				int i = 0;
				int count = this.sprites.Count;
				while (i < count)
				{
					UIAtlas.Sprite sprite = this.sprites[i];
					if (this.mCoordinates == UIAtlas.Coordinates.TexCoords)
					{
						sprite.outer = UIMath.ConvertToTexCoords(sprite.outer, mainTexture.width, mainTexture.height);
						sprite.inner = UIMath.ConvertToTexCoords(sprite.inner, mainTexture.width, mainTexture.height);
					}
					else
					{
						sprite.outer = UIMath.ConvertToPixels(sprite.outer, mainTexture.width, mainTexture.height, true);
						sprite.inner = UIMath.ConvertToPixels(sprite.inner, mainTexture.width, mainTexture.height, true);
					}
					i++;
				}
			}
		}
	}

	public float pixelSize
	{
		get
		{
			return (!(this.mReplacement != null)) ? this.mPixelSize : this.mReplacement.pixelSize;
		}
		set
		{
			if (this.mReplacement != null)
			{
				this.mReplacement.pixelSize = value;
			}
			else
			{
				float num = Mathf.Clamp(value, 0.25f, 4f);
				if (this.mPixelSize != num)
				{
					this.mPixelSize = num;
					this.MarkAsDirty();
				}
			}
		}
	}

	public int padding
	{
		get
		{
			return (!(this.mReplacement != null)) ? this.mPadding : this.mReplacement.padding;
		}
		set
		{
			if (this.mReplacement != null)
			{
				this.mReplacement.padding = value;
			}
			else
			{
				int num = Mathf.Clamp(value, 0, 8);
				if (this.mPadding != num)
				{
					this.mPadding = num;
					this.MarkAsDirty();
				}
			}
		}
	}

	public bool trimming
	{
		get
		{
			return (!(this.mReplacement != null)) ? this.mTrimming : this.mReplacement.trimming;
		}
		set
		{
			if (this.mReplacement != null)
			{
				this.mReplacement.trimming = value;
			}
			else if (this.mTrimming != value)
			{
				this.mTrimming = value;
				this.MarkAsDirty();
			}
		}
	}

	public bool useUnityPacker
	{
		get
		{
			return (!(this.mReplacement != null)) ? this.mUseUnityPacker : this.mReplacement.useUnityPacker;
		}
		set
		{
			if (this.mReplacement != null)
			{
				this.mReplacement.useUnityPacker = value;
			}
			else if (this.mUseUnityPacker != value)
			{
				this.mUseUnityPacker = value;
				this.MarkAsDirty();
			}
		}
	}

	public bool forceSquare
	{
		get
		{
			return (!(this.mReplacement != null)) ? this.mForceSquare : this.mReplacement.forceSquare;
		}
		set
		{
			if (this.mReplacement != null)
			{
				this.mReplacement.forceSquare = value;
			}
			else if (this.mForceSquare != value)
			{
				this.mForceSquare = value;
				this.MarkAsDirty();
			}
		}
	}

	public UIAtlas replacement
	{
		get
		{
			return this.mReplacement;
		}
		set
		{
			UIAtlas uiatlas = value;
			if (uiatlas == this)
			{
				uiatlas = null;
			}
			if (this.mReplacement != uiatlas)
			{
				if (uiatlas != null && uiatlas.replacement == this)
				{
					uiatlas.replacement = null;
				}
				if (this.mReplacement != null)
				{
					this.MarkAsDirty();
				}
				this.mReplacement = uiatlas;
				this.MarkAsDirty();
			}
		}
	}

	public UIAtlas.Sprite GetSprite(string name)
	{
		if (this.mReplacement != null)
		{
			return this.mReplacement.GetSprite(name);
		}
		if (!string.IsNullOrEmpty(name))
		{
			int i = 0;
			int count = this.sprites.Count;
			while (i < count)
			{
				UIAtlas.Sprite sprite = this.sprites[i];
				if (!string.IsNullOrEmpty(sprite.name) && name == sprite.name)
				{
					return sprite;
				}
				i++;
			}
		}
		return null;
	}

	private static int CompareString(string a, string b)
	{
		return a.CompareTo(b);
	}

	public BetterList<string> GetListOfSprites()
	{
		if (this.mReplacement != null)
		{
			return this.mReplacement.GetListOfSprites();
		}
		BetterList<string> betterList = new BetterList<string>();
		int i = 0;
		int count = this.sprites.Count;
		while (i < count)
		{
			UIAtlas.Sprite sprite = this.sprites[i];
			if (sprite != null && !string.IsNullOrEmpty(sprite.name))
			{
				betterList.Add(sprite.name);
			}
			i++;
		}
		return betterList;
	}

	public BetterList<string> GetListOfSprites(string match)
	{
		if (this.mReplacement != null)
		{
			return this.mReplacement.GetListOfSprites(match);
		}
		if (string.IsNullOrEmpty(match))
		{
			return this.GetListOfSprites();
		}
		BetterList<string> betterList = new BetterList<string>();
		int i = 0;
		int count = this.sprites.Count;
		while (i < count)
		{
			UIAtlas.Sprite sprite = this.sprites[i];
			if (sprite != null && !string.IsNullOrEmpty(sprite.name) && string.Equals(match, sprite.name, StringComparison.OrdinalIgnoreCase))
			{
				betterList.Add(sprite.name);
				return betterList;
			}
			i++;
		}
		string[] array = match.Split(new char[]
		{
			' '
		}, StringSplitOptions.RemoveEmptyEntries);
		for (int j = 0; j < array.Length; j++)
		{
			array[j] = array[j].ToLower();
		}
		int k = 0;
		int count2 = this.sprites.Count;
		while (k < count2)
		{
			UIAtlas.Sprite sprite2 = this.sprites[k];
			if (sprite2 != null && !string.IsNullOrEmpty(sprite2.name))
			{
				string text = sprite2.name.ToLower();
				int num = 0;
				for (int l = 0; l < array.Length; l++)
				{
					if (text.Contains(array[l]))
					{
						num++;
					}
				}
				if (num == array.Length)
				{
					betterList.Add(sprite2.name);
				}
			}
			k++;
		}
		return betterList;
	}

	private bool References(UIAtlas atlas)
	{
		return !(atlas == null) && (atlas == this || (this.mReplacement != null && this.mReplacement.References(atlas)));
	}

	public static bool CheckIfRelated(UIAtlas a, UIAtlas b)
	{
		return !(a == null) && !(b == null) && (a == b || a.References(b) || b.References(a));
	}

	public void MarkAsDirty()
	{
		if (this.mReplacement != null)
		{
			this.mReplacement.MarkAsDirty();
		}
		UISprite[] array = UnityEngine.Object.FindObjectsOfType<UISprite>();
		int i = 0;
		int num = array.Length;
		while (i < num)
		{
			UISprite uisprite = array[i];
			if (UIAtlas.CheckIfRelated(this, uisprite.Atlas))
			{
				UIAtlas atlas = uisprite.Atlas;
				uisprite.Atlas = null;
				uisprite.Atlas = atlas;
			}
			i++;
		}
		UIBitmapFont[] array2 = Resources.FindObjectsOfTypeAll(typeof(UIBitmapFont)) as UIBitmapFont[];
		int j = 0;
		int num2 = array2.Length;
		while (j < num2)
		{
			UIBitmapFont uibitmapFont = array2[j];
			if (UIAtlas.CheckIfRelated(this, uibitmapFont.atlas))
			{
				UIAtlas atlas2 = uibitmapFont.atlas;
				uibitmapFont.atlas = null;
				uibitmapFont.atlas = atlas2;
			}
			j++;
		}
		UILabel[] array3 = UnityEngine.Object.FindObjectsOfType<UILabel>();
		int k = 0;
		int num3 = array3.Length;
		while (k < num3)
		{
			UILabel uilabel = array3[k];
			if (uilabel.font is UIBitmapFont && UIAtlas.CheckIfRelated(this, ((UIBitmapFont)uilabel.font).atlas))
			{
				UIFont font = uilabel.font;
				uilabel.font = null;
				uilabel.font = font;
			}
			k++;
		}
	}

	//[HideInInspector]
	[SerializeField]
	private Material[] materials;

	//[HideInInspector]
	[SerializeField]
	private List<string> paths = new List<string>();

	//[HideInInspector]
	[SerializeField]
	private List<UIAtlas.Sprite> sprites = new List<UIAtlas.Sprite>();

	//[HideInInspector]
	[SerializeField]
	private UIAtlas.Coordinates mCoordinates = UIAtlas.Coordinates.TexCoords;

	//[HideInInspector]
	[SerializeField]
	private float mPixelSize = 1f;

	//[HideInInspector]
	[SerializeField]
	private UIAtlas mReplacement;

	//[HideInInspector]
	[SerializeField]
	private int mPadding;

	//[HideInInspector]
	[SerializeField]
	private bool mTrimming;

	//[HideInInspector]
	[SerializeField]
	private bool mUseUnityPacker;

	//[HideInInspector]
	[SerializeField]
	private bool mForceSquare;

	private int mPMA = -1;

	[Serializable]
	public class Sprite
	{
		public bool hasPadding
		{
			get
			{
				return this.paddingLeft != 0f || this.paddingRight != 0f || this.paddingTop != 0f || this.paddingBottom != 0f;
			}
		}

		public string name = "Unity Bug";

		public Rect outer = new Rect(0f, 0f, 1f, 1f);

		public Rect inner = new Rect(0f, 0f, 1f, 1f);

		public bool rotated;

		public float pixelSizeFactor = 1f;

		public bool halfSpriteX;

		public bool halfSpriteY;

		public Rect adjustments = new Rect(0f, 0f, 0f, 0f);

		public float paddingLeft;

		public float paddingRight;

		public float paddingTop;

		public float paddingBottom;
	}

	public enum Coordinates
	{
		Pixels,
		TexCoords
	}
}
