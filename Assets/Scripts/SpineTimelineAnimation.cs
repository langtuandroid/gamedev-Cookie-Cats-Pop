using System;
using Spine;
using Spine.Unity;
using UnityEngine;

public class SpineTimelineAnimation : MonoBehaviour
{
	private void Start()
	{
		float startTime = this.startFrame / 30f;
		float endTime = this.endFrame / 30f;
		TrackEntry track = this.spine.PlayAnimation(0, this.animationName, true, true);
		track.time = startTime;
		this.spine.Update(0f);
		this.spine.skeleton.UpdateWorldTransform();
		UpdateBonesDelegate value = delegate(ISkeletonAnimation animatedSkeletonComponent)
		{
			track.time = Mathf.Repeat(track.time - startTime, endTime - startTime) + startTime;
			this.spine.skeleton.Update(0f);
			this.spine.state.Update(0f);
		};
		this.spine.UpdateLocal += value;
	}

	public SkeletonAnimation spine;

	public string animationName;

	public float startFrame;

	public float endFrame;
}
