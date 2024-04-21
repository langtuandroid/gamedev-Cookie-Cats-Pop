using System;
using System.Collections;
using System.Collections.Generic;
using Cloud;

namespace TactileModules.UserSupport.Cloud
{
	public interface IUserSupportCloud
	{
		IEnumerator Submit(string message, string email, string name, Response result, string messageContext = "support_message");

		IEnumerator GetAll(Response response);

		IEnumerator GetArticles(Response response);

		IEnumerator SetRead(Response response);

		IEnumerator ClaimAllAttachments(Response response);

		IEnumerator ClaimAttachments(string messageId, List<string> attachmentNames, Response response);

		IEnumerator CheckUnread(Response response);

		IEnumerator DismissedBackup(string messageId, Response response);

		IEnumerator AppliedBackup(string messageId, Response response);
	}
}
