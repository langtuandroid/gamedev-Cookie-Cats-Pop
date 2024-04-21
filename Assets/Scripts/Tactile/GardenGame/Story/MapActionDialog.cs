using System;
using System.Collections;
using Fibers;
using Tactile.GardenGame.Story.Dialog;
using TactileModules.Validation;
using UnityEngine;

namespace Tactile.GardenGame.Story
{
	public class MapActionDialog : MapActionResult<DialogOverlayResult>, IMapActionLocalizable
	{
		public DialogOverlayView DialogOverlayViewPrefab
		{
			get
			{
				return this.dialogOverlayViewPrefab;
			}
			set
			{
				this.dialogOverlayViewPrefab = value;
			}
		}

		public static bool IsAMapActionDialogRunning
		{
			get
			{
				return MapActionDialog.aMapActionDialogIsRunning;
			}
		}

		public string SplitScreenImagePath
		{
			get
			{
				return this.splitScreenImagePath;
			}
			set
			{
				this.splitScreenImagePath = value;
			}
		}

		public string ImagePath
		{
			get
			{
				return this.imagePath;
			}
			set
			{
				this.imagePath = value;
			}
		}

		public string Text
		{
			get
			{
				return this.text;
			}
			set
			{
				this.text = value;
			}
		}

		public int SpeakerIndex
		{
			get
			{
				return this.speakerIndex;
			}
			set
			{
				this.speakerIndex = value;
			}
		}

		public DialogActorSlot[] Characters
		{
			get
			{
				return this.characters;
			}
			set
			{
				this.characters = value;
			}
		}

		public Texture2D LoadImage()
		{
			return Resources.Load<Texture2D>(this.imagePath);
		}

		public Texture2D LoadSplitScreenImage()
		{
			return Resources.Load<Texture2D>(this.splitScreenImagePath);
		}

		protected override IEnumerator Logic(IStoryMapController map, DialogOverlayResult result)
		{
			while (MapActionDialog.IsAMapActionDialogRunning)
			{
				yield return null;
			}
			MapActionDialog.aMapActionDialogIsRunning = true;
			yield return new Fiber.OnExit(delegate()
			{
				MapActionDialog.aMapActionDialogIsRunning = false;
			});
			yield return map.Dialog.PlayDialog(this.characters, this.speakerIndex, L.Get(this.text), this.imagePath, this.splitScreenImagePath, result, this.DialogOverlayViewPrefab);
			yield break;
		}

		string IMapActionLocalizable.GetLocalizableText()
		{
			return this.Text;
		}

		string IMapActionLocalizable.GetContext()
		{
			if (this.speakerIndex >= 0 && this.speakerIndex < this.Characters.Length)
			{
				DialogActorSlot dialogActorSlot = this.Characters[this.speakerIndex];
				string arg = (!(dialogActorSlot.character != null)) ? "?" : dialogActorSlot.character.Name;
				return string.Format("{0}_{1}", arg, dialogActorSlot.poseId);
			}
			return "?";
		}

		[SerializeField]
		private string imagePath;

		[SerializeField]
		private string text;

		[SerializeField]
		private int speakerIndex;

		[SerializeField]
		private DialogActorSlot[] characters;

		[SerializeField]
		private string splitScreenImagePath;

		[SerializeField]
		[OptionalSerializedField]
		private DialogOverlayView dialogOverlayViewPrefab;

		private static bool aMapActionDialogIsRunning;
	}
}
