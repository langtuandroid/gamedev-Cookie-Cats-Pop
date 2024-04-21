using System;
using UnityEngine;

[SingletonAssetPath("Assets/[Database]/Resources/SoundDatabase.asset")]
public class SoundDatabase : SingletonAsset<SoundDatabase>
{
	public SoundDatabase.PowerSounds GetPowerSounds(MatchFlag color)
	{
		string text = color;
		if (text != null)
		{
			if (text == "Blue")
			{
				return this.powerBlue;
			}
			if (text == "Red")
			{
				return this.powerRed;
			}
			if (text == "Yellow")
			{
				return this.powerYellow;
			}
			if (text == "Green")
			{
				return this.powerGreen;
			}
		}
		return null;
	}

	[Header("Music")]
	public SoundDefinition ingameMusic;

	public SoundDefinition mapMusic;

	[Header("Gameplay")]
	public SoundDefinition aim;

	public SoundDefinition pinkThrow;

	public SoundDefinition releaseForShot;

	public SoundDefinition shoot;

	public SoundDefinition shotHits;

	public SoundDefinition bubblePop;

	public SoundDefinition deathBubblePop;

	public SoundDefinition deathBubbleSquish;

	public SoundDefinition kittenReleased;

	public SoundDefinition swapBubbles;

	public SoundDefinition receivePoints;

	public SoundDefinition poppedByPower;

	public SoundDefinition aftermathCannon;

	public SoundDefinition morphBubbleChange;

	public SoundDefinition minusTwo;

	public SoundDefinition plusTwo;

	public SoundDefinition hitBouncer;

	public SoundDefinition fortuneCookieCollected;

	public SoundDefinition cookieToPowercat;

	public SoundDefinition powerActivation;

	public SoundDefinition burn;

	public SoundDefinition frog;

	public SoundDefinition seagullHit;

	[Header("Squid Sounds")]
	public SoundDefinition squidLaugh;

	public SoundDefinition squidJump;

	public SoundDefinition squidHit;

	public SoundDefinition squidDeath;

	[Header("Power Cats")]
	public SoundDatabase.PowerSounds powerRed = new SoundDatabase.PowerSounds();

	public SoundDatabase.PowerSounds powerGreen = new SoundDatabase.PowerSounds();

	public SoundDatabase.PowerSounds powerYellow = new SoundDatabase.PowerSounds();

	public SoundDatabase.PowerSounds powerBlue = new SoundDatabase.PowerSounds();

	[Header("UI")]
	public SoundDefinition showObjective;

	public SoundDefinition goalAchieved;

	public SoundDefinition levelResultStar;

	public SoundDefinition levelFailed;

	public SoundDefinition fortuneCatActivated;

	public SoundDefinition levelResultFireworks;

	public SoundDefinition victoriousMotif;

	[Header("VIP")]
	public SoundDefinition vipReward;

	public SoundDefinition vipRewardBling;

	[Header("Reward")]
	public SoundDefinition rewardChestAnticipate;

	public SoundDefinition rewardChestOpen;

	[Header("Slides And Ladders")]
	public SoundDefinition slideDown;

	public SoundDefinition climbLadder;

	public SoundDefinition wheelAvailable;

	public SoundDefinition wheelSpinning;

	public SoundDefinition wheelStopped;

	[Header("Boss Fight")]
	public SoundDefinition bossEvilLaugh;

	public SoundDefinition bossKittenUhOh;

	public SoundDefinition bossKittenMeow;

	public SoundDefinition bossKittenSwoosh;

	public SoundDefinition bossSuction;

	public SoundDefinition bossSuctionPlop;

	public SoundDefinition bossBlowingWind;

	public SoundDefinition bossBlowingSingleBubble;

	public SoundDefinition bossHit;

	public SoundDefinition bossDestroyed;

	public SoundDefinition bossKittenYipee;

	public SoundDefinition bossDoorClose;

	public SoundDefinition bossNoisesLoop;

	public SoundDefinition bossMusic;

	[Serializable]
	public class PowerSounds
	{
		public SoundDefinition thanksForCookie;

		public SoundDefinition ready;

		public SoundDefinition activated;

		public SoundDefinition resolve;
	}
}
