using System;
using System.Collections.Generic;

namespace TactileModules.RuntimeTools.Random
{
	public class SeededLottery<T> : Lottery<T>
	{
		public SeededLottery(IRandom random)
		{
			this.random = random;
		}

		public SeededLottery(IRandom random, Lottery<T> source) : base(source)
		{
			this.random = random;
		}

		public SeededLottery(IRandom random, ICollection<T> source) : base(source)
		{
			this.random = random;
		}

		protected override float GetNextRandomFloat()
		{
			return this.random.GetNextFloat();
		}

		private IRandom random;
	}
}
