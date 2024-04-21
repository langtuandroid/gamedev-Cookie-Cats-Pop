using System;
using UnityEngine;

namespace TactileModules.PuzzleGames.StoryMapEvent
{
	public class ViewFactory : IViewFactory
	{
		public IStoryMapEventAnnounceView CreateFeatureStartedView()
		{
			StoryMapEventAnnounceView original = Resources.Load<StoryMapEventAnnounceView>("StoryMapEvent/FeatureStartedView");
			return UnityEngine.Object.Instantiate<StoryMapEventAnnounceView>(original);
		}

		public IStoryMapEventAnnounceView CreateFeatureEndedView()
		{
			StoryMapEventAnnounceView original = Resources.Load<StoryMapEventAnnounceView>("StoryMapEvent/FeatureEndedView");
			return UnityEngine.Object.Instantiate<StoryMapEventAnnounceView>(original);
		}

		public IStoryMapEventAnnounceView CreateFeatureReminderView()
		{
			StoryMapEventAnnounceView original = Resources.Load<StoryMapEventAnnounceView>("StoryMapEvent/FeatureReminderView");
			return UnityEngine.Object.Instantiate<StoryMapEventAnnounceView>(original);
		}

		public StoryMapSideButton CreateSideButton()
		{
			StoryMapSideButton original = Resources.Load<StoryMapSideButton>("StoryMapEvent/SideButton");
			return UnityEngine.Object.Instantiate<StoryMapSideButton>(original);
		}

		public StoryMapEventRewardView CreateRewardView()
		{
			StoryMapEventRewardView original = Resources.Load<StoryMapEventRewardView>("StoryMapEvent/RewardView");
			return UnityEngine.Object.Instantiate<StoryMapEventRewardView>(original);
		}

		public StoryMapEventEndOfContentView CreateEndOfContentView()
		{
			StoryMapEventEndOfContentView original = Resources.Load<StoryMapEventEndOfContentView>("StoryMapEvent/EndOfContentView");
			return UnityEngine.Object.Instantiate<StoryMapEventEndOfContentView>(original);
		}
	}
}
