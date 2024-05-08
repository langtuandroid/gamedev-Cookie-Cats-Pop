using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class MapAvatarController : MonoBehaviour
{
	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<MapAvatare> OnPlayerAvatarUpdated;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<MapAvatare> OnOtherAvatarUpdated;

	public void UpdateAllAvatars()
	{
	}

	public void Initialize(MapAvatarController.IDataRetriever dataRetriever)
	{
	}

	public IEnumerator MoveAvatar(LevelProxy source, LevelProxy dest, MapAvatare avatar)
	{
		yield break;
	}

	public void UpdatePlayerAvatarPosition(int overrideIndex = -1)
	{
	}

	public void LockPlayerAvatarPositionUntilAnimation()
	{
	}

	public void UpdateOtherAvatars(int overrideIndex = -1)
	{
	}

	public void UpdatePlayerAvatar(int overrideIndex = -1)
	{
	}

	public MapAvatare playerAvatar
	{
		get
		{
			return null;
		}
	}

	public void SetFlashActive(bool active)
	{
	}

	public void SetPlayerAvatarActive(bool active)
	{
	}

	public const float FriendAvatarZOffset = -150f;

	public interface IDataRetriever
	{
	}

	public interface IAvatarsInfoProvider
	{
	}
}
