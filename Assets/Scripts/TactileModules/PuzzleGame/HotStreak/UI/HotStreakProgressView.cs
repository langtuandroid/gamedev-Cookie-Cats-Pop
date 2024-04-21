using System;
using System.Collections;
using Fibers;
using TactileModules.FeatureManager;
using TactileModules.Validation;
using UnityEngine;

namespace TactileModules.PuzzleGame.HotStreak.UI
{
    public class HotStreakProgressView : UIView
    {
        protected HotStreakManager Manager
        {
            get
            {
                return TactileModules.FeatureManager.FeatureManager.GetFeatureHandler<HotStreakManager>();
            }
        }

        protected override void ViewLoad(object[] parameters)
        {
            this.updateFiber.Start(this.UpdateTimer());
            this.SetProgress(this.CalculateProgressAsPercentage());
            this.description.text = L.Get("Beat levels without losing and get a boost!");
        }

        protected virtual void SetProgress(float progress)
        {
            if (this.progressBar != null)
            {
                this.progressBar.fillAmount = progress;
            }
        }

        private float CalculateProgressAsPercentage()
        {
            if (this.Manager.GetTiers().Count == 0)
            {
                return 0f;
            }
            int progress = this.Manager.Progress;
            float num = (float)(this.Manager.CurrentTierIndex + 1) / (float)this.Manager.GetTiers().Count;
            if (num < 1f)
            {
                float num2 = (float)progress - (float)this.Manager.CurrentTier.RequiredWins;
                float num3 = (float)this.Manager.GetTiers()[this.Manager.CurrentTierIndex + 1].RequiredWins - (float)this.Manager.CurrentTier.RequiredWins;
                int count = this.Manager.GetTiers().Count;
                float num4 = num2 / num3 / (float)count;
                num += num4;
            }
            return num;
        }

        protected override void ViewDidAppear()
        {
            this.animationFiber.Start(this.PlayCurrentTierAnimation());
        }

        protected override void ViewDidDisappear()
        {
            this.updateFiber.Terminate();
            this.animationFiber.Terminate();
        }

        private IEnumerator UpdateTimer()
        {
            for (; ; )
            {
                this.timerLabel.text = this.Manager.GetTimeRemainingForHotStreakAsString();
                yield return FiberHelper.Wait(1f, (FiberHelper.WaitFlag)0);
            }
            yield break;
        }

        protected virtual IEnumerator PlayCurrentTierAnimation()
        {
            if (this.Manager.CurrentTierIndex >= 0)
            {
                yield return FiberAnimation.ScaleTransform(this.tierPivots[this.Manager.CurrentTierIndex].transform, Vector3.zero, Vector3.one, this.curve, 1f);
            }
            yield break;
        }

        protected void Dismiss(UIEvent e)
        {
            base.Close(0);
        }

        [SerializeField]
        private UILabel timerLabel;

        [SerializeField]
        protected UILabel description;

        [SerializeField]
        protected GameObject[] tierPivots;

        [SerializeField]
        [OptionalSerializedField]
        protected UIGridLayout[] rewardGrids;

        [SerializeField]
        [OptionalSerializedField]
        private UIFilledSprite progressBar;

        [SerializeField]
        protected AnimationCurve curve;

        private Fiber updateFiber = new Fiber();

        private Fiber animationFiber = new Fiber();
    }
}
