using System;
using System.Collections;
using TactileModules.Foundation;

namespace Tactile.XperiaGamesClub
{
	public class XperiaGiftPopupManager : MapPopupManager.IMapPopup
	{
		public XperiaGiftPopupManager(XperiaGamesClubManager xperiaGamesClubManager, XperiaGiftPopupManager.IDataProvider provider)
		{
			if (xperiaGamesClubManager == null)
			{
				throw new ArgumentNullException();
			}
			this.provider = provider;
			MapPopupManager.Instance.RegisterPopupObject(this);
		}

		private bool ShouldShowPopup
		{
			get
			{
				return ManagerRepository.Get<XperiaGamesClubManager>().IsSonyDevice && !XperiaGiftPopupManager.PersistedXperiaUserRewarded;
			}
		}

		public void TryShowPopup(int unlockedLevelIndex, MapPopupManager.PopupFlow popupFlow)
		{
			if (this.ShouldShowPopup)
			{
				popupFlow.AddPopup(this.ShowPopup());
			}
		}

		public IEnumerator ShowPopup()
		{
			XperiaGiftPopupManager.PersistedXperiaUserRewarded = true;
			this.provider.GiveRewards();
			UIViewManager.UIViewState vs = this.provider.ShowGiftView();
			yield return vs.WaitForClose();
			yield break;
		}

		private static bool PersistedXperiaUserRewarded
		{
			get
			{
				return TactilePlayerPrefs.GetBool("XperiaUserRewarded", false);
			}
			set
			{
				TactilePlayerPrefs.SetBool("XperiaUserRewarded", value);
			}
		}

		private XperiaGiftPopupManager.IDataProvider provider;

		public interface IDataProvider
		{
			UIViewManager.UIViewState ShowGiftView();

			void GiveRewards();
		}
	}
}
