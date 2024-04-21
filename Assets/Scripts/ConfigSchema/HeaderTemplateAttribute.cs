using System;

namespace ConfigSchema
{
	[AttributeUsage(AttributeTargets.Property)]
	public class HeaderTemplateAttribute : Attribute
	{
		public HeaderTemplateAttribute(string header)
		{
			this.Header = header;
		}

		public string Header { get; private set; }
	}
}
