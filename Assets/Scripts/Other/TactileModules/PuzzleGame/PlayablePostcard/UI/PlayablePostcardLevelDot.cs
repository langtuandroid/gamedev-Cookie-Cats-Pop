using System;
using TactileModules.FeatureManager;
using TactileModules.Foundation;
using TactileModules.PuzzleGame.PlayablePostcard.Model;
using UnityEngine;

namespace TactileModules.PuzzleGame.PlayablePostcard.UI
{
    public class PlayablePostcardLevelDot : MapDotBase
    {
        private LevelDatabaseCollection LevelDatabaseCollection
        {
            get
            {
                return ManagerRepository.Get<LevelDatabaseCollection>();
            }
        }

        private PlayablePostcardInstanceCustomData InstanceCustomData
        {
            get
            {
                return TactileModules.FeatureManager.FeatureManager.GetFeatureHandler<PlayablePostcardHandler>().InstanceCustomData;
            }
        }

        [Instantiator.SerializeProperty]
        public override int LevelId
        {
            get
            {
                return this.levelId;
            }
            set
            {
                this.levelId = value;
                if (Application.isPlaying)
                {
                    this.UpdateUI();
                }
            }
        }

        [Instantiator.SerializeProperty]
        public float Scale
        {
            get
            {
                return this.scalePivot.localScale.x;
            }
            set
            {
                this.scalePivot.localScale = new Vector3(value, value, 1f);
            }
        }

        protected LevelProxy LevelProxy
        {
            get
            {
                PlayablePostcardLevelDatabase levelDatabase = this.LevelDatabaseCollection.GetLevelDatabase<PlayablePostcardLevelDatabase>("PlayablePostcard");
                return levelDatabase.GetLevel(this.LevelId);
            }
        }

        public override bool IsUnlocked
        {
            get
            {
                return this.levelId <= this.InstanceCustomData.FarthestCompletedLevel + 1;
            }
        }

        public override bool IsCompleted
        {
            get
            {
                return this.levelId <= this.InstanceCustomData.FarthestCompletedLevel;
            }
        }

        public override void Initialize()
        {
            this.UpdateUI();
        }

        public override void UpdateUI()
        {
            this.enabledRoot.SetActive(!this.IsCompleted);
            this.completedRoot.SetActive(this.IsCompleted);
            this.SetItemGraphic();
        }

        private void SetItemGraphic()
        {
            this.itemBackground.gameObject.SetActive(this.levelId == 0);
            this.itemCharacter.gameObject.SetActive(this.levelId == 1);
            this.itemCostume.gameObject.SetActive(this.levelId == 2);
            this.itemProp.gameObject.SetActive(this.levelId == 3);
            this.itemText.gameObject.SetActive(this.levelId == 4);
        }

        [SerializeField]
        private GameObject itemBackground;

        [SerializeField]
        private GameObject itemCharacter;

        [SerializeField]
        private GameObject itemCostume;

        [SerializeField]
        private GameObject itemProp;

        [SerializeField]
        private GameObject itemText;

        [SerializeField]
        private GameObject enabledRoot;

        [SerializeField]
        private GameObject completedRoot;

        [SerializeField]
        private Transform scalePivot;

        private int levelId;
    }
}
