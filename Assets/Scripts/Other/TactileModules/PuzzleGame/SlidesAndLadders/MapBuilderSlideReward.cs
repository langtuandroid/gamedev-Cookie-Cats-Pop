using System;
using UnityEngine;

namespace TactileModules.PuzzleGame.SlidesAndLadders
{
	[ExecuteInEditMode]
	public class MapBuilderSlideReward : MonoBehaviour
	{
		public string Guid
		{
			get
			{
				return (!string.IsNullOrEmpty(this.guid)) ? this.guid : string.Empty;
			}
		}

		[SerializeField]
		[HideInInspector]
		private string guid;
	}
}
