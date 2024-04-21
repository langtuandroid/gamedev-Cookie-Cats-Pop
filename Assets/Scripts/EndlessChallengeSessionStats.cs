using System;

public class EndlessChallengeSessionStats
{
	public EndlessChallengeSessionStats(LevelSession session)
	{
		this.CookieJarSmall = 0;
		this.CookieJarFilUp = 0;
		this.BoosterRainbow = 0;
		this.BoosterFinalPower = 0;
		this.Coins = 0;
		this.CheckpointsReached = 0;
		this.ExtraMoves = 0;
	}

	public int CookieJarSmall { get; private set; }

	public int CookieJarFilUp { get; private set; }

	public int BoosterRainbow { get; private set; }

	public int BoosterFinalPower { get; private set; }

	public int Coins { get; private set; }

	public int CheckpointsReached { get; private set; }

	public int ExtraMoves { get; private set; }

	public void CookieJarSmallReceived()
	{
		this.CookieJarSmall++;
	}

	public void CookieJarFillUpReceived()
	{
		this.CookieJarFilUp++;
	}

	public void RainbowBoosterReceived(int amount)
	{
		this.BoosterRainbow += amount;
	}

	public void FinalPowerBoosterReceived(int amount)
	{
		this.BoosterFinalPower += amount;
	}

	public void ReachedNewCheckPoint()
	{
		this.CheckpointsReached++;
	}

	public void CoinsReceived(int amount)
	{
		this.Coins += amount;
	}

	public void GotExtraMoves(int extraMoves)
	{
		this.ExtraMoves += extraMoves;
	}
}
