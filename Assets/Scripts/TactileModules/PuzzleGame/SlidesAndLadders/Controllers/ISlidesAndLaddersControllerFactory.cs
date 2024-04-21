using System;
using TactileModules.PuzzleGame.SlidesAndLadders.UI;
using TactileModules.PuzzleGame.SlidesAndLadders.Views;

namespace TactileModules.PuzzleGame.SlidesAndLadders.Controllers
{
	public interface ISlidesAndLaddersControllerFactory
	{
		SlidesAndLaddersPlayFlow CreatePlayFlow(LevelProxy levelToPlay);

		SlidesAndLaddersMapFlow CreateAndPushMapFlow();

		SlidesAndLaddersRewardController CreateRewardController();

		SlidesAndLaddersWheelTutorialController CreateTutorialController(SlidesAndLaddersWheelWidget wheelWidget, UIScrollablePanel scrollablePanel, SlidesAndLaddersMapButtonView slidesAndLaddersMapButtonView);

		SlidesAndLaddersWheelWidgetController CreateWheelController(SlidesAndLaddersWheelWidget wheelWidget);
	}
}
