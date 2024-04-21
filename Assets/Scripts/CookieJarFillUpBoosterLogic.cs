using System;
using System.Collections.Generic;

public class CookieJarFillUpBoosterLogic : CookieJarBoosterLogic
{
	protected override Dictionary<MatchFlag, int> CookiesToGive(List<GamePowers.Power> availablePowers)
	{
		Dictionary<MatchFlag, int> dictionary = new Dictionary<MatchFlag, int>();
		foreach (GamePowers.Power power in availablePowers)
		{
			dictionary[power.Color] = power.MaxValue - power.Value;
		}
		return dictionary;
	}
}
