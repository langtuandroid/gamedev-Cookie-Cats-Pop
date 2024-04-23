using System;

namespace TactileModules.UserSupport.Model
{
	public class SubmitMessageData
	{
		public string Name { get; set; }

		public string Email { get; set; }

		public string Message { get; set; }

		public string Context { get; set; }

		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"[SubmitMessageData] Name: ",
				this.Name,
				", Email: ",
				this.Email,
				", Message: ",
				this.Message
			});
		}
	}
}
