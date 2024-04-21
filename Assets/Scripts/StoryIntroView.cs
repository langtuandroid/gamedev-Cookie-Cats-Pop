using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using Spine;
using Spine.Unity;
using TactileModules.FacebookExtras;
using TactileModules.Foundation;
using UnityEngine;

public class StoryIntroView : UIView
{
	protected override void ViewLoad(object[] parameters)
	{
		this.parameters = (StoryIntroView.StoryIntroViewParameters)parameters[0];
		this.devButton.gameObject.SetActive(false);
	}

	private void PlayClicked(UIEvent e)
	{
		if (this.playClicked != null)
		{
			this.playClicked();
		}
	}

	private void CloseView()
	{
		this.parameters.done(this.didCheatSkip);
		base.Close(0);
	}

	private IEnumerator DoFacebookInfo()
	{
		Boot.IsRequestsBlocked += false;
		UIViewManager.UIViewStateGeneric<FacebookLoginInfoView> vs = UIViewManager.Instance.ShowView<FacebookLoginInfoView>(new object[0]);
		yield return vs.WaitForClose();
		Boot.IsRequestsBlocked += true;
		if ((int)vs.ClosingResult == 1)
		{
			this.logInButtonPivot.SetActive(false);
			if (PuzzleGame.PlayerState.FarthestUnlockedLevelHumanNumber > 1)
			{
				this.parameters.done(true);
				base.Close(0);
			}
		}
		yield break;
	}

	private void ButtonFacebookLoginInfoClicked(UIEvent e)
	{
		FiberCtrl.Pool.Run(this.DoFacebookInfo(), false);
	}

	public UIInstantiator devButton;

	private StoryIntroView.StoryIntroViewParameters parameters;

	private Action playClicked;

	private bool didCheatSkip;

	public GameObject logInButtonPivot;

	public class StoryIntroViewParameters
	{
		public Action<bool> done;

		public GameView gameView;

		public LevelSession session;
	}

	[Serializable]
	public class AlarmState : StoryState
	{
		private FacebookLoginManager FacebookLoginManager
		{
			get
			{
				return ManagerRepository.Get<FacebookLoginManager>();
			}
		}

		public override void Enter()
		{
            base.Enter();
            this.eyesFiber = new Fiber(FiberHelper.RunSerial(new IEnumerator[]
            {
                this.eyes.PlayTimeline("animation", 0f, this.eyes.GetEventTime("animation", "Cats_loops_start"), 3.2f, 1f, null),
                this.eyes.PlayLoopBetweenEvents("animation", "Cats_loops_start", "Cats_loops_Stop", -1f)
            }));
            this.alarmPivot.SetActive(false);
        }

		public override void Exit()
		{
            base.Exit();
            this.eyesFiber.Terminate();
        }

		public override IEnumerator Logic()
		{
            this.playButton.SetActive(false);
            this.logInButtonPivot.SetActive(false);
            yield return FiberHelper.Wait(2f, (FiberHelper.WaitFlag)0);
            this.screenAnimation.Play();
            yield return FiberHelper.Wait(0.5f, (FiberHelper.WaitFlag)0);
            this.tvSound.Play();
            AudioManager.Instance.SetMusic(this.music, true);
            yield return FiberHelper.Wait(1.5f, (FiberHelper.WaitFlag)0);
            this.alarmPivot.SetActive(true);
            AudioSource alarmAudioSource = this.alarmSound.Play();
            yield return new Fiber.OnExit(delegate ()
            {
                if (alarmAudioSource != null)
                {
                    alarmAudioSource.Stop();
                }
            });
            yield return FiberHelper.Wait(0.5f, (FiberHelper.WaitFlag)0);
            this.playButton.SetActive(true);
            this.logInButtonPivot.SetActive(ManagerRepository.Get<FacebookClient>().IsInitialized && !this.FacebookLoginManager.IsLoggedInAndUserRegistered);
            bool playClicked = false;
            this.introView.playClicked = delegate ()
            {
                playClicked = true;
            };
            while (!playClicked)
            {
                yield return null;
            }
            this.introView.playClicked = null;
            UICamera.DisableInput();
            yield break;
		}

		public StoryIntroView introView;

		public GameObject playButton;

		public UnityEngine.Animation screenAnimation;

