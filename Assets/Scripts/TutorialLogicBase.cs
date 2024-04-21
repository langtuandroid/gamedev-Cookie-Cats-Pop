using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Fibers;

public abstract class TutorialLogicBase
{
    protected ILevelSession Session { get; private set; }

    ////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public event Action<ITutorialStep> ChangedStep;



    public ITutorialStep CurrentStep
    {
        get
        {
            return (this.currentIndex >= this.steps.Count) ? null : this.steps[this.currentIndex];
        }
    }

    public void Initialize(ILevelSession session)
    {
        this.Session = session;
        this.steps = new List<ITutorialStep>(this.Session.GetTutorialSteps());
        GameEventManager.Instance.OnGameEvent += this.HandleGameEvent;
    }

    public void Destroy()
    {
        GameEventManager.Instance.OnGameEvent -= this.HandleGameEvent;
        Fiber.TerminateIfAble(this.fiber);
    }

    public void Begin()
    {
        this.fiber.Start(this.RunAllSteps());
    }

    public bool HasSteps
    {
        get
        {
            return this.steps != null && this.steps.Count > 0;
        }
    }

    private void NextStep()
    {
        this.currentIndex++;
    }

    protected bool IsInLastStep
    {
        get
        {
            return this.currentIndex == this.steps.Count - 1;
        }
    }

    private void HandleGameEvent(GameEvent e)
    {
        this.lastGameEvents.Add(e.type);
    }

    protected IEnumerator WaitForGameEvents(params GameEventType[] eventTypes)
    {
        this.lastGameEvents.Clear();
        for (; ; )
        {
            bool found = false;
            foreach (GameEventType item in eventTypes)
            {
                if (this.lastGameEvents.Contains(item))
                {
                    found = true;
                    break;
                }
            }
            if (found)
            {
                break;
            }
            yield return null;
        }
        yield break;
    }

    private IEnumerator RunAllSteps()
    {
        GameEventManager.Instance.Emit(2000);
        while (this.CurrentStep != null)
        {
            this.ChangedStep(this.CurrentStep);
            yield return this.WaitForStep(this.CurrentStep);
            this.NextStep();
        }
        this.ChangedStep(null);
        yield break;
    }

    protected abstract IEnumerator WaitForStep(ITutorialStep step);

    private List<GameEventType> lastGameEvents = new List<GameEventType>();

    private Fiber fiber = new Fiber();

    private int currentIndex;

    private List<ITutorialStep> steps;
}
