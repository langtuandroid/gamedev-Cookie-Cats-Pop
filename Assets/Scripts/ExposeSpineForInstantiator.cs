using System;
using UnityEngine;

[RequireComponent(typeof(SkeletonAnimation))]
public class ExposeSpineForInstantiator : MonoBehaviour
{
	[Instantiator.SerializeProperty]
	public string AnimationName
	{
		get
		{
			SkeletonAnimation component = base.GetComponent<SkeletonAnimation>();
			return component.AnimationName;
		}
		set
		{
			SkeletonAnimation component = base.GetComponent<SkeletonAnimation>();
			component.AnimationName = value;
		}
	}
}
