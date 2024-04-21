using System;
using System.Collections.Generic;
using UnityEngine;

public class SpineInstantiator : Instantiator
{
	public override void CreateInstance()
	{
		base.CreateInstance();
		if (this.prefab != null)
		{
			int index = UnityEngine.Random.Range(0, this.animationChoices.Count);
			SkeletonAnimation instance = base.GetInstance<SkeletonAnimation>();
			if (Application.isPlaying)
			{
				instance.PlayAnimation(0, this.animationChoices[index], this.loop, false);
			}
			else
			{
				instance.AnimationName = this.animationChoices[index];
			}
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	[SerializeField]
	private List<string> animationChoices;

	[SerializeField]
	private bool loop;
}
