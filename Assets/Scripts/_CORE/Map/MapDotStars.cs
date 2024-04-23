using System;
using UnityEngine;

public class MapDotStars : MonoBehaviour
{
	public bool Disabled { get; set; }

	public int MaxVisibleStars
	{
		get
		{
			return this.maxVisibleStars;
		}
		set
		{
			this.maxVisibleStars = value;
			this.UpdateState();
		}
	}

	public void SetStars(int numStars, int numHardStars = 0)
	{
		this.numStars = numStars;
		this.numHardStars = numHardStars;
		this.UpdateState();
	}

	private void UpdateState()
	{
		for (int i = 0; i < this.stars.Length; i++)
		{
			bool flag = i < this.numStars && (this.maxVisibleStars < 0 || i < this.maxVisibleStars);
			this.stars[i].SetActive(flag);
			if (flag)
			{
				this.stars[i].GetComponent<UISprite>().SpriteName = ((i >= this.numHardStars) ? this.normalSpriteName : this.hardSpriteName);
			}
		}
	}

	[SerializeField]
	private GameObject[] stars;

	[SerializeField]
	[UISpriteName]
	private string normalSpriteName;

	[SerializeField]
	[UISpriteName]
	private string hardSpriteName;

	private int maxVisibleStars = -1;

	private int numStars;

	private int numHardStars;
}
