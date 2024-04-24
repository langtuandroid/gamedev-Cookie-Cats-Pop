using System;
using System.Collections;
using Fibers;
using Tactile.GardenGame.MapSystem;
using Tactile.GardenGame.Story;
using TactileModules.GardenGame.MapSystem.Assets;
using TactileModules.PuzzleGames.GameCore;
using TactileModules.TactileCloud;
using UnityEngine;

namespace Tactile.GardenGame.MapEditor
{
	public class MapEditorBoot : MonoBehaviour
	{
		private void Start()
		{
			UIViewManager.CreateInstance(null, false, 10, 0.3f);
			IFullScreenManager fullScreenManager = new FullScreenManager(null);
			UIController uiController = new UIController(UIViewManager.Instance);
			FlowStack flowStack = new FlowStack();
			RequestMetaDataProviderRegistry requestMetaDataProviderRegistry = new RequestMetaDataProviderRegistry();
			CloudClient cloudClient = new CloudClient(requestMetaDataProviderRegistry);
			UserSettingsManager.CreateInstance(cloudClient, delegate(UserSettingsManager u)
			{
			}, null);
			this.propsManager = new PropsManager(UserSettingsManager.Instance);
			this.mainMapController = new MainMapController(uiController, this.propsManager, new AssetModel(), flowStack, UserSettingsManager.Instance);
			this.mainMapController.ScreenAcquired();
			MapEditorBoot.AudioMock storyAudio = new MapEditorBoot.AudioMock();
			IStoryMapControllerFactory storyMapControllerFactory = new StoryMapControllerFactory(uiController, this.propsManager, storyAudio);
			this.storyManager = new StoryManager(null, null, null, null, null, null, null, null, this.propsManager);
			this.storyController = new StoryController(this.storyManager, storyMapControllerFactory, this.propsManager, this.mainMapController);
			this.runTaskFiber = new Fiber(FiberBucket.Manual);
			FiberCtrl.Pool.Run(this.StartTaskDelayed(), false);
		}

		private IEnumerator StartTaskDelayed()
		{
			for (int i = 0; i < 10; i++)
			{
				yield return null;
			}
			this.StartTask();
			yield break;
		}

		public void StartTask()
		{
			UIViewManager.Instance.CloseAll(new IUIView[]
			{
				UIViewManager.Instance.FindView(typeof(MainMapButtonsView))
			});
			this.storyManager.JumpToTask(this.Chapter, this.Task, this.ActionType, GardenGameSetup.Get);
			this.runTaskFiber.Start(this.storyController.PlayTask(this.Task, this.ActionType, null));
		}

		private void Update()
		{
			this.mainMapController.Step(Time.deltaTime);
			this.runTaskFiber.Step();
			if (UnityEngine.Input.GetKey(KeyCode.A))
			{
				this.StartTask();
			}
		}

		public MapTask Task;

		public int Chapter;

		public MapAction.ActionType ActionType;

		public MapAction StartAction;

		private MainMapController mainMapController;

		private StoryController storyController;

		private StoryManager storyManager;

		private PropsManager propsManager;

		private Fiber runTaskFiber;

		private class AudioMock : IStoryAudio
		{
			public void PlayMusic(SoundDefinition soundDefinition)
			{
			}

			public void StopMusic()
			{
			}

			public void PlaySound(SoundDefinition soundDefinition)
			{
			}
		}
	}
}
