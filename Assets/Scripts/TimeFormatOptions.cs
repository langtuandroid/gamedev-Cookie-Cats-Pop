using System;

[Flags]
public enum TimeFormatOptions
{
	None = 0,
	DisallowDaysFormatting = 1,
	HideHoursIfZero = 2,
	All = 3
}
