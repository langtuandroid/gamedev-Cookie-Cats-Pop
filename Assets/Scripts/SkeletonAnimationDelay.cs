using System;
using Spine;
using UnityEngine;

public class SkeletonAnimationDelay : MonoBehaviour
{
	private void Start()
	{
		SkeletonAnimation component = base.GetComponent<SkeletonAnimation>();
		TrackEntry trackEntry = component.AnimationState.Tracks[0];
		trackEntry.Time = this.delay;
	}

	[SerializeField]
	private float delay;
}
