using System;
using System.Collections;
using System.Collections.Generic;
using Cloud;
using TactileModules.FeatureManager;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.PuzzleGames.EndlessChallenge.Data;

namespace TactileModules.PuzzleGames.EndlessChallenge
{
	public class EndlessChallengeCloud
	{
		public EndlessChallengeCloud(CloudClientBase cloudClientBase)
		{
			if (cloudClientBase == null)
			{
				throw new ArgumentNullException("cloudClientBase");
			}
			this.cloudClientBase = cloudClientBase;
		}

		private ICloudInterfaceBase CloudInterfaceBase
		{
			get
			{
				return this.cloudClientBase.cloudInterface;
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

		private string UserId
		{
			get
			{
				return (!this.HasValidUser) ? null : this.CachedMe.CloudId;
			}
		}

		private string FeatureId
		{
			get
			{
				string result = string.Empty;
				ActivatedFeatureInstanceData activatedFeature = this.Manager.GetActivatedFeature();
				if (activatedFeature != null)
				{
					result = activatedFeature.Id;
				}
				return result;
			}
		}

		private EndlessChallengeHandler Manager
		{
			get
			{
				return TactileModules.FeatureManager.FeatureManager.GetFeatureHandler<EndlessChallengeHandler>();
			}
		}

		public IEnumerator Join(int farthestUnlockedLevelNumber, EndlessChallengeCloud.EndlessChallengeResponse response)
		{
			yield return this.CloudInterfaceBase.EndlessChallengeJoin(this.UserId, this.FeatureId, farthestUnlockedLevelNumber, response);
			yield break;
		}

		public IEnumerator GetStatus(EndlessChallengeCloud.EndlessChallengeResponse response)
		{
			yield return this.CloudInterfaceBase.EndlessChallengeStatus(this.UserId, this.FeatureId, response);
			yield break;
		}

		public IEnumerator SubmitScore(int highestRow, EndlessChallengeCloud.EndlessChallengeResponse response)
		{
			yield return this.CloudInterfaceBase.EndlessChallengeSubmitScore(this.UserId, this.FeatureId, highestRow, response);
			yield break;
		}

		public const int DEFAULT_PRESENT_RESULT = -100;

		private readonly CloudClientBase cloudClientBase;

		public class EndlessChallengeResponse : Response
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
		}
	}
}
