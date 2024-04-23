using System;

namespace com.amazon.device.iap.cpt.log
{
	public class AmazonLogging
	{
		public static void LogError(AmazonLogging.AmazonLoggingLevel reportLevel, string service, string message)
		{
			if (reportLevel == AmazonLogging.AmazonLoggingLevel.Silent)
			{
				return;
			}
			string message2 = string.Format("{0} error: {1}", service, message);
			switch (reportLevel)
			{
			case AmazonLogging.AmazonLoggingLevel.ErrorsAsExceptions:
				throw new Exception(message2);
			}
		}

		public static void LogWarning(AmazonLogging.AmazonLoggingLevel reportLevel, string service, string message)
		{
			switch (reportLevel)
			{
			}
		}

		public static void Log(AmazonLogging.AmazonLoggingLevel reportLevel, string service, string message)
		{
			if (reportLevel != AmazonLogging.AmazonLoggingLevel.Verbose)
			{
				return;
			}
		}

		public static AmazonLogging.SDKLoggingLevel pluginToSDKLoggingLevel(AmazonLogging.AmazonLoggingLevel pluginLoggingLevel)
		{
			switch (pluginLoggingLevel)
			{
			case AmazonLogging.AmazonLoggingLevel.Silent:
				return AmazonLogging.SDKLoggingLevel.LogOff;
			case AmazonLogging.AmazonLoggingLevel.Critical:
				return AmazonLogging.SDKLoggingLevel.LogCritical;
			case AmazonLogging.AmazonLoggingLevel.ErrorsAsExceptions:
			case AmazonLogging.AmazonLoggingLevel.Errors:
				return AmazonLogging.SDKLoggingLevel.LogError;
			case AmazonLogging.AmazonLoggingLevel.Warnings:
			case AmazonLogging.AmazonLoggingLevel.Verbose:
				return AmazonLogging.SDKLoggingLevel.LogWarning;
			default:
				return AmazonLogging.SDKLoggingLevel.LogWarning;
			}
		}

		private const string errorMessage = "{0} error: {1}";

		private const string warningMessage = "{0} warning: {1}";

		private const string logMessage = "{0}: {1}";

		public enum AmazonLoggingLevel
		{
			Silent,
			Critical,
			ErrorsAsExceptions,
			Errors,
			Warnings,
			Verbose
		}

		public enum SDKLoggingLevel
		{
			LogOff,
			LogCritical,
			LogError,
			LogWarning
		}
	}
}
