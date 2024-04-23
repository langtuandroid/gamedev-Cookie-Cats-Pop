using System;
using UnityEngine;

public class CannonTracerDots : MonoBehaviour
{
	public void Initialize(Transform boardRoot)
	{
		this.boardRoot = boardRoot;
		GameObject gameObject = new GameObject("DotsPool");
		gameObject.transform.parent = base.transform;
		gameObject.transform.localPosition = Vector3.back;
		this.dotsPool = PoolManager.Pools.Create("dots", gameObject);
		this.cannon.AimingStarted += delegate(AimingState aim)
		{
			this.updateDots = true;
		};
		this.cannon.AimingEnded += delegate(AimingState aim)
		{
			this.updateDots = false;
			this.dotsPool.DespawnAll();
		};
	}

	private void Update()
	{
		if (this.updateDots)
		{
			this.CalculateDots(this.cannon.CurrentAimingState);
		}
	}

	private void CalculateDots(AimingState aimingState)
	{
		this.dotsPool.DespawnAll();
		TutorialHintArrowView tutorialHintArrowView = UIViewManager.Instance.FindView<TutorialHintArrowView>();
		int layer = (!(tutorialHintArrowView == null)) ? tutorialHintArrowView.gameObject.layer : base.gameObject.layer;
		foreach (AimInfo aimInfo in aimingState.Aims)
		{
			if (aimInfo.IsValidForShot)
			{
				Vector2 vector = aimInfo.aimOriginInBoardSpace;
				bool flag = true;
				bool flag2 = false;
				float num = Mathf.Repeat(Time.timeSinceLevelLoad, 1f);
				foreach (Vector2 vector2 in aimInfo.collisionPoints)
				{
					Vector2 vector3 = vector2 - vector;
					int num2 = Mathf.FloorToInt(vector3.magnitude / SingletonAsset<LevelVisuals>.Instance.unitsPerDot);
					Vector2 normalized = vector3.normalized;
					float num3 = vector3.magnitude / (float)num2;
					for (int i = 0; i < num2; i++)
					{
						float num4 = num3 * ((float)i + num);
						if (!flag && !this.cannon.HasSuperAim && num4 > SingletonAsset<LevelVisuals>.Instance.tracerBounceLength)
						{
							flag2 = true;
							break;
						}
						Transform transform = this.dotsPool.Spawn(SingletonAsset<LevelVisuals>.Instance.aimDot);
						transform.gameObject.SetLayerRecursively(layer);
						transform.transform.position = this.boardRoot.TransformPoint(vector + normalized * num4);
						Color color = (this.ColorRetriever != null) ? this.ColorRetriever(num4) : Color.white;
						transform.GetComponent<UISprite>().Color = color;
					}
					if (flag2)
					{
						break;
					}
					vector = vector2;
					flag = false;
				}
			}
		}
	}

	public GameCannon cannon;

	public Func<float, Color> ColorRetriever;

	private SpawnPool dotsPool;

	private Transform boardRoot;

	private bool updateDots;
}
