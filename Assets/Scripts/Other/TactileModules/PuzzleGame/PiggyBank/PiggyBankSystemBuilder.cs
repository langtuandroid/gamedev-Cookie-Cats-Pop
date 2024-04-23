using System;
using Shared.PiggyBank.Module.Interfaces;
using Tactile;
using TactileModules.PuzzleGame.MainLevels;
using TactileModules.PuzzleGame.PiggyBank.Controllers;
using TactileModules.PuzzleGame.PiggyBank.Interfaces;
using TactileModules.PuzzleGame.PiggyBank.Models;
using TactileModules.PuzzleGame.PiggyBank.Popups;
using TactileModules.SagaCore;

namespace TactileModules.PuzzleGame.PiggyBank
{
	public static class PiggyBankSystemBuilder
	{
		public static PiggyBankSystem Build(MapFacade mapFacade, MainProgressionManager mainProgression, IIAPProvider iapProvider, IMapProvider mapProvider, IItemsProvider itemsProvider, IPiggyBankProvider provider, UserSettingsManager userSettingsManager, ConfigurationManager configurationManager, InAppPurchaseManager inAppPurchaseManager, MapPopupManager mapPopupManager)
		{
			PersistableDataProvider<PiggyBankPersistableState> persistableData = new PersistableDataProvider<PiggyBankPersistableState>(userSettingsManager);
			ConfigProvider<PiggyBankConfig> config = new ConfigProvider<PiggyBankConfig>(configurationManager);
			PiggyBankRewards rewards = new PiggyBankRewards(provider, itemsProvider, persistableData, config);
			PiggyBankProgression piggyBankProgression = new PiggyBankProgression(provider, mainProgression, persistableData, config);
			PiggyBankControllerFactory controllerFactory = new PiggyBankControllerFactory(mainProgression, iapProvider, provider, inAppPurchaseManager, piggyBankProgression, rewards);
			PiggyBankMapPlugin piggyBankMapPlugin = new PiggyBankMapPlugin(piggyBankProgression, mainProgression, mapProvider);
			mapFacade.MapPlugins.Add(piggyBankMapPlugin);
			mapPopupManager.RegisterPopupObject(new PiggyBankTutorialPopup(controllerFactory, piggyBankMapPlugin, piggyBankProgression));
			mapPopupManager.RegisterPopupObject(new PiggyBankFreeOpenPopup(controllerFactory, piggyBankMapPlugin, piggyBankProgression));
			return new PiggyBankSystem(controllerFactory, piggyBankProgression, rewards);
		}
	}
}
