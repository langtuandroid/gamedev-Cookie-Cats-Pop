using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Fibers;
using JetBrains.Annotations;
using NinjaUI;
using TactileModules.PuzzleGames.StarTournament.Views;
using TactileModules.SagaCore;
using UnityEngine;

namespace TactileModules.PuzzleGames.StarTournament
{
	public class StarTournamentStarsRewardPopup : MapPopupManager.IMapPopup, IMapPlugin
	{
		public StarTournamentStarsRewardPopup([NotNull] StarTournamentManager starTournamentManager, MapFacade mapFacade)
		{
			if (starTournamentManager == null)
			{
				throw new ArgumentNullException("starTournamentManager");
			}
			this.manager = starTournamentManager;
			mapFacade.MapPlugins.Add(this);
		}

		private bool ShouldShowPopup()
		{
			return this.manager.Config.FeatureEnabled && this.manager.IsStarTournamentActive && this.manager.CustomInstanceData.StartedViewPresented && this.manager.ActiveLevel.StarsToShow > 0;
		}

		public void TryShowPopup(int unlockedLevelIndex, MapPopupManager.PopupFlow popupFlow)
		{
			if (!this.manager.Config.FeatureEnabled)
			{
				return;
			}
			if (this.ShouldShowPopup())
			{
				popupFlow.AddPopup(this.ShowPopup());
			}
		}

		private IEnumerator ShowPopup()
		{
			UICamera.DisableInput();
			if (StarTournamentStarsRewardPopup._003C_003Ef__mg_0024cache0 == null)
			{
				StarTournamentStarsRewardPopup._003C_003Ef__mg_0024cache0 = new Fiber.OnExitHandler(UICamera.EnableInput);
			}
			yield return new Fiber.OnExit(StarTournamentStarsRewardPopup._003C_003Ef__mg_0024cache0);
			int starsObtained = this.manager.ActiveLevel.StarsToShow;
			int levelId = this.manager.ActiveLevel.LevelId;
			Vector3 position;
			this.mapContentController.TryGetDotPosition(levelId, out position);
			StarTournamentMapButton mapButton = this.manager.Provider.GetSideButtonsArea().FindButton<StarTournamentMapButton>();
			if (position != Vector3.zero)
			{
				List<IEnumerator> defferedList = new List<IEnumerator>();
				List<GameObject> starObjects = new List<GameObject>(starsObtained);
				GameObject starPrefab = this.manager.Provider.GetStarPrefab();
				Vector2 size = starPrefab.GetComponent<UIElement>().Size;
				int offsetFromCenter = (int)((float)(starsObtained - 1) * (size.x * 0.5f));
				for (int i = 0; i < starsObtained; i++)
				{
					GameObject starObject = UnityEngine.Object.Instantiate<GameObject>(starPrefab);
					starObject.transform.SetParent(this.mapContentController.ContentRoot, false);
					position.z = -25f;
					starObject.transform.localPosition = position;
					starObject.transform.localScale = Vector3.zero;
					starObject.SetLayerRecursively(this.mapContentController.ContentRoot.gameObject.layer);
					starObjects.Add(starObject);
					starObject.transform.SetParent(null);
					float animDuration = 0.2f;
					defferedList.Add(FiberAnimation.ScaleTransform(starObject.transform, starObject.transform.localScale, Vector3.one, null, animDuration));
					defferedList.Add(FiberAnimation.MoveLocalTransform(starObject.transform, starObject.transform.localPosition, new Vector3((float)i * size.x - (float)offsetFromCenter, 0f, -100f), null, animDuration));
					yield return FiberHelper.RunParallel(defferedList.ToArray());
					yield return CameraShaker.ShakeDecreasing(0.15f, 5f, 15f, 0f, false);
				}
				defferedList.Clear();
				for (int j = 0; j < starsObtained; j++)
				{
					GameObject starObject2 = starObjects[j];
					starObject2.transform.SetParent(mapButton.transform);
					float animDuration2 = 0.3f;
					defferedList.Add(FiberAnimation.RotateTransform(starObject2.transform, Vector3.zero, Vector3.forward * 360f, null, animDuration2));
					defferedList.Add(FiberAnimation.ScaleTransform(starObject2.transform, starObject2.transform.localScale, Vector3.zero, null, animDuration2));
					defferedList.Add(FiberAnimation.MoveLocalTransform(starObject2.transform, starObject2.transform.localPosition, new Vector3(30f, 0f, starObject2.transform.position.z), null, animDuration2));
					yield return FiberHelper.RunParallel(defferedList.ToArray());
					yield return FiberAnimation.ShakeDecreasingLocalPosition(mapButton.transform, 0.2f, 5f, 15f, 0f);
					UnityEngine.Object.Destroy(starObject2);
				}
				defferedList.Clear();
			}
			this.manager.ActiveLevel.Reset();
			yield break;
		}

		void IMapPlugin.ViewsCreated(MapIdentifier mapId, MapContentController mapContent, MapFlow mapFlow)
		{
			if (mapId == "Main")
			{
				this.mapContentController = mapContent;
			}
		}

		void IMapPlugin.ViewsDestroyed(MapIdentifier mapId, MapContentController mapContent)
		{
			if (mapId == "Main")
			{
				this.mapContentController = null;
			}
		}

		private readonly StarTournamentManager manager;

		private MapContentController mapContentController;

		[CompilerGenerated]
		private static Fiber.OnExitHandler _003C_003Ef__mg_0024cache0;
	}
}
