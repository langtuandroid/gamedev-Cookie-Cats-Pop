using System;
using UnityEngine;

public class PieceSpriteVisuals : PieceVisuals
{
	public override void VisualsChanged()
	{
		this.SetColor((!base.Highlighted) ? Color.gray : Color.white);
	}

	private void SetColor(Color color)
	{
		this.SetSpriteColorKeepAlpha(this.sprite, color);
		for (int i = 0; i < this.additionalSprites.Length; i++)
		{
			this.SetSpriteColorKeepAlpha(this.additionalSprites[i], color);
		}
	}

	private void SetSpriteColorKeepAlpha(UISprite sprite, Color color)
	{
		float alpha = sprite.Alpha;
		sprite.Color = color;
		sprite.Alpha = alpha;
	}

	public UISprite sprite;

	public UISprite[] additionalSprites;
}
