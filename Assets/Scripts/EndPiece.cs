using System;
using UnityEngine;

[RequireComponent(typeof(UIElement))]
public class EndPiece : MonoBehaviour
{
	public UIElement UIElement
	{
		get
		{
			if (this.element == null)
			{
				this.element = base.GetComponent<UIElement>();
			}
			return this.element;
		}
	}

	private UIElement element;
}
