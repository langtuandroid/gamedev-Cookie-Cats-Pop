using System;
using System.Collections.Generic;

namespace TactileModules.PuzzleGames.EndlessChallenge.Data
{
	public class LocalPersistableState
	{
		public LocalPersistableState()
		{
			this.Entry = new Entry();
			this.Entries = new List<Entry>();
			this.Users = new List<CloudUser>();
		}

		[JsonSerializable("en", null)]
		public Entry Entry { get; set; }

		[JsonSerializable("ens", typeof(Entry))]
		public List<Entry> Entries { get; set; }

		[JsonSerializable("tcu", typeof(CloudUser))]
		public List<CloudUser> Users { get; set; }

		public CloudUser GetUser(string userId)
		{
			for (int i = 0; i < this.Users.Count; i++)
			{
				CloudUser cloudUser = this.Users[i];
				if (userId == cloudUser.CloudId)
				{
					return cloudUser;
				}
			}
			return null;
		}
	}
}
