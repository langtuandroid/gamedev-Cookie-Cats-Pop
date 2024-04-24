using System;
using System.Collections.Generic;
using UnityEngine;

namespace TactileModules.SagaCore
{
	public class MapFacade
	{
		public MapFacade(CloudClient cloudClient, VipManager vipManager)
		{
			this.cloudClient = cloudClient;
			this.vipManager = vipManager;
			this.MapPlugins = new List<IMapPlugin>();
		}

		public List<IMapPlugin> MapPlugins { get; private set; }

		public MapSystemSetup Setup
		{
			get
			{
				return SingletonAsset<MapSystemSetup>.Instance;
			}
		}

		public MapAvatar GetDefaultMeAvatarPrefab()
		{
			return Resources.Load<MapAvatar>("Map/DefaultAvatars/PlayerAvatar");
		}

		public MapAvatar GetDefaultFriendAvatarPrefab()
		{
			return Resources.Load<MapAvatar>("Map/DefaultAvatars/FriendAvatar");
		}

		public FlashAtCurrentLevel GetDefaultDotFlasherPrefab()
		{
			return Resources.Load<FlashAtCurrentLevel>("Map/DefaultAvatars/FlashAtCurrentLevel");
		}

		public MapContentController CreateMapContentController(MapIdentifier mapIdentifier)
		{
			return new MapContentController(mapIdentifier, this.cloudClient, this.vipManager, this);
		}

		private readonly CloudClient cloudClient;

		private readonly VipManager vipManager;
	}
}
