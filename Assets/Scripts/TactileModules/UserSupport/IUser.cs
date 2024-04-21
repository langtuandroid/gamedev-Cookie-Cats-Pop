using System;

namespace TactileModules.UserSupport
{
	public interface IUser
	{
		string Name { get; set; }

		string Email { get; set; }

		string StoredMessage { get; set; }

		void Clear();

		bool PrefersTranslation { get; set; }
	}
}
