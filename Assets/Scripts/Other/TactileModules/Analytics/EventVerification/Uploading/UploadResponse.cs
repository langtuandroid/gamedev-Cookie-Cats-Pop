using System;
using System.Collections;

namespace TactileModules.Analytics.EventVerification.Uploading
{
	public class UploadResponse : IUploadResponse
	{
		public UploadResponse(string responseText)
		{
			this.ParseResponseText(responseText);
		}

		public ReturnCode ResponseCode { get; private set; }

		private void ParseResponseText(string responseText)
		{
			Hashtable hashtable = MiniJSON.jsonDecode(responseText) as Hashtable;
			try
			{
				this.ResponseCode = (ReturnCode)((double)hashtable["rc"]);
			}
			catch
			{
				this.ResponseCode = ReturnCode.ClientConnectionError;
			}
		}
	}
}
