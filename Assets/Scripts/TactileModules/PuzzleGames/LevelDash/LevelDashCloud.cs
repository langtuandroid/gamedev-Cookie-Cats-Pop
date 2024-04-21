using System;
using System.Collections;
using System.Collections.Generic;
using Cloud;
using Fibers;
using JetBrains.Annotations;
using TactileModules.TactileCloud;

namespace TactileModules.PuzzleGames.LevelDash
{
	public class LevelDashCloud
	{
		public LevelDashCloud([NotNull] CloudClientBase cloudClientBase, [NotNull] ICloudInterfaceBase cloudInterfaceBase)
		{
			if (cloudClientBase == null)
			{
				throw new ArgumentNullException("cloudClientBase");
			}
			if (cloudInterfaceBase == null)
			{
				throw new ArgumentNullException("cloudInterfaceBase");
			}
			this.cloudClientBase = cloudClientBase;
			this.cloudInterfaceBase = cloudInterfaceBase;
		}

		public IEnumerator SendJoinRequest(string requestedFeatureId, int completedLevelHumanNumber, LevelDashCloud.LevelDashUpdateResultDelegate responseCallback)
		{
			if (this.hasSentJoinRequest)
			{
				yield break;
			}
			yield return new Fiber.OnExit(delegate()
			{
				this.hasSentJoinRequest = false;
			});
			this.hasSentJoinRequest = true;
			if (!this.HasValidDevice)
			{
				responseCallback(ReturnCode.MissingDeviceInContext, string.Empty, null, null, null);
				yield break;
			}
			LevelDashCloud.LevelDashResponse response = new LevelDashCloud.LevelDashResponse();
			yield return this.cloudInterfaceBase.LevelDashJoinRequest((!this.HasValidUser) ? null : this.CachedMe.CloudId, requestedFeatureId, completedLevelHumanNumber, response);
			if (response.Success)
			{
				responseCallback(null, response.FeatureId, response.Entry, response.Entries, response.Users);
			}
			else
			{
				responseCallback(response.ReturnCode, string.Empty, null, null, null);
			}
			yield break;
		}

		public IEnumerator SendGetStatusRequest(string requestedFeatureId, LevelDashCloud.LevelDashUpdateResultDelegate responseCallback)
		{
			if (this.hasSentStatusRequest)
			{
				yield break;
			}
			yield return new Fiber.OnExit(delegate()
			{
				this.hasSentStatusRequest = false;
			});
			this.hasSentStatusRequest = true;
			if (!this.HasValidDevice)
			{
				responseCallback(ReturnCode.MissingDeviceInContext, string.Empty, null, null, null);
				yield break;
			}
			LevelDashCloud.LevelDashResponse response = new LevelDashCloud.LevelDashResponse();
			yield return this.cloudInterfaceBase.LevelDashStatusRequest((!this.HasValidUser) ? null : this.CachedMe.CloudId, requestedFeatureId, RequestPriority.Interactive, response);
			if (response.Success)
			{
				responseCallback(null, response.FeatureId, response.Entry, response.Entries, response.Users);
			}
			else
			{
				responseCallback(response.ReturnCode, string.Empty, null, null, null);
			}
			yield break;
		}

		public IEnumerator SendSubmitScore(string requestedFeatureId, int completedLevelHumanNumber, LevelDashCloud.LevelDashSubmitScoreResultDelegate responseCallback)
		{
			if (this.hasSentSubmitScoreRequest)
			{
				yield break;
			}
			yield return new Fiber.OnExit(delegate()
			{
				this.hasSentSubmitScoreRequest = false;
			});
			this.hasSentSubmitScoreRequest = true;
			if (!this.HasValidDevice)
			{
				responseCallback(ReturnCode.MissingDeviceInContext, string.Empty);
				yield break;
			}
			LevelDashCloud.LevelDashResponse response = new LevelDashCloud.LevelDashResponse();
			yield return this.cloudInterfaceBase.LevelDashSubmitScoreRequest((!this.HasValidUser) ? null : this.CachedMe.CloudId, requestedFeatureId, completedLevelHumanNumber, RequestPriority.Interactive, response);
			if (response.Success)
			{
				responseCallback(null, response.FeatureId);
			}
			else
			{
				responseCallback(response.ReturnCode, string.Empty);
			}
			yield break;
		}