		public SkeletonAnimation eyes;

		public SoundDefinition music;

		public SoundDefinition tvSound;

		public GameObject alarmPivot;

		public SoundDefinition alarmSound;

		public GameObject logInButtonPivot;

		private Fiber eyesFiber;
	}

	[Serializable]
	public class CatIntroductionState : StoryState
	{
		public override void Enter()
		{
			base.Enter();
			for (int i = 0; i < this.characterPivots.Length; i++)
			{
				this.characterPivots[i].characterPivot.SetActive(false);
			}
			AudioManager.Instance.SetMusic(null, true);
		}

		public override IEnumerator Logic()
		{
			yield return FiberHelper.Wait(0.25f, (FiberHelper.WaitFlag)0);
			foreach (StoryIntroView.CatIntroductionState.CharacterPivot cp in this.characterPivots)
			{
				if (cp.overlayTarget != null)
				{
					this.overlayPivot.position = cp.overlayTarget.position;
					this.overlayPivot.localRotation = cp.overlayTarget.localRotation;
					this.overlayPivot.gameObject.SetActive(true);
				}
				else
				{
					this.overlayPivot.gameObject.SetActive(false);
				}
				cp.sound.Play();
				cp.characterPivot.SetActive(true);
				cp.skeleton.state.GetCurrent(0).Time = cp.startAnimTime;
				if (cp.loopStart > 0f || cp.loopEnd > 0f)
				{
					this.Loop(cp.skeleton, cp.loopStart, cp.loopEnd);
				}
				yield return FiberHelper.RunParallel(new IEnumerator[]
				{
					this.AnimateName(cp.nameTransform, cp.nameFromLeft),
					FiberAnimation.ScaleTransform(cp.nameStretchTransform, Vector3.one, new Vector3(this.nameStretchSize.x, this.nameStretchSize.y, 1f), this.nameStretchCurve, 0f),
					FiberHelper.Wait(this.delayBetweenCharacters, (FiberHelper.WaitFlag)0)
				});
			}
			yield return FiberHelper.Wait(this.endDelay, (FiberHelper.WaitFlag)0);
			yield break;
		}

		private IEnumerator AnimateName(Transform nameTransform, bool moveFromLeft)
		{
			Vector3 sourcePos = nameTransform.TransformPoint(new Vector3((!moveFromLeft) ? 350f : -350f, 0f, 0f));
			sourcePos = nameTransform.parent.InverseTransformPoint(sourcePos);
			yield return FiberAnimation.MoveLocalTransform(nameTransform, sourcePos, nameTransform.localPosition, this.nameMoveCurve, 0f);
			yield break;
		}

		private void Loop(SkeletonAnimation spine, float startTime, float endTime)
		{
			TrackEntry track = spine.state.GetCurrent(0);
			bool isLoopStarted = false;
			UpdateBonesDelegate value = delegate(ISkeletonAnimation animatedSkeletonComponent)
			{
				if (!isLoopStarted)
				{
					if (track.time > startTime)
					{
						isLoopStarted = true;
					}
				}
				else
				{
					track.time = Mathf.Repeat(track.time - startTime, endTime - startTime) + startTime;
					spine.skeleton.Update(0f);
					spine.state.Update(0f);
				}
			};
			spine.UpdateLocal += value;
		}

		public float delayBetweenCharacters;

		public Transform overlayPivot;

		public StoryIntroView.CatIntroductionState.CharacterPivot[] characterPivots;

		public AnimationCurve nameMoveCurve;

		public float endDelay;

		public AnimationCurve nameStretchCurve;

		public Vector2 nameStretchSize;

		[Serializable]
		public class CharacterPivot
		{
			public GameObject characterPivot;

			public Transform overlayTarget;

			public float startAnimTime;

			public SkeletonAnimation skeleton;

			public Transform nameTransform;

			public bool nameFromLeft;

			public SoundDefinition sound;

			public Transform nameStretchTransform;

			public float loopStart;

			public float loopEnd;
		}
	}

	[Serializable]
	public class Flag : StoryState
	{
		public override void Enter()
		{
			base.Enter();
			AudioManager.Instance.SetMusic(this.music, true);
		}

