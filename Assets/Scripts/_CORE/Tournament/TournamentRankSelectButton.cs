using System;
using UnityEngine;

public class TournamentRankSelectButton : AnimatedButton, ITournamentRankSelectButton
{
	public TournamentRank Rank { get; private set; }

	public bool Selected
	{
		get
		{
			return this.selected;
		}
		set
		{
			this.selected = value;
			this.ReflectState();
		}
	}

	public void Init(TournamentRank rank)
	{
		this.Rank = rank;
		TournamentSetup.RankSetup rankSetup = SingletonAsset<TournamentSetup>.Instance.GetRankSetup(rank);
		this.rankIcon.SpriteName = rankSetup.iconSpriteName;
		this.ReflectState();
	}

	protected override void ReflectState()
	{
		base.ReflectState();
		this.selectionPivot.gameObject.SetActive(this.selected);
		if (this.selected)
		{
			this.background.Color = Color.white;
			this.rankIcon.Color = Color.white;
		}
		else
		{
			this.background.Color = new Color(0.9f, 0.9f, 0.9f);
			this.rankIcon.Color = new Color(0.9f, 0.9f, 0.9f);
		}
	}

	[SerializeField]
	private UISprite rankIcon;

	[SerializeField]
	private UISprite background;

	[SerializeField]
	private GameObject selectionPivot;

	private bool selected;
}
