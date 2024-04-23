using System;
using System.Collections;

namespace Cloud
{
	public class FacebookPaymentResponse : Response
	{
		public Hashtable Payment
		{
			get
			{
				return (Hashtable)base.data["payment"];
			}
		}
	}
}
