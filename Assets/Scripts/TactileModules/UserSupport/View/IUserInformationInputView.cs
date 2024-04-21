using System;

namespace TactileModules.UserSupport.View
{
	public interface IUserInformationInputView
	{
		event Action Closed;

		event Action Input;

		event Action<string> Submit;

		void Close(int result);
	}
}
