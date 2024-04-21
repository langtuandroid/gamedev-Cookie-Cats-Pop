using System;
using System.Collections;
using Tactile.GardenGame.MapSystem.Visuals;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem
{
	public class MapSpeaker : MapComponent
	{
		public Vector3 SpeechBubbleLocation
		{
			get
			{
				return this.speechBubbleLocation;
			}
			set
			{
				this.speechBubbleLocation = value;
			}
		}

		public SpeechBubble SpeechBubblePrefab
		{
			get
			{
				return this.speechBubblePrefab;
			}
			set
			{
				this.speechBubblePrefab = value;
			}
		}

		protected override void Initialized()
		{
			this.speechBubble = UnityEngine.Object.Instantiate<SpeechBubble>(this.speechBubblePrefab);
			this.speechBubble.Hide();
			this.isLayerSet = false;
		}

		public override void Destroy()
		{
			if (this.speechBubble != null)
			{
				UnityEngine.Object.Destroy(this.speechBubble.gameObject);
			}
		}

		private void LateUpdate()
		{
			if (base.OwnerMap == null)
			{
				return;
			}
			if (!this.isLayerSet)
			{
				this.isLayerSet = true;
				this.speechBubble.gameObject.SetLayerRecursively(Mathf.RoundToInt(base.OwnerMap.GUICamera.depth));
			}
			Vector3 position = base.transform.TransformPoint(this.speechBubbleLocation);
			Vector3 position2 = base.OwnerMap.Camera.CachedCamera.WorldToViewportPoint(position);
			Vector3 position3 = base.OwnerMap.GUICamera.ViewportToWorldPoint(position2);
			position3.z = 50f;
			this.speechBubble.transform.position = position3;
		}

		public IEnumerator Speak(string text)
		{
			yield return this.speechBubble.Say(text);
			yield break;
		}

		public IEnumerator HideSpeechBubble()
		{
			yield return this.speechBubble.FadeOut();
			yield break;
		}

		public void HideSpeechBubbleInstantly()
		{
			this.speechBubble.Hide();
		}

		[SerializeField]
		private Vector3 speechBubbleLocation;

		[SerializeField]
		private SpeechBubble speechBubblePrefab;

		private MapCamera camera;

		private SpeechBubble speechBubble;

		private bool isLayerSet;
	}
}
