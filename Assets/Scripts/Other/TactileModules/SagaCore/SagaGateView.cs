using System;
using System.Collections;
using Fibers;
using JetBrains.Annotations;
using TactileModules.FacebookExtras;
using TactileModules.Foundation;
using UnityEngine;

namespace TactileModules.SagaCore
{
    public class SagaGateView : UIView
    {
        protected override void ViewLoad(object[] parameters)
        {
            if (GateManager.Instance.HasPendingKey)
            {
                int currentGateKeys = GateManager.Instance.CurrentGateKeys;
                this.UpdateUI(currentGateKeys - 1);
                foreach (GameObject gameObject in this.hideUntilAnimated)
                {
                    gameObject.SetActive(false);
                }
            }
            else
            {
                if (GateManager.Instance.CurrentGateComplete)
                {
                    GateManager.Instance.ResetGateState();
                }
                this.UpdateUI(-1);
            }
        }

        protected override void ViewDidAppear()
        {

            if (GateManager.Instance.HasPendingKey)
            {
                GateManager.Instance.HasPendingKey = false;
                this.animFiber.Start(this.AnimateKey());
            }
        }

        protected override void ViewDidDisappear()
        {
            Fiber.TerminateIfAble(this.animFiber);
        }

        private void Update()
        {
            if (GateManager.Instance.SecondsUntilNextPlay <= 0)
            {
                this.playButton.SetActive(true);
                this.playCooldownButton.SetActive(false);
            }
            else
            {
                this.playButton.SetActive(false);
                this.playCooldownButton.SetActive(true);
                this.cooldownCounter.text = GateManager.Instance.GetFormattedTimeUntilNextPlay();
            }
        }

        private void SetCheckmarks(int amount)
        {
            for (int i = 0; i < 3; i++)
            {
                this.checkmarks[i].SetActive(i < amount);
            }
        }

        private void UpdateUI(int overrideKeyAmount = -1)
        {
            int num = GateManager.Instance.CurrentGateKeys;
            if (overrideKeyAmount > -1)
            {
                num = overrideKeyAmount;
            }
            this.SetCheckmarks(num);
            if (num == 0)
            {
                this.description.text = L.Get("Collect 3 keys to unlock the gate");
            }
            else if (num == 1)
            {
                this.description.text = L.Get("Collect 2 more keys to unlock the gate");
            }
            else if (num == 2)
            {
                this.description.text = L.Get("Only 1 more key to unlock the gate");
            }
            else
            {
                this.description.text = L.Get("Congratulations! You unlocked the gate!");
            }
        }

        private void BuyButton(UIEvent e)
        {
            if (!GateManager.Instance.CurrentGateComplete)
            {
                this.animFiber.Start(this.Buy());
            }
        }

        private IEnumerator AnimateKey()
        {
            int num = Mathf.Clamp(GateManager.Instance.CurrentGateKeys, 1, 3);
            this.UpdateUI(-1);
            if (num < 3)
            {
                foreach (GameObject gameObject in this.hideUntilAnimated)
                {
                    gameObject.SetActive(true);
                }
            }
            else
            {
                this.CloseWithProgression();
            }
            yield break;
        }

        private IEnumerator Buy()
        {
            ShopItem item = ShopManager.Instance.GetShopItem("ShopItemUnlockGate");
            UIViewManager.UIViewStateGeneric<BuyShopItemView> vs = UIViewManager.Instance.ShowView<BuyShopItemView>(new object[]
            {
                item,
                false
            });
            yield return vs.WaitForClose();
            if ((int)vs.ClosingResult == 0)
            {
                GateManager.Instance.UnlockGate();
                yield return this.DoUnlockedProgression();
            }
            yield break;
        }

        private IEnumerator DoUnlockedProgression()
        {
            base.Close(SagaGateView.Result.CloseWithProgression);
            yield break;
        }

        private void CloseWithProgression()
        {
            base.Close(SagaGateView.Result.CloseWithProgression);
        }

        [UsedImplicitly]
        private void PlayButton(UIEvent e)
        {
            base.Close(SagaGateView.Result.PlayLevel);
        }

        private void FBButton(UIEvent e)
        {
            if (ManagerRepository.Get<FacebookLoginManager>().IsLoggedInAndUserRegistered)
            {
                UIViewManager.Instance.ShowView<FacebookSelectFriendsAndRequestView>(new object[]
                {
                    FacebookSelectFriendsAndRequestView.RequestType.Key
                });
            }
            else
            {
                FiberCtrl.Pool.Run(this.DoFacebookLogin(), false);
            }
        }

        private IEnumerator DoFacebookLogin()
        {
            FacebookLoginManager facebookLoginManager = ManagerRepository.Get<FacebookLoginManager>();
            yield return facebookLoginManager.EnsureLoggedInAndUserRegistered();
            if (facebookLoginManager.IsLoggedInAndUserRegistered)
            {
                if (GateManager.Instance.PlayerOnGate)
                {
                    UIViewManager.Instance.ShowView<FacebookSelectFriendsAndRequestView>(new object[]
                    {
                        FacebookSelectFriendsAndRequestView.RequestType.Key
                    });
                }
                else
                {
                    base.Close(SagaGateView.Result.Dismiss);
                }
            }
            yield break;
        }

        private void QuestCooldownButton(UIEvent e)
        {
            UIViewManager.Instance.ShowView<MessageBoxView>(new object[]
            {
                L.Get(this.cooldownMessageTitle),
                L.Get(this.cooldownMessageText),
                L.Get("OK")
            });
        }

        private void CheatClicked(UIEvent e)
        {
        }

        private void DismissButton(UIEvent e)
        {
            if (this.animFiber.IsTerminated)
            {

                base.Close(0);
            }
        }

        [Header("Gate View")]
        public GameObject[] hideUntilAnimated;

        public UILabel description;

        public GameObject playButton;

        public GameObject playCooldownButton;

        public UILabel cooldownCounter;

        public GameObject[] checkmarks;

        public string KeyHoleUnchecked;

        public string KeyHoleChecked;

        [SerializeField]
        [LocalizedStringField]
        private string cooldownMessageTitle = "The bees are out buzzing";

        [SerializeField]
        [LocalizedStringField]
        private string cooldownMessageText = "The bees are out buzzing. Try again when they get back.";

        private Fiber animFiber = new Fiber();

        public enum Result
        {
            Dismiss,
            PlayLevel,
            CloseWithProgression
        }
    }
}
