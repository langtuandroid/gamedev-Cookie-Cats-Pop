using System;
using TactileModules.Foundation;
using UnityEngine;

public class TournamentLeaderboardItem : MonoBehaviour
{
	public void Init(int position, TournamentCloudManager.Score scoreInfo, CloudClientBase cloudClient)
	{
		this.scoreInfo = scoreInfo;
		this.hatIcon.gameObject.SetActive(false);
		this.positionNumber.text = "#" + position.ToString();
		this.userName.text = scoreInfo.FirstName;
		this.startingColor = this.BG.Color;
		this.score.text = L.FormatNumber(scoreInfo.score);
		if (this.meRoot != null)
		{
			string deviceId = (!cloudClient.HasValidDevice) ? string.Empty : cloudClient.CachedDevice.CloudId;
			string userId = (!cloudClient.HasValidUser) ? string.Empty : cloudClient.CachedMe.CloudId;
			bool active = scoreInfo.IsOwnedByDeviceOrUser(deviceId, userId);
			this.isMe = active;
			this.meRoot.SetActive(active);
		}
		TournamentSetup.RankSetup rankSetup = SingletonAsset<TournamentSetup>.Instance.GetRankSetup(TournamentRank.Bronze + position - 1);
		UIFontStyle fontStyle = rankSetup.fontStyle;
		if (fontStyle)
		{
			this.positionNumber.fontStyle = fontStyle;
		}
	}

	public void AppearInPanel()
	{
		if (!string.IsNullOrEmpty(this.scoreInfo.facebookId))
		{
			this.nonFBPortraitObject.SetActive(false);
		}
		else
		{
			bool flag = false;
			if (this.isMe && false)
			{
				this.nonFBPortraitTexture.gameObject.SetActive(false);
				flag = true;
			}
			if (!flag)
			{
				this.nonFBPortraitTexture.SetTexture(SingletonAsset<TournamentSetup>.Instance.GetRandomPortrait(this.scoreInfo.deviceId.GetHashCode()));
			}
		}
		if (this.isMe)
		{
			this.userName.text = L.Get("You");
			this.BG.Color = this.YouColor;
		}
		else
		{
			this.BG.Color = this.startingColor;
			if (string.IsNullOrEmpty(this.userName.text))
			{
				this.userName.text = this.GetRandomName(this.scoreInfo);
			}
		}
	}

	private void UpdateFirstPlaceHat()
	{
	}

	public string GetRandomName(TournamentCloudManager.Score scoreInfo)
	{
		int hashCode = scoreInfo.deviceId.GetHashCode();
		int value = hashCode % this.nameList.Length;
		return this.nameList[Mathf.Abs(value)];
	}

	public Color YouColor;

	public UISprite BG;

	public UILabel positionNumber;

	public GameObject nonFBPortraitObject;

	public UITextureQuad nonFBPortraitTexture;

	public UILabel userName;

	public UILabel score;

	public UISprite hatIcon;

	public GameObject meRoot;

	private TournamentCloudManager.Score scoreInfo;

	private bool isMe;

	private Color startingColor;

	private CloudClientBase cloudClient;

	private string[] nameList = new string[]
	{
		"Sarah",
		"Jacob",
		"Jennifer",
		"Keith",
		"Sally",
		"Eric",
		"Connor",
		"Madeleine",
		"Sam",
		"Una",
		"Steven",
		"Lillian",
		"Claire",
		"Luke",
		"Heather",
		"Ruth",
		"Harry",
		"Austin",
		"Steven",
		"Joan",
		"Luke",
		"Diana",
		"Carl",
		"Adam",
		"Brian",
		"Richard",
		"Cameron",
		"Jane",
		"Christian",
		"Trevor",
		"Sebastian",
		"Frank",
		"Liam",
		"Diana",
		"Arya",
		"Sally",
		"Julian",
		"Natalie",
		"Molly",
		"Ruth",
		"Jason",
		"Grace",
		"Faith",
		"Max",
		"Fiona",
		"Vladimir",
		"Diana",
		"Stephen",
		"Colin",
		"Keith",
		"Emma",
		"David",
		"Sally",
		"Colin",
		"Warren",
		"Alexander",
		"Lucas",
		"Wendy",
		"Nathan",
		"Lillian",
		"Anne",
		"Neil",
		"Sally",
		"Brandon",
		"Caroline",
		"Amelia",
		"Ruth",
		"Ryan",
		"Wanda",
		"Leah",
		"Jason",
		"Alexandra",
		"Jasmine",
		"Bella",
		"Nathan",
		"Sam",
		"Anna",
		"Matt",
		"Brandon",
		"Warren",
		"Oliver",
		"Sue",
		"Karen",
		"Dharma",
		"Rebecca",
		"Rachel",
		"Sean",
		"Suzy",
		"Amelia",
		"Oliver",
		"Benjamin",
		"Elizabeth",
		"Diana",
		"Eleanor",
		"Madonna",
		"Lisa",
		"Dylan",
		"John",
		"Gordon",
		"Jason",
		"Dorothy",
		"Amanda",
		"Dorothy",
		"Bella",
		"Christian",
		"Wanda",
		"Sarah",
		"Wanda",
		"Pippa",
		"Karen",
		"Matt",
		"Angela",
		"Amelia",
		"Michelle",
		"Karen",
		"Pippa",
		"Penelope",
		"Max",
		"Benjamin",
		"Joseph",
		"Line",
		"Alexander",
		"Michael",
		"Heather",
		"Stewart",
		"Michael",
		"Boris",
		"Dorothy",
		"Sam",
		"Wen",
		"Faith",
		"Jennifer",
		"Henrik",
		"Sofie",
		"Tracey",
		"Lauren",
		"Yvonne",
		"Chloe",
		"Alison",
		"Lucas",
		"Chloe",
		"Stewart",
		"Nicola",
		"Jane",
		"John",
		"Thomas",
		"Cameron",
		"Claire",
		"Joan",
		"Claire",
		"Jan",
		"Hannah",
		"Connor",
		"Abigail",
		"Heather",
		"Owen",
		"Emma",
		"Joshua",
		"Wanda",
		"Rachel",
		"Amanda",
		"Alexander",
		"Eric",
		"Dylan",
		"Andrew",
		"Trevor",
		"Robert",
		"Eric",
		"Brian",
		"Jan",
		"Pippa",
		"Brandon",
		"Dan",
		"Amelia",
		"Lily",
		"Nathan",
		"Rebecca",
		"Sam",
		"Hannah",
		"Carol",
		"Bernadette",
		"Jake",
		"Jennifer",
		"Stephen",
		"Olivia",
		"Nathan",
		"David",
		"Jack",
		"Colin",
		"Dan",
		"Mary",
		"Sue",
		"Dan",
		"Michael",
		"Chloe",
		"Lillian",
		"Maria",
		"Leonard",
		"William"
	};
}
