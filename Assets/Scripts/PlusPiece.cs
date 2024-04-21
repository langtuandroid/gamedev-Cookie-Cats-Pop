using System;
using System.Collections;
using UnityEngine;

public class PlusPiece : CPPiece
{
	public override MatchFlag MatchFlag
	{
		get
		{
			return this.matchColor;
		}
	}

	public override IEnumerator AnimatePop()
	{
		ZLayer org = this.ZSorter().layer;
		this.ZSorter().layer = (ZLayer)15000;
		yield return FiberAnimation.ScaleTransform(base.transform, Vector3.one, Vector3.one * 2f, SingletonAsset<LevelVisuals>.Instance.bigBubblePopScale, 0f);
		base.transform.localScale = Vector3.zero;
		EffectPool.Instance.SpawnEffect("BubblePopSmoke", base.transform.position, base.gameObject.layer, new object[0]);
		this.ZSorter().layer = org;
		yield break;
	}

	[SerializeField]
	private MatchFlag matchColor;
}
