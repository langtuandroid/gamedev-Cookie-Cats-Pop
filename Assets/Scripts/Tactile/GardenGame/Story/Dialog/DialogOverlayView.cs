using System;
using System.Collections;
using System.Diagnostics;
using Fibers;
using UnityEngine;

namespace Tactile.GardenGame.Story.Dialog
{
	public class DialogOverlayView : UIView, IDialogOverlayView
	{
		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action SkipClicked;

		protected override void ViewLoad(object[] parameters)
		{
			this.Initialize();
		}

		public void Initialize()
		{
			this.left.Initialize();
			this.right.Initialize();
		}

		public IEnumerator PlayDialog(DialogActorSlot[] slots, int speakerIndex, string text, string imagePath, string splitScreenImagePath, DialogOverlayResult result)
		{
			float totalDuration = Time.time;
			yield return FiberHelper.RunParallel(new IEnumerator[]
			{
				this.left.Animate(slots[0].character, slots[0].poseId, (speakerIndex != 0) ? null : text, (speakerIndex != 0) ? null : imagePath, null),
				this.right.Animate(slots[1].character, slots[1].poseId, (speakerIndex == 0) ? null : text, (speakerIndex == 0) ? null : imagePath, splitScreenImagePath)
			});
			float startTime = Time.time;
			yield return new Fiber.OnExit(delegate()
			{
				if (this.skipped)
				{
					result.WasSkipped = true;
				}
				result.TotalDuration = Time.time - totalDuration;
				result.SecondsWaited = Time.time - startTime;
			});
			yield return this.WaitForDismissOrTextNotAnimating();
			if (this.skipped)
			{
				yield return this.CloseDialog();
				yield break;
			}
			startTime = Time.time;
			if (this.left.IsTextAnimating || this.right.IsTextAnimating)
			{
				if (result != null)
				{
					result.WasSkipped = true;
				}
				this.left.ShowTextInstantly();
				this.right.ShowTextInstantly();
			}
			yield return this.WaitForDismiss();
			yield break;
		}

		public IEnumerator CloseDialog()
		{
			yield return FiberHelper.RunParallel(new IEnumerator[]
			{
				this.left.Animate(null, null, null, null, null),
				this.right.Animate(null, null, null, null, null)
			});
			yield break;
		}

		private IEnumerator WaitForDismissOrTextNotAnimating()
		{
			this.dismissed = false;
			while (!this.dismissed)
			{
				if (this.left.HasTextFinishedAnimating)
				{
					break;
				}
				if (this.right.HasTextFinishedAnimating)
				{
					break;
				}
				yield return null;
			}
			yield break;
		}

		private void DismissClicked(UIEvent e)
		{
			this.dismissed = true;
		}

		private void Update()
		{
			if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
			{
				this.dismissed = true;
			}
		}

		private IEnumerator WaitForDismiss()
		{
			this.dismissed = false;
			while (!this.dismissed)
			{
				yield return null;
			}
			yield break;
		}

		public void ShowTextInstantly()
		{
			this.left.ShowTextInstantly();
			this.right.ShowTextInstantly();
		}

		private void OnSkipClicked(UIEvent e)
		{
			this.DismissClicked(e);
			this.skipped = true;
			if (this.SkipClicked != null)
			{
				this.SkipClicked();
			}
		}

		[SerializeField]
		private ActorDialogPanel left;

		[SerializeField]
		private ActorDialogPanel right;

		private bool dismissed;

		private bool skipped;
	}
}
