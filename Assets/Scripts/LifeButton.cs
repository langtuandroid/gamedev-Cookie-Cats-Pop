using System;
using Tactile;
using UnityEngine;

public class LifeButton : MonoBehaviour
{
	[Instantiator.SerializeProperty]
	public int MyInt { get; set; }

	private void OnEnable()
	{
		this.RefreshLives(null);
		InventoryManager.Instance.InventoryChanged += this.RefreshLives;
	}

	private void OnDisable()
	{
		InventoryManager.Instance.InventoryChanged -= this.RefreshLives;
	}

	private void Update()
	{
		int regenerationTimeLeft = LivesManager.Instance.GetRegenerationTimeLeft();
		if (regenerationTimeLeft > 0)
		{
			TimeSpan timeSpan = TimeSpan.FromSeconds((double)regenerationTimeLeft);
			this.TimeCounterLabel.text = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
		}
		else
		{
			this.TimeCounterLabel.text = L.Get("Full");
		}
	}

	private void RefreshLives(InventoryManager.ItemChangeInfo info)
	{
		if (info == null || info.Item != "Life")
		{
			return;
		}
		this.LifeCounterLabel.text = InventoryManager.Instance.Lives.ToString();
	}

	public UILabel LifeCounterLabel;

	public UILabel TimeCounterLabel;
}
