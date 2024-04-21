using System;
using UnityEngine;

public class ExposeSpineSkinForInstantiator : MonoBehaviour
{
	[Instantiator.SerializeProperty]
	public string SkinName
	{
		get
		{
			return (!(this.skeletonAnimation != null) || this.skeletonAnimation.skeleton == null || this.skeletonAnimation.skeleton.skin == null) ? string.Empty : this.skeletonAnimation.skeleton.Skin.Name;
		}
		set
		{
			if (this.skeletonAnimation == null)
			{
				return;
			}
			if (string.IsNullOrEmpty(value))
			{
				return;
			}
			if (this.skeletonAnimation.skeleton != null)
			{
				this.skeletonAnimation.skeleton.SetSkin(value);
			}
		}
	}

	public SkeletonAnimation skeletonAnimation;
}
