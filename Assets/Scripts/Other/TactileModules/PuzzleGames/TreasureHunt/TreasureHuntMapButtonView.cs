using System;
using System.Diagnostics;
using JetBrains.Annotations;
using TactileModules.FeatureManager;
using UnityEngine;

namespace TactileModules.PuzzleGames.TreasureHunt
{
    public class TreasureHuntMapButtonView : UIView
    {
        ////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public event Action ExitClicked;



        private void Update()
        {
            this.timeLeftLabel.text = TactileModules.FeatureManager.FeatureManager.GetFeatureHandler<TreasureHuntManager>().TimeLeftAsText;
            if (UIViewManager.Instance.IsEscapeKeyDownAndAvailable(base.gameObject.layer))
            {
                this.ExitClicked();
            }
        }

        [UsedImplicitly]
        private void ExitButtonClicked(UIEvent e)
        {
            this.ExitClicked();
        }

        [SerializeField]
        private UILabel timeLeftLabel;
    }
}
