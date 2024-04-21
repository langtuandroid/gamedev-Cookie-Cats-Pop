using System;
using UnityEngine;

public class ToggleBySafeArea : MonoBehaviour
{
	private bool HasSafeArea()
	{
		return false;
	}

	private void OnEnable()
	{
		bool flag = this.HasSafeArea();
		if (this.hasSafeAreaPivot != null)
		{
			this.hasSafeAreaPivot.SetActive(flag);
		}
		if (this.noSafeAreaPivot != null)
		{
			this.noSafeAreaPivot.SetActive(!flag);
		}
		if (flag)
		{
			base.transform.position += this.pushVectorForSafeArea;
		}
	}

	public GameObject hasSafeAreaPivot;

	public GameObject noSafeAreaPivot;

	public Vector3 pushVectorForSafeArea;
}
