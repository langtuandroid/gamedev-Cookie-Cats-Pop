using System;
using System.Collections;
using Fibers;
using TactileModules.MapFeature;
using TactileModules.PuzzleGame.SlidesAndLadders.Controllers;
using TactileModules.PuzzleGame.SlidesAndLadders.Views;

namespace TactileModules.PuzzleGame.SlidesAndLadders
{
	public class SlidesAndLaddersMapFeatureProvider : IMapFeatureProvider
	{
		public SlidesAndLaddersMapFeatureProvider(ISlidesAndLaddersControllerFactory controllerFactory)
		{
			this.controllerFactory = controllerFactory;
		}

		void IMapFeatureProvider.SwitchToFeatureMapView()
		{
			this.controllerFactory.CreateAndPushMapFlow();
		}

		IEnumerator IMapFeatureProvider.ShowFeatureStartView(EnumeratorResult<bool> viewClosingResult)
		{
			UIViewManager.UIViewStateGeneric<SlidesAndLaddersRewardInfoView> vs = UIViewManager.Instance.ShowView<SlidesAndLaddersRewardInfoView>(new object[]
			{
				false
			});
			yield return vs.WaitForClose();
			viewClosingResult.value = ((int)vs.ClosingResult == 1);
			yield break;
		}

		IEnumerator IMapFeatureProvider.ShowFeatureStartSessionView(EnumeratorResult<bool> viewClosingResult)
		{
			UIViewManager.UIViewStateGeneric<SlidesAndLaddersRewardInfoView> vs = UIViewManager.Instance.ShowView<SlidesAndLaddersRewardInfoView>(new object[]
			{
				true
			});
			yield return vs.WaitForClose();
			viewClosingResult.value = ((int)vs.ClosingResult == 1);
			yield break;
		}

		IEnumerator IMapFeatureProvider.ShowFeatureEndedView()
		{
			UIViewManager.UIViewStateGeneric<SlidesAndLaddersEndView> vs = UIViewManager.Instance.ShowView<SlidesAndLaddersEndView>(new object[]
			{
				true
			});
			yield return vs.WaitForClose();
			yield break;
		}

		void IMapFeatureProvider.Save()
		{
		}

		private readonly ISlidesAndLaddersControllerFactory controllerFactory;
	}
}
