using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Fibers;
using Tactile;
using UnityEngine;

namespace TactileModules.SagaCore
{
	public class SagaAvatarController : MapAvatar.IDataRetriever
	{
		public SagaAvatarController(CloudClient cloudClient, MapFacade mapFacade)
		{
			this.cloudClient = cloudClient;
			this.mapFacade = mapFacade;
		}

		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<SagaAvatarController> AvatarsChanged = delegate (SagaAvatarController A_0)
        {
        };



        public Dictionary<string, MapAvatar> Avatars
		{
			get
			{
				return this.avatars;
			}
		}

		public Transform AvatarRoot
		{
			get
			{
				return this.avatarRoot;
			}
		}

		public FlashAtCurrentLevel ProgressMarker
		{
			get
			{
				return this.progressMarker;
			}
			set
			{
				this.progressMarker = value;
			}
		}

		public void AttachToDotProvider(SagaAvatarController.IDotPositions dotPositionsProvider, SagaAvatarController.IAvatarInfoProvider provider, float zOffset, Rect visualBounds)
		{
			this.provider = provider;
			this.dotPositionsProvider = dotPositionsProvider;
			this.visualBounds = visualBounds;
			this.avatarRoot = new GameObject("AvatarsRoot").transform;
			this.avatarRoot.parent = dotPositionsProvider.ContentRoot;
			this.avatarRoot.localPosition = Vector3.forward * zOffset;
			this.avatarRoot.localScale = Vector3.one;
			this.avatarRoot.localRotation = Quaternion.identity;
			this.avatarRoot.gameObject.SetLayerRecursively(dotPositionsProvider.ContentRoot.gameObject.layer);
			if (this.currentAvatarState == null)
			{
				this.GetCombinedProviderData(out this.currentAvatarState, out this.currentProgressPosition);
			}
			this.RebuildAvatarsInstantlyFromCurrentState();
			this.RebuildProgressionMarker();
		}

		public void DetachFromDotProvider()
		{
			this.ClearAvatars();
			if (this.avatarRoot != null)
			{
				UnityEngine.Object.Destroy(this.avatarRoot.gameObject);
			}
			this.dotPositionsProvider = null;
			this.provider = null;
			this.AvatarsChanged(this);
		}

		public void GetCombinedProviderData(out Dictionary<string, SagaAvatarInfo> result, out int progressMarkerPosition)
		{
			result = new Dictionary<string, SagaAvatarInfo>();
			if (this.provider != null)
			{
				progressMarkerPosition = this.provider.GetProgressMarkerDotIndex();
				SagaAvatarInfo sagaAvatarInfo = this.provider.CreateMeAvatarInfo();
				if (sagaAvatarInfo != null)
				{
					result["me"] = sagaAvatarInfo;
				}
				Dictionary<CloudUser, SagaAvatarInfo> dictionary = this.provider.CreateFriendsAvatarInfos();
				if (dictionary != null)
				{
					foreach (KeyValuePair<CloudUser, SagaAvatarInfo> keyValuePair in dictionary)
					{
						result[keyValuePair.Key.CloudId] = keyValuePair.Value;
					}
				}
			}
			else
			{
				progressMarkerPosition = -1;
			}
		}

		public void Refresh()
		{
			if (this.avatarsIsAnimating)
			{
				return;
			}
			this.GetCombinedProviderData(out this.currentAvatarState, out this.currentProgressPosition);
			this.RebuildAvatarsInstantlyFromCurrentState();
			this.RebuildProgressionMarker();
		}

		public MapAvatar GetMeAvatar()
		{
			return this.avatars.GetValueOrDefault("me");
		}

		private void ClearAvatars()
		{
			foreach (KeyValuePair<string, MapAvatar> keyValuePair in this.avatars)
			{
				if (keyValuePair.Value != null)
				{
					UnityEngine.Object.Destroy(keyValuePair.Value.gameObject);
				}
			}
			this.avatars.Clear();
		}

		private void RebuildAvatarsInstantlyFromCurrentState()
		{
			this.ClearAvatars();
			foreach (KeyValuePair<string, SagaAvatarInfo> keyValuePair in this.currentAvatarState)
			{
				this.ConstructAvatar(keyValuePair.Key, keyValuePair.Value);
				this.UpdateAvatarPosition(keyValuePair.Key);
			}
			this.AvatarsChanged(this);
		}

