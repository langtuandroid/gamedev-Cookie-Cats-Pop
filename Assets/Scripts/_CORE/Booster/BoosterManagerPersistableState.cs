using System;
using System.Collections.Generic;
using Tactile;
using UnityEngine;

[SettingsProvider("bo", false, new Type[]
{

})]
public class BoosterManagerPersistableState : IPersistableState<BoosterManagerPersistableState>, IPersistableState
{
	private BoosterManagerPersistableState()
	{
		this.needAttention = new Dictionary<string, int>();
	}

	[JsonSerializable("att", typeof(int))]
	public Dictionary<string, int> needAttention { get; set; }

	public void MergeFromOther(BoosterManagerPersistableState newState, BoosterManagerPersistableState lastCloudState)
	{
		foreach (KeyValuePair<string, int> keyValuePair in newState.needAttention)
		{
			if (!this.needAttention.ContainsKey(keyValuePair.Key))
			{
				this.needAttention.Add(keyValuePair.Key, keyValuePair.Value);
			}
			else
			{
				this.needAttention[keyValuePair.Key] = Mathf.Max(keyValuePair.Value, this.needAttention[keyValuePair.Key]);
			}
		}
	}
}
