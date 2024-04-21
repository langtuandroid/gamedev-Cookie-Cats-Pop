using System;
using UnityEngine;

public class ZSorter : MonoBehaviour
{
	private void LateUpdate()
	{
		float z = base.transform.position.z;
		float y = base.transform.position.y;
		float num = this.layer.Z();
		if (Mathf.Abs(z - num) > 0.01f)
		{
			Vector3 position = base.transform.position;
			position.z = num + y / 10000f;
			base.transform.position = position;
		}
	}

	public ZLayer layer;
}
