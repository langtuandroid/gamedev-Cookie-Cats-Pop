using System;
using TactileModules.FeatureManager;
using TactileModules.Foundation;
using UnityEngine;

namespace TactileModules.PuzzleGames.LevelDash.Views.Handlers
{
	public class LevelDashMapButtonHandler : IDisposable
	{
		public LevelDashMapButtonHandler(SideButtonsArea sideButtonsArea, LevelDashMapButton levelDashMapButtonPrefab)
		{
			this.sideButtonsArea = sideButtonsArea;
			this.levelDashMapButtonPrefab = levelDashMapButtonPrefab;
			this.TryToInitializeLevelDashButton();
		}

		public void Dispose()
		{
            TactileModules.FeatureManager.FeatureManager.GetFeatureHandler<LevelDashManager>().OnLevelDashStarted -= this.CreateLevelDashButton;
		}

		private void TryToInitializeLevelDashButton()
		{
			LevelDashManager featureHandler = TactileModules.FeatureManager.FeatureManager.GetFeatureHandler<LevelDashManager>();
			if (featureHandler != null)
			{
				if (featureHandler.HasActiveFeature() && featureHandler.HasPresentedStartView())
				{
					this.CreateLevelDashButton(featureHandler);
				}
				else
				{
					featureHandler.OnLevelDashStarted += this.CreateLevelDashButton;
				}
			}
		}

		private void CreateLevelDashButton(LevelDashManager levelDashManager)
		{
			this.CreateSideButton(levelDashManager, this.sideButtonsArea, this.levelDashMapButtonPrefab);
		}

		private void CreateSideButton(LevelDashManager levelDashManager, SideButtonsArea buttonsArea, LevelDashMapButton mapButtonPrefab)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(mapButtonPrefab.gameObject);
			gameObject.name = "LevelDashButton";
			UIButton componentInChildren = gameObject.GetComponentInChildren<UIButton>();
			componentInChildren.Clicked += ManagerRepository.Get<ILevelDashSystem>().LevelDashViewController.OnSideButtonClicked;
			LevelDashMapButton component = gameObject.GetComponent<LevelDashMapButton>();
			component.Init(new Func<string>(levelDashManager.GetTimeRemainingAsString), new Func<bool>(levelDashManager.ShouldShowMapButton));
			buttonsArea.InitButton(component);
		}

		private SideButtonsArea sideButtonsArea;

		private LevelDashMapButton levelDashMapButtonPrefab;
	}
}
