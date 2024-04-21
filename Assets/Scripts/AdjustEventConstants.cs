using System;
using Tactile.SagaCore;
using TactileModules.PuzzleGames.GameCore.Analytics;

public class AdjustEventConstants : IAdjustInventoryEvents, IAdjustProgressionEvents
{
	public string ADJUST_IO_COINS_USED_EVENT_TOKEN
	{
		get
		{
			return "MISSING";
		}
	}

	public string ADJUST_IO_LEVEL_3_COMPLETED_EVENT_TOKEN
	{
		get
		{
			return "obcq39";
		}
	}

	public string ADJUST_IO_LEVEL_5_COMPLETED_EVENT_TOKEN
	{
		get
		{
			return "5q897g";
		}
	}

	public string ADJUST_IO_LEVEL_7_COMPLETED_EVENT_TOKEN
	{
		get
		{
			return "js7nn7";
		}
	}

	public string ADJUST_IO_LEVEL_10_COMPLETED_EVENT_TOKEN
	{
		get
		{
			return "tgq250";
		}
	}

	public string ADJUST_IO_LEVEL_20_COMPLETED_EVENT_TOKEN
	{
		get
		{
			return "49r882";
		}
	}

	public string ADJUST_IO_LEVEL_30_COMPLETED_EVENT_TOKEN
	{
		get
		{
			return "ndkuql";
		}
	}

	public string ADJUST_IO_LEVEL_40_COMPLETED_EVENT_TOKEN
	{
		get
		{
			return "as1qmb";
		}
	}

	public string ADJUST_IO_GATE_1_UNLOCKED_EVENT_TOKEN
	{
		get
		{
			return "gsk8qw";
		}
	}

	public static string ADJUST_IO_APP_TOKEN = "l2nmhylvpqm8";

	public static string ADJUST_IO_USER_REGISTERED_EVENT_TOKEN = "tr4xrb";

	public static string ADJUST_IO_USER_CHEATED_EVENT_TOKEN = "hgf4wh";

	public static string ADJUST_IO_IAP_EVENT_TOKEN = "4od49i";

	public static string ADJUST_IO_USER_IS_PAYING_EVENT_TOKEN = "5wed5x";

	public static string ADJUST_IO_MISSION_STARTED_EVENT_TOKEN = "MISSING";
}
