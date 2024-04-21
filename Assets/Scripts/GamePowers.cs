using System;
using System.Collections.Generic;
using UnityEngine;

public class GamePowers
{
	public GamePowers()
	{
		this.powers = new Dictionary<MatchFlag, GamePowers.Power>();
	}

	public GamePowers.Power this[MatchFlag color]
	{
		get
		{
			return (!this.powers.ContainsKey(color)) ? null : this.powers[color];
		}
	}

	public void Initialize(LevelSession session)
	{
		int maxValue = (session.AdjustedOverrideChargeAmount <= 0) ? 20 : session.AdjustedOverrideChargeAmount;
		foreach (MatchFlag matchFlag in session.AdjustedEnabledPowerupColors)
		{
			GamePowers.Power power = new GamePowers.Power(this, maxValue, matchFlag);
			this.powers.Add(matchFlag, power);
			GamePowers.Power power2 = power;
			power2.Activated = (Action)Delegate.Combine(power2.Activated, new Action(delegate()
			{
				this.ResetOverrideCombination();
				if (this.PowerActivated != null)
				{
					this.PowerActivated(power);
				}
			}));
		}
		this.Reset();
	}

	public void Reset()
	{
		foreach (KeyValuePair<MatchFlag, GamePowers.Power> keyValuePair in this.powers)
		{
			keyValuePair.Value.Reset();
		}
		if (this.OnReset != null)
		{
			this.OnReset();
		}
	}

	public List<GamePowers.Power> ActivatedPowers
	{
		get
		{
			List<GamePowers.Power> list = new List<GamePowers.Power>();
			foreach (KeyValuePair<MatchFlag, GamePowers.Power> keyValuePair in this.powers)
			{
				if (keyValuePair.Value.IsActivated)
				{
					list.Add(keyValuePair.Value);
				}
			}
			return list;
		}
	}

	public PieceId GetPowerPiece()
	{
		List<GamePowers.Power> activatedPowers = this.ActivatedPowers;
		if (activatedPowers.Count == 0)
		{
			return PieceId.Empty;
		}
		return PieceId.Create<ComboPowerPiece>(string.Empty);
	}

	public void FillInstantly(FillPowerPiece piece)
	{
		GamePowers.Power power = this[piece.MatchFlag];
		if (power == null)
		{
			return;
		}
		if (power.IsCharged)
		{
			return;
		}
		power.FillInstantly(piece.MatchFlag, piece.transform.position);
	}

	public void CollectPieceForCharging(Piece piece)
	{
		GamePowers.Power power = this[piece.MatchFlag];
		if (power == null)
		{
			return;
		}
		if (power.IsCharged)
		{
			return;
		}
		power.AddValue(1, piece.MatchFlag, piece.transform.position);
	}

	public void DoAwardFullBonusPoints()
	{
		if (this.OnAwardPointsForFull != null)
		{
			this.OnAwardPointsForFull();
		}
	}

	public bool AnyActive
	{
		get
		{
			foreach (KeyValuePair<MatchFlag, GamePowers.Power> keyValuePair in this.powers)
			{
				if (keyValuePair.Value.IsActivated)
				{
					return true;
				}
			}
			return false;
		}
	}

	public bool IsColorActivated(MatchFlag color)
	{
		GamePowers.Power power;
		return this.powers.TryGetValue(color, out power) && power.IsActivated;
	}

	public bool UseThreewayShot
	{
		get
		{
			if (!this.IsColorActivated("Yellow"))
			{
				return false;
			}
			int num = 0;
			foreach (KeyValuePair<MatchFlag, GamePowers.Power> keyValuePair in this.powers)
			{
				if (keyValuePair.Value.IsActivated)
				{
					num++;
				}
			}
			return num < 3;
		}
	}

	public void SetOverrideCombination(PowerCombination combination)
	{
		this.overrideCombination = combination;
	}

	public void ResetOverrideCombination()
	{
		this.overrideCombination = PowerCombination.None;
	}

	public PowerCombination CurrentCombination
	{
		get
		{
			if (this.overrideCombination != PowerCombination.None)
			{
				return this.overrideCombination;
			}
			PowerCombination result = default(PowerCombination);
			if (this.IsColorActivated("Yellow"))
			{
				result.EnableColor(PowerColor.Yellow);
			}
			if (this.IsColorActivated("Red"))
			{
				result.EnableColor(PowerColor.Red);
			}
			if (this.IsColorActivated("Blue"))
			{
				result.EnableColor(PowerColor.Blue);
			}
			if (this.IsColorActivated("Green"))
			{
				result.EnableColor(PowerColor.Green);
			}
			return result;
		}
	}

	public IEnumerable<GamePowers.Power> AvailablePowers
	{
		get
		{
			foreach (KeyValuePair<MatchFlag, GamePowers.Power> item in this.powers)
			{
				yield return item.Value;
			}
			yield break;
		}
	}

	public const int DEFAULT_CHARGE = 20;

	public NestedEnabler ChargingEnabled;

	private Dictionary<MatchFlag, GamePowers.Power> powers;

	public Action<GamePowers.Power> PowerActivated;

	public Action OnReset;

	public Action OnAwardPointsForFull;

	public readonly Dictionary<MatchFlag, PieceId> PowerPiecePerColor = new Dictionary<MatchFlag, PieceId>
	{
		{
			"Red",
			PieceId.Create<FirePiece>(string.Empty)
		},
		{
			"Blue",
			PieceId.Create<PowerPieceVertical>(string.Empty)
		},
		{
			"Green",
			PieceId.Create<PowerPieceHorizontal>(string.Empty)
		},
		{
			"Yellow",
			PieceId.Create<PowerPieceTriple>(string.Empty)
		}
	};

	private PowerCombination overrideCombination = PowerCombination.None;

	public class Power
	{
		public Power(GamePowers powers, int maxValue, MatchFlag color)
		{
			this.powers = powers;
			this.MaxValue = maxValue;
			this.Color = color;
		}

		public int MaxValue { get; private set; }

		public int Value
		{
			get
			{
				return this.value;
			}
		}

		public void AddValue(int amount, MatchFlag color, Vector3 worldPosition)
		{
			this.value = Mathf.Min(this.value + amount, this.MaxValue);
			if (this.ValueChanged != null)
			{
				this.ValueChanged(color, worldPosition);
			}
		}

		public void FillInstantly(MatchFlag color, Vector3 worldPosition)
		{
			this.value = this.MaxValue;
			if (this.ValueChanged != null)
			{
				this.ValueChanged(color, worldPosition);
			}
		}

		public float Progress
		{
			get
			{
				return (float)this.Value / (float)this.MaxValue;
			}
		}

		public bool IsCharged
		{
			get
			{
				return this.value >= this.MaxValue;
			}
		}

		public bool IsActivated { get; private set; }

		public MatchFlag Color { get; private set; }

		public bool IsChargingEnabled
		{
			get
			{
				return this.powers.ChargingEnabled;
			}
		}

		public void Activate()
		{
			this.IsActivated = true;
			this.value = 0;
			if (this.Activated != null)
			{
				this.Activated();
			}
		}

		public void Reset()
		{
			this.IsActivated = false;
		}

		public PieceId GetPieceId()
		{
			return PieceId.Create<ComboPowerPiece>(string.Empty);
		}

		private GamePowers powers;

		private int value;

		public Action<MatchFlag, Vector3> ValueChanged;

		public Action Activated;
	}
}
