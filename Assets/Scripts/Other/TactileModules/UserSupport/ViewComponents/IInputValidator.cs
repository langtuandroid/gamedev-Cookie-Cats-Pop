using System;

namespace TactileModules.UserSupport.ViewComponents
{
	public interface IInputValidator
	{
		bool IsValid();

		string GetErrorMessage();
	}
}