		public IEnumerator SendGetRewardStatusRequest(string requestedFeatureId, LevelDashCloud.LevelDashGetRewardStatusResultDelgate responseCallback)
		{
			if (this.hasSentRewardStatusRequest)
			{
				yield break;
			}
			yield return new Fiber.OnExit(delegate()
			{
				this.hasSentRewardStatusRequest = false;
			});
			this.hasSentRewardStatusRequest = true;
			if (!this.HasValidDevice)
			{
				responseCallback(ReturnCode.MissingDeviceInContext, RewardStatus.Unknown, null, null);
				yield break;
			}
			LevelDashCloud.LevelDashResponse response = new LevelDashCloud.LevelDashResponse();
			yield return this.cloudInterfaceBase.LevelDashGetRewardRequest((!this.HasValidUser) ? null : this.CachedMe.CloudId, requestedFeatureId, RequestPriority.Interactive, response);
			if (response.Success)
			{
				responseCallback(null, response.RewardStatus, response.Entries, response.Users);
			}
			else
			{
				responseCallback(response.ReturnCode, RewardStatus.Unknown, null, null);
			}
			yield break;
		}

		private bool HasValidDevice
		{
			get
			{
				return this.cloudClientBase.HasValidDevice;
			}
		}

		private bool HasValidUser
		{
			get
			{
				return this.cloudClientBase.HasValidUser;
			}
		}

		private CloudUser CachedMe
		{
			get
			{
				return this.cloudClientBase.CachedMe;
			}
		}

		private CloudDevice CachedDevice
		{
			get
			{
				return this.cloudClientBase.CachedDevice;
			}
		}

		private CloudClientBase cloudClientBase;

		private ICloudInterfaceBase cloudInterfaceBase;

		private bool hasSentJoinRequest;

		private bool hasSentStatusRequest;

		private bool hasSentSubmitScoreRequest;

		private bool hasSentRewardStatusRequest;

		public delegate void LevelDashUpdateResultDelegate(object error, string featureId, Entry entry, List<Entry> entries, List<CloudUser> users);

		public delegate void LevelDashSubmitScoreResultDelegate(object error, string featureId);

		public delegate void LevelDashGetRewardStatusResultDelgate(object error, RewardStatus rewardStatus, List<Entry> entries, List<CloudUser> users);

		private class LevelDashResponse : Response
		{
			public Entry Entry
			{
				get
				{
					return JsonSerializer.HashtableToObject<Entry>((Hashtable)base.data["entry"]);
				}
			}

			public List<Entry> Entries
			{
				get
				{
					Hashtable hashtable = (Hashtable)base.data["competitors"];
					if (hashtable == null || hashtable.Count <= 0)
					{
						return null;
					}
					List<Entry> list = new List<Entry>();
					ArrayList arrayList = (ArrayList)hashtable["entries"];
					IEnumerator enumerator = arrayList.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							object obj = enumerator.Current;
							Hashtable table = (Hashtable)obj;
							list.Add(JsonSerializer.HashtableToObject<Entry>(table));
						}
					}
					finally
					{
						IDisposable disposable;
						if ((disposable = (enumerator as IDisposable)) != null)
						{
							disposable.Dispose();
						}
					}
					return list;
				}
			}

			public List<CloudUser> Users
			{
				get
				{
					Hashtable hashtable = (Hashtable)base.data["competitors"];
					if (hashtable == null || hashtable.Count <= 0)
					{
						return null;
					}
					List<CloudUser> list = new List<CloudUser>();
					ArrayList arrayList = (ArrayList)hashtable["users"];
					IEnumerator enumerator = arrayList.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							object obj = enumerator.Current;
							Hashtable table = (Hashtable)obj;
							list.Add(JsonSerializer.HashtableToObject<CloudUser>(table));
						}
					}
					finally
					{
						IDisposable disposable;
						if ((disposable = (enumerator as IDisposable)) != null)
						{
							disposable.Dispose();
						}
					}
					return list;
				}
			}

			public string FeatureId
			{
				get
				{
					return (string)base.data["featureId"];
				}
			}

			public RewardStatus RewardStatus
			{
				get
				{
					return (RewardStatus)((double)base.data["rewardStatus"]);
				}
			}
		}
	}
}
