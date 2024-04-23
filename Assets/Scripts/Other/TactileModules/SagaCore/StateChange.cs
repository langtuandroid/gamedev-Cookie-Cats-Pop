using System;

namespace TactileModules.SagaCore
{
	internal class StateChange
	{
		public bool IsMoving
		{
			get
			{
				return this.oldDotIndex >= 0 && this.newDotIndex >= 0 && this.oldDotIndex != this.newDotIndex;
			}
		}

		public string id;

		public int oldDotIndex;

		public int newDotIndex;
	}
}
