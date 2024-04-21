using System;
using System.Collections;
using TactileModules.SagaCore;
using UnityEngine;

public class StoryOutroView : UIView
{
	protected override void ViewLoad(object[] parameters)
	{
		this.parameters = (StoryOutroView.StoryOutroViewParameters)parameters[0];
	}

	private void PlayClicked(UIEvent e)
	{
		this.CloseView();
	}

	private void CloseView()
	{
		this.parameters.done();
		base.Close(0);
	}

	private StoryOutroView.StoryOutroViewParameters parameters;

	public class StoryOutroViewParameters
	{
		public Action done;

		public MainMapFlow mainMapFlow;
	}

	[Serializable]
	public class ShowMap : StoryState
	{
		public override void Enter()
		{
			base.Enter();
			this.mapButtonsView = UIViewManager.Instance.FindView<MainMapButtonsView>();
			this.mapButtonsView.transform.parent.gameObject.SetActive(false);
			this.mainMapFlow = this.view.parameters.mainMapFlow;
			this.mainMapFlow.MapContentController.MapView.GetComponentInChildren<UIScrollablePanel>().SetScroll(new Vector2(0f, 0f));
			IEnumerator enumerator = this.mainMapFlow.MapContentController.MapView.MapStreamer.SpawnedContentRoot.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					Transform transform = (Transform)obj;
					if (transform.GetComponent<RuntimeEndPiece>() == null)
					{
						transform.gameObject.SetActive(false);
					}
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			this.mainMapFlow.MapContentController.Avatars.AvatarRoot.gameObject.SetActive(false);
			PlayerAvatarMapLocation componentInChildren = this.mainMapFlow.MapContentController.MapView.gameObject.GetComponentInChildren<PlayerAvatarMapLocation>();
			if (componentInChildren != null)
			{
				componentInChildren.enabled = false;
			}
		}

		public override void Exit()
		{
			base.Exit();
			PlayerAvatarMapLocation componentInChildren = this.mainMapFlow.MapContentController.MapView.gameObject.GetComponentInChildren<PlayerAvatarMapLocation>();
			if (componentInChildren != null)
			{
				componentInChildren.enabled = true;
			}
			this.mainMapFlow = null;
		}

		public override IEnumerator Logic()
		{
			SagaMapView mainMapView = this.mainMapFlow.MapContentController.MapView;
			Vector2 start = mainMapView.GetComponentInChildren<UIScrollablePanel>().actualScrollOffset;
			Vector2 end = start + new Vector2(0f, -2000f);
			AnimationCurve curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
			this.ScrollMapView(start);
			this.view.transform.parent.localPosition = this.mainMapFlow.MapContentController.MapView.transform.localPosition;
			this.octopusTransform.parent.parent = null;
			this.octopusTransform.gameObject.SetLayerRecursively(this.mainMapFlow.MapContentController.MapView.gameObject.layer);
			yield return FiberHelper.RunParallel(new IEnumerator[]
			{
				FiberHelper.RunSerial(new IEnumerator[]
				{
					FiberHelper.Wait(2f, (FiberHelper.WaitFlag)0),
					this.clouds.AnimateCloudsApart(2f),
					FiberHelper.RunDelayed(0f, delegate
					{
						this.octopusAnimation.Play();
					}),
					FiberHelper.Wait(3f, (FiberHelper.WaitFlag)0),
					FiberHelper.RunParallel(new IEnumerator[]
					{
						FiberAnimation.Animate(5f, curve, delegate(float t)
						{
							this.ScrollMapView(Vector2.Lerp(start, end, t));
						}, false),
						FiberAnimation.MoveLocalTransform(this.shipPlatform, this.shipPlatform.localPosition, this.shipPlatform.localPosition + Vector3.down * this.shipPlatform.gameObject.GetElementSize().y, AnimationCurve.EaseInOut(0f, 0f, 1f, 1f), 2.4f)
					}),
					FiberAnimation.Animate(3f, curve, delegate(float t)
					{
						this.ScrollMapView(Vector2.Lerp(end, start, t));
					}, false)
				}),
				this.ModifyLevelDots(11f)
			});
			this.mapButtonsView.transform.parent.gameObject.SetActive(true);
			this.mapButtonsView.transform.parent.localScale = new Vector3(1f, 1.5f, 1f);
			this.mainMapFlow.MapContentController.Avatars.AvatarRoot.gameObject.SetActive(true);
			this.mainMapFlow.MapContentController.Avatars.ProgressMarker.gameObject.SetActive(false);
			MapAvatar avatar = this.mainMapFlow.MapContentController.Avatars.GetMeAvatar();
			yield return FiberAnimation.ScaleTransform(avatar.transform, Vector3.one * 0.001f, Vector3.one, AnimationCurve.EaseInOut(0f, 0f, 1f, 1f), 0.5f);
			yield return FiberAnimation.ScaleTransform(this.mapButtonsView.transform.parent, new Vector3(1f, 1.5f, 1f), Vector3.one, AnimationCurve.EaseInOut(0f, 0f, 1f, 1f), 0.5f);
			this.view.CloseView();
			yield break;
		}

		private IEnumerator ModifyLevelDots(float duration)
		{
			SpawnPool spawnPool = this.mainMapFlow.MapContentController.MapView.gameObject.GetComponentInChildren<SpawnPool>(true);
			while (duration > 0f)
			{
				duration -= Time.deltaTime;
				foreach (Transform transform in spawnPool)
				{
					LevelDot component = transform.gameObject.GetComponent<LevelDot>();
					if (component != null)
					{
						component.gameObject.SetActive(component.transform.position.y < this.octopusTransform.position.y);
					}
				}
				yield return null;
			}
			yield break;
		}

		private void ScrollMapView(Vector2 position)
		{
			SagaMapView mapView = this.mainMapFlow.MapContentController.MapView;
			UIScrollablePanel componentInChildren = mapView.GetComponentInChildren<UIScrollablePanel>();
			componentInChildren.SetScroll(position);
			Vector3 localPosition = mapView.transform.parent.localPosition;
			localPosition.y = -position.y;
			mapView.transform.parent.localPosition = localPosition;
		}

		public StoryOutroView view;

		public Transform octopusTransform;

		public Transform shipPlatform;

		private MainMapButtonsView mapButtonsView;

		private MainMapFlow mainMapFlow;

		public StoryClouds clouds;

		public Animation octopusAnimation;
	}
}
