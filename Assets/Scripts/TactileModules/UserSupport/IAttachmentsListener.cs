using System;
using System.Collections.Generic;

namespace TactileModules.UserSupport
{
	public interface IAttachmentsListener
	{
		void AttachmentClaimed(List<ItemAmount> attachments, Action claimedCelebrationCompleteCallback);

		void AdsOffClaimed();
	}
}
