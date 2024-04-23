using System;
using System.Collections;
using System.Collections.Generic;
using TactileModules.Validation;
using UnityEngine;

public class MapAvatar : MonoBehaviour
{
	public CloudUser CloudUser { get; private set; }

	public string DeviceId { get; private set; }

	public Transform FramePivot
	{
		get
		{
			return this.pivot;
		}
	}

	private void Awake()
	{
		if (this.vipPivot != null)
		{
			this.SetVIP(false);
		}
	}

	public void Initialize(MapAvatar.IDataRetriever dataRetriever, CloudUser cloudUser, string deviceId = null)
	{
		this.retriever = dataRetriever;
		this.CloudUser = cloudUser;
		this.DeviceId = deviceId;
		this.UpdatePortrait();
		this.SetVIP((!this.IsMe()) ? dataRetriever.GetVIPStatusForCloudUser(this.CloudUser) : dataRetriever.IsPlayerVIP);
	}

	private void OnEnable()
	{
		this.portrait.progress.gameObject.SetActive(false);
	}

	protected bool IsMe()
	{
		return this.retriever.GetCloudDeviceId != null && this.retriever.GetCloudDeviceId == this.DeviceId;
	}

	public MapAvatar.BackgroundSide GetSideFromLocalPosition(Vector3 localPosition)
	{
		return this.retriever.DetermineSideFromLocalPosition(localPosition);
	}

	public void UpdateFromLocalPosition(Vector3 localPosition)
	{
		base.transform.localPosition = localPosition;
		MapAvatar.BackgroundSide sideFromLocalPosition = this.GetSideFromLocalPosition(localPosition);
		this.SetPivotPositionFromSide(sideFromLocalPosition);
		this.SetFrameFromSide(sideFromLocalPosition);
	}

	public IEnumerator AnimateFramePivotToNewSide(MapAvatar.BackgroundSide newBackgroundSide)
	{
		this.SetFrameFromSide(MapAvatar.BackgroundSide.None);
		Vector3 newPivotLocalPos = this.GetLocalPosForPivot(newBackgroundSide);
		if (this.pivot != null)
		{
			yield return FiberAnimation.MoveLocalTransform(this.pivot, this.pivot.localPosition, newPivotLocalPos, null, 0.2f);
		}
		this.SetFrameFromSide(newBackgroundSide);
		yield break;
	}

	public IEnumerator AnimateFramePivotToDefaultSide()
	{
		MapAvatar.BackgroundSide newSide = this.GetSideFromLocalPosition(base.transform.localPosition);
		yield return this.AnimateFramePivotToNewSide(newSide);
		yield break;
	}

	public IEnumerator AnimateToLocalPosition(Vector3 newLocalPosition, AnimationCurve curve)
	{
		MapAvatar.BackgroundSide newSide = this.GetSideFromLocalPosition(newLocalPosition);
		Vector3 newPivotLocalPos = this.GetLocalPosForPivot(newSide);
		this.SetFrameFromSide(MapAvatar.BackgroundSide.None);
		List<IEnumerator> animList = new List<IEnumerator>();
		animList.Add(FiberAnimation.MoveLocalTransform(base.transform, base.transform.localPosition, newLocalPosition, curve, 0f));
		if (this.pivot != null)
		{
			animList.Add(FiberAnimation.MoveLocalTransform(this.pivot, this.pivot.localPosition, newPivotLocalPos, curve, 0f));
		}
		yield return FiberHelper.RunParallel(animList);
		this.SetFrameFromSide(newSide);
		yield break;
	}

	public virtual void UpdatePortrait()
	{
		if (this.retriever == null)
		{
			return;
		}
		if (this.retriever.FacebookClient.IsSessionValid)
		{
			if (this.IsMe())
			{
				FacebookUser cachedMe = this.retriever.FacebookClient.CachedMe;
				if (cachedMe != null)
				{
					this.portrait.Load(this.retriever.FacebookClient, cachedMe.Id, null);
				}
			}
			else if (this.CloudUser != null)
			{
				this.portrait.Load(this.retriever.FacebookClient, this.CloudUser.FacebookId, null);
			}
			else
			{
				this.portrait.Reset();
			}
		}
		else
		{
			this.portrait.Reset();
		}
	}

