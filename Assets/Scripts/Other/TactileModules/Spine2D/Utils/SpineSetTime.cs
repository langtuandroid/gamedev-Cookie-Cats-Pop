using System;
using Spine;
using UnityEngine;

namespace TactileModules.Spine2D.Utils
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(SkeletonAnimation))]
	public class SpineSetTime : MonoBehaviour
	{
		private void OnEnable()
		{
			if (Application.isPlaying)
			{
				this.SetSkeletonAnimationStartTime();
			}
		}

		private void SetSkeletonAnimationStartTime()
		{
			SkeletonAnimation component = base.GetComponent<SkeletonAnimation>();
			if (component.state != null)
			{
				TrackEntry current = component.state.GetCurrent(0);
				current.Time = current.EndTime * this.startTime;
			}
		}

		[SerializeField]
		[Range(0f, 1f)]
		protected float startTime;
	}
}
