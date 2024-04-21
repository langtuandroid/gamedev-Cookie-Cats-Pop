using System;
using UnityEngine;

[SingletonAssetPath("Assets/[Database]/Resources/ViewAnimatorCurves.asset")]
public class ViewAnimatorCurves : SingletonAsset<ViewAnimatorCurves>
{
	public ViewAnimatorCurves.Curve GetCurve(string name)
	{
		for (int i = 0; i < this.curves.Length; i++)
		{
			if (this.curves[i].name == name)
			{
				return this.curves[i];
			}
		}
		return null;
	}

	public ViewAnimatorCurves.Curve[] curves;

	[Serializable]
	public class Curve
	{
		public string name;

		public AnimationCurve In;

		public AnimationCurve Out;

		public AnimationCurve InScale;

		public AnimationCurve OutScale;
	}
}
