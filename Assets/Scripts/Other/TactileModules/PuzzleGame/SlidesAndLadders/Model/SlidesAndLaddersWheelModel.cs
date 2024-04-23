using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using TactileModules.PuzzleGame.SlidesAndLadders.Data;
using UnityEngine;

namespace TactileModules.PuzzleGame.SlidesAndLadders.Model
{
	public class SlidesAndLaddersWheelModel
	{
		public SlidesAndLaddersWheelModel(List<WheelSlot> wheelSlots, IDataProvider<SlidesAndLaddersInstanceCustomData> data)
		{
			this.wheelSlots = wheelSlots;
			this.data = data;
		}

		public WheelSlot Roll()
		{
			bool noItemsSlots = this.data.Get().NumberOfCurrentWheelSpins >= 1;
			return this.Roll(noItemsSlots);
		}

		private WheelSlot Roll(bool noItemsSlots)
		{
			List<WheelSlot> list = new List<WheelSlot>(this.wheelSlots);
			if (noItemsSlots)
			{
				list.RemoveAll((WheelSlot x) => x.stepsToAdd == 0);
			}
			WheelSlot randomWheelSlot = this.GetRandomWheelSlot(list);
			this.UpdatePersistableState(randomWheelSlot);
			return randomWheelSlot;
		}

		private WheelSlot GetRandomWheelSlot(List<WheelSlot> wheelSlots)
		{
			wheelSlots.Shuffle<WheelSlot>();
			return wheelSlots[0];
		}

		public Vector3 GetAngleForWheelSlot(WheelSlot wheelSlot)
		{
			int wheelIndex = this.wheelSlots.IndexOf(wheelSlot);
			return this.CalculateAngle(wheelIndex);
		}

		private Vector3 CalculateAngle(int wheelIndex)
		{
			float num = 360f / (float)this.wheelSlots.Count;
			float num2 = num * (float)wheelIndex;
			float num3 = num / 2f;
			Vector3 result = new Vector3(0f, 0f, num2 + num3);
			return result;
		}

		private void UpdatePersistableState(WheelSlot wheelSlot)
		{
			this.data.Get().NumberOfCurrentWheelSpins++;
			if (wheelSlot.stepsToAdd != 0)
			{
				this.data.Get().CanSpinWheel = false;
			}
		}

		public bool CanSpinWheel()
		{
			return this.data.Get().CanSpinWheel;
		}

		public void Reset()
		{
			this.data.Get().NumberOfCurrentWheelSpins = 0;
			this.data.Get().CanSpinWheel = true;
		}

		[UsedImplicitly]
		private void WheelClicked(UIEvent e)
		{
			if (this.data.Get().CanSpinWheel)
			{
				this.Roll();
			}
		}

		private readonly IDataProvider<SlidesAndLaddersInstanceCustomData> data;

		private readonly List<WheelSlot> wheelSlots;
	}
}
