using System;

namespace TactileModules.GameCore.ButtonArea
{
	public class ButtonAreaSystem : IButtonAreaSystem
	{
		public ButtonAreaSystem(IButtonAreaModel model, IButtonAreaController controller)
		{
			this.Model = model;
			this.Controller = controller;
		}

		public IButtonAreaModel Model { get; private set; }

		public IButtonAreaController Controller { get; private set; }

		private readonly IButtonAreaController controller;
	}
}
