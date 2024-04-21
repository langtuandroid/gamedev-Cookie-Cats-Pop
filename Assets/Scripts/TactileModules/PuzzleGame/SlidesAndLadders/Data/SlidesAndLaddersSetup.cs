using System;
using System.Collections.Generic;

namespace TactileModules.PuzzleGame.SlidesAndLadders.Data
{
	[SingletonAssetPath("Assets/[Database]/Resources/SlidesAndLadders/SlidesAndLaddersSetup.asset")]
	public class SlidesAndLaddersSetup : SingletonAsset<SlidesAndLaddersSetup>
	{
		public List<WheelSlot> wheelSlots;
	}
}
