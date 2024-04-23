using System;

namespace TactileModules.PuzzleGame.PiggyBank.Interfaces
{
	public interface IPiggyBankTutorialViewText
	{
		string NextFreeOpenText(int nextFreeOpenLevelHumanNumber);

		string HowItWorksText(int coinsRequiredForPaidOpening);

		string BuyToOpenDescriptionText(int nextFreeOpenLevelHumanNumber);
	}
}
