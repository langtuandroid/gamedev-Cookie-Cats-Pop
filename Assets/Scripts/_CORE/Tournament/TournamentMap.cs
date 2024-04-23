using System;
using UnityEngine;

public class TournamentMap : MonoBehaviour
{
	public void SetBackground(string backgroundName)
	{
		this.background.TextureResource = backgroundName;
	}

	public UIResourceQuad background;
}
