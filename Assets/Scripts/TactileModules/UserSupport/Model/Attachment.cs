using System;
using System.Collections.Generic;

namespace TactileModules.UserSupport.Model
{
	public class Attachment
	{
		public static List<string> GetNonRenderableNames()
		{
			return new List<string>
			{
				"AdsOff"
			};
		}

		[JsonSerializable("name", null)]
		public string Name { get; set; }

		[JsonSerializable("amount", null)]
		public int Amount { get; set; }

		[JsonSerializable("claimed", null)]
		public bool Claimed { get; set; }

		public static ItemAmount ToItemAmount(Attachment attachment)
		{
			return new ItemAmount
			{
				Amount = attachment.Amount,
				ItemId = attachment.Name
			};
		}

		public const string ADS_OFF = "AdsOff";
	}
}
