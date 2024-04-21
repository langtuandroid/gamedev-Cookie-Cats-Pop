using System;
using System.Collections.Generic;
using UnityEngine;

namespace TactileModules.GameCore.MenuTutorial
{
	[Serializable]
	public class MenuTutorialDefinition : IMenuTutorialDefinition
	{
		public int LevelRequired
		{
			get
			{
				return this.levelRequired;
			}
		}

		public List<MenuTutorialStep> Steps
		{
			get
			{
				return this.steps;
			}
		}

		[SerializeField]
		private int levelRequired;

		[SerializeField]
		private bool unfolded;

		[SerializeField]
		private List<MenuTutorialStep> steps = new List<MenuTutorialStep>();
	}
}
