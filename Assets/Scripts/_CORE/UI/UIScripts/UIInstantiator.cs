using System;
using UnityEngine;

[RequireComponent(typeof(UIElement))]
public class UIInstantiator : Instantiator
{
	private UIElement Element
	{
		get
		{
			return base.GetComponent<UIElement>();
		}
	}

	private Vector2 Size
	{
		get
		{
			return this.Element.Size;
		}
		set
		{
			this.Element.Size = value;
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.clear;
		Gizmos.DrawCube(base.transform.position, this.Size);
	}

	public override void CreateInstance()
	{
		base.CreateInstance();
		UIElement instance = base.GetInstance<UIElement>();
		if (instance != null)
		{
			instance.SetSizeFlags(UIAutoSizing.AllCorners);
			if (this.Element.SizeIsValid)
			{
				instance.SetSizeAndDoLayout(this.Size);
			}
			else
			{
				this.Size = instance.Size;
			}
			this.instance.hideFlags |= HideFlags.NotEditable;
		}
	}
}
