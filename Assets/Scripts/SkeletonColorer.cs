using System;
using UnityEngine;

[RequireComponent(typeof(SkeletonAnimation))]
[ExecuteInEditMode]
public class SkeletonColorer : MonoBehaviour
{
	private void Awake()
	{
		this.skeletonAnimation = base.GetComponent<SkeletonAnimation>();
	}

	private void Update()
	{
		if (this.skeletonAnimation != null && this.skeletonAnimation.skeleton != null)
		{
			this.skeletonAnimation.skeleton.SetColor(this.color);
		}
	}

	public Color color = Color.white;

	private SkeletonAnimation skeletonAnimation;
}
