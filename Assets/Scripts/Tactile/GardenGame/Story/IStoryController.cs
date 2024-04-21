using System;
using System.Collections;

namespace Tactile.GardenGame.Story
{
	public interface IStoryController
	{
		IEnumerator TryStartAndWaitForPendingIntro();

		IEnumerator TryPlayEnterMapOrPendingIntro();

		bool Step();

		bool IsStoryInProgress();

		bool IsIdleBehaviourRunning();

		void Destroy();

		event Action<MapTask> TaskStarted;

		event Action<MapTask> TaskEnded;

		void StartTask(MapTask task, MapAction.ActionType taskType);

		void StartIdleBehaviour(MapTask task);
	}
}
