using System;
using JetBrains.Annotations;
using TactileModules.FeatureManager;
using UnityEngine;

namespace TactileModules.PuzzleGames.EndlessChallenge
{
    public class EndlessChallengeEndView : UIView
    {
        private EndlessChallengeHandler Handler
        {
            get
            {
                return TactileModules.FeatureManager.FeatureManager.GetFeatureHandler<EndlessChallengeHandler>();
            }
        }

        protected override void ViewLoad(object[] parameters)
        {
            AudioManager.Instance.SetMusic(null, true);
            SingletonAsset<SoundDatabase>.Instance.levelFailed.Play();
            if (parameters != null && parameters.Length > 0)
            {
                this.currentRow = (int)parameters[0];
            }
            this.happyImage.SetActive(true);
            this.descriptionRoundScore.text = string.Format(L.Get("You reached row {0}"), this.currentRow);
            this.descriptionBestScore.text = string.Format(L.Get("Your best: {0}"), this.Handler.AllTimeHighestRow);
        }

        [UsedImplicitly]
        private void OnCloseClicked(UIEvent e)
        {
            base.Close(0);
        }

        [SerializeField]
        private GameObject happyImage;

        [SerializeField]
        private UILabel descriptionRoundScore;

        [SerializeField]
        private UILabel descriptionBestScore;

        private int currentRow;
    }
}
