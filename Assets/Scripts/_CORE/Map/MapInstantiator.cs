using System;
using UnityEngine;

public class MapInstantiator : Instantiator
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
			return (!(this.Element != null)) ? Vector2.one : this.Element.Size;
		}
		set
		{
			if (this.Element != null)
			{
				this.Element.Size = value;
			}
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.clear;
		if (this.instance != null && this.instance.GetElement())
		{
			Gizmos.DrawCube(base.transform.position, this.instance.GetElementSize());
		}
	}

	public override void CreateInstance()
	{
		base.CreateInstance();
		UIElement instance = base.GetInstance<UIElement>();
		if (instance != null && this.Element != null)
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

	public bool useAssetBundle;
}
