using System;
using System.Collections.Generic;
using UnityEngine;

namespace NinjaUI
{
	public class DizzyStars : MonoBehaviour
	{
		public void Initialize()
		{
			this.element = this.GetElement();
			for (int i = 0; i < this.numStars; i++)
			{
				UISprite uisprite = new GameObject(i.ToString()).AddComponent<UISprite>();
				uisprite.Atlas = this.atlas;
				uisprite.SpriteName = this.spriteName;
				this.sprites.Add(uisprite);
				uisprite.transform.parent = base.transform;
			}
		}

		private void Update()
		{
			this.radius = this.element.Size * 0.5f;
			this.radius.z = 0.1f;
			for (int i = 0; i < this.sprites.Count; i++)
			{
				UISprite uisprite = this.sprites[i];
				uisprite.transform.localScale = Vector3.one * this.spriteSize;
				float f = this.speed * Time.timeSinceLevelLoad + (float)i / (float)this.sprites.Count * 3.14159274f * 2f;
				uisprite.transform.localPosition = new Vector3(Mathf.Sin(f) * this.radius.x, Mathf.Cos(f) * this.radius.y, Mathf.Cos(f) * this.radius.z);
			}
		}

		public Vector3 radius = Vector3.one;

		public float speed = 1f;

		public string spriteName;

		public UIAtlas atlas;

		public float yFactor = 0.1f;

		public int numStars = 5;

		public float spriteSize = 50f;

		private UIElement element;

		private List<UISprite> sprites = new List<UISprite>();
	}
}
