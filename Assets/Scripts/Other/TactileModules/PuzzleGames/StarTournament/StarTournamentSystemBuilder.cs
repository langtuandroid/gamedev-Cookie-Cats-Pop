using System;
using Tactile;
using TactileModules.FeatureManager;
using TactileModules.Portraits;
using TactileModules.PuzzleCore.LevelPlaying;
using TactileModules.PuzzleGame.MainLevels;
using TactileModules.PuzzleGames.Configuration;
using TactileModules.SagaCore;

namespace TactileModules.PuzzleGames.StarTournament
{
    public static class StarTournamentSystemBuilder
    {
        public static IStarTournamentSystem Build(TactileModules.FeatureManager.FeatureManager featureManager, IStarTournamentProvider provider, CloudClientBase cloudClientBase, ISagaCoreSystem sagaCore, IPlayFlowEvents playFlowEvents, MapPopupManager mapPopupManager, IMainProgression mainProgression, ConfigurationManager configurationManager, InventoryManager inventoryManager, IRandomPortraitsAndNames randomPortraitsAndNames)
        {
            ConfigGetter<StarTournamentConfig> configGetter = new ConfigGetter<StarTournamentConfig>(configurationManager);
            StarTournamentManager starTournamentManager = new StarTournamentManager(featureManager, provider, cloudClientBase, playFlowEvents, mainProgression, configGetter, inventoryManager, randomPortraitsAndNames);
            mapPopupManager.RegisterPopupObject(new StarTournamentEndedPopup(starTournamentManager));
            mapPopupManager.RegisterPopupObject(new StarTournamentStartedPopup(starTournamentManager));
            mapPopupManager.RegisterPopupObject(new StarTournamentOldEndedPopup(starTournamentManager));
            mapPopupManager.RegisterPopupObject(new StarTournamentStarsRewardPopup(starTournamentManager, sagaCore.MapFacade));
            mapPopupManager.RegisterPopupObject(new StarTournamentStartSessionPopup(starTournamentManager));
            return new StarTournamentSystem(starTournamentManager);
        }
    }
}
