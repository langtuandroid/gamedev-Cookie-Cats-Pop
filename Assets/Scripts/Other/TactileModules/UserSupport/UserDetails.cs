using System;

namespace TactileModules.UserSupport
{
	public class UserDetails
	{
		[JsonSerializable("name", null)]
		public string Name { get; set; }

		[JsonSerializable("storedMessage", null)]
		public string StoredMessage { get; set; }

		[JsonSerializable("translation", null)]
		public bool PrefersTranslation { get; set; }
	}
}
