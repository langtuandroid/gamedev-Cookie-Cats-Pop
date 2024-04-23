using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using TactileModules.GameCore.MainProgression;

namespace TactileModules.GameCore.MenuTutorial
{
	public class MenuTutorialEngine
	{
		public MenuTutorialEngine(MenuTutorialModel model, IMenuTutorialDatabase database, IMainProgressionModel mainProgression, IRunningTutorialFactory runningTutorialFactory, Func<bool> enabledFunction)
		{
			this.model = model;
			this.database = database;
			this.mainProgression = mainProgression;
			this.runningTutorialFactory = runningTutorialFactory;
			this.enabledFunction = enabledFunction;
			this.deadTutorialsCache = new List<IRunningTutorial>();
			this.mainProgression.ProgressChanged += this.UpdateRunningTutorials;
			this.UpdateRunningTutorials();
		}

		private void UpdateRunningTutorials()
		{
			int progress = this.mainProgression.Progress;
			foreach (IMenuTutorialDefinition menuTutorialDefinition in this.database.Tutorials)
			{
				if (menuTutorialDefinition.LevelRequired == progress)
				{
					IRunningTutorial runningTutorial = this.runningTutorialFactory.CreateTutorial(menuTutorialDefinition);
					this.model.AddRunningTutorial(runningTutorial);
					runningTutorial.Start();
				}
			}
			this.deadTutorialsCache.Clear();
			foreach (IRunningTutorial runningTutorial2 in this.model.RunningTutorials)
			{
				if (runningTutorial2.Definition.LevelRequired != progress)
				{
					this.deadTutorialsCache.Add(runningTutorial2);
				}
			}
			foreach (IRunningTutorial runningTutorial3 in this.deadTutorialsCache)
			{
				runningTutorial3.Stop();
				this.model.RemoveRunningTutorial(runningTutorial3);
			}
			if (!this.model.Empty)
			{
				if (this.fiber == null)
				{
					this.fiber = new Fiber(FiberBucket.Update);
				}
				if (this.fiber.IsTerminated)
				{
					this.fiber.Start(this.Loop());
				}
			}
		}

		private IEnumerator Loop()
		{
			for (;;)
			{
				while (!this.enabledFunction())
				{
					yield return null;
				}
				this.deadTutorialsCache.Clear();
				foreach (IRunningTutorial runningTutorial in this.model.RunningTutorials)
				{
					if (!runningTutorial.Step())
					{
						runningTutorial.Stop();
						this.deadTutorialsCache.Add(runningTutorial);
					}
				}
				if (this.deadTutorialsCache.Count > 0)
				{
					foreach (IRunningTutorial tutorial in this.deadTutorialsCache)
					{
						this.model.RemoveRunningTutorial(tutorial);
					}
				}
				if (this.model.Empty)
				{
					break;
				}
				yield return null;
			}
			this.fiber.Terminate();
			yield break;
		}

		private readonly IMenuTutorialModel model;

		private readonly IMainProgressionModel mainProgression;

		private readonly IMenuTutorialDatabase database;

		private readonly IRunningTutorialFactory runningTutorialFactory;

		private readonly Func<bool> enabledFunction;

		private Fiber fiber;

		private readonly List<IRunningTutorial> deadTutorialsCache;
	}
}
