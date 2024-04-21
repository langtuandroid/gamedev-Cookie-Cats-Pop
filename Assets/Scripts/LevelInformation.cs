using System;
using System.Runtime.InteropServices;

namespace TactileModules.PuzzleCore.LevelPlaying
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct LevelInformation
	{
		public bool IsTutorial { get; set; }

		public string LevelType { get; set; }

		public int TotalGoalPieces { get; set; }
	}
}
