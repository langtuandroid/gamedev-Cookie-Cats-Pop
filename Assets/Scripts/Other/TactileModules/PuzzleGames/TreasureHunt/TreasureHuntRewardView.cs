using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using JetBrains.Annotations;
using TactileModules.FeatureManager;
using UnityEngine;

namespace TactileModules.PuzzleGames.TreasureHunt
{
    public class TreasureHuntRewardView : ExtensibleView<TreasureHuntRewardView.IExtension>
    {
        protected override void ViewLoad(object[] parameters)
        {
            TreasureHuntManager featureHandler = TactileModules.FeatureManager.FeatureManager.GetFeatureHandler<TreasureHuntManager>();
            this.ClaimButtonDisabled.gameObject.SetActive(false);
            this.ClaimButton.gameObject.SetActive(true);
            if (base.Extension != null)
            {
                base.Extension.Initialize(featureHandler.Config.Rewards);
            }
        }

        protected override void ViewWillDisappear()
        {
            this.animationFiber.Terminate();
        }

        private void SetClaimButtonEnabled(bool enabled)
        {
            this.ClaimButtonDisabled.gameObject.SetActive(!enabled);
            this.ClaimButton.gameObject.SetActive(enabled);
        }

        private IEnumerator ClaimRewards()
        {
            UICamera.DisableInput();
            this.SetClaimButtonEnabled(false);
            if (base.Extension != null)
            {
                yield return base.Extension.AnimateClaim();
            }
            base.Close(1);
            UICamera.EnableInput();
            yield break;
        }

        [UsedImplicitly]
        private void Claim(UIEvent e)
        {
            if (!this.claimed)
            {
                this.claimed = true;
                this.animationFiber.Start(this.ClaimRewards());
            }
        }

        [SerializeField]
        private UIInstantiator ClaimButton;

        [SerializeField]
        private UIInstantiator ClaimButtonDisabled;

        private bool claimed;

        private readonly Fiber animationFiber = new Fiber();

        public interface IExtension
        {
            void Initialize(List<ItemAmount> rewards);

            IEnumerator AnimateClaim();
        }
    }
}
