using System;
using System.Collections;

namespace Cloud
{
	public class PurchaseResponse : Response
	{
		public Hashtable Receipt
		{
			get
			{
				return (Hashtable)base.data["receipt"];
			}
		}

		public bool IsNew
		{
			get
			{
				return (bool)base.data["isNew"];
			}
		}

		public bool IsSandbox
		{
			get
			{
				return base.data.ContainsKey("isSandbox") && (bool)base.data["isSandbox"];
			}
		}
	}
}
