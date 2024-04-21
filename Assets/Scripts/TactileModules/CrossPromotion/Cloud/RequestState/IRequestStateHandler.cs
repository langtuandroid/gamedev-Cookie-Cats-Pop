using System;

namespace TactileModules.CrossPromotion.Cloud.RequestState
{
	public interface IRequestStateHandler
	{
		void SetLastSuccessfulRequestTimestamp(DateTime dateTime);

		DateTime GetLastSuccessfulRequestTimestamp();
	}
}
