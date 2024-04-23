using System;

[Flags]
public enum UIAutoSizing
{
	FlexibleWidth = 1,
	FlexibleHeight = 2,
	LeftAnchor = 4,
	RightAnchor = 8,
	TopAnchor = 16,
	BottomAnchor = 32,
	AllCorners = 60
}