	public void AddAdditionalIcon(string iconType, GameObject iconObjectPrefab)
	{
		if (!this.additionalIcons.ContainsKey(iconType))
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(iconObjectPrefab);
			gameObject.SetLayerRecursively(base.gameObject.layer);
			gameObject.transform.SetParent(this.additionalIconsLayout.transform, false);
			this.additionalIcons.Add(iconType, gameObject);
		}
		this.additionalIconsLayout.Layout();
	}

	public void RemoveAdditionalIcon(string iconType)
	{
		if (this.additionalIcons.ContainsKey(iconType))
		{
			UnityEngine.Object.Destroy(this.additionalIcons[iconType]);
			this.additionalIcons.Remove(iconType);
		}
	}

	public void ClearAllAdditionalIcons()
	{
		foreach (KeyValuePair<string, GameObject> keyValuePair in this.additionalIcons)
		{
			UnityEngine.Object.Destroy(keyValuePair.Value);
		}
		this.additionalIcons.Clear();
	}

	public void SetVIP(bool isVIP)
	{
		if (this.vipPivot == null)
		{
			return;
		}
		if (this.vipPivot.activeSelf != isVIP)
		{
			this.vipPivot.SetActive(isVIP);
		}
	}

	public void SetFrameFromSide(MapAvatar.BackgroundSide side)
	{
		if (this.backgroundLeft != null)
		{
			this.backgroundLeft.SetActive(side == MapAvatar.BackgroundSide.Left);
		}
		if (this.backgroundRight != null)
		{
			this.backgroundRight.SetActive(side == MapAvatar.BackgroundSide.Right);
		}
		if (this.backgroundNone != null)
		{
			this.backgroundNone.SetActive(side == MapAvatar.BackgroundSide.None);
		}
	}

	public void SetPivotPositionFromSide(MapAvatar.BackgroundSide side)
	{
		if (this.pivot != null)
		{
			this.pivot.localPosition = this.GetLocalPosForPivot(side);
		}
	}

	private Vector3 GetLocalPosForPivot(MapAvatar.BackgroundSide side)
	{
		if (this.pivot != null)
		{
			float num = 0f;
			if (side == MapAvatar.BackgroundSide.Left)
			{
				num = -0.5f;
			}
			else if (side == MapAvatar.BackgroundSide.Right)
			{
				num = 0.5f;
			}
			Vector3 localPosition = this.pivot.localPosition;
			localPosition.x = (this.GetElementSize().x + this.pivot.GetElementSize().x) * num;
			return localPosition;
		}
		return Vector3.zero;
	}

	public FacebookPortraitWithProgress portrait;

	[OptionalSerializedField]
	public GameObject backgroundLeft;

	[OptionalSerializedField]
	public GameObject backgroundRight;

	[OptionalSerializedField]
	public GameObject backgroundNone;

	[OptionalSerializedField]
	public GameObject vipPivot;

	[SerializeField]
	private Transform pivot;

	[Header("Additional Icons")]
	[SerializeField]
	[OptionalSerializedField]
	private UIAdvancedFlowLayout additionalIconsLayout;

	protected MapAvatar.IDataRetriever retriever;

	private Dictionary<string, GameObject> additionalIcons = new Dictionary<string, GameObject>();

	public enum BackgroundSide
	{
		Left,
		None,
		Right
	}

	public interface IDataRetriever
	{
		CloudUser GetCachedMe { get; }

		string GetCloudDeviceId { get; }

		bool IsPlayerVIP { get; }

		bool GetVIPStatusForCloudUser(CloudUser cloudUser);

		FacebookClient FacebookClient { get; }

		MapAvatar.BackgroundSide DetermineSideFromLocalPosition(Vector3 avatarLocalPosition);
	}
}
