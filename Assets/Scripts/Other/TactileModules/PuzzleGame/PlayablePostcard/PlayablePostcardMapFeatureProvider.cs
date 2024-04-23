using System;
using System.Collections;
using System.Diagnostics;
using Fibers;
using TactileModules.MapFeature;
using TactileModules.PuzzleGame.PlayablePostcard.Model;
using TactileModules.PuzzleGame.PlayablePostcard.Views;

namespace TactileModules.PuzzleGame.PlayablePostcard
{
	public class PlayablePostcardMapFeatureProvider : IMapFeatureProvider
	{
		public PlayablePostcardMapFeatureProvider(IPlayablePostcardProvider provider)
		{
			this.provider = provider;
		}

		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action OnSwitchToFeatureMapView;



		void IMapFeatureProvider.SwitchToFeatureMapView()
		{
			this.OnSwitchToFeatureMapView();
		}

		IEnumerator IMapFeatureProvider.ShowFeatureStartView(EnumeratorResult<bool> viewClosingResult)
		{
			UIViewManager.UIViewStateGeneric<PlayablePostcardInfoView> vs = UIViewManager.Instance.ShowView<PlayablePostcardInfoView>(new object[0]);
			vs.View.Initialize(false);
			yield return vs.WaitForClose();
			viewClosingResult.value = ((int)vs.ClosingResult == 1);
			yield break;
		}

		IEnumerator IMapFeatureProvider.ShowFeatureStartSessionView(EnumeratorResult<bool> viewClosingResult)
		{
			UIViewManager.UIViewStateGeneric<PlayablePostcardInfoView> vs = UIViewManager.Instance.ShowView<PlayablePostcardInfoView>(new object[0]);
			vs.View.Initialize(true);
			yield return vs.WaitForClose();
			viewClosingResult.value = ((int)vs.ClosingResult == 1);
			yield break;
		}

		IEnumerator IMapFeatureProvider.ShowFeatureEndedView()
		{
			UIViewManager.UIViewStateGeneric<PlayablePostcardEndView> vs = UIViewManager.Instance.ShowView<PlayablePostcardEndView>(new object[]
			{
				true
			});
			yield return vs.WaitForClose();
			yield break;
		}

		void IMapFeatureProvider.Save()
		{
			this.provider.Save();
		}

		private readonly IPlayablePostcardProvider provider;
	}
}
