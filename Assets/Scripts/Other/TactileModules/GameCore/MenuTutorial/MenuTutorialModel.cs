using System;
using System.Collections.Generic;
using TactileModules.GameCore.MenuTutorial.Assets;

namespace TactileModules.GameCore.MenuTutorial
{
	public class MenuTutorialModel : IMenuTutorialModel
	{
		public MenuTutorialModel(IAssetModel assets)
		{
			this.assets = assets;
			this.runningTutorials = new List<IRunningTutorial>();
		}

		public void AddRunningTutorial(IRunningTutorial tutorial)
		{
			this.runningTutorials.Add(tutorial);
		}

		public void RemoveRunningTutorial(IRunningTutorial tutorial)
		{
			this.runningTutorials.Remove(tutorial);
		}

		public IEnumerable<IRunningTutorial> RunningTutorials
		{
			get
			{
				return this.runningTutorials;
			}
		}

		public IEnumerable<IMenuTutorialDefinition> TutorialDefinitions
		{
			get
			{
				return this.assets.GetMenuTutorialDatabase().Tutorials;
			}
		}

		public bool Empty
		{
			get
			{
				return this.runningTutorials.Count == 0;
			}
		}

		private readonly IAssetModel assets;

		private readonly List<IRunningTutorial> runningTutorials;
	}
}
