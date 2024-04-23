using System;
using System.Collections.Generic;
using Shared.PiggyBank.Module.Interfaces;
using TactileModules.PuzzleGame.PiggyBank.Interfaces;
using UnityEngine;

namespace TactileModules.PuzzleGame.PiggyBank.Models
{
	public class PiggyBankRewards : IPiggyBankRewards
	{
		public PiggyBankRewards(IPiggyBankProvider provider, IItemsProvider itemsProvider, IDataProvider<PiggyBankPersistableState> persistableData, IDataProvider<PiggyBankConfig> config)
		{
			this.provider = provider;
			this.persistableData = persistableData;
			this.config = config;
			this.itemsProvider = itemsProvider;
		}

		public int CollectedCoins
		{
			get
			{
				return this.persistableData.Get().CollectedCoins;
			}
			private set
			{
				this.persistableData.Get().CollectedCoins = Mathf.Clamp(value, 0, this.persistableData.Get().Capacity);
				this.SavePersistabelState();
			}
		}

		public int Capacity
		{
			get
			{
				if (this.persistableData.Get().Capacity < this.config.Get().InitialCapacity)
				{
					this.Capacity = this.config.Get().InitialCapacity;
					this.SavePersistabelState();
				}
				return this.persistableData.Get().Capacity;
			}
			private set
			{
				this.persistableData.Get().Capacity = Mathf.Clamp(value, 0, this.MaxCapacity);
				this.SavePersistabelState();
			}
		}

		public bool PaidOpeningReady()
		{
			return this.CollectedCoins >= this.config.Get().CoinsRequiredForPaidOpening;
		}

		public int MaxCapacity
		{
			get
			{
				return this.config.Get().MaximumCapacity;
			}
		}

		public int CapacityIncrease
		{
			get
			{
				return this.config.Get().CapacityIncrease;
			}
		}

		public void IncreaseCapacity()
		{
			this.Capacity += this.config.Get().CapacityIncrease;
		}

		public int AvailableCapacityIncrease
		{
			get
			{
				return Mathf.Min(this.CapacityIncrease, this.MaxCapacity - this.Capacity);
			}
		}

		public int FreeOpenInterval
		{
			get
			{
				return this.config.Get().Interval;
			}
		}

		public int CoinsRequiredForPaidOpening
		{
			get
			{
				return this.config.Get().CoinsRequiredForPaidOpening;
			}
		}

		public float CoinProgress
		{
			get
			{
				return (float)this.CollectedCoins / (float)this.Capacity;
			}
		}

		public void ResetCoins()
		{
			this.CollectedCoins = 0;
		}

		public void AddPlayedCoins()
		{
			this.AddCoins(this.config.Get().CoinsPerLevel);
		}

		public void AddBoosterCoins()
		{
			this.AddCoins(this.config.Get().CoinsPerBooster);
		}

		private void AddCoins(int coins)
		{
			this.CollectedCoins += coins;
		}

		public void GiveContentToPlayer()
		{
			this.itemsProvider.GiveContentToPlayer(this.CollectedCoins);
		}

		public void GiveOfferItemsToPlayer(List<ItemAmount> items)
		{
			this.itemsProvider.GiveOfferItemsToPlayer(items);
		}

		public void SavePersistabelState()
		{
			this.provider.SavePersistableState();
		}

		private readonly IPiggyBankProvider provider;

		private readonly IItemsProvider itemsProvider;

		private readonly IDataProvider<PiggyBankPersistableState> persistableData;

		private readonly IDataProvider<PiggyBankConfig> config;
	}
}
