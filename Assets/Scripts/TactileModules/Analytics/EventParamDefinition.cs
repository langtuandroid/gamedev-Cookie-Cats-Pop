using System;
using System.Reflection;

namespace TactileModules.Analytics
{
	public class EventParamDefinition
	{
		public MemberInfo MemberInfo { get; set; }

		public string MemberName { get; set; }

		public string ParamName { get; set; }

		public Type ValueType { get; set; }

		public bool Required { get; set; }

		public object GetValue(object eventObject)
		{
			object value = MemberInfoUtils.GetValue(this.MemberInfo, eventObject);
			return value.GetType().GetMethod("GetValue").Invoke(value, null);
		}
	}
}
