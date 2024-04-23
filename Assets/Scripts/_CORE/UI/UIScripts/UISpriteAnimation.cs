using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(UISprite))]
public class UISpriteAnimation : MonoBehaviour
{
	public int framesPerSecond
	{
		get
		{
			return this.mFPS;
		}
		set
		{
			this.mFPS = value;
		}
	}

	public string namePrefix
	{
		get
		{
			return this.mPrefix;
		}
		set
		{
			if (this.mPrefix != value)
			{
				this.mPrefix = value;
				this.RebuildSpriteList();
			}
		}
	}

	private void Start()
	{
		this.RebuildSpriteList();
	}

	private void Update()
	{
		if (this.mSpriteNames.Count > 1 && Application.isPlaying)
		{
			this.mDelta += Time.deltaTime;
			float num = ((float)this.mFPS <= 0f) ? 0f : (1f / (float)this.mFPS);
			if (num < this.mDelta)
			{
				this.mDelta = ((num <= 0f) ? 0f : (this.mDelta - num));
				if (++this.mIndex >= this.mSpriteNames.Count)
				{
					this.mIndex = 0;
				}
				this.mSprite.SpriteName = this.mSpriteNames[this.mIndex];
			}
		}
	}

	private void RebuildSpriteList()
	{
		if (this.mSprite == null)
		{
			this.mSprite = base.GetComponent<UISprite>();
		}
		this.mSpriteNames.Clear();
		if (this.mSprite != null && this.mSprite.Atlas != null)
		{
			List<UIAtlas.Sprite> spriteList = this.mSprite.Atlas.spriteList;
			int i = 0;
			int count = spriteList.Count;
			while (i < count)
			{
				UIAtlas.Sprite sprite = spriteList[i];
				if (string.IsNullOrEmpty(this.mPrefix) || sprite.name.StartsWith(this.mPrefix))
				{
					this.mSpriteNames.Add(sprite.name);
				}
				i++;
			}
			this.mSpriteNames.Sort();
		}
	}

	[HideInInspector]
	[SerializeField]
	private int mFPS = 30;

	[HideInInspector]
	[SerializeField]
	private string mPrefix = string.Empty;

	private UISprite mSprite;

	private float mDelta;

	private int mIndex;

	private List<string> mSpriteNames = new List<string>();
}
