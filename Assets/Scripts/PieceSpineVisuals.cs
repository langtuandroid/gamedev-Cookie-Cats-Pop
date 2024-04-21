using System;
using TactileModules.Validation;
using UnityEngine;

public class PieceSpineVisuals : PieceVisuals
{
	public override void VisualsChanged()
	{
		this.SetSpineColor(this.spine);
		if (this.instantiator != null)
		{
			this.SetSpineColor(this.instantiator.GetInstance<Transform>().gameObject.GetComponentInChildren<SkeletonAnimation>());
		}
	}

	private void SetSpineColor(SkeletonAnimation spine)
	{
		if (spine == null)
		{
			return;
		}
		spine.skeleton.SetColor((!base.Highlighted) ? Color.grey : Color.white);
	}

	[SerializeField]
	[OptionalSerializedField]
	private SkeletonAnimation spine;

	[SerializeField]
	private UIInstantiator instantiator;
}
