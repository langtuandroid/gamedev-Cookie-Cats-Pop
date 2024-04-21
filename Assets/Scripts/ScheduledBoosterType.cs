using System;
using System.Collections.Generic;

namespace TactileModules.PuzzleGame.ScheduledBooster.Data
{
	[Serializable]
	public struct ScheduledBoosterType
	{
		public static List<string> GetIdentifiers()
		{
			List<string> list;
			List<string> result;
			CollectionExtensions.GetConstNamesAndValues<ScheduledBoosterType, string>(out list, out result);
			return result;
		}

		public const string SpecialContinue = "SpecialContinue";

		public const string Paintbrush = "Paintbrush";

		public string ID;
	}
}
