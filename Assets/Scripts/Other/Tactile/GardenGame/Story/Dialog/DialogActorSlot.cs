using System;

namespace Tactile.GardenGame.Story.Dialog
{
	[Serializable]
	public class DialogActorSlot
	{
		public bool Equals(DialogActorSlot other)
		{
			return this.character == other.character && this.poseId == other.poseId;
		}

		public string Name
		{
			get
			{
				return (!(this.character != null)) ? "None" : (this.character.Name + " / " + this.poseId);
			}
		}

		public DialogCharacter character;

		public string poseId;
	}
}
