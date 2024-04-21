using System;
using UnityEngine;

public class Keyhole : MonoBehaviour
{
	public void SetState(Keyhole.LockState lockState)
	{
		if (lockState != Keyhole.LockState.Locked)
		{
			if (lockState == Keyhole.LockState.Opened)
			{
				this.keyholeLocked.gameObject.SetActive(true);
				this.keyholeUnlocked.gameObject.SetActive(true);
			}
		}
		else
		{
			this.keyholeLocked.gameObject.SetActive(true);
			this.keyholeUnlocked.gameObject.SetActive(false);
		}
	}

	public UISprite keyholeLocked;

	public UISprite keyholeUnlocked;

	public enum LockState
	{
		Locked,
		Opened
	}
}
