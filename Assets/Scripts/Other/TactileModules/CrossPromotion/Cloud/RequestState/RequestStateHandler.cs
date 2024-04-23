using System;
using TactileModules.TactilePrefs;

namespace TactileModules.CrossPromotion.Cloud.RequestState
{
	public class RequestStateHandler : IRequestStateHandler
	{
		public RequestStateHandler(ILocalStorageObject<RequestState> localStorageObject)
		{
			this.localStorageObject = localStorageObject;
			this.requestState = localStorageObject.Load();
		}

		public void SetLastSuccessfulRequestTimestamp(DateTime dateTime)
		{
			this.requestState.Timestamp = dateTime;
			this.localStorageObject.Save(this.requestState);
		}

		public DateTime GetLastSuccessfulRequestTimestamp()
		{
			return this.requestState.Timestamp;
		}

		private readonly ILocalStorageObject<RequestState> localStorageObject;

		private readonly RequestState requestState;
	}
}
