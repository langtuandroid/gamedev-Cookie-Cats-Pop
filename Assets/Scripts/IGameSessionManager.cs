using System;

public interface IGameSessionManager
{
	event Action NewSessionStarted;

	bool HasPendingSessionForRecipient(object recipient);

	void ConsumeSessionForRecipient(object recipient);
}
