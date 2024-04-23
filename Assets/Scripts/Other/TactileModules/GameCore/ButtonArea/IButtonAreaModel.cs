using System;

namespace TactileModules.GameCore.ButtonArea
{
	public interface IButtonAreaModel
	{
		void RegisterButton(ButtonAreaButton buttonPrefab, Action<ButtonAreaButton> onCreated, Action<ButtonAreaButton> onDestroyed, Action onClicked);

		void RegisterButton(ButtonAreaCategory category, ButtonAreaButton buttonPrefab, Action<ButtonAreaButton> onCreated, Action<ButtonAreaButton> onDestroyed, Action onClicked);

		void UnregisterButton(ButtonAreaButton buttonPrefab);

		void PushArea(ButtonArea buttonArea);

		void PopArea(ButtonArea buttonArea);

		event Action<ButtonAreaButton> ButtonCreated;

		event Action<ButtonAreaButton> ButtonDestroyed;

		event Action<ButtonAreaButton, ButtonArea> ButtonChangedArea;
	}
}
