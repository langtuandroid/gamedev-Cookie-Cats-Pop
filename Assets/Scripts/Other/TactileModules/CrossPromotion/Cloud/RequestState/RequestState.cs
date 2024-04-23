using System;

namespace TactileModules.CrossPromotion.Cloud.RequestState
{
	public class RequestState
	{
		[JsonSerializable("Timestamp", null)]
		public DateTime Timestamp { get; set; }
	}
}
