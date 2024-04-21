using System;
using TactileModules.PuzzleGame.SlidesAndLadders.Controllers;
using TactileModules.PuzzleGame.SlidesAndLadders.Model;

namespace TactileModules.PuzzleGame.SlidesAndLadders
{
	public class SlidesAndLaddersSystem
	{
		public SlidesAndLaddersSystem(SlidesAndLaddersHandler handler, ISlidesAndLaddersControllerFactory controllerFactory)
		{
			this.handler = handler;
			this.controllerFactory = controllerFactory;
		}

		public SlidesAndLaddersHandler Handler
		{
			get
			{
				return this.handler;
			}
		}

		public ISlidesAndLaddersControllerFactory GetControllerFactory()
		{
			return this.controllerFactory;
		}

		private readonly SlidesAndLaddersHandler handler;

		private readonly ISlidesAndLaddersControllerFactory controllerFactory;
	}
}
