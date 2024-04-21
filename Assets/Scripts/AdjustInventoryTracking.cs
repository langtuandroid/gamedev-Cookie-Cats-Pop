using System;

using TactileModules.Inventory;

public class AdjustInventoryTracking : IAdjustInventoryTracking
{
	public AdjustInventoryTracking(AdjustEventConstants adjustEventConstants)
	{
		this.adjustEventConstants = adjustEventConstants;
	}

	public void TrackAdjustCoinsUsed()
	{
		
	}

	private readonly AdjustEventConstants adjustEventConstants;
}
