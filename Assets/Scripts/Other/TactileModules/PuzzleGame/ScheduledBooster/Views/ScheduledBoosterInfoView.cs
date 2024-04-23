using System;
using JetBrains.Annotations;
using TactileModules.Foundation;
using TactileModules.PuzzleGame.ScheduledBooster.Model;
using TactileModules.PuzzleGame.ScheduledBooster.UI;
using UnityEngine;

namespace TactileModules.PuzzleGame.ScheduledBooster.Views
{
	public class ScheduledBoosterInfoView : UIView
	{
		private ScheduledBoosters ScheduledBoosters
		{
			get
			{
				return ManagerRepository.Get<ScheduledBoosterSystem>().ScheduledBoosters;
			}
		}

		private IScheduledBooster Booster
		{
			get
			{
				return this.ScheduledBoosters.GetBooster(this.boosterType);
			}
		}

		protected override void ViewLoad(object[] parameters)
		{
			if (parameters.Length > 0)
			{
				this.boosterType = (string)parameters[0];
			}
			GameObject prefab = this.CreateBoosterInfoPrefab();
			this.AssignPrefabToPivot(prefab);
			UISafeTimer uisafeTimer = new UISafeTimer(base.gameObject, new Action(this.UpdateTimerLabel), 1f);
			uisafeTimer.Run();
		}

		private GameObject CreateBoosterInfoPrefab()
		{
			GameObject boosterInfoPrefab = this.Booster.Definition.boosterInfoPrefab;
			return UnityEngine.Object.Instantiate<GameObject>(boosterInfoPrefab);
		}

		private void AssignPrefabToPivot(GameObject prefab)
		{
			prefab.transform.parent = this.spawnPivot;
			prefab.transform.localPosition = Vector3.zero;
			prefab.SetLayerRecursively(base.gameObject.layer);
		}

		private void UpdateTimerLabel()
		{
			this.timer.text = this.ScheduledBoosters.GetBooster(this.boosterType).GetTimeRemainingAsFormattedString();
		}

		[UsedImplicitly]
		private void OkButtonClicked(UIEvent e)
		{
			base.Close(1);
		}

		[UsedImplicitly]
		private void CancelClicked(UIEvent e)
		{
			base.Close(0);
		}

		[UsedImplicitly]
		private void DismissClicked(UIEvent e)
		{
			base.Close(0);
		}

		[SerializeField]
		private UILabel timer;

		[SerializeField]
		private Transform spawnPivot;

		private string boosterType;
	}
}
