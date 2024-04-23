using System;

namespace TactileModules.UserSupport.ViewComponents
{
	public class InputDefaultOrEmptyValidator : InputValidator
	{
		public override bool IsValid()
		{
			return !string.IsNullOrEmpty(base.Input) && !base.Input.Equals(base.GetInputField().DefaultValue);
		}

		public override string GetErrorMessage()
		{
			return base.GetInputField().DefaultValue + L.Get(" cannot be empty");
		}
	}
}