		public override IEnumerator Logic()
		{
			yield return this.flag.PlayUntilEvent("animation", "Loop Start");
			yield return this.flag.PlayLoopBetweenEvents("animation", "Loop Start", "loop End", 2f);
			yield break;
		}

		public SoundDefinition music;

		public SkeletonAnimation flag;
	}

	[Serializable]
	public class ShipApproaching : StoryState
	{
		public override IEnumerator Logic()
		{
			this.pivot.SendMessage("Update", SendMessageOptions.DontRequireReceiver);
			yield return FiberHelper.Wait(4f, (FiberHelper.WaitFlag)0);
			yield break;
		}
	}

	[Serializable]
	public class MamaOctopus : StoryState
	{
		public override IEnumerator Logic()
		{
			yield return FiberHelper.Wait(3.2f, (FiberHelper.WaitFlag)0);
			yield break;
		}
	}

	[Serializable]
	public class KittenInBubble : StoryState
	{
		public override void Initialize()
		{
			base.Initialize();
			this.gameView = this.introView.parameters.gameView;
			this.camera = this.gameView.ViewCamera;
			this.cameraOriginalSize = this.camera.orthographicSize;
		}

		public override void Enter()
		{
			base.Enter();
		}

		public override IEnumerator Logic()
		{
			this.gameView.gameObject.SetActive(true);
			this.gameView.DisablePointsEffects();
			this.gameView.cannonOperator.DisableDanceAnimation = true;
			Tile goalTile = this.FindGoalTile();
			Vector3 target = goalTile.WorldPosition;
			float targetZoom = 0.25f;
			this.ZoomCamera(target, targetZoom);
			List<Tile> rowTiles = this.FindPiecesBelowTile(goalTile);
			IEnumerator[] enums = new IEnumerator[rowTiles.Count + 2];
			float startDelay = 0f;
			enums[0] = this.MovePieceOnTile(goalTile, 150f, startDelay, 1f);
			enums[1] = FiberHelper.RunDelayed(1f, delegate
			{
				this.AddForcesToBubbleWobble(goalTile.WorldPosition, goalTile.Coord.y);
			});
			for (int i = 0; i < rowTiles.Count; i++)
			{
				enums[i + 2] = this.MovePieceOnTile(rowTiles[i], 200f, startDelay + 1f + (float)i * 0.01f, 0.5f);
			}
			yield return FiberHelper.RunParallel(enums);
			yield return FiberHelper.Wait(0.3f, (FiberHelper.WaitFlag)0);
			yield return FiberAnimation.Animate(2f, AnimationCurve.EaseInOut(0f, 0f, 1f, 1f), delegate(float t)
			{
				this.ZoomCamera(Vector3.Lerp(target, Vector3.zero, t), Mathf.Lerp(targetZoom, 1f, t));
			}, false);
			yield return FiberHelper.Wait(0.3f, (FiberHelper.WaitFlag)0);
			this.gameView.StartStartupSequence();
			yield return FiberHelper.RunParallel(new IEnumerator[]
			{
				FiberHelper.RunDelayed(0.5f, delegate
				{
					AudioManager.Instance.SetMusic(this.music, true);
				}),
				this.WaitForKittenToBeSaved()
			});
			yield return FiberHelper.Wait(0.25f, (FiberHelper.WaitFlag)0);
			UIViewManager.Instance.CloseAll(new IUIView[]
			{
				this.introView
			});
			yield break;
		}

		private void ZoomCamera(Vector3 worldPositionTarget, float zoom)
		{
			worldPositionTarget.z = this.camera.transform.position.z;
			this.camera.transform.position = worldPositionTarget;
			this.camera.orthographicSize = this.cameraOriginalSize * zoom;
		}

		private Tile FindGoalTile()
		{
			using (IEnumerator<Tile> enumerator = this.gameView.Board.FindAllWithPieceClass<GoalPiece>().GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					return enumerator.Current;
				}
			}
			return Tile.Invalid;
		}

		private List<Tile> FindPiecesBelowTile(Tile belowThis)
		{
			int y = belowThis.Coord.y;
			List<Tile> list = new List<Tile>();
			foreach (Tile item in this.gameView.Board.GetOccupiedTiles())
			{
				if (item.Coord.y > y)
				{
					list.Add(item);
				}
			}
			return list;
		}

