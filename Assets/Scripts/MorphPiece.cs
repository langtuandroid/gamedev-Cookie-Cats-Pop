using System;
using UnityEngine;

public class MorphPiece : CPPiece
{
	public override MatchFlag MatchFlag
	{
		get
		{
			return this.matchColor;
		}
	}

	private void Awake()
	{
		if (this.sprite == null)
		{
			this.sprite = base.GetComponent<UISprite>();
		}
		this.defaultMatchColor = this.matchColor;
	}

	public override void SpawnedByBoard(Board board)
	{
		if (this.defaultMatchColor != this.matchColor)
		{
			this.UpdateMatchFlag(this.defaultMatchColor);
		}
	}

	public void UpdateMatchFlag(MatchFlag newMatchFlag)
	{
		this.matchColor = newMatchFlag;
		this.sprite.SpriteName = "Morph" + this.MatchFlag;
	}

	[SerializeField]
	private MatchFlag matchColor;

	private UISprite sprite;

	private MatchFlag defaultMatchColor;
}
