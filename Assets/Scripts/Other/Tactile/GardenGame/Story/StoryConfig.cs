using System;
using System.Collections.Generic;
using ConfigSchema;

namespace Tactile.GardenGame.Story
{
	[ConfigProvider("StoryConfig")]
	public class StoryConfig
	{
		public StoryConfig()
		{
			this.StoryRewards = new List<StoryConfig.Reward>();
			this.TimedTasks = new List<StoryConfig.TimedTask>();
		}

		[Required]
		[Description("Story progression rewards")]
		[JsonSerializable("Rewards", typeof(StoryConfig.Reward))]
		public List<StoryConfig.Reward> StoryRewards { get; set; }

		[JsonSerializable("TimedTasks", typeof(StoryConfig.TimedTask))]
		public List<StoryConfig.TimedTask> TimedTasks { get; set; }

		[RequireAll]
		[ObsoleteJsonName("Package")]
		public class Reward
		{
			[JsonSerializable("Chapter", null)]
			public int Chapter { get; set; }

			[Description("Normalized chapter progression needed to claim reward (0-1)")]
			[JsonSerializable("Chapter Progression", null)]
			public float NormalizedProgression { get; set; }

			[Description("Index used to define type for images and animations")]
			[JsonSerializable("Reward type", null)]
			public int RewardType { get; set; }

			[HeaderTemplate("{{ self.Type }} x {{ self.Amount }}")]
			[JsonSerializable("Items", typeof(ItemAmount))]
			public List<ItemAmount> Items { get; set; }
		}

		public class TimedTask
		{
			[Description("The name of the PREFAB for the task (i.e. not the localizable title!)")]
			[JsonSerializable("TaskName", null)]
			public string TaskObjectName { get; set; }

			[JsonSerializable("TimerEnabled", null)]
			[Description("Can be used to disable a task timer that is built in to the client")]
			public bool TimerEnabled { get; set; }

			[JsonSerializable("WaitTimeInSeconds", null)]
			public int WaitTimeInSeconds { get; set; }

			[JsonSerializable("CoinSkipCost", null)]
			public int CoinSkipCost { get; set; }
		}
	}
}
