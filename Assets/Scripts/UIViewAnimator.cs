using System;
using System.Collections;
using UnityEngine;

public abstract class UIViewAnimator : MonoBehaviour
{
	public abstract IEnumerator AnimateIn();

	public abstract IEnumerator AnimateOut();
}
