using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Fibers;
using TactileModules.Analytics.Interfaces;

namespace TactileModules.Placements
{
	public class PlacementRunner : IPlacementRunner
	{
		public PlacementRunner(IPlacementRunnableRegistry registry, IPlacementViewMediator viewMediator, IConfigPropertyGetter<PlacementConfigData> configPropertyGetter, IAnalytics analytics)
		{
			this.registry = registry;
			this.viewMediator = viewMediator;
			this.configPropertyGetter = configPropertyGetter;
			this.analytics = analytics;
		}

		public IEnumerator Run(NonBreakablePlacementIdentifier placement)
		{
			this.viewMediator.ResetViewShownCount();
			List<IPlacementRunnable> runnables = this.registry.GetRunnables(placement, PlacementBehavior.Unskippable);
			foreach (IPlacementRunnable runnable in runnables)
			{
				yield return this.RunAsNonBreakingOnly(runnable, placement);
			}
			runnables = this.registry.GetRunnables(placement, PlacementBehavior.Skippable);
			int viewLimit = this.GetPlacementViewLimit(placement);
			foreach (IPlacementRunnable runnable2 in runnables)
			{
				if (this.viewMediator.GetViewShownCount() >= viewLimit)
				{
					break;
				}
				yield return this.RunAsNonBreakingOnly(runnable2, placement);
			}
			yield break;
		}

		public IEnumerator Run(BreakablePlacementIdentifier placement, EnumeratorResult<bool> wasFlowBroken)
		{
			this.viewMediator.ResetViewShownCount();
			List<IPlacementRunnable> runnables = this.registry.GetRunnables(placement, PlacementBehavior.Unskippable);
			foreach (IPlacementRunnable runnable in runnables)
			{
				EnumeratorResult<bool> didRunnableBreakFlow = new EnumeratorResult<bool>();
				yield return this.RunAsAny(runnable, didRunnableBreakFlow, placement);
				if (didRunnableBreakFlow)
				{
					wasFlowBroken.value = true;
					yield break;
				}
			}
			runnables = this.registry.GetRunnables(placement, PlacementBehavior.Skippable);
			int viewLimit = this.GetPlacementViewLimit(placement);
			foreach (IPlacementRunnable runnable2 in runnables)
			{
				if (this.viewMediator.GetViewShownCount() >= viewLimit)
				{
					break;
				}
				EnumeratorResult<bool> didRunnableBreakFlow2 = new EnumeratorResult<bool>();
				yield return this.RunAsAny(runnable2, didRunnableBreakFlow2, placement);
				if (didRunnableBreakFlow2)
				{
					wasFlowBroken.value = true;
					yield break;
				}
			}
			yield break;
		}

		private IEnumerator RunAsAny(IPlacementRunnable runnable, EnumeratorResult<bool> wasFlowBroken, PlacementIdentifier placement)
		{
			IPlacementRunnableNoBreak runnableNoBreak = runnable as IPlacementRunnableNoBreak;
			if (runnableNoBreak != null)
			{
				yield return runnableNoBreak.Run(this.viewMediator);
				yield break;
			}
			IPlacementRunnableCanBreak runnableCanBreak = runnable as IPlacementRunnableCanBreak;
			if (runnableCanBreak != null)
			{
				yield return runnableCanBreak.Run(this.viewMediator, wasFlowBroken);
				yield break;
			}
			this.LogError(runnable.ID, placement);
			yield break;
		}

		private IEnumerator RunAsNonBreakingOnly(IPlacementRunnable runnable, PlacementIdentifier placement)
		{
			IPlacementRunnableNoBreak runnableNoBreak = runnable as IPlacementRunnableNoBreak;
			if (runnableNoBreak != null)
			{
				yield return runnableNoBreak.Run(this.viewMediator);
				yield break;
			}
			this.LogError(runnable.ID, placement);
			yield break;
		}

		private void LogError(string runnableID, PlacementIdentifier placement)
		{
			ClientErrorEvent eventObject = new ClientErrorEvent("PlacementRunnableTypeMismatch", new StackTrace().ToString(), null, runnableID, placement.ID, null, null, null, null);
			this.analytics.LogEvent(eventObject, -1.0, null);
		}

		private int GetPlacementViewLimit(PlacementIdentifier placement)
		{
			int num = 0;
			PlacementConfigData placementConfigData = this.configPropertyGetter.Get(placement.ID);
			if (placementConfigData != null)
			{
				num = placementConfigData.ViewLimit;
			}
			return (num <= 0) ? int.MaxValue : num;
		}

		private readonly IPlacementRunnableRegistry registry;

		private readonly IPlacementViewMediator viewMediator;

		private readonly IConfigPropertyGetter<PlacementConfigData> configPropertyGetter;

		private readonly IAnalytics analytics;
	}
}
