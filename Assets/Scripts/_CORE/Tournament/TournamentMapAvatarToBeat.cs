using System;
using TactileModules.Foundation;
using UnityEngine;

public class TournamentMapAvatarToBeat : MonoBehaviour
{
	public void Initialize(CloudClientBase cloudClient)
	{
		this.cloudClient = cloudClient;
	}

	public void UpdateUI(TournamentCloudManager.Score score)
	{
		if (score != null)
		{
			this.scoreInfo = score;
			if (this.playerScore != null)
			{
				this.playerScore.text = L.FormatNumber(score.score);
			}
			if (!string.IsNullOrEmpty(score.facebookId))
			{
				this.avatarFB.SetActive(true);
				this.avatarNonFB.SetActive(false);
			}
			else
			{
				this.avatarFB.SetActive(false);
				this.avatarNonFB.SetActive(true);
				this.nonFBPortraitTexture.SetTexture(SingletonAsset<TournamentSetup>.Instance.GetRandomPortrait(this.scoreInfo.deviceId.GetHashCode()));
			}
		}
		else
		{
			if (this.playerScore != null)
			{
				this.playerScore.text = TournamentManager.Instance.GetTotalScoreForTournament();
			}
			string text = (!this.cloudClient.HasValidUser) ? string.Empty : this.cloudClient.CachedMe.CloudId;
			string text2 = (this.cloudClient.CachedDevice == null) ? string.Empty : this.cloudClient.CachedDevice.CloudId;
			if (!string.IsNullOrEmpty(text))
			{
				this.avatarNonFB.SetActive(false);
			}
			else if (false)
			{
				
			}
			else
			{
				this.avatarFB.SetActive(false);
				this.nonFBPortraitTexture.SetTexture(SingletonAsset<TournamentSetup>.Instance.GetRandomPortrait(text2.GetHashCode()));
			}
		}
	}
	
	public UILabel playerScore;

	public GameObject avatarFB;

	public GameObject avatarNonFB;

	public UITextureQuad nonFBPortraitTexture;

	private TournamentCloudManager.Score scoreInfo;

	private CloudClientBase cloudClient;
}
