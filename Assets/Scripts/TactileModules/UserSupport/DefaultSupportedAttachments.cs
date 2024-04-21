using System;
using System.Collections.Generic;

namespace TactileModules.UserSupport
{
	public class DefaultSupportedAttachments : ISupportedAttachments
	{
		public DefaultSupportedAttachments(ISupportedAttachments additionalSupportedAttachments)
		{
			this.defaultSupportedAttachments.AddRange(additionalSupportedAttachments.GetSupportedAttachments());
		}

		public List<string> GetSupportedAttachments()
		{
			return this.defaultSupportedAttachments;
		}

		private readonly List<string> defaultSupportedAttachments = new List<string>
		{
			"AdsOff"
		};
	}
}
