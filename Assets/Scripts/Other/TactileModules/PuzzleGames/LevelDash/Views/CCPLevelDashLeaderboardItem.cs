using System;
using System.Collections.Generic;
using TactileModules.Validation;
using UnityEngine;

namespace TactileModules.PuzzleGames.LevelDash.Views
{
	public class CCPLevelDashLeaderboardItem : MonoBehaviour
	{
		public void Init(int rank, Entry entry, CloudUser user, CloudClientBase cloudClient, bool isMe, int rewardsCount, int levelProgression)
		{
			this.cloudClient = cloudClient;
			this.SetLevelProgressionVisuals(levelProgression);
			this.ConfigureRankVisuals(rank, rewardsCount);
			this.SetName(isMe, entry, user);
			this.SetProtrait(isMe, user);
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
				this.prizeSprite.gameObject.SetActive(true);
				this.prizeSprite.SpriteName = this.prizeSpriteNames[rank - 1];
			}
			else
			{
				this.prizeSprite.gameObject.SetActive(false);
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

		private void SetProtrait(bool isMe, CloudUser user)
		{
			string text = string.Empty;
			if (user != null && !string.IsNullOrEmpty(user.FacebookId))
			{
				text = user.FacebookId;
			}
			else if (isMe)
			{
				text = ((!this.cloudClient.HasValidUser) ? string.Empty : this.cloudClient.CachedMe.ExternalId);
			}
			if (string.IsNullOrEmpty(text))
			{
				bool flag = false;
				if (isMe && false)
				{
					
					this.nonFBPortraitSprite.gameObject.SetActive(false);
					flag = true;
				}
				if (!flag)
				{
					
				}
			}
			else
			{
				this.nonFBPortraitObject.SetActive(false);
			}
		}

		protected void SetName(string playerName, bool isMe)
		{
			if (isMe)
			{
				this.userName.text = L.Get("You");
				this.userName.fontStyle = this.myFontStyle;
			}
			else
			{
				this.userName.text = playerName;
				if (string.IsNullOrEmpty(this.userName.text))
				{
					this.userName.text = RandomHelper.RandomName(-1);
					this.userName.fontStyle = this.otherFontStyle;
				}
			}
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

		[SerializeField]
		private UILabel positionNumber;

		[SerializeField]
		private GameObject nonFBPortraitObject;

		[SerializeField]
		private UISprite nonFBPortraitSprite;

		[SerializeField]
		private UISprite portraitFrameSprite;

		[SerializeField]
		private UILabel userName;

		[SerializeField]
		private UILabel score;

		[SerializeField]
		private UISprite prizeSprite;

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

		[Header("Font Styles")]
		[SerializeField]
		[OptionalSerializedField]
		private UIFontStyle myFontStyle;

		[SerializeField]
		[OptionalSerializedField]
		private UIFontStyle otherFontStyle;

		private CloudClientBase cloudClient;
	}
}