		private void RebuildProgressionMarker()
		{
			if (this.progressMarker != null)
			{
				UnityEngine.Object.Destroy(this.progressMarker.gameObject);
			}
			this.progressMarker = UnityEngine.Object.Instantiate<FlashAtCurrentLevel>(this.mapFacade.GetDefaultDotFlasherPrefab());
			this.progressMarker.transform.parent = this.avatarRoot;
			this.progressMarker.gameObject.SetLayerRecursively(this.avatarRoot.gameObject.layer);
			this.UpdateProgressionMarkerPosition();
		}

		private void UpdateProgressionMarkerPosition()
		{
			Vector3 localPosition;
			if (this.currentProgressPosition >= 0 && this.dotPositionsProvider.TryGetDotPosition(this.currentProgressPosition, out localPosition))
			{
				this.progressMarker.transform.localPosition = localPosition;
				this.progressMarker.gameObject.SetActive(true);
			}
			else
			{
				this.progressMarker.gameObject.SetActive(false);
			}
		}

		private List<StateChange> GetMovingChangesBetweenStates(Dictionary<string, SagaAvatarInfo> newState)
		{
			List<StateChange> list = new List<StateChange>();
			foreach (KeyValuePair<string, SagaAvatarInfo> keyValuePair in newState)
			{
				if (this.currentAvatarState.ContainsKey(keyValuePair.Key))
				{
					StateChange stateChange = new StateChange
					{
						id = keyValuePair.Key,
						oldDotIndex = this.currentAvatarState[keyValuePair.Key].dotIndex,
						newDotIndex = newState[keyValuePair.Key].dotIndex
					};
					if (stateChange.IsMoving)
					{
						list.Add(stateChange);
					}
				}
			}
			return list;
		}

		public bool AnyAvatarsNeedMoving()
		{
			Dictionary<string, SagaAvatarInfo> newState;
			int num;
			this.GetCombinedProviderData(out newState, out num);
			List<StateChange> movingChangesBetweenStates = this.GetMovingChangesBetweenStates(newState);
			return movingChangesBetweenStates.Count > 0;
		}

		public IEnumerator AnimateProgressIfAny()
		{
			this.avatarsIsAnimating = true;
			UICamera.DisableInput();
			if (SagaAvatarController._003C_003Ef__mg_0024cache0 == null)
			{
				SagaAvatarController._003C_003Ef__mg_0024cache0 = new Fiber.OnExitHandler(UICamera.EnableInput);
			}
			yield return new Fiber.OnExit(SagaAvatarController._003C_003Ef__mg_0024cache0);
			Dictionary<string, SagaAvatarInfo> newState;
			this.GetCombinedProviderData(out newState, out this.currentProgressPosition);
			this.UpdateProgressionMarkerPosition();
			this.progressMarker.gameObject.SetActive(false);
			List<StateChange> changeList = this.GetMovingChangesBetweenStates(newState);
			foreach (StateChange change in changeList)
			{
				if (change.IsMoving && this.avatars.ContainsKey(change.id))
				{
					yield return this.MoveAvatar(this.avatars[change.id], change.oldDotIndex, change.newDotIndex);
				}
			}
			this.progressMarker.gameObject.SetActive(true);
			this.currentAvatarState = newState;
			this.avatarsIsAnimating = false;
			yield break;
		}

		public IEnumerator MoveAvatar(string avatarId, int fromDot, int toDot)
		{
			yield return this.MoveAvatar(this.avatars[avatarId], fromDot, toDot);
			yield break;
		}

		public IEnumerator MoveAvatar(string avatarId, int fromDot, int toDot, SagaAvatarController.AvatarMoveAnimation customAnimation)
		{
			yield return this.MoveAvatar(this.avatars[avatarId], fromDot, toDot, customAnimation);
			yield break;
		}

		private IEnumerator MoveAvatar(MapAvatar avatar, int fromDot, int toDot)
		{
			yield return this.MoveAvatar(avatar, fromDot, toDot, new SagaAvatarController.AvatarMoveAnimation(this.DefaultMoveAnimation));
			yield break;
		}

		private IEnumerator DefaultMoveAnimation(MapAvatar avatar, Vector3 fromPos, Vector3 toPos)
		{
			avatar.UpdateFromLocalPosition(fromPos);
			yield return avatar.AnimateToLocalPosition(toPos, this.mapFacade.Setup.AvatarMoveCurve);
			yield break;
		}

