using System;

namespace Shared.OneLifeChallenge
{
	public interface IOneLifeChallengeSystem
	{
		OneLifeChallengeManager Manager { get; }

		ControllerFactory ControllerFactory { get; }
	}
}
