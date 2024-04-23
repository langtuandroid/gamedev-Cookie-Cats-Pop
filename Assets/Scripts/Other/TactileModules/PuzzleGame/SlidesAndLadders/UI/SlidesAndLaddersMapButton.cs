using System;
using TactileModules.FeatureManager;
using TactileModules.MapFeature;
using TactileModules.PuzzleGame.SlidesAndLadders.Model;

namespace TactileModules.PuzzleGame.SlidesAndLadders.UI
{
	public class SlidesAndLaddersMapButton : MapFeatureSideButton
	{
		public override object Data
		{
			get
			{
				return null;
			}
		}

		protected override MapFeatureHandler MapFeatureHandler
		{
			get
			{
				return TactileModules.FeatureManager.FeatureManager.GetFeatureHandler<SlidesAndLaddersHandler>();
			}
		}

		protected override bool VisibilityCheckerImplementor(object data)
		{
			return true;
		}
	}
}
