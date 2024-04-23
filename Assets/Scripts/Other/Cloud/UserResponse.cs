using System;
using System.Collections;

namespace Cloud
{
	public class UserResponse : Response
	{
		public CloudUser User
		{
			get
			{
				Hashtable hashtable = base.data["user"] as Hashtable;
				return (hashtable == null) ? null : JsonSerializer.HashtableToObject<CloudUser>(hashtable);
			}
		}
	}
}
