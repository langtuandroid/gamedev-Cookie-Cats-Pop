using System;
using UnityEngine;

public abstract class PieceVisuals : MonoBehaviour
{
	public bool Highlighted
	{
		get
		{
			return this.highlighted;
		}
		set
		{
			if (this.highlighted == value)
			{
				return;
			}
			this.highlighted = value;
			this.VisualsChanged();
		}
	}

	public abstract void VisualsChanged();

	private bool highlighted = true;
}
