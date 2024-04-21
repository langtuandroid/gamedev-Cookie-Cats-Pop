using System;
using System.Collections.Generic;
using TactileModules.Validation;
using UnityEngine;

namespace TactileModules.PuzzleGame.ScheduledBooster.Data
{
	[Serializable]
	public class ScheduledBoosterDefinition
	{
		public List<BoosterLogic> listOfBoosterLogic;

		[SerializeField]
		public ScheduledBoosterType type;

		[SerializeField]
		public ScheduledBoosterLocation location;

		[SerializeField]
		[UISpriteName]
		public string icon;

		[OptionalSerializedField]
		public GameObject boosterLargerIconPrefab;

		[SerializeField]
		public GameObject boosterInfoPrefab;
	}
}
