using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Fibers;
using TactileModules.PuzzleGames.GameCore;

namespace TactileModules.SagaCore
{
    public abstract class MapFlow : IMapFlow, INotifiedFlow, IFullScreenOwner, SagaAvatarController.IAvatarInfoProvider, IFlow, IFiberRunnable
    {
        public MapFlow(MapIdentifier mapIdentifier, MapFacade mapFacade, IFullScreenManager fullScreenManager, IFlowStack flowStack)
        {
            this.mapFacade = mapFacade;
            this.fullScreenManager = fullScreenManager;
            this.FlowStack = flowStack;
            this.MapContentController = mapFacade.CreateMapContentController(mapIdentifier);
            this.MapContentController.DotClicked += this.DotClicked;
            this.AvatarProgressedToDotIndex = new HookList<int>();
            this.PreAvatarProgression = new HookList<MapFlow.SequenceInfo>();
        }

        public IHookList<int> AvatarProgressedToDotIndex { get; private set; }

        public IHookList<MapFlow.SequenceInfo> PreAvatarProgression { get; private set; }

        public MapContentController MapContentController { get; private set; }

        public int VisibleAvatarDotIndex { get; private set; }

        protected IFlowStack FlowStack { get; private set; }

        public bool DidChangeScreenWhileInOtherFlow { get; private set; }

        IEnumerator IFiberRunnable.Run()
        {
            this.VisibleAvatarDotIndex = this.GetFarthestUnlockedDotIndex();
            if (this.autoPushToFullscreen)
            {
                yield return this.fullScreenManager.Push(this);
            }
            this.runningLoop = true;
            while (this.runningLoop)
            {
                yield return null;
            }
            yield return this.fullScreenManager.Pop();
            yield break;
        }

        void IFiberRunnable.OnExit()
        {
        }

        void IFullScreenOwner.ScreenLost()
        {
            this.UnregisterEnterForeground();
            this.ableToRunStartSequence = false;
            this.popupSequenceFiber.Terminate();
            this.AfterScreenLost();
            this.mapFacade.MapPlugins.ForEach(delegate (IMapPlugin p)
            {
                p.ViewsDestroyed(this.MapContentController.MapIdentifier, this.MapContentController);
            });
            this.MapContentController.DestroyViews();
        }

        IEnumerator IFullScreenOwner.ScreenAcquired()
        {
            this.ableToRunStartSequence = false;
            this.DidChangeScreenWhileInOtherFlow = true;
            this.MapContentController.CreateViews(this, this.GetMapStreamerDataProvider());
            this.MapContentController.JumpToDot(this.GetFarthestUnlockedDotIndex());
            this.mapFacade.MapPlugins.ForEach(delegate (IMapPlugin p)
            {
                p.ViewsCreated(this.MapContentController.MapIdentifier, this.MapContentController, this);
            });
            yield return this.AfterScreenAcquired();
            this.RegisterEnterForeground();
            yield break;
        }

        void IFullScreenOwner.ScreenReady()
        {
            this.ableToRunStartSequence = true;
        }

        void INotifiedFlow.Enter(IFlow previousFlow)
        {
            INextMapDot nextMapDot = previousFlow as INextMapDot;
            this.RunStartSequence(nextMapDot);
        }

        void INotifiedFlow.Leave(IFlow nextFlow)
        {
            this.DidChangeScreenWhileInOtherFlow = false;
        }

        private void DotClicked(MapDotBase dot)
        {
            if (!this.IsLevelDotPlayable(dot.LevelId))
            {
                return;
            }
            if (this.FlowStack.Top == this)
            {
                this.StartFlowForDot(dot.LevelId);
            }
        }

        protected virtual bool IsLevelDotPlayable(int dotIndex)
        {
            return dotIndex <= this.GetFarthestUnlockedDotIndex();
        }

        public void StartFlowForDot(int dotIndex)
        {
            IFlow flow = this.CreateFlowForDot(dotIndex);
            if (flow != null)
            {
                this.FlowStack.Push(flow);
            }
        }

        protected void EndThisFlow()
        {
            this.runningLoop = false;
        }

        protected bool IsMapIdle()
        {
            return this.popupSequenceFiber.IsTerminated && this.FlowStack.Top == this && this.fullScreenManager.Top == this;
        }

