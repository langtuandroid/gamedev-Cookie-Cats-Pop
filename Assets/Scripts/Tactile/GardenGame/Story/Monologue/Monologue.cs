using System;
using UnityEngine;

namespace Tactile.GardenGame.Story.Monologue
{
	[Serializable]
	public class Monologue
	{
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

		public string GetTextLocalized()
		{
			return L.Get(this.text);
		}

		[SerializeField]
		private string text;
	}
}
