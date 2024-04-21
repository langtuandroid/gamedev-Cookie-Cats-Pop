using System;

public class PlacementIdentifier
{
	public PlacementIdentifier(string id)
	{
		this.ID = id;
	}

	public string ID { get; private set; }

	public static readonly NonBreakablePlacementIdentifier PreAnimateAvatar = new NonBreakablePlacementIdentifier("PreAnimateAvatar");

	public static readonly BreakablePlacementIdentifier PostAnimateAvatar = new BreakablePlacementIdentifier("PostAnimateAvatar");

	public static readonly BreakablePlacementIdentifier SessionStart = new BreakablePlacementIdentifier("SessionStart");

	public static readonly BreakablePlacementIdentifier MapIdle = new BreakablePlacementIdentifier("MapIdle");

	public static readonly NonBreakablePlacementIdentifier LevelStartShown = new NonBreakablePlacementIdentifier("LevelStartShown");
}
