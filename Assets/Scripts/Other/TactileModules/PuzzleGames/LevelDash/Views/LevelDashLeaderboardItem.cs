using System;
using System.Collections.Generic;
using UnityEngine;

namespace TactileModules.PuzzleGames.LevelDash.Views
{
	public class LevelDashLeaderboardItem : MonoBehaviour
	{
		public void Init(int rank, Entry entry, CloudUser user, CloudClientBase cloudClient, bool isMe, int rewardsCount, int levelProgression)
		{
			this.cloudClient = cloudClient;
			this.score.Color = ((!isMe) ? this.textLabelColors.levelLabelColor : this.textLabelColors.myLevelLabelColor);
			this.SetLevelProgressionVisuals(levelProgression);
			this.backgroundSprite.SpriteName = ((!isMe) ? this.backgroundSpriteNames[1] : this.backgroundSpriteNames[0]);
			this.ConfigureRankVisuals(rank, rewardsCount);
			this.SetName(isMe, entry, user);
			this.SetProtrait(isMe, entry, user);
		}

		private void SetLevelProgressionVisuals(int levelProgression)
		{
			if (levelProgression > 0)
			{
				this.score.gameObject.SetActive(true);
				if (levelProgression == 1)
				{
					this.score.text = string.Format(L.Get("Completed {0} Level"), levelProgression);
				}
				else
				{
					this.score.text = string.Format(L.Get("Completed {0} Levels"), levelProgression);
				}
			}
			else
			{
				this.score.gameObject.SetActive(false);
			}
		}

		private void ConfigureRankVisuals(int rank, int rewardsCount)
		{
			this.positionNumber.text = rank.ToString();
			if (rank <= rewardsCount)
			{
				this.leaderIconSprite.gameObject.SetActive(true);
				this.leaderIconSprite.SpriteName = this.leaderSpriteNames[rank - 1];
				this.prizeTextureQuad.gameObject.SetActive(true);
				this.prizeTextureQuad.TextureResource = this.prizeTexturesPathes[rank - 1];
			}
			else
			{
				this.prizeTextureQuad.gameObject.SetActive(false);
				this.leaderIconSprite.gameObject.SetActive(false);
			}
		}

		private void SetName(bool isMe, Entry entry, CloudUser user)
		{
			string text = this.GetFirstName(isMe, user);
			if (string.IsNullOrEmpty(text))
			{
				text = ((!isMe) ? RandomHelper.RandomName(entry.DeviceId.GetHashCode()) : L.Get("You"));
			}
			this.SetName(text, isMe);
		}

		private void SetProtrait(bool isMe, Entry entry, CloudUser user)
		{
			this.portraitFrameSprite.Color = ((!isMe) ? this.portraitFramesColors[1] : this.portraitFramesColors[0]);
			string text = string.Empty;
			if (user != null && !string.IsNullOrEmpty(user.FacebookId))
			{
				text = user.FacebookId;
			}
			if (string.IsNullOrEmpty(text))
			{
				if (isMe && false)
				{
					
				}
				else
				{
					this.nonFBPortraitTexture.SetTexture(RandomHelper.RandomPortrait(entry.DeviceId.GetHashCode()));
					this.nonFBPortraitTexture.gameObject.SetActive(true);
				}
			}
			else
			{
				this.SetFacebookPortrait(text);
			}
		}

		private void SetFacebookPortrait(string facebookId)
		{
			this.nonFBPortraitTexture.gameObject.SetActive(false);
		}

		protected void SetName(string playerName, bool isMe)
		{
			this.userName.text = playerName;
			this.userName.Color = ((!isMe) ? this.textLabelColors.nameLabelColor : this.textLabelColors.myNameLabelColor);
		}

		protected string GetFirstName(bool isMe, CloudUser user)
		{
			string text = string.Empty;
			if (user != null)
			{
				text = user.DisplayName;
			}
			else if (isMe)
			{
				text = ((!this.cloudClient.HasValidUser) ? string.Empty : this.cloudClient.CachedMe.DisplayName);
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

		[Header("Base Components")]
		[SerializeField]
		protected UILabel positionNumber;

		[SerializeField]
		protected UITextureQuad nonFBPortraitTexture;

		[SerializeField]
		protected UISprite portraitFrameSprite;

		[SerializeField]
		protected UILabel userName;

		[SerializeField]
		protected UILabel score;

		[SerializeField]
		protected List<Color> portraitFramesColors;

		[SerializeField]
		protected LevelDashLeaderboardItem.TextLabelColors textLabelColors;

		[Header("Item Specific Components")]
		[SerializeField]
		private UIResourceQuad prizeTextureQuad;

		[SerializeField]
		private UISprite leaderIconSprite;

		[SerializeField]
		private UISprite backgroundSprite;

		[SerializeField]
		private List<string> leaderSpriteNames;

		[SerializeField]
		private List<string> backgroundSpriteNames;

		[SerializeField]
		private List<string> prizeTexturesPathes;

		private CloudClientBase cloudClient;

		[Serializable]
		protected struct TextLabelColors
		{
			public Color myNameLabelColor;

			public Color myLevelLabelColor;

			public Color nameLabelColor;

			public Color levelLabelColor;
		}
	}
}
