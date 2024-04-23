using System;
using System.Collections.Generic;
using UnityEngine;

namespace TactileModules.PuzzleGame.SlidesAndLadders.UI
{
	public class SlidesAndLaddersSpline : MonoBehaviour
	{
		public Spline2 Spline { get; set; }

		public List<Transform> Nodes
		{
			get
			{
				return this.nodes;
			}
			set
			{
				this.nodes = value;
			}
		}

		public Vector3 RewardPosition
		{
			get
			{
				return this.rewardPosition;
			}
		}

		private void OnEnable()
		{
			this.Spline = this.ConstructSpline();
		}

		public Spline2 ConstructSpline()
		{
			Spline2 spline = new Spline2();
			for (int i = 0; i < this.nodes.Count; i++)
			{
				spline.Nodes.Add(this.nodes[i].localPosition);
			}
			spline.CalculateSpline();
			return spline;
		}

		[SerializeField]
		[HideInInspector]
		private List<Transform> nodes = new List<Transform>();

		[SerializeField]
		[HideInInspector]
		private Vector3 rewardPosition;

		[SerializeField]
		private MapBuilderSlideReward mapReward;
	}
}
