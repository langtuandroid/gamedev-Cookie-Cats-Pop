using System;
using UnityEngine;

namespace TactileModules.PuzzleCore.LevelPlaying
{
	public class AssetFactory : IAssetFactory
	{
		public ILevelStartView CreateLevelStartView()
		{
			LevelStartView original = Resources.Load<LevelStartView>("LevelPlaying/LevelStartView");
			return UnityEngine.Object.Instantiate<LevelStartView>(original);
		}
	}
}
