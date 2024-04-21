using System;
using UnityEngine;

[Serializable]
public struct InventoryItem
{
	public static implicit operator InventoryItem(string val)
	{
		return new InventoryItem
		{
			value = val
		};
	}

	public static implicit operator string(InventoryItem t)
	{
		return t.value;
	}

	public override string ToString()
	{
		return this.value;
	}

	public static bool operator ==(InventoryItem a, InventoryItem b)
	{
		return object.ReferenceEquals(a, b) || ((object)a != (object)null && (object)b != (object)null && a.value == b.value);
	}

	public static bool operator !=(InventoryItem a, InventoryItem b)
	{
		return !(a == b);
	}

	public override bool Equals(object obj)
	{
		return obj is InventoryItem && this.value == ((InventoryItem)obj).value;
	}

	public override int GetHashCode()
	{
		return this.value.GetHashCode();
	}

	public const string BonusDrop = "BonusDrop";

	public const string BoosterRainbowBubble = "BoosterRainbow";

	public const string BoosterFinalPower = "BoosterFinalPower";

	public const string BoosterExtraMoves = "BoosterExtraMoves";

	public const string BoosterSuperQueue = "BoosterSuperQueue";

	public const string BoosterShield = "BoosterShield";

	public const string BoosterSuperAim = "BoosterSuperAim";

	public const string BoosterSuperQueue_Unlimited = "BoosterSuperQueue_Unlimited";

	public const string BoosterShield_Unlimited = "BoosterShield_Unlimited";

	public const string BoosterSuperAim_Unlimited = "BoosterSuperAim_Unlimited";

	public const string Continue = "Continue";

	public const string UnlimitedLives = "UnlimitedLives";

	public const string Coin = "Coin";

	public const string Life = "Life";

	public const string None = null;

	[SerializeField]
	private string value;

	public const string TicketBronze = "TicketBronze";

	public const string TicketSilver = "TicketSilver";

	public const string TicketGold = "TicketGold";

	public const string TournamentLife = "TournamentLife";

	public const string Star = "Star";
}
