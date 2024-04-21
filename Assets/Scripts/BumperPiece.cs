using System;
using UnityEngine;

public class BumperPiece : CPPiece
{
	public override void SpawnedByBoard(Board board)
	{
		base.SpawnedByBoard(board);
		this.sprite.SpriteName = this.spriteNames[UnityEngine.Random.Range(0, this.spriteNames.Length)];
	}

	public override bool IsRotatable
	{
		get
		{
			return true;
		}
	}

	public override bool CanMoveBySpring
	{
		get
		{
			return false;
		}
	}

	public override void Hit(IHitResolver resolver)
	{
	}

	public UISprite sprite;

	public string[] spriteNames;
}
