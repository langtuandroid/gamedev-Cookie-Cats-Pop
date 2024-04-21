using System;

namespace TactileModules.PuzzleGame.SlidesAndLadders.Model
{
	public class SlidesAndLaddersHelperMethods
	{
		public static string GetIdForLadder(int levelDotHumanNumber)
		{
			return "Ladder" + levelDotHumanNumber;
		}

		public static string GetIdForSlide(int levelDotHumanNumber)
		{
			return "Slide" + levelDotHumanNumber;
		}

		private const string LADDER_ID_PREFIX = "Ladder";

		private const string SLIDE_ID_PREFIX = "Slide";
	}
}
