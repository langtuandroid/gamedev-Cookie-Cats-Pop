using System;
using System.Collections.Generic;
using ConfigSchema;
using Tactile;
using TactileModules.FeatureManager.DataClasses;

namespace TactileModules.PuzzleGames.StarTournament
{
	[ConfigProvider("StarTournamentConfig")]
	public class StarTournamentConfig
	{
		[JsonSerializable("Enabled", null)]
		public bool FeatureEnabled { get; set; }

		[JsonSerializable("LevelRequired", null)]
		public int LevelRequired { get; set; }

		[HeaderTemplate("Rank {{ self.Rank }}")]
		[JsonSerializable("Rewards", typeof(StarTournamentConfig.Reward))]
		public List<StarTournamentConfig.Reward> Rewards { get; set; }

		[JsonSerializable("FeatureNotificationSettings", null)]
		public FeatureNotificationSettings FeatureNotificationSettings { get; set; }

		[ObsoleteJsonName("Package")]
		public class Reward : IEquatable<StarTournamentConfig.Reward>
		{
			public Reward()
			{
				this.Rank = -1;
				this.Items = new List<ItemAmount>();
			}

			[JsonSerializable("Rank", null)]
			public int Rank { get; set; }

			[HeaderTemplate("{{ self.Type }} x {{ self.Amount }}")]
			[JsonSerializable("Items", typeof(ItemAmount))]
			public List<ItemAmount> Items { get; set; }

			public bool Equals(StarTournamentConfig.Reward other)
			{
				return other != null && this.Rank == other.Rank && (this.Items != null || other.Items == null) && (this.Items == null || this.Items.Equals(other.Items));
			}
		}
	}
}
