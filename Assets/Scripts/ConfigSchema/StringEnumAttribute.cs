using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;

namespace ConfigSchema
{
	[AttributeUsage(AttributeTargets.Property)]
	public class StringEnumAttribute : Attribute
	{
		public StringEnumAttribute([NotNull] params string[] enums)
		{
			if (enums == null)
			{
				throw new ArgumentNullException("enums");
			}
			this.Enums = enums;
		}

		public StringEnumAttribute(Type type, string methodName)
		{
			MethodInfo method = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			object obj = method.Invoke(null, null);
			this.Enums = ((List<string>)obj).ToArray();
		}

		public string[] Enums { get; private set; }
	}
}
