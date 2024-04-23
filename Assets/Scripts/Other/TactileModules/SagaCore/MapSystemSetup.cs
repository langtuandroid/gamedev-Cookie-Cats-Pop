using System;
using UnityEngine;

namespace TactileModules.SagaCore
{
	[SingletonAssetPath("Assets/[ModuleAssets]/Resources/Map/MapSystemSetup.asset")]
	public class MapSystemSetup : SingletonAsset<MapSystemSetup>
	{
		public AnimationCurve AvatarMoveCurve
		{
			get
			{
				return this.avatarMoveCurve;
			}
		}

		[SerializeField]
		private AnimationCurve avatarMoveCurve;
	}
}
