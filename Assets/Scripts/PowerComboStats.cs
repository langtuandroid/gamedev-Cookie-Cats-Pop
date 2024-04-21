using System;

public class PowerComboStats
{
	public int CatsInCombo
	{
		get
		{
			int num = 0;
			if (this.Blue >= 0)
			{
				num++;
			}
			if (this.Red >= 0)
			{
				num++;
			}
			if (this.Green >= 0)
			{
				num++;
			}
			if (this.Yellow >= 0)
			{
				num++;
			}
			return num;
		}
	}

	public int PurchasedCatsInCombo
	{
		get
		{
			int num = 0;
			if (this.Blue >= 0 && this.Blue < 100)
			{
				num++;
			}
			if (this.Red >= 0 && this.Red < 100)
			{
				num++;
			}
			if (this.Green >= 0 && this.Green < 100)
			{
				num++;
			}
			if (this.Yellow >= 0 && this.Yellow < 100)
			{
				num++;
			}
			return num;
		}
	}

	public void SetColorByMatchflag(MatchFlag matchflag, int value)
	{
		string text = matchflag;
		if (text != null)
		{
			if (!(text == "Blue"))
			{
				if (!(text == "Green"))
				{
					if (!(text == "Red"))
					{
						if (text == "Yellow")
						{
							this.Yellow = value;
						}
					}
					else
					{
						this.Red = value;
					}
				}
				else
				{
					this.Green = value;
				}
			}
			else
			{
				this.Blue = value;
			}
		}
	}

	public int Red = -1;

	public int Blue = -1;

	public int Green = -1;

	public int Yellow = -1;
}
