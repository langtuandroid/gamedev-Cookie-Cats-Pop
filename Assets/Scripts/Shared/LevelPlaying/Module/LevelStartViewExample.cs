using System;
using System.Collections.Generic;
using TactileModules.PuzzleCore.LevelPlaying;
using UnityEngine;

namespace Shared.LevelPlaying.Module
{
	public class LevelStartViewExample : LevelStartView
	{
		public override void Initialize(LevelProxy levelProxy, bool isRetrying, IPlayFlowContext context)
		{
			this.levelName.text = context.GetLevelDescriptionForEndUser();
		}

		private void PressedPlay(UIEvent b)
		{
			base.InvokePlayButtonClicked(new List<SelectedBooster>());
		}

		private void PressedClose(UIEvent b)
		{
			base.InvokeDismissButtonClicked();
		}

		[SerializeField]
		private UILabel levelName;
	}
}
