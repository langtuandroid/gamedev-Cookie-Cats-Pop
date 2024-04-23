using System;

namespace TactileModules.UserSupport.ViewComponents
{
	public class InputEmailValidator : InputValidator
	{
		public override bool IsValid()
		{
			return base.Input.Contains("@") && base.Input.Contains(".");
		}

		public override string GetErrorMessage()
		{
			return "'" + base.Input + "' " + L.Get("Is not a valid email address");
		}
	}
}
