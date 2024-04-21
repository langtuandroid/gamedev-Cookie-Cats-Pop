using System;
using JetBrains.Annotations;

[TactileAnalytics.EventAttribute("clientError", true)]
public class ClientErrorEvent
{
	public ClientErrorEvent([NotNull] string errorName, [NotNull] string stackTrace, Exception exception = null, string data0 = null, string data1 = null, string data2 = null, string data3 = null, string data4 = null, string data5 = null)
	{
		if (errorName == null)
		{
			throw new ArgumentNullException("errorName");
		}
		if (stackTrace == null)
		{
			throw new ArgumentNullException("stackTrace");
		}
		this.errorName = errorName;
		this.stackTrace = stackTrace;
		if (exception != null)
		{
			this.exception = exception.ToString();
		}
		this.data0 = data0;
		this.data1 = data1;
		this.data2 = data2;
		this.data3 = data3;
		this.data4 = data4;
		this.data5 = data5;
	}

	private TactileAnalytics.RequiredParam<string> errorName { get; set; }

	private TactileAnalytics.RequiredParam<string> stackTrace { get; set; }

	private TactileAnalytics.OptionalParam<string> exception { get; set; }

	private TactileAnalytics.OptionalParam<string> data0 { get; set; }

	private TactileAnalytics.OptionalParam<string> data1 { get; set; }

	private TactileAnalytics.OptionalParam<string> data2 { get; set; }

	private TactileAnalytics.OptionalParam<string> data3 { get; set; }

	private TactileAnalytics.OptionalParam<string> data4 { get; set; }

	private TactileAnalytics.OptionalParam<string> data5 { get; set; }
}
