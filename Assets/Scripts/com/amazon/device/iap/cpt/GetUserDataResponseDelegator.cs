using System;
using System.Collections.Generic;

namespace com.amazon.device.iap.cpt
{
	public sealed class GetUserDataResponseDelegator : IDelegator
	{
		public GetUserDataResponseDelegator(GetUserDataResponseDelegate responseDelegate)
		{
			this.responseDelegate = responseDelegate;
		}

		public void ExecuteSuccess()
		{
		}

		public void ExecuteSuccess(Dictionary<string, object> objectDictionary)
		{
			this.responseDelegate(GetUserDataResponse.CreateFromDictionary(objectDictionary));
		}

		public void ExecuteError(AmazonException e)
		{
		}

		public readonly GetUserDataResponseDelegate responseDelegate;
	}
}
