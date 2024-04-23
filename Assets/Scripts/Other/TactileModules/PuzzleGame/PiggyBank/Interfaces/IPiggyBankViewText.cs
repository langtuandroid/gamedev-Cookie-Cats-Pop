using System;

namespace TactileModules.PuzzleGame.PiggyBank.Interfaces
{
	public interface IPiggyBankViewText
	{
		string FreeOpenNotAvailableText(int nextFreeOpenLevelHumanNumber);

		string CollectCoinsNotAvailableText(int coinsRequiredForPaidOpening);

		string NextFreeAvailableAtText(int nextFreeOpenLevelHumanNumber);
	}
}
