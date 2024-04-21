using System;
using Tactile;

[ConfigProvider("ReviewConfig")]
public class ReviewConfig
{
	[JsonSerializable("AskForReviewEveryNthRun", null)]
	public int AskForReviewEveryNthRun { get; set; }

	[JsonSerializable("AskForReviewMinInterval", null)]
	public int AskForReviewMinInterval { get; set; }

	[JsonSerializable("Layout", null)]
	public int Layout { get; set; }
}
