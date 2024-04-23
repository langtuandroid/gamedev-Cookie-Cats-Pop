using System;
using Shared.PiggyBank.Module.Interfaces;
using UnityEngine;

namespace TactileModules.PuzzleGame.PiggyBank.UI
{
	public class PiggyBankCollector : ExtensibleVisual<IPiggyBankCollector>
	{
		private void Awake()
		{
			base.gameObject.SetActive(false);
		}

		public void Activate()
		{
			if (base.Extension != null)
			{
				base.Extension.Activate(this.animationName);
			}
		}

		[SerializeField]
		private string animationName = "PiggieBankJumpIn";
	}
}
