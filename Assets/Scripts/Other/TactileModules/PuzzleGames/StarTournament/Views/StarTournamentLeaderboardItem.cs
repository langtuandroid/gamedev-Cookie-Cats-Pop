using System;
using System.Collections.Generic;
using TactileModules.PuzzleGame.MainLevels;
using UnityEngine;

namespace TactileModules.PuzzleGames.StarTournament.Views
{
	public class StarTournamentLeaderboardItem : MonoBehaviour
	{
		public void Init(StarTournamentManager manager, IMainProgression mainProgression, CloudClientBase cloudClientBase, int position, Entry entry, bool isMe)
		{
			this.manager = manager;
			this.cloudClientBase = cloudClientBase;
			if (isMe)
			{
				this.levelLabel.text = string.Format(L.Get("Level {0}"), mainProgression.GetFarthestCompletedLevelHumanNumber());
				this.levelLabel.Color = this.textLabelColors.myLevelLabelColor;
				this.backgroundSprite.Color = this.textLabelColors.myBackgroundColor;
			}
			else
			{
				this.levelLabel.text = string.Format(L.Get("Level {0}"), entry.MaxLevel);
				this.levelLabel.Color = this.textLabelColors.levelLabelColor;
				this.backgroundSprite.Color = this.textLabelColors.backgroundColor;
			}
			this.score.text = L.FormatNumber(entry.Stars);
			this.SetPosition(position);
			CloudUser cloudUser = manager.GetCloudUser(entry.UserId);
			this.SetName(isMe, entry, cloudUser);
			this.SetPortrait(isMe, entry, cloudUser);
		}

		private void SetPosition(int rank)
		{
			this.positionNumber.text = rank.ToString();
			if (rank <= this.manager.GetRewardsAmount())
			{
				this.leaderIconSprite.gameObject.SetActive(true);
				this.leaderIconSprite.SpriteName = this.leaderSpriteNames[rank - 1];
				this.prizeLabel.gameObject.SetActive(true);
				this.prizeSprite.gameObject.SetActive(true);
				this.prizeSprite.SpriteName = this.prizeSpriteNames[rank - 1];
				StarTournamentConfig.Reward reward = this.manager.GetReward(rank);
				if (reward.Items.Count > 1)
				{
				}
				this.prizeLabel.text = reward.Items[0].Amount + "\ncoins";
			}
			else
			{
				this.prizeLabel.gameObject.SetActive(false);
				this.prizeSprite.gameObject.SetActive(false);
				this.leaderIconSprite.gameObject.SetActive(false);
			}
		}

		private void SetName(bool isMe, Entry entry, CloudUser user)
		{
			this.userName.text = this.GetFirstName(isMe, user);
			if (string.IsNullOrEmpty(this.userName.text))
			{
				if (isMe)
				{
					this.userName.text = L.Get("You");
					this.userName.Color = this.textLabelColors.myNameLabelColor;
				}
				else
				{
					this.userName.text = RandomNames.GetRandomName(entry.DeviceId);
					this.userName.Color = this.textLabelColors.nameLabelColor;
				}
			}
		}

		private void SetPortrait(bool isMe, Entry entry, CloudUser user)
		{
			this.portraitFrameSprite.SpriteName = ((!isMe) ? this.portraitFramesNames[1] : this.portraitFramesNames[0]);
			string text = string.Empty;
			if (user != null && !string.IsNullOrEmpty(user.FacebookId))
			{
				text = user.FacebookId;
			}
			else if (isMe)
			{
				text = ((!this.cloudClientBase.HasValidUser) ? string.Empty : this.cloudClientBase.CachedMe.ExternalId);
			}
			if (string.IsNullOrEmpty(text))
			{
				bool flag = false;
				if (isMe && false)
				{
					this.nonFBPortraitTexture.gameObject.SetActive(false);
					flag = true;
				}
				if (!flag)
				{
					this.nonFBPortraitTexture.SetTexture(this.manager.RandomPortraitsAndNames.GetRandomPortrait(entry.DeviceId.GetHashCode()));
				}
			}
			else
			{
				this.nonFBPortraitObject.SetActive(false);
			}
		}

		private string GetFirstName(bool isMe, CloudUser user)
		{
			string text = string.Empty;
			if (user != null)
			{
				text = user.DisplayName;
			}
			else if (isMe)
			{
				text = ((!this.cloudClientBase.HasValidUser) ? string.Empty : this.cloudClientBase.CachedMe.DisplayName);
			}
			if (!string.IsNullOrEmpty(text))
			{
				string[] array = text.Split(new char[]
				{
					' '
				});
				if (array.Length > 0)
				{
					return array[0];
				}
			}
			return string.Empty;
		}

		[SerializeField]
		private UILabel positionNumber;

		[SerializeField]
		private GameObject nonFBPortraitObject;

		[SerializeField]
		private UITextureQuad nonFBPortraitTexture;

		[SerializeField]
		private UISprite portraitFrameSprite;

		[SerializeField]
		private UILabel userName;

		[SerializeField]
		private UILabel score;

		[SerializeField]
		private UILabel levelLabel;

		[SerializeField]
		private UISprite prizeSprite;

		[SerializeField]
		private UILabel prizeLabel;

		[SerializeField]
		private UISprite leaderIconSprite;

		[SerializeField]
		private UISprite backgroundSprite;

		[SerializeField]
		private List<string> portraitFramesNames;

		[SerializeField]
		private List<string> prizeSpriteNames;

		[SerializeField]
		private List<string> leaderSpriteNames;

		[SerializeField]
		private StarTournamentLeaderboardItem.TextLabelColors textLabelColors;

		private StarTournamentManager manager;

		private CloudClientBase cloudClientBase;

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
}
