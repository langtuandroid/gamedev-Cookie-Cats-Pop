using System;

namespace TactileModules.PuzzleGame.PlayablePostcard.Model
{
	public class PostcardItemTypeAndId
	{
		[JsonSerializable("Type", null)]
		public PostcardItemType Type { get; set; }

		[JsonSerializable("Id", null)]
		public string Id { get; set; }
	}
}