        private void RunStartSequence(INextMapDot nextMapDot)
        {
            if (this.popupSequenceFiber.IsTerminated)
            {
                this.popupSequenceFiber.Start(this.StartSequence(nextMapDot));
            }
        }

        protected void RunPostPlayFlowSequence()
        {
            if (this.popupSequenceFiber.IsTerminated)
            {
                this.popupSequenceFiber.Start(this.PostPlayFlowSequence());
            }
        }

        protected IEnumerator AnimateAvatarProgressIfAny()
        {
            UICamera.DisableInput();
            if (MapFlow._003C_003Ef__mg_0024cache0 == null)
            {
                MapFlow._003C_003Ef__mg_0024cache0 = new Fiber.OnExitHandler(UICamera.EnableInput);
            }
            yield return new Fiber.OnExit(MapFlow._003C_003Ef__mg_0024cache0);
            yield return FiberHelper.RunParallel(new IEnumerator[]
            {
                this.MapContentController.PanToDot(this.GetFarthestUnlockedDotIndex(), 0.2f, null),
                this.MapContentController.Avatars.AnimateProgressIfAny()
            });
            this.VisibleAvatarDotIndex = this.GetFarthestUnlockedDotIndex();
            yield return this.AvatarProgressedToDotIndex.InvokeAll(this.GetFarthestUnlockedDotIndex());
            yield break;
        }

        private IEnumerator WaitForScreenToBeReady()
        {
            while (!this.ableToRunStartSequence)
            {
                yield return null;
            }
            yield break;
        }

        private IEnumerator StartSequence(INextMapDot nextMapDot)
        {
            yield return this.WaitForScreenToBeReady();
            int nextDotIndexToOpen = -1;
            if (nextMapDot != null && nextMapDot.NextDotIndexToOpen > 0)
            {
                nextDotIndexToOpen = nextMapDot.NextDotIndexToOpen;
            }
            yield return this.PreAvatarProgression.InvokeAll(new MapFlow.SequenceInfo(nextDotIndexToOpen));
            yield return this.PostPlayFlowSequence(nextDotIndexToOpen);
            this.ableToRunStartSequence = false;
            yield break;
        }

        protected IEnumerator PostPlayFlowSequence()
        {
            yield return this.PostPlayFlowSequence(-1);
            yield break;
        }

        private void RegisterEnterForeground()
        {
            ActivityManager.onResumeEvent += this.OnApplicationWillEnterForeground;
        }

        private void UnregisterEnterForeground()
        {
            ActivityManager.onResumeEvent -= this.OnApplicationWillEnterForeground;
        }

        protected virtual MapStreamer.IDataProvider GetMapStreamerDataProvider()
        {
            return null;
        }

        protected abstract IEnumerator PostPlayFlowSequence(int nextDotIndexToOpen);

        protected abstract IEnumerator AfterScreenAcquired();

        protected abstract void AfterScreenLost();

        protected abstract int GetFarthestUnlockedDotIndex();

        protected abstract IFlow CreateFlowForDot(int dotIndex);

        public abstract SagaAvatarInfo CreateMeAvatarInfo();

        public abstract Dictionary<CloudUser, SagaAvatarInfo> CreateFriendsAvatarInfos();

        public virtual int GetProgressMarkerDotIndex()
        {
            return this.GetFarthestUnlockedDotIndex();
        }

        protected virtual void OnApplicationWillEnterForeground()
        {
        }

        public bool autoPushToFullscreen = true;

        private readonly MapFacade mapFacade;

        private readonly IFullScreenManager fullScreenManager;

        private readonly Fiber popupSequenceFiber = new Fiber(FiberBucket.Update);

        private bool runningLoop;

        private bool ableToRunStartSequence;

        [CompilerGenerated]
        private static Fiber.OnExitHandler _003C_003Ef__mg_0024cache0;

        public class SequenceInfo
        {
            public SequenceInfo(int dotIndexToOpen)
            {
                this.DotIndexToOpen = dotIndexToOpen;
            }

            public int DotIndexToOpen { get; private set; }

            public bool AnyDotToOpen
            {
                get
                {
                    return this.DotIndexToOpen > 0;
                }
            }
        }
    }
}
