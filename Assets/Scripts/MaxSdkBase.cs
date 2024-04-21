using System;
using UnityEngine;

public abstract class MaxSdkBase
{
	protected static void ValidateAdUnitIdentifier(string adUnitIdentifier, string debugPurpose)
	{
		if (string.IsNullOrEmpty(adUnitIdentifier))
		{
		}
	}

	protected static void InitCallbacks()
	{
		Type typeFromHandle = typeof(MaxSdkCallbacks);
		MaxSdkCallbacks component = new GameObject("MaxSdkCallbacks", new Type[]
		{
			typeFromHandle
		}).GetComponent<MaxSdkCallbacks>();
		if (MaxSdkCallbacks.Instance != component)
		{
		}
	}

	public enum ConsentDialogState
	{
		Unknown,
		Applies,
		DoesNotApply
	}

	public enum BannerPosition
	{
		TopLeft,
		TopCenter,
		TopRight,
		Centered,
		BottomLeft,
		BottomCenter,
		BottomRight
	}

	public class SdkConfiguration
	{
		public MaxSdkBase.ConsentDialogState ConsentDialogState;
	}

	public struct Reward
	{
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"Reward: ",
				this.Amount,
				" ",
				this.Label
			});
		}

		public bool IsValid()
		{
			return !string.IsNullOrEmpty(this.Label) && this.Amount > 0;
		}

		public string Label;

		public int Amount;
	}
}
