using System;
using TactileModules.GameCore.ButtonArea;

namespace Tactile.GameCore.Settings.Buttons
{
	public abstract class ButtonHandler
	{
		public ButtonHandler(IButtonAreaModel buttonAreaModel, ButtonAreaButton buttonAsset)
		{
			buttonAreaModel.RegisterButton(buttonAsset, new Action<ButtonAreaButton>(this.HandleButtonCreated), new Action<ButtonAreaButton>(this.HandleButtonDestroyed), new Action(this.HandleButtonClicked));
		}

		protected virtual void HandleButtonCreated(ButtonAreaButton button)
		{
		}

		protected virtual void HandleButtonDestroyed(ButtonAreaButton button)
		{
		}

		private void HandleButtonClicked()
		{
			this.Clicked();
		}

		protected abstract void Clicked();
	}
}
