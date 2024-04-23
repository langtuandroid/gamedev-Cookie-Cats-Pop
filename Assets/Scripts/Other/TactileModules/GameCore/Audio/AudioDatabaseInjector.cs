using System;
using TactileModules.GameCore.Audio.Assets;

namespace TactileModules.GameCore.Audio
{
	public class AudioDatabaseInjector
	{
		public AudioDatabaseInjector(IAssetsModel assets)
		{
			this.assets = assets;
		}

		public IAssetsModel Assets
		{
			get
			{
				return this.assets;
			}
		}

		private readonly IAssetsModel assets;
	}
}
