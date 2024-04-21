using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using TactileModules.Foundation;
using UnityEngine;

public class HighscoreCell : MonoBehaviour
{
	private FacebookClient FacebookClient
	{
		get
		{
			return ManagerRepository.Get<FacebookClient>();
		}
	}

	private CloudClient CloudClient
	{
		get
		{
			return ManagerRepository.Get<CloudClient>();
		}
	}

	private void Awake()
	{
		this.portraitLoaded = false;
		this.isPortraitLoading = false;
		this.cloudUser = null;
		this.UpdateSpinner();
	}

	private void OnDestroy()
	{
		if (this.fiber != null)
		{
			this.fiber.Terminate();
		}
		this.imageDownloadFiber.Terminate();
	}

	public void Initialize(CloudScore cloudScore, int rank)
	{
		this.cloudScore = cloudScore;
		if (string.IsNullOrEmpty(cloudScore.facebookId))
		{
			this.cloudUser = this.CloudClient.GetUserForCloudId(this.cloudScore.UserId);
		}
		else
		{
			this.lastLoadFacebookId = cloudScore.facebookId;
			this.imageDownloadFiber.Start(this.LoadCr(cloudScore.facebookId));
		}
		if (cloudScore.deviceId == null)
		{
			cloudScore.deviceId = string.Empty;
		}
		this.portraitLoaded = false;
		this.UpdateStats(rank);
	}

	private void UpdateStats(int rank)
	{
		if (this.numberOneRoot != null)
		{
			this.numberOneRoot.SetActive(rank < 2);
		}
		if (this.meRoot != null)
		{
			this.meRoot.SetActive(this.CloudClient.CachedMe != null && this.CloudClient.CachedMe == this.cloudUser);
			if (!this.meRoot.activeSelf)
			{
				string text = (!this.CloudClient.HasValidDevice) ? string.Empty : this.CloudClient.CachedDevice.CloudId;
				if (!string.IsNullOrEmpty(text))
				{
					this.meRoot.SetActive(this.cloudScore.deviceId == text);
				}
			}
		}
		this.scoreLabel.text = L.FormatNumber(this.cloudScore.Score);
		string arg = (this.cloudUser == null) ? ((!this.meRoot.activeSelf) ? L.Get(this.GetRandomName()) : L.Get("You")) : this.cloudUser.GetFirstName(15);
		if (!string.IsNullOrEmpty(this.cloudScore.displayName))
		{
			arg = this.GetFirstName(this.cloudScore.displayName, 15);
		}
		this.nameLabel.text = string.Format("{0}. {1}", rank, arg);
		this.rankLabel.text = string.Empty;
	}

	public void AppearInPanel()
	{
		if (this.cloudUser == null)
		{
			this.portraitQuad.SetTexture(this.GetRandomPortrait());
		}
		else
		{
			this.UpdateSpinner();
			if (!this.portraitLoaded && !this.isPortraitLoading)
			{
				this.isPortraitLoading = true;
				if (this.fiber == null)
				{
					this.fiber = new Fiber(FiberBucket.Update);
				}
				this.fiber.Start(this.LoadPortrait());
			}
		}
	}

	private void UpdateSpinner()
	{
		if (this.cloudUser != null)
		{
			this.loadingPivot.SetActive(this.isPortraitLoading);
		}
		else
		{
			this.loadingPivot.SetActive(false);
		}
	}

	private IEnumerator LoadPortrait()
	{
		yield return this.FacebookClient.LoadPortrait(this.cloudUser.FacebookId, delegate(object error, Texture2D texture)
		{
			if (error == null)
			{
				this.isPortraitLoading = false;
				this.portraitLoaded = true;
				this.portraitQuad.Color = Color.white;
				this.portraitQuad.SetTexture(texture);
			}
			else
			{
				this.isPortraitLoading = false;
				this.portraitLoaded = false;
			}
		});
		this.UpdateSpinner();
		yield break;
	}

	private IEnumerator LoadCr(string facebookId)
	{
		this.loadingPivot.gameObject.SetActive(true);
		if (!string.IsNullOrEmpty(facebookId))
		{
			IEnumerator e = this.LoadImageFromFacebookIdCr(facebookId, delegate(object err)
			{
			});
			while (e.MoveNext())
			{
				object obj = e.Current;
				yield return obj;
			}
		}
		if (this.loadingPivot != null)
		{
			this.loadingPivot.gameObject.SetActive(false);
		}
		yield break;
	}

	private IEnumerator LoadImageFromFacebookIdCr(string facebookId, Action<object> callback)
	{
		IEnumerator e = this.FacebookClient.LoadPortrait(facebookId, delegate(object error, Texture2D tex)
		{
			if (tex != null)
			{
				if (facebookId == this.lastLoadFacebookId)
				{
					this.portraitQuad.SetTexture(tex);
				}
			}
			else
			{
				string arg = "Failed to load facebook portrait for " + facebookId;
				if (error != null)
				{
					arg = arg + ". Error: " + error;
				}
			}
			callback(error);
		});
		while (e.MoveNext())
		{
			object obj = e.Current;
			yield return obj;
		}
		yield break;
	}

	public string GetFirstName(string name, int maxLength = 20)
	{
		string[] array = name.Split(new string[]
		{
			" "
		}, StringSplitOptions.None);
		string text = (array.Length <= 0) ? name : array[0];
		return text.Substring(0, Mathf.Min(text.Length, maxLength));
	}

	public string GetRandomName()
	{
		int hashCode = this.cloudScore.deviceId.GetHashCode();
		int value = hashCode % this.nameList.Length;
		return this.nameList[Mathf.Abs(value)];
	}

	private Texture2D GetRandomPortrait()
	{
		int hashCode = this.cloudScore.deviceId.GetHashCode();
		int value = hashCode % this.randomPortraitTextures.Count;
		return this.randomPortraitTextures[Mathf.Abs(value)];
	}

	public UILabel scoreLabel;

	public UILabel nameLabel;

	public UITextureQuad portraitQuad;

	public GameObject loadingPivot;

	public GameObject numberOneRoot;

	public GameObject meRoot;

	public UILabel rankLabel;

	public List<Texture2D> randomPortraitTextures;

	private CloudScore cloudScore;

	private CloudUser cloudUser;

	private bool portraitLoaded;

	private bool isPortraitLoading;

	private Fiber fiber;

	private Fiber imageDownloadFiber = new Fiber();

	private string lastLoadFacebookId;

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
