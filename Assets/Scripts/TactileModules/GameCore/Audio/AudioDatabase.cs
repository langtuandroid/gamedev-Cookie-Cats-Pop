using System;
using System.Collections.Generic;
using TactileModules.PuzzleGames.GameCore;
using UnityEngine;

namespace TactileModules.GameCore.Audio
{
	public class AudioDatabase : ScriptableObject
	{
		public ISoundDefinition GetInventoryAudio(InventoryItem inventoryItem, AudioDatabase.InventoryItemSoundType type)
		{
			AudioDatabase.InventoryAudio inventoryAudio = this.inventoryAudio.Find((AudioDatabase.InventoryAudio i) => i.inventoryItem == inventoryItem && i.type == type);
			if (inventoryAudio != null)
			{
				return inventoryAudio.soundDefinition;
			}
			switch (type)
			{
			case AudioDatabase.InventoryItemSoundType.UsedItemStarted:
				return this.defaultUsedItemStarted;
			case AudioDatabase.InventoryItemSoundType.UsedItemCompleted:
				return this.defaultUsedItemCompleted;
			case AudioDatabase.InventoryItemSoundType.ReceivedItemStarted:
				return this.defaultReceivedItemStarted;
			case AudioDatabase.InventoryItemSoundType.ReceivedItemCompleted:
				return this.defaultReceivedItemCompleted;
			default:
				return new SoundDefinition();
			}
		}

		public ISoundDefinition ViewSlideIn
		{
			get
			{
				return this.viewSlideIn;
			}
		}

		public ISoundDefinition ViewSlideOut
		{
			get
			{
				return this.viewSlideOut;
			}
		}

		public ISoundDefinition DefaultButtonClick
		{
			get
			{
				return this.defaultButtonClick;
			}
		}

		public ISoundDefinition ButtonDismissClick
		{
			get
			{
				return this.buttonDismissClick;
			}
		}

		public ISoundDefinition ButtonConfirmClick
		{
			get
			{
				return this.buttonConfirmClick;
			}
		}

		public IMusicTrackComponent FindFullScreenSoundDefinition(IFullScreenOwner owner)
		{
			string type = owner.GetType().Name;
			return this.fullScreenMusic.Find((FullScreenSoundDefinition f) => f.Type == type);
		}

		[SerializeField]
		private List<FullScreenSoundDefinition> fullScreenMusic = new List<FullScreenSoundDefinition>();

		[SerializeField]
		private SoundDefinition viewSlideIn;

		[SerializeField]
		private SoundDefinition viewSlideOut;

		[SerializeField]
		private SoundDefinition defaultButtonClick;

		[SerializeField]
		private SoundDefinition buttonDismissClick;

		[SerializeField]
		private SoundDefinition buttonConfirmClick;

		[SerializeField]
		private SoundDefinition defaultUsedItemStarted;

		[SerializeField]
		private SoundDefinition defaultUsedItemCompleted;

		[SerializeField]
		private SoundDefinition defaultReceivedItemStarted;

		[SerializeField]
		private SoundDefinition defaultReceivedItemCompleted;

		[SerializeField]
		private List<AudioDatabase.InventoryAudio> inventoryAudio = new List<AudioDatabase.InventoryAudio>();

		public enum InventoryItemSoundType
		{
			UsedItemStarted,
			UsedItemCompleted,
			ReceivedItemStarted,
			ReceivedItemCompleted
		}

		[Serializable]
		public class InventoryAudio
		{
			public InventoryItem inventoryItem;

			public SoundDefinition soundDefinition;

			public AudioDatabase.InventoryItemSoundType type;
		}
	}
}
