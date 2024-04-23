using System;

namespace TactileModules.PuzzleGames.StoryMapEvent
{
	public class ReminderTimeStamp
	{
		[JsonSerializable("LastShownTimeStamp", null)]
		public int LastShownTimeStamp { get; set; }
	}
}
