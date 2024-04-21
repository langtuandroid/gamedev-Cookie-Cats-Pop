using System;
using Tactile;

[SettingsProvider("commonPublic", true, new Type[]
{

})]
public class PublicUserSettings : IPersistableState<PublicUserSettings>, IPersistableState
{
	[JsonSerializable("tsp", null)]
	public float TotalSecondsPlayed { get; set; }

	[JsonSerializable("sp", null)]
	public int SessionsPlayed { get; set; }

	[JsonSerializable("vip", null)]
	public bool UserIsVip { get; set; }

	public void MergeFromOther(PublicUserSettings newest, PublicUserSettings last)
	{
		this.TotalSecondsPlayed = newest.TotalSecondsPlayed;
		this.SessionsPlayed = newest.SessionsPlayed;
		this.UserIsVip = newest.UserIsVip;
	}
}
