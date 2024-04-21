using System;
using UnityEngine;

public class ExposeSkeletonForInstantiator : MonoBehaviour
{
	[Instantiator.SerializeProperty]
	public string OctopusAnimation
	{
		get
		{
			return this.skeleton.AnimationName;
		}
		set
		{
			this.skeleton.AnimationName = value;
		}
	}

	public SkeletonAnimation skeleton;
}
