using System;
using System.Collections.Generic;
using UnityEngine;

namespace TactileModules.GameCore.MenuTutorial
{
	public class MenuTutorialDatabase : ScriptableObject, IMenuTutorialDatabase
	{
		public IEnumerable<IMenuTutorialDefinition> Tutorials
		{
			get
			{
				foreach (MenuTutorialDefinition tutorial in this.tutorials)
				{
					yield return tutorial;
				}
				yield break;
			}
		}

		[SerializeField]
		private List<MenuTutorialDefinition> tutorials = new List<MenuTutorialDefinition>();
	}
}
