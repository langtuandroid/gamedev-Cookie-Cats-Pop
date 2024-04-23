using System;

namespace ConfigSchema
{
	[AttributeUsage(AttributeTargets.Property)]
	public class DevelopmentStateAttribute : Attribute
	{
		public DevelopmentStateAttribute(params DevelopmentStateAttribute.DevelopmentState[] developmentState)
		{
			if (developmentState.Length == 0)
			{
				throw new Exception("Amount of developmentStates can not be 0!");
			}
			this.DevelopmentStates = developmentState;
		}

		public readonly DevelopmentStateAttribute.DevelopmentState[] DevelopmentStates;

		public enum DevelopmentState
		{
			Default,
			dev,
			prod
		}
	}
}
