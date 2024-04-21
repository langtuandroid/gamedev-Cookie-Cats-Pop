using System;
using TactileModules.Foundation;
using UnityEngine;

public class TournamentDot : MapDot
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
			return this.LevelDatabaseCollection.GetLevelDatabase<TournamentLevelDatabase>("Tournament");
		}
	}

	protected override void UpdateUI(bool gameIsRunning)
	{
		if (!gameIsRunning)
		{
			return;
		}
		if (!base.Level.IsValid)
		{
			this.nameLabel.text = "???";
			return;
		}
		if (this.nameLabel != null)
		{
			this.nameLabel.text = L.FormatNumber(TournamentManager.Instance.GetScoreForDot(base.Level.Index));
		}
		this.enabledRoot.SetActive(base.Level.IsUnlocked);
		this.disabledRoot.SetActive(!this.enabledRoot.activeSelf);
		this.stars.SetStars(base.Level.Stars, base.Level.GetHardStars());
	}

	[SerializeField]
	private GameObject enabledRoot;

	[SerializeField]
	private GameObject disabledRoot;

	[SerializeField]
	private UILabel nameLabel;

	[SerializeField]
	private MapDotStars stars;
}
