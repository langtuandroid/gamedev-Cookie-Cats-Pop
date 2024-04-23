using System;
using Tactile;

namespace TactileModules.PuzzleGame.ReengagementRewards
{
	[SettingsProvider("rer", false, new Type[]
	{

	})]
	public class PersistableState : IPersistableState<PersistableState>, IPersistableState
	{
		public PersistableState()
		{
			this.LastDateVisitedMap = DateHelper.DefaultTime;
			this.LevelIndex = -1;
			this.IsPopupShown = false;
		}

		[JsonSerializable("ldvm", null)]
		public DateTime LastDateVisitedMap { get; set; }

		[JsonSerializable("li", null)]
		public int LevelIndex { get; set; }

		[JsonSerializable("ips", null)]
		public bool IsPopupShown { get; set; }

		public bool DataEquals(PersistableState other)
		{
			return !(this.LastDateVisitedMap != other.LastDateVisitedMap) && this.LevelIndex == other.LevelIndex && this.IsPopupShown == other.IsPopupShown;
		}

		public void MergeFromOther(PersistableState newest, PersistableState last)
		{
			this.LastDateVisitedMap = ((!(newest.LastDateVisitedMap < this.LastDateVisitedMap)) ? this.LastDateVisitedMap : newest.LastDateVisitedMap);
			this.LevelIndex = Math.Max(this.LevelIndex, newest.LevelIndex);
			this.IsPopupShown |= newest.IsPopupShown;
		}
	}
}
