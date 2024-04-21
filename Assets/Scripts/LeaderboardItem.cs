using System;
using System.Collections.Generic;
using TactileModules.Foundation;
using UnityEngine;

public class LeaderboardItem : MonoBehaviour
{
	public void InitFacebookClient(int position, CloudScore score)
	{
		FacebookClient facebookClient = ManagerRepository.Get<FacebookClient>();
		this.positionNumber.text = position.ToString();
		this.scoreLabel.text = L.FormatNumber(score.Score);
		this.backgroundImage.Color = ((!CloudScoreHelper.IsMe(score)) ? this.textLabelColors.backgroundColor : this.textLabelColors.myBackgroundColor);
		this.userName.Color = ((!CloudScoreHelper.IsMe(score)) ? this.textLabelColors.nameLabelColor : this.textLabelColors.myNameLabelColor);
		this.SetPosition(position);
		CloudUser cloudUser = (score.UserId == null) ? null : ManagerRepository.Get<CloudClient>().GetUserForCloudId(score.UserId);
		if (cloudUser != null)
		{
			string text = (!CloudScoreHelper.IsMe(score)) ? cloudUser.GetFirstName(15) : L.Get("You");
			this.userName.text = text;
			if (string.IsNullOrEmpty(cloudUser.FacebookId))
			{
				if (CloudScoreHelper.IsMe(score) && facebookClient.IsSessionValid && facebookClient.CachedMe != null)
				{
					this.portrait.Load(facebookClient, facebookClient.CachedMe.Id, null);
					this.nonFBPortraitTexture.gameObject.SetActive(false);
				}
				else
				{
					this.ShowRandomPortrait(score);
				}
			}
			else
			{
				this.portrait.Load(facebookClient, cloudUser.FacebookId, null);
				this.nonFBPortraitObject.SetActive(false);
			}
		}
		else
		{
			this.userName.text = RandomHelper.RandomName(score.UserId.GetHashCode());
			this.ShowRandomPortrait(score);
		}
	}

	public void InitTournamentClient()
	{
	}

	public void SetPosition(int rank)
	{
		this.positionNumber.text = rank.ToString();
		if (rank <= 3)
		{
			this.leaderIconSprite.gameObject.SetActive(true);
			this.leaderIconSprite.SpriteName = this.leaderSpriteNames[rank - 1];
		}
		else
		{
			this.leaderIconSprite.gameObject.SetActive(false);
		}
	}

	private void ShowRandomPortrait(CloudScore score)
	{
		this.portrait.gameObject.SetActive(false);
		Texture2D texture = RandomHelper.RandomPortrait(score.deviceId.GetHashCode());
		this.nonFBPortraitTexture.SetTexture(texture);
	}

	[SerializeField]
	private UILabel positionNumber;

	[SerializeField]
	private FacebookPortraitWithProgress portrait;

	[SerializeField]
	private GameObject nonFBPortraitObject;

	[SerializeField]
	private UITextureQuad nonFBPortraitTexture;

	[SerializeField]
	private UILabel userName;

	[SerializeField]
	private UILabel scoreLabel;

	[SerializeField]
	private GameObject meRoot;

	[SerializeField]
	private UISprite backgroundImage;

	[SerializeField]
	private UISprite leaderIconSprite;

	[SerializeField]
	private LeaderboardItem.TextLabelColors textLabelColors;

	[SerializeField]
	private List<string> leaderSpriteNames;

	[Serializable]
	private struct TextLabelColors
	{
		public Color myNameLabelColor;

		public Color myLevelLabelColor;

		public Color myBackgroundColor;

		public Color nameLabelColor;

		public Color levelLabelColor;

		public Color backgroundColor;
	}
}
