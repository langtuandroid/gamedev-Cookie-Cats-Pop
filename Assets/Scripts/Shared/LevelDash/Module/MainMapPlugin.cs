using System;
using TactileModules.PuzzleGames.LevelDash.Providers;
using TactileModules.PuzzleGames.LevelDash.Views.Handlers;
using TactileModules.SagaCore;

namespace Shared.LevelDash.Module
{
	public class MainMapPlugin : IMapPlugin
	{
		public MainMapPlugin(ILevelDashMapAvatarModifierProvider provider)
		{
			this.provider = provider;
		}

		public void ViewsCreated(MapIdentifier mapId, MapContentController mapContent, MapFlow mapFlow)
		{
			if (mapId == "Main")
			{
				this.handler = new LevelDashMapAvatarHandler(mapContent, this.provider);
			}
		}

		public void ViewsDestroyed(MapIdentifier mapId, MapContentController mapContent)
		{
			if (mapId == "Main" && this.handler != null)
			{
				this.handler.Dispose();
				this.handler = null;
			}
		}

		private readonly ILevelDashMapAvatarModifierProvider provider;

		private LevelDashMapAvatarHandler handler;
	}
}
