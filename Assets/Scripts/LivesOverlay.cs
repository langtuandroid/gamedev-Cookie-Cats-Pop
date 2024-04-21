using System;
using System.Collections;
using Fibers;
using Tactile;
using UnityEngine;

public class LivesOverlay : UIView
{
	protected override void ViewWillDisappear()
	{
		this.animFiber.Terminate();
	}

	public void WobbleCoinBar(float soundPitch = 1f)
	{
		this.animFiber.Start(this.Animate());
	}

	public void GiveLife(Vector3 position, int visibleLives, bool isTournament, float duration, Action callback = null)
	{
		this.UpdateAnimatorSprite();
		int num = (!isTournament) ? InventoryManager.Instance.Lives : TournamentManager.Instance.Lives;
		this.lifeBar.lifeCounterLabel.text = (num - visibleLives).ToString();
		this.heartAnimator.GiveCoins(position, visibleLives, duration, null, delegate
		{
			this.lifeBar.lifeCounterLabel.text = ((!isTournament) ? InventoryManager.Instance.Lives : TournamentManager.Instance.Lives).ToString();
			if (callback != null)
			{
				callback();
			}
		});
	}

	private IEnumerator Animate()
	{
		float dur = 0.4f;
		float scale = 1.4f;
		Vector3 scaleBegin = new Vector3(1f, 1f, 1f);
		Vector3 scaleEnd = new Vector3(scale, scale, 1f);
		yield return FiberAnimation.ScaleTransform(this.lifeBar.transform, scaleBegin, scaleEnd, this.scaleCurve, dur);
		yield break;
	}

	private void LifeButtonClicked(UIEvent e)
	{
		if (this.OnLifeButtonClicked != null)
		{
			this.OnLifeButtonClicked();
		}
	}

	public void ChangeLivesType(InventoryItem type)
	{
		this.lifeBar.LivesType = type;
		this.UpdateAnimatorSprite();
	}

	private void UpdateAnimatorSprite()
	{
		string text = this.lifeBar.LivesType;
		if (text != null)
		{
			if (!(text == "TournamentLife"))
			{
				if (text == "Life")
				{
					this.heartAnimator.sprite.SpriteName = "heart";
				}
			}
			else
			{
				this.heartAnimator.sprite.SpriteName = "heartTournament";
			}
		}
	}

	public void Refresh()
	{
		this.ChangeLivesType(this.lifeBar.LivesType);
	}

	public void OffsetLivesButton(Vector2 offset)
	{
		if (this.lifeBar != null)
		{
			this.lifeBar.transform.localPosition += new Vector3(offset.x, offset.y, 0f);
		}
	}

	public CoinsAnimator heartAnimator;

	public LifeBar lifeBar;

	private Fiber animFiber = new Fiber();

	public AnimationCurve scaleCurve;

	public Action OnLifeButtonClicked;
}
