using System;
using Tactile;
using TactileModules.Foundation;

[ConfigProvider("LivesConfig")]
public class LivesConfig
{
	[JsonSerializable("LifeRegenerationTime", null)]
	public int LifeRegenerationTime { get; set; }

	[JsonSerializable("InfiniteLivesDurationInSeconds", null)]
	public int InfiniteLivesDurationInSeconds { get; set; }

	[JsonSerializable("InfiniteLivesPopupInterval", null)]
	public int InfiniteLivesPopupInterval { get; set; }

	[JsonSerializable("LevelRequiredForInfiniteLivesPopup", null)]
	public int LevelRequiredForInfiniteLivesPopup { get; set; }

	[JsonSerializable("MinSecondsBetweenShowSendLivesAtStart", null)]
	public int MinSecondsBetweenShowSendLivesAtStart { get; set; }

	[JsonSerializable("NotLoggedInMaxlives", null)]
	public int NotLoggedInMaxlives { get; set; }

	[JsonSerializable("LoggedInMaxlives", null)]
	public int LoggedInMaxlives { get; set; }

	public int LifeRegenerationMaxCount
	{
		get
		{
			return (!ManagerRepository.Get<FacebookClient>().IsSessionValid) ? this.NotLoggedInMaxlives : this.LoggedInMaxlives;
		}
	}
}
