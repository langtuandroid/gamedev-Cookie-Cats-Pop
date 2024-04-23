using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace TactileModules.GameCore.ButtonArea
{
	public class ButtonAreaModel : IButtonAreaModel
	{
		public ButtonAreaModel()
		{
			this.registeredButtons = new List<ButtonAreaModel.RegisteredButton>();
		}

		public void RegisterButton(ButtonAreaButton buttonPrefab, Action<ButtonAreaButton> onCreated, Action<ButtonAreaButton> onDestroyed, Action onClicked)
		{
			this.RegisterButton(string.Empty, buttonPrefab, onCreated, onDestroyed, onClicked);
		}

		public void RegisterButton(ButtonAreaCategory category, ButtonAreaButton buttonPrefab, Action<ButtonAreaButton> onCreated, Action<ButtonAreaButton> onDestroyed, Action onClicked)
		{
			ButtonAreaModel.RegisteredButton item = new ButtonAreaModel.RegisteredButton(this)
			{
				Category = category,
				Prefab = buttonPrefab,
				Created = onCreated,
				Destroyed = onDestroyed,
				Clicked = onClicked
			};
			this.registeredButtons.Add(item);
		}

		public void UnregisterButton(ButtonAreaButton buttonPrefab)
		{
			for (int i = 0; i < this.registeredButtons.Count; i++)
			{
				if (this.registeredButtons[i].Prefab == buttonPrefab)
				{
					this.registeredButtons.RemoveAt(i);
					break;
				}
			}
		}

		public void PushArea(ButtonArea buttonArea)
		{
			foreach (ButtonAreaModel.RegisteredButton registeredButton in this.registeredButtons)
			{
				registeredButton.PushArea(buttonArea);
			}
		}

		public void PopArea(ButtonArea buttonArea)
		{
			foreach (ButtonAreaModel.RegisteredButton registeredButton in this.registeredButtons)
			{
				registeredButton.PopArea(buttonArea);
			}
		}

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<ButtonAreaButton> ButtonCreated;

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<ButtonAreaButton> ButtonDestroyed;

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<ButtonAreaButton, ButtonArea> ButtonChangedArea;

		private readonly List<ButtonAreaModel.RegisteredButton> registeredButtons;

		private class RegisteredButton
		{
			public RegisteredButton(ButtonAreaModel model)
			{
				this.model = model;
				this.areas = new List<ButtonArea>();
			}

			public ButtonAreaButton Prefab { get; set; }

			public ButtonAreaCategory Category { get; set; }

			public ButtonAreaButton Instance { get; private set; }

			private bool Match(ButtonArea area)
			{
				if (area.IsInstanceArea)
				{
					if (!area.PrefabMatch(this.Prefab))
					{
						return false;
					}
				}
				else if (area.Category != this.Category)
				{
					return false;
				}
				return true;
			}

			public void PushArea(ButtonArea area)
			{
				if (!this.Match(area))
				{
					return;
				}
				this.areas.Add(area);
				area.Destroyed += delegate()
				{
					if (this.Instance != null)
					{
						this.PopArea(area);
					}
				};
				if (this.areas.Count == 1)
				{
					if (this.Prefab == null)
					{
						throw new Exception("ButtonArea.PushArea with null prefab. area=" + area.name);
					}
					this.Instance = UnityEngine.Object.Instantiate<ButtonAreaButton>(this.Prefab);
					this.Instance.Clicked += delegate()
					{
						if (this.Clicked != null)
						{
							this.Clicked();
						}
					};
					if (this.Created != null)
					{
						this.Created(this.Instance);
					}
					if (this.model.ButtonCreated != null)
					{
						this.model.ButtonCreated(this.Instance);
					}
				}
				if (this.model.ButtonChangedArea != null)
				{
					this.model.ButtonChangedArea(this.Instance, area);
				}
			}

			public void PopArea(ButtonArea area)
			{
				if (!this.Match(area))
				{
					return;
				}
				this.areas.Remove(area);
				if (this.areas.Count == 0)
				{
					if (this.model.ButtonDestroyed != null)
					{
						this.model.ButtonDestroyed(this.Instance);
					}
					if (this.Destroyed != null)
					{
						this.Destroyed(this.Instance);
					}
					if (Application.isPlaying)
					{
						UnityEngine.Object.Destroy(this.Instance);
					}
					else
					{
						UnityEngine.Object.DestroyImmediate(this.Instance);
					}
					this.Instance = null;
				}
				else if (this.model.ButtonChangedArea != null)
				{
					this.model.ButtonChangedArea(this.Instance, this.areas[this.areas.Count - 1]);
				}
			}

			public Action<ButtonAreaButton> Created;

			public Action<ButtonAreaButton> Destroyed;

			public Action Clicked;

			private readonly ButtonAreaModel model;

			private readonly List<ButtonArea> areas;
		}
	}
}
