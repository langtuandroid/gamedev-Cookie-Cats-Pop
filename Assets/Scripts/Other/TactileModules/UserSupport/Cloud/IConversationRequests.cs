using System;
using System.Collections;
using TactileModules.UserSupport.Cloud.Requests;
using TactileModules.UserSupport.Model;

namespace TactileModules.UserSupport.Cloud
{
	public interface IConversationRequests
	{
		event Action<CloudRequestsSequence> RequestsFailed;

		IEnumerator LoadMessages();

		IEnumerator SubmitMessage(string message);

		IEnumerator SetRead();

		IEnumerator Claim();

		IEnumerator BackupDismissed(Message message);

		IEnumerator BackupApplied(Message message);
	}
}
