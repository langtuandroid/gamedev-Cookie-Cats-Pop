using System;
using UnityEngine;

public class GateKeyHoles : MonoBehaviour
{
	private void Awake()
	{
		foreach (UIInstantiator uiinstantiator in this.keyholes)
		{
			uiinstantiator.CreateInstance();
		}
	}

	public void Refresh(int numKeys)
	{
		for (int i = 0; i < 3; i++)
		{
			if (i < numKeys)
			{
				this.keyholes[i].GetInstance<Keyhole>().SetState(Keyhole.LockState.Opened);
			}
			else
			{
				this.keyholes[i].GetInstance<Keyhole>().SetState(Keyhole.LockState.Locked);
			}
		}
	}

	[Tooltip("Should contain exactly 3 keyholes!")]
	public UIInstantiator[] keyholes;
}
