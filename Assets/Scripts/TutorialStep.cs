using System;
using System.Collections.Generic;

[Serializable]
public class TutorialStep : ITutorialStep
{
	public string Message
	{
		get
		{
			return L.Get(this.message);
		}
	}

	public List<int> activeTiles = new List<int>();

	public string message = "<Tutorial Message>";

	public TutorialStep.DismissType dismissType;

	public InventoryItem useBoosterType;

	public int pointAtTile = -1;

	public bool slowAiming;

	public bool highlightShooter;

	public bool highlightPowercats;

	public bool highlightBasket;

	public bool showTraceLine;

	public bool highlightFinger = true;

	public bool highlightBoss;

	public enum DismissType
	{
		TapToContinue,
		Shoot,
		SwapQueue,
		UsePower,
		WaitATurn,
		UseBooster,
		TapToContinueTight,
		WaitForFreePowerClaimed,
		BossNewStage
	}
}
