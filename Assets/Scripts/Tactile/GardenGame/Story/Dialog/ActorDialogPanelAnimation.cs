using System;
using UnityEngine;

namespace Tactile.GardenGame.Story.Dialog
{
	public class ActorDialogPanelAnimation : ScriptableObject
	{
		public AnimationCurve characterRevealCurve;

		public AnimationCurve speechBoxScaleCurve;

		public AnimationCurve speechBoxAlphaCurve;

		public AnimationCurve characterFocusScaleCurve;

		public Color CharacterUnfocusedColor;
	}
}