		private IEnumerator MovePieceOnTile(Tile tile, float distance, float delay, float moveDuration)
		{
			Vector3 end = tile.WorldPosition;
			Vector3 start = end + Vector3.down * distance;
			tile.Piece.transform.position = start;
			if (delay > 0f)
			{
				yield return FiberHelper.Wait(delay, (FiberHelper.WaitFlag)0);
			}
			yield return FiberHelper.RunParallel(new IEnumerator[]
			{
				FiberAnimation.MoveTransform(tile.Piece.transform, start, end, AnimationCurve.Linear(0f, 0f, 1f, 1f), moveDuration),
				FiberAnimation.RotateTransform(tile.Piece.transform, new Vector3(0f, 0f, UnityEngine.Random.value * 90f), new Vector3(0f, 0f, 0f), null, moveDuration)
			});
			yield break;
		}

		private void AddForcesToBubbleWobble(Vector3 center, int maxYCoord)
		{
			float piecesSpringArea = SingletonAsset<LevelVisuals>.Instance.piecesSpringArea;
			foreach (Tile tile in this.gameView.Board.GetOccupiedTiles())
			{
				if (tile.Coord.y <= maxYCoord)
				{
					CPPiece cppiece = tile.Piece as CPPiece;
					float num = (center - cppiece.transform.position).magnitude;
					if (num >= 1f && num < piecesSpringArea)
					{
						num = (piecesSpringArea - num) / piecesSpringArea;
						cppiece.ActivateSpring(center, num * 10f);
					}
				}
			}
		}

		private void ModifySavedKittensArea()
		{
			Vector3 localPosition = this.gameView.savedKittens.transform.localPosition;
			localPosition.x -= 90f;
			this.gameView.savedKittens.transform.localPosition = localPosition;
			this.gameView.savedKittens.transform.localScale = new Vector3(0.001f, 1f, 1f);
		}

		private IEnumerator WaitForKittenToBeSaved()
		{
			UICamera.EnableInput();
			this.ModifySavedKittensArea();
			SavedKitten savedKitten = null;
			Action<SavedKitten> kittenSpawned = null;
			kittenSpawned = delegate(SavedKitten s)
			{
				savedKitten = s;
				s.fallingDuration += 1f;
				SavedKittens savedKittens2 = this.gameView.savedKittens;
				savedKittens2.KittenSpawned = (Action<SavedKitten>)Delegate.Remove(savedKittens2.KittenSpawned, kittenSpawned);
			};
			SavedKittens savedKittens = this.gameView.savedKittens;
			savedKittens.KittenSpawned = (Action<SavedKitten>)Delegate.Combine(savedKittens.KittenSpawned, kittenSpawned);
			bool isLevelComplete = false;
			this.introView.parameters.session.StateChanged += delegate(LevelSession obj)
			{
				if (obj.SessionState == LevelSessionState.Completed)
				{
					isLevelComplete = true;
				}
			};
			while (!isLevelComplete && savedKitten == null)
			{
				yield return null;
			}
			this.gameView.cannonOperator.catSpine.PlayAnimation(0, "Idle_lookup", true, false);
			AudioManager.Instance.SetMusic(null, true);
			this.rescueMusic.Play();
			UICamera.DisableInput();
			yield return FiberAnimation.Animate(3f, AnimationCurve.EaseInOut(0f, 0f, 1f, 1f), delegate(float t)
			{
				this.ZoomCamera(Vector3.Lerp(Vector3.zero, new Vector3(0f, -180f, 0f), t), Mathf.Lerp(1f, 0.6f, t));
			}, false);
			yield break;
		}

		public StoryIntroView introView;

		public SoundDefinition music;

		public SoundDefinition rescueMusic;

		private GameView gameView;

		private Camera camera;

		private float cameraOriginalSize;
	}

	[Serializable]
	public class ToTheRescue : StoryState
	{
		public override IEnumerator Logic()
		{
			AudioManager.Instance.SetMusic(this.music, false);
			yield return FiberHelper.Wait(3.0666666f, (FiberHelper.WaitFlag)0);
			yield return UIViewManager.Instance.FadeCameraFrontFill(1f, 0f, 0);
			this.introView.CloseView();
			yield break;
		}

		public StoryIntroView introView;

		public SoundDefinition music;
	}
}
