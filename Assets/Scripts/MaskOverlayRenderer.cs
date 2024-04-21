using System;
using UnityEngine;

public class MaskOverlayRenderer : MonoBehaviour
{
	private void OnPostRender()
	{
		if (this.onRender != null)
		{
			this.onRender();
		}
	}

	public Action onRender;
}
