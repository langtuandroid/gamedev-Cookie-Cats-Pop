using System;
using Spine;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(SkeletonAnimation))]
public class SpineColorizer : MonoBehaviour
{
	public Color Color
	{
		get
		{
			return this.color;
		}
	}

	private void OnEnable()
	{
		if (Application.isPlaying)
		{
			this.SetColor(this.color);
		}
	}

	public void SetColor(Color spineColor)
	{
		SkeletonAnimation component = base.GetComponent<SkeletonAnimation>();
		if (component.skeleton != null)
		{
			component.skeleton.SetColor(spineColor);
		}
	}

	public void SetSlotsColor(Color c, Predicate<Slot> filter = null)
	{
		Color color = this.color;
		this.SetColor(Color.white);
		SkeletonAnimation component = base.GetComponent<SkeletonAnimation>();
		if (component.skeleton == null)
		{
			return;
		}
		foreach (Slot slot in component.skeleton.slots)
		{
			if (filter != null)
			{
				if (filter(slot))
				{
					slot.SetColor(c);
				}
				else
				{
					slot.SetColor(color);
				}
			}
			else
			{
				slot.SetColor(c);
			}
		}
	}

	[SerializeField]
	protected Color color;
}
