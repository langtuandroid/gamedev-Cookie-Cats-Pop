using System;
using System.Collections;
using TactileModules.Analytics.Interfaces;

namespace TactileModules.PuzzleCore.LevelPlaying
{
	public class BasicEventDecorator : IEventDecorator<BasicEvent>, IEventDecorator
	{
		public BasicEventDecorator(IPlayFlowEvents playFlowEvents)
		{
			playFlowEvents.PlayFlowCreated += this.HandlePlayFlowCreated;
		}

		private void HandlePlayFlowCreated(ICorePlayFlow playFlow)
		{
			playFlow.LevelStartedHook.Register(new Func<ILevelAttempt, IEnumerator>(this.HandleLevelStarted));
			playFlow.LevelEndedHook.Register(new Func<ILevelAttempt, IEnumerator>(this.HandleLevelEnded));
			playFlow.LevelSessionStarted += this.HandleLevelSessionStarted;
			playFlow.LevelSessionEnded += this.HandleLevelSessionEnded;
		}

		private void HandleLevelSessionStarted(ILevelSessionRunner sessionInfo)
		{
			this.currentSessionId = sessionInfo.SessionId;
			this.levelProxy = sessionInfo.LevelProxy;
		}

		private void HandleLevelSessionEnded(ILevelSessionRunner sessionInfo)
		{
			this.currentSessionId = string.Empty;
			this.levelProxy = LevelProxy.Invalid;
		}

		private IEnumerator HandleLevelStarted(ILevelAttempt levelAttempt)
		{
			yield break;
		}

		private IEnumerator HandleLevelEnded(ILevelAttempt attempt)
		{
			yield break;
		}

		public void Decorate(BasicEvent eventObject)
		{
			if (!string.IsNullOrEmpty(this.currentSessionId))
			{
				eventObject.SetLevelPlayingParameters(this.currentSessionId);
			}
		}

		private string currentSessionId;

		private LevelProxy levelProxy = LevelProxy.Invalid;
	}
}
