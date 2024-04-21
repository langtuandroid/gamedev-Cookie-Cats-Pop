using System;
using System.Collections;

namespace TactileModules.FacebookExtras
{
	public interface IFacebookLoginManagerProvider
	{
		CloudClientBase CloudClient { get; }

		FacebookClient FacebookClient { get; }

		string ProductName { get; }

		IEnumerator PostSuccessfulLogin();
	}
}
