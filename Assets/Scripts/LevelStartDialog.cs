using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class LevelStartDialog : MonoBehaviour
{
	////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action ClickedDismiss;



	////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action ClickedPlay;



	////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<int> ClickedCheat;



	private void HandlePlayClicked(UIEvent e)
	{
		this.ClickedPlay();
	}

	private void HandleClosedClicked(UIEvent e)
	{
		this.ClickedDismiss();
	}

	private void HandleCheatClicked(UIEvent e)
	{
		this.ClickedCheat((int)e.payload);
	}

	public void SetStars(int numStars, int numHardStars)
	{
		for (int i = 0; i < this.stars.Length; i++)
		{
			this.stars[i].sprite.gameObject.SetActive(i < numStars);
			this.stars[i].sprite.SpriteName = ((i >= numHardStars) ? this.stars[i].normalSpriteName : this.stars[i].hardSpriteName);
		}
	}

	[SerializeField]
	public LevelStartDialog.StarSprite[] stars;

	[SerializeField]
	public List<UIInstantiator> boosterButtons;

	[SerializeField]
	public UILabel nextGoalLabel;

	[SerializeField]
	public UILabel levelLabel;

	[SerializeField]
	public UILabel chooseBoostersLabel;

	[SerializeField]
	public List<GameObject> hardObjects;

	[SerializeField]
	public Transform buttonAddonPivot;

	[SerializeField]
	public UIInstantiator buttonPlay;

	[SerializeField]
	public List<UIButton> cheatButtons;

	[SerializeField]
	public GameObject slideAndLadders;

	[SerializeField]
	public GameObject slide;

	[SerializeField]
	public GameObject ladder;

	[SerializeField]
	public GameObject chest;

	[Serializable]
	public class StarSprite
	{
		public UISprite sprite;

		[UISpriteName]
		public string normalSpriteName;

		[UISpriteName]
		public string hardSpriteName;
	}
}
