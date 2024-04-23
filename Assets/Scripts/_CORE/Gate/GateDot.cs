using System;
using TactileModules.Foundation;
using TactileModules.PuzzleGame.MainLevels;
using UnityEngine;

public class GateDot : MapDot
{
	private LevelDatabaseCollection LevelDatabaseCollection
	{
		get
		{
			return ManagerRepository.Get<LevelDatabaseCollection>();
		}
	}

	protected override LevelDatabase LevelContext
	{
		get
		{
			return this.LevelDatabaseCollection.GetLevelDatabase<MainLevelDatabase>("Main");
		}
	}

	public override void UpdateUI()
	{
		this.UpdateUI(true);
	}

	protected override void UpdateUI(bool isGameRunning)
	{
		if (!isGameRunning)
		{
			return;
		}
		this.GetButton().payload = this.LevelId;
		Vector3 position = base.transform.position;
		position.z -= 1f;
		base.transform.position = position;
		int farthestUnlockedLevelIndex = PuzzleGame.PlayerState.FarthestUnlockedLevelIndex;
		bool flag = farthestUnlockedLevelIndex == this.LevelId;
		if (flag)
		{
			this.SetStars(GateManager.Instance.CurrentGateKeys);
		}
		else
		{
			this.SetStars(0);
		}
		this.enabledRoot.SetActive(base.Level.IsCompleted);
		this.disabledRoot.SetActive(!base.Level.IsCompleted);
		Collider component = base.GetComponent<Collider>();
		if (component != null)
		{
			component.enabled = !base.Level.IsCompleted;
		}
	}

	private void SetStars(int amount)
	{
		for (int i = 0; i < 3; i++)
		{
			this.stars[i].gameObject.SetActive(i < amount);
		}
	}

	public GameObject enabledRoot;

	public GameObject disabledRoot;

	public UIWidget[] stars;
}
