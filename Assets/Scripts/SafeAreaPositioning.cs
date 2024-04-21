using System;
using UnityEngine;

[RequireComponent(typeof(UIElement))]
public class SafeAreaPositioning : MonoBehaviour
{
	private bool HasSafeArea()
	{
		return false;
	}

	public void HandleSafeAreaPositioning()
	{
		if (this.HasSafeArea())
		{
			base.GetComponent<UIElement>().LocalPosition += this.addedSafeAreaPosition;
		}
	}

	[SerializeField]
	private Vector2 addedSafeAreaPosition;
}