		private IEnumerator MoveAvatar(MapAvatar avatar, int fromDot, int toDot, SagaAvatarController.AvatarMoveAnimation customAnimation)
		{
			Vector3 sourcePos;
			this.dotPositionsProvider.TryGetDotPosition(fromDot, out sourcePos);
			Vector3 destPos;
			this.dotPositionsProvider.TryGetDotPosition(toDot, out destPos);
			yield return customAnimation(avatar, sourcePos, destPos);
			yield break;
		}

		private void ConstructAvatar(string cloudId, SagaAvatarInfo info)
		{
			MapAvatar mapAvatar = info.visualPrefab;
			if (mapAvatar == null)
			{
				if (cloudId == "me")
				{
					mapAvatar = this.mapFacade.GetDefaultMeAvatarPrefab();
				}
				else
				{
					mapAvatar = this.mapFacade.GetDefaultFriendAvatarPrefab();
				}
			}
			MapAvatar mapAvatar2 = UnityEngine.Object.Instantiate<MapAvatar>(mapAvatar);
			mapAvatar2.transform.parent = this.avatarRoot;
			mapAvatar2.gameObject.SetLayerRecursively(this.avatarRoot.gameObject.layer);
			string deviceId = (!(cloudId == "me")) ? null : ((MapAvatar.IDataRetriever)this).GetCloudDeviceId;
			CloudUser userForCloudId = this.cloudClient.GetUserForCloudId(cloudId);
			mapAvatar2.Initialize(this, userForCloudId, deviceId);
			this.avatars.Add(cloudId, mapAvatar2);
		}

		private void UpdateAvatarPosition(string id)
		{
			MapAvatar mapAvatar = this.avatars[id];
			int dotIndex = this.currentAvatarState[id].dotIndex;
			Vector3 localPosition;
			if (this.dotPositionsProvider.TryGetDotPosition(dotIndex, out localPosition))
			{
				mapAvatar.UpdateFromLocalPosition(localPosition);
			}
		}

		CloudUser MapAvatar.IDataRetriever.GetCachedMe
		{
			get
			{
				return this.cloudClient.CachedMe;
			}
		}

		string MapAvatar.IDataRetriever.GetCloudDeviceId
		{
			get
			{
				return (!this.cloudClient.HasValidDevice) ? string.Empty : this.cloudClient.CachedDevice.CloudId;
			}
		}

		public bool IsPlayerVIP { get; }

		bool MapAvatar.IDataRetriever.GetVIPStatusForCloudUser(CloudUser cloudUser)
		{
			if (cloudUser == null)
			{
				return false;
			}
			PublicUserSettings friend = UserSettingsManager.GetFriend<PublicUserSettings>(cloudUser);
			return friend != null && friend.UserIsVip;
		}

		


		MapAvatar.BackgroundSide MapAvatar.IDataRetriever.DetermineSideFromLocalPosition(Vector3 avatarLocalPosition)
		{
			if (this.visualBounds == Rect.zero)
			{
				return MapAvatar.BackgroundSide.None;
			}
			float num = avatarLocalPosition.x - this.visualBounds.center.x;
			return (num <= 0f) ? MapAvatar.BackgroundSide.Right : MapAvatar.BackgroundSide.Left;
		}

		public const string ME_ID = "me";

		private readonly CloudClient cloudClient;

		private readonly MapFacade mapFacade;

		private SagaAvatarController.IDotPositions dotPositionsProvider;

		private Dictionary<string, SagaAvatarInfo> currentAvatarState;

		private int currentProgressPosition;

		private SagaAvatarController.IAvatarInfoProvider provider;

		private readonly Dictionary<string, MapAvatar> avatars = new Dictionary<string, MapAvatar>();

		private Transform avatarRoot;

		private FlashAtCurrentLevel progressMarker;

		private Rect visualBounds;

		private bool avatarsIsAnimating;

		[CompilerGenerated]
		private static Fiber.OnExitHandler _003C_003Ef__mg_0024cache0;

		public delegate Dictionary<string, SagaAvatarInfo> AvatarInfoDelegate();

		public delegate IEnumerator AvatarMoveAnimation(MapAvatar avatar, Vector3 fromPos, Vector3 toPos);

		public interface IDotPositions
		{
			Transform ContentRoot { get; }

			bool TryGetDotPosition(int dotIndex, out Vector3 position);
		}

		public interface IAvatarInfoProvider
		{
			SagaAvatarInfo CreateMeAvatarInfo();

			Dictionary<CloudUser, SagaAvatarInfo> CreateFriendsAvatarInfos();

			int GetProgressMarkerDotIndex();
		}
	}
}
