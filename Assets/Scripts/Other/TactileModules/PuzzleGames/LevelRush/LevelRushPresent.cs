using System;
using System.Collections;
using System.Diagnostics;
using JetBrains.Annotations;
using NinjaUI;
using UnityEngine;

namespace TactileModules.PuzzleGames.LevelRush
{
	[RequireComponent(typeof(UIButton))]
	public class LevelRushPresent : ExtensibleVisual<LevelRushPresent.IExtension>
	{
		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<LevelRushPresent> Clicked;



		public int RewardIndex { get; private set; }

		public void Initialize(LevelRushPresent.ILabelDataProvider dataProvider, int rewardIndex)
		{
			this.RewardIndex = rewardIndex;
			this.dataProvider = dataProvider;
			if (base.Extension != null)
			{
				base.Extension.Initialize(this);
			}
		}

		public IEnumerator AnimateDrop(Vector3 dropOffset)
		{
			base.transform.position += dropOffset;
			yield return FiberHelper.RunParallel(new IEnumerator[]
			{
				FiberAnimation.MoveTransform(base.transform, base.transform.position, base.transform.position - dropOffset, this.dropCurve, 0.3f),
				FiberAnimation.ScaleTransform(base.transform, Vector3.zero, Vector3.one, this.dropScaleCurve, 0.3f)
			});
			yield return this.OnDropAnimationComplete();
			yield break;
		}

		private IEnumerator OnDropAnimationComplete()
		{
			if (base.Extension != null)
			{
				base.Extension.OnDropAnimationCompleted();
			}
			yield return CameraShaker.ShakeDecreasing(0.3f, 10f, 30f, 0f, false);
			yield break;
		}

		[UsedImplicitly]
		private void ShowInfo(UIEvent e)
		{
			this.Clicked(this);
		}

		private void Update()
		{
			int secondsLeft = this.dataProvider.GetSecondsLeft();
			bool flag = this.dataProvider.IsRewardOnNextLevelToComplete(this.RewardIndex);
			this.timerLabel.text = L.FormatSecondsAsColumnSeparated(secondsLeft, (!flag) ? L.Get("Ended") : L.Get("Last chance!"), false);
		}

		[SerializeField]
		private UILabel timerLabel;

		[SerializeField]
		private AnimationCurve dropCurve;

		[SerializeField]
		private AnimationCurve dropScaleCurve;

		private LevelRushPresent.ILabelDataProvider dataProvider;

		public interface ILabelDataProvider
		{
			int GetSecondsLeft();

			bool IsRewardOnNextLevelToComplete(int rewardIndex);
		}

		public interface IExtension
		{
			void Initialize(LevelRushPresent present);

			void OnDropAnimationCompleted();
		}
	}
}
