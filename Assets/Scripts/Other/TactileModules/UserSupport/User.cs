using System;
using TactileModules.TactilePrefs;

namespace TactileModules.UserSupport
{
	public class User : IUser
	{
		public User(IUserStorageFactory storageFactory)
		{
			this.storageFactory = storageFactory;
			this.LoadStoredUser();
		}

		public string Name
		{
			get
			{
				return this.GetName();
			}
			set
			{
				this.SetName(value);
			}
		}

		public string Email
		{
			get
			{
				return this.GetEmail();
			}
			set
			{
				this.SetEmail(value);
			}
		}

		private void LoadStoredUser()
		{
			this.storage = this.storageFactory.GetStorage("UserSupport", "_User");
			this.userData = this.storage.Load();
		}

		private string GetName()
		{
			return this.userData.Name;
		}

		private void SetName(string n)
		{
			this.userData.Name = n;
			this.storage.Save(this.userData);
		}

		private string GetEmail()
		{
			return string.Empty;
		}

		private void SetEmail(string e)
		{
		}
		

		public void Clear()
		{
			this.storage.Delete();
		}

		public bool PrefersTranslation
		{
			get
			{
				return this.userData.PrefersTranslation;
			}
			set
			{
				this.userData.PrefersTranslation = value;
				this.storage.Save(this.userData);
			}
		}

		public override string ToString()
		{
			return "[TactileModules.UserSupport.User] Name: " + this.Name;
		}

		public string StoredMessage
		{
			get
			{
				return this.userData.StoredMessage;
			}
			set
			{
				this.userData.StoredMessage = value;
			}
		}

		public const string PLAYER_PREFS_DOMAIN = "UserSupport";

		public const string PLAYER_PREFS_USER_KEY = "_User";

		private IUserStorageFactory storageFactory;

		private UserDetails userData;

		private ILocalStorageObject<UserDetails> storage;
	}
}
