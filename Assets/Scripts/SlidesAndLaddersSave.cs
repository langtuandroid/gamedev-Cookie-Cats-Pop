using System;
using TactileModules.PuzzleGame.SlidesAndLadders;

public class SlidesAndLaddersSave : ISlidesAndLaddersSave
{
	public void Save()
	{
		PuzzleGame.UserSettings.SaveLocal();
	}
}
