using System;
using TactileModules.PuzzleGame.MainLevels;
using UnityEngine;

public class MapProgressionEnabler : MonoBehaviour
{
	[Instantiator.SerializeProperty]
	public int EnableAtDotIndex
	{
		get
		{
			return this.enableAtDotIndex;
		}
		set
		{
			this.enableAtDotIndex = value;
			if (Application.isPlaying)
			{
				this.ToggleRoadAnimationComponent();
			}
		}
	}

	private void ToggleRoadAnimationComponent()
	{
		int farthestUnlockedLevelIndex = MainProgressionManager.Instance.GetFarthestUnlockedLevelIndex();
		bool flag = farthestUnlockedLevelIndex >= this.EnableAtDotIndex;
		if (this.component != null)
		{
			this.component.enabled = flag;
			this.component.gameObject.SetActive(flag);
		}
	}

	public MonoBehaviour component;

	private int enableAtDotIndex;
}
