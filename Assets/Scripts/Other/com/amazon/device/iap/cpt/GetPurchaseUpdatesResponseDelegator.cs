using System;
using System.Collections.Generic;

namespace com.amazon.device.iap.cpt
{
	public sealed class GetPurchaseUpdatesResponseDelegator : IDelegator
	{
		public GetPurchaseUpdatesResponseDelegator(GetPurchaseUpdatesResponseDelegate responseDelegate)
		{
			this.responseDelegate = responseDelegate;
		}

		public void ExecuteSuccess()
		{
		}

		public void ExecuteSuccess(Dictionary<string, object> objectDictionary)
		{
			this.responseDelegate(GetPurchaseUpdatesResponse.CreateFromDictionary(objectDictionary));
		}

		public void ExecuteError(AmazonException e)
		{
		}

		public readonly GetPurchaseUpdatesResponseDelegate responseDelegate;
	}
}
