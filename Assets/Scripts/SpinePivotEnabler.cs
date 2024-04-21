using System;
using Spine;
using UnityEngine;

public class SpinePivotEnabler : MonoBehaviour
{
    private void Update()
    {
        if (this.isInitialized)
        {
            return;
        }
        this.isInitialized = true;
        if (this.spine.state != null)
        {
            this.spine.state.Event += this.HandleEvent;
            this.spine.state.End += this.HandleEnd;
        }
    }

    private void HandleEvent(Spine.AnimationState state, int trackIndex, Spine.Event e)
    {
        string name = state.GetCurrent(trackIndex).animation.name;
        for (int i = 0; i < this.pivots.Length; i++)
        {
            this.pivots[i].TryEnable(name, e.Data.Name, SpinePivotEnabler.EventType.SpineEvent);
        }
    }

    private void HandleEnd(Spine.AnimationState state, int trackIndex)
    {
        string name = state.GetCurrent(trackIndex).animation.name;
        for (int i = 0; i < this.pivots.Length; i++)
        {
            this.pivots[i].TryEnable(name, null, SpinePivotEnabler.EventType.AnimEnded);
        }
    }

    public SkeletonAnimation spine;

    public SpinePivotEnabler.Pivot[] pivots;

    private bool isInitialized;

    public enum EventType
    {
        SpineEvent,
        AnimEnded
    }

    [Serializable]
    public class Pivot
    {
        public void TryEnable(string aName, string eName, SpinePivotEnabler.EventType eventType)
        {
            if (aName != this.animName)
            {
                return;
            }
            if (eventType != this.eventType)
            {
                return;
            }
            if (eventType == SpinePivotEnabler.EventType.SpineEvent && eName != this.eventName)
            {
                return;
            }
            if (this.pivot != null)
            {
                this.pivot.SetActive(this.enable);
            }
            if (this.particleSystem != null)
            {
                if (this.enable)
                {
                    this.particleSystem.Play();
                }
                else
                {
                    this.particleSystem.Stop();
                }
                var _temp = particleSystem.emission;
                _temp.enabled = this.enable;
            }
        }

        public GameObject pivot;

        public ParticleSystem particleSystem;

        public string animName;

        public string eventName;

        public bool enable;

        public SpinePivotEnabler.EventType eventType;
    }
}
