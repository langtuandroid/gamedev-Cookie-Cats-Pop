using System;
using System.Collections;
using System.Collections.Generic;
using Tactile.GardenGame.MapSystem;
using Tactile.GardenGame.Story.Dialog;
using Tactile.GardenGame.Story.Monologue;
using TactileModules.GameCore.StreamingAssets;

namespace Tactile.GardenGame.Story
{
	public interface IStoryMapController
	{
		T GetMapComponent<T>(string id) where T : MapComponent;

		PropsManager PropsManager { get; }

		IMapScriptDialogPresenter Dialog { get; }

		float Darkness { get; set; }

		IEnumerator FadeToBlack();

		IEnumerator FadeToMap();

		void InstantBlack();

		void RemoveBlack();

		IEnumerator HideBars();

		IEnumerator ShowBars();

		IEnumerator HideFullScreenImage();

		IEnumerator ShowFullScreenMessage(string message, float duration);

		IEnumerator ShowCelebration(string message);

		MonologueManager Monologues { get; }

		IEnumerator ShowFullScreenImage(StreamingAsset image);

		MapCamera Camera { get; }

		IEnumerator ChooseProp(MapProp prop);

		IEnumerable<T> IterateObjectsWithComponent<T>();

		IEnumerator ShowUI();

		IEnumerator HideUI();

		void Destroy();

		void PlayMusic(SoundDefinition soundDefinition);

		void StopMusic();

		void PlaySound(SoundDefinition soundDefinition);
	}
}
