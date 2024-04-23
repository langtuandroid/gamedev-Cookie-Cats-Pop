using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class TournamentUnlockedView : ExtensibleView<TournamentUnlockedView.IExtension>
{
	public void Initialize(List<ItemAmount> rewards, Action completed)
	{
		this.continueIsClicked = false;
		this.title.text = string.Format(L.Get(this.titleFormattedText), 3);
		this.description.text = this.descriptionText;
		this.completedCallback = completed;
		if (base.Extension != null)
		{
			base.Extension.SetRewardVisuals(rewards);
		}
	}

	[UsedImplicitly]
	private void ContinueClicked(UIEvent e)
	{
		if (this.continueIsClicked)
		{
			return;
		}
		this.continueIsClicked = true;
		FiberCtrl.Pool.Run(this.RewardFlow(), false);
	}

	private IEnumerator RewardFlow()
	{
		base.Close(0);
		if (base.Extension != null)
		{
			yield return base.Extension.AnimateGivingRewards();
		}
		this.completedCallback();
		yield break;
	}

	[SerializeField]
	private UILabel title;

	[SerializeField]
	private UILabel description;

	[SerializeField]
	[LocalizedStringField]
	private string titleFormattedText = "You just received {0} FREE Bronze Tickets!";

	[SerializeField]
	[LocalizedStringField]
	private string descriptionText = "Prove you are the chambeeon by joining a tournament!";

	private bool continueIsClicked;

	private Action completedCallback;

	public interface IExtension
	{
		void SetRewardVisuals(List<ItemAmount> rewards);

		IEnumerator AnimateGivingRewards();
	}
}
