using System;
using System.Collections;
using UnityEngine;

public abstract class AnimatingEffect : MonoBehaviour
{
	public abstract IEnumerator Animate();
}
