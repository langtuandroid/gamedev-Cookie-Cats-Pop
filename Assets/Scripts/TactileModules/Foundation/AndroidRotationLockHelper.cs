using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace TactileModules.Foundation
{
	public static class AndroidRotationLockHelper
	{
		public static void Initialize()
		{
			if (AndroidRotationLockHelper._003C_003Ef__mg_0024cache0 == null)
			{
				AndroidRotationLockHelper._003C_003Ef__mg_0024cache0 = new Action<bool>(AndroidRotationLockHelper.HandleOnWindowFocusChanged);
			}
			ActivityManager.onWindowFocusChangedEvent += AndroidRotationLockHelper._003C_003Ef__mg_0024cache0;
		}

		private static void HandleOnWindowFocusChanged(bool v)
		{
			if (DeviceUtilsAndroid.getAccelerometerRotation() == 1)
			{
				Screen.orientation = ScreenOrientation.AutoRotation;
			}
			else
			{
				Screen.orientation = AndroidRotationLockHelper.GetScreenOrientation(Screen.orientation, false);
			}
		}

		public static ScreenOrientation GetScreenOrientation(ScreenOrientation defaultOrientation, bool useAccelerometer)
		{
			int num = (!useAccelerometer) ? DeviceUtilsAndroid.getUserRotation(4) : DeviceUtilsAndroid.getRotation();
			AndroidRotationLockHelper.DeviceRotation rotation = (AndroidRotationLockHelper.DeviceRotation)num;
			ScreenOrientation screenOrientation = AndroidRotationLockHelper.RotationToOrientation(rotation);
			if (AndroidRotationLockHelper.IsOrientationValid(screenOrientation))
			{
				return screenOrientation;
			}
			return defaultOrientation;
		}

		private static ScreenOrientation RotationToOrientation(AndroidRotationLockHelper.DeviceRotation rotation)
		{
			switch (rotation)
			{
			case AndroidRotationLockHelper.DeviceRotation.Rotation_0:
				return ScreenOrientation.Portrait;
			case AndroidRotationLockHelper.DeviceRotation.Rotation_90:
				return ScreenOrientation.LandscapeLeft;
			case AndroidRotationLockHelper.DeviceRotation.Rotation_180:
				return ScreenOrientation.PortraitUpsideDown;
			case AndroidRotationLockHelper.DeviceRotation.Rotation_270:
				return ScreenOrientation.LandscapeRight;
			default:
				return ScreenOrientation.Unknown;
			}
		}

		private static bool IsOrientationValid(ScreenOrientation orientation)
		{
			switch (orientation)
			{
			case ScreenOrientation.Portrait:
				return Screen.autorotateToPortrait;
			case ScreenOrientation.PortraitUpsideDown:
				return Screen.autorotateToPortraitUpsideDown;
			case ScreenOrientation.LandscapeLeft:
				return Screen.autorotateToLandscapeLeft;
			case ScreenOrientation.LandscapeRight:
				return Screen.autorotateToLandscapeRight;
			default:
				return false;
			}
		}

		[CompilerGenerated]
		private static Action<bool> _003C_003Ef__mg_0024cache0;

		public enum DeviceRotation
		{
			Rotation_0,
			Rotation_90,
			Rotation_180,
			Rotation_270,
			NotFound
		}
	}
}
