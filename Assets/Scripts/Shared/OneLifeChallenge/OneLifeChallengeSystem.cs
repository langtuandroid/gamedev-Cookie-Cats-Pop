using System;

namespace Shared.OneLifeChallenge
{
	public class OneLifeChallengeSystem : IOneLifeChallengeSystem
	{
		public OneLifeChallengeSystem(OneLifeChallengeManager manager, ControllerFactory controllerFactory)
		{
			this.manager = manager;
			this.controllerFactory = controllerFactory;
		}

		public OneLifeChallengeManager Manager
		{
			get
			{
				return this.manager;
			}
		}

		public ControllerFactory ControllerFactory
		{
			get
			{
				return this.controllerFactory;
			}
		}

		private readonly OneLifeChallengeManager manager;

		private readonly ControllerFactory controllerFactory;
	}
}
