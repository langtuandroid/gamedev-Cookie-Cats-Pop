using System;
using Tactile;
using TactileModules.FacebookExtras;
using TactileModules.Placements;
using TactileModules.PuzzleGame.MainLevels;
using TactileModules.PuzzleGames.Configuration;

public static class FacebookPlacementsBuilder
{
	public static void Build(IPlacementRunnableRegistry registry, ConfigurationManager configurationManager, FacebookLoginManager loginManager, IMainProgression mainProgression, FacebookClient facebookClient, SendLivesAtStartManager sendLivesAtStartManager)
	{
		ConfigGetter<FacebookNotification> configGetter = new ConfigGetter<FacebookNotification>(configurationManager);
		registry.RegisterRunnable(new PlayWithFriendsPlacement(configGetter, loginManager, mainProgression), PlacementIdentifier.SessionStart, PlacementBehavior.Skippable);
		registry.RegisterRunnable(new SendLivesToFriendsPlacement(facebookClient, sendLivesAtStartManager), PlacementIdentifier.SessionStart, PlacementBehavior.Skippable);
	}
}
