using System;
using Shared.LevelDash.Module;
using TactileModules.FeatureManager;
using TactileModules.PuzzleGames.LevelDash.Popups;
using TactileModules.PuzzleGames.LevelDash.Providers;
using TactileModules.SagaCore;

namespace TactileModules.PuzzleGames.LevelDash
{
    public static class LevelDashSystemBuilder
    {
        public static ILevelDashSystem Build(TactileModules.FeatureManager.FeatureManager featureManager, CloudClientBase cloudClient, MapPopupManager mapPopupManager, ISagaCoreSystem sagaCore, GameSessionManager gameSessionManager, ILevelDashDataProvider levelDashDataProvider, ILevelDashMapAvatarModifierProvider mapAvatarModifierProvider, ILevelDashViewProvider levelDashViewProvider)
        {
            LevelDashManager manager = new LevelDashManager(featureManager, cloudClient, levelDashDataProvider);
            LevelDashViewController levelDashViewController = new LevelDashViewController(manager, levelDashDataProvider, levelDashViewProvider);
            mapPopupManager.RegisterPopupObject(new LevelDashStartPopup(manager, levelDashViewController));
            mapPopupManager.RegisterPopupObject(new LevelDashStartSessionPopup(manager, levelDashViewController));
            mapPopupManager.RegisterPopupObject(new LevelDashEndPopup(manager, levelDashViewController, gameSessionManager));
            mapPopupManager.RegisterPopupObject(new LevelDashPreviousEndPopup(manager, levelDashViewController));
            MainMapPlugin item = new MainMapPlugin(mapAvatarModifierProvider);
            sagaCore.MapFacade.MapPlugins.Add(item);
            LevelDashCommandHandler.InjectDependencies(levelDashViewController);
            return new LevelDashSystem(manager, levelDashViewController);
        }
    }
}
