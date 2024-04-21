using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using Tactile;
using UnityEngine;

namespace TactileModules.PuzzleGames.StarTournament.Views
{
	public class StarTournamentRewardView : UIView
	{
		private ButtonWithTitle claimButtonEnabled
		{
			get
			{
				return this.claimButton.GetInstance<ButtonWithTitle>();
			}
		}

		public void Initialize(StarTournamentConfig.Reward reward)
		{
			int rank = reward.Rank;
			this.descriptionLabel.text = string.Format(L.Get(this.formattedRankText), rank);
			this.SetIcon(rank);
			this.claimButtonEnabled.Disabled = false;
			this.itemReward = new List<ItemAmount>(reward.Items);
			this.rewardGrid.Initialize(this.itemReward, true);
		}

		protected override void ViewDidAppear()
		{
			this.currencyOverlay = base.ObtainOverlay<CurrencyOverlay>();
		}

		protected override void ViewWillDisappear()
		{
			this.animFiber.Terminate();
		}

		protected override void ViewDidDisappear()
		{
			base.ReleaseOverlay<CurrencyOverlay>();
		}

		private void SetIcon(int rank)
		{
			switch (rank)
			{
			case 1:
				this.trophyIcon.TextureResource = "StarTournament/StarTrophys_Gold";
				break;
			case 2:
				this.trophyIcon.TextureResource = "StarTournament/StarTrophys_Silver";
				break;
			case 3:
				this.trophyIcon.TextureResource = "StarTournament/StarTrophys_Bronze";
				break;
			default:
				this.trophyIcon.TextureResource = "StarTournament/StarTrophys_Bronze";
				break;
			}
		}

		private void ClaimClicked(UIEvent e)
		{
			FiberCtrl.Pool.Run(this.AnimateClaimSuccess(), false);
		}

		private IEnumerator AnimateClaimSuccess()
		{
			this.claimButtonEnabled.Disabled = true;
			foreach (ItemAmount itemAmount in this.itemReward)
			{
				InventoryManager.Instance.Add(itemAmount.ItemId, itemAmount.Amount, "StarTournamentReward");
			}
			yield return this.rewardGrid.Animate(false, true);
			base.Close(0);
			yield break;
		}

		private IEnumerator AnimateCoins(int numCoins, Vector3 spawnPos)
		{
			UICamera.DisableInput();
			Vector3 sourcePos = spawnPos;
			Vector3 destPos = new Vector3(0f, 0f, -150f);
			sourcePos.z -= 1f;
			destPos.z -= 1f;
			Vector3 scaleSource = new Vector3(0.5f, 0.5f, 1f);
			Vector3 scaleDest = new Vector3(1f, 1f, 1f);
			for (int i = 0; i < numCoins; i++)
			{
				GameObject go = UnityEngine.Object.Instantiate<GameObject>(this.coinAnimationPrefab, spawnPos, Quaternion.identity);
				go.SetLayerRecursively(base.gameObject.layer);
				go.transform.SetParent(this.currencyOverlay.coinButton.transform);
				yield return FiberHelper.RunParallel(new IEnumerator[]
				{
					FiberAnimation.MoveLocalTransform(go.transform, go.transform.localPosition, destPos, this.travelCurve, this.coinAnimationDuration),
					FiberAnimation.ScaleTransform(go.transform, scaleSource, scaleDest, this.scaleCurve, this.coinAnimationDuration)
				});
				UnityEngine.Object.Destroy(go);
			}
			yield return FiberHelper.Wait(0.1f, (FiberHelper.WaitFlag)0);
			UICamera.EnableInput();
			yield break;
		}

		[SerializeField]
		private UILabel descriptionLabel;

		[SerializeField]
		private UIResourceQuad trophyIcon;

		[SerializeField]
		private UIInstantiator claimButton;

		[SerializeField]
		private RewardGrid rewardGrid;

		[SerializeField]
		private string formattedRankText = "Buzztastic! You achieved rank {0} in the Star Tournament!";

		[Header("Fancy Animation")]
		[SerializeField]
		private GameObject coinAnimationPrefab;

		[SerializeField]
		private float coinAnimationDuration = 0.1f;

		[SerializeField]
		private AnimationCurve travelCurve;

		[SerializeField]
		private AnimationCurve scaleCurve;

		private List<ItemAmount> itemReward = new List<ItemAmount>();

		private readonly Fiber animFiber = new Fiber();

		private CurrencyOverlay currencyOverlay;
	}
}
