using System;

namespace com.amazon.device.iap.cpt
{
	public class AmazonLogger
	{
		public AmazonLogger(string tag)
		{
			this.tag = tag;
		}

		public void Debug(string msg)
		{
			AmazonLogging.Log(AmazonLogging.AmazonLoggingLevel.Verbose, this.tag, msg);
		}

		public string getTag()
		{
			return this.tag;
		}

		private readonly string tag;
	}
}
