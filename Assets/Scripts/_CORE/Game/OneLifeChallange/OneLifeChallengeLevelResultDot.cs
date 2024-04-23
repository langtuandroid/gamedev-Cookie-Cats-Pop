using System;
using System.Collections;
using UnityEngine;

public class OneLifeChallengeLevelResultDot : MonoBehaviour
{
	public void SetDotRoot(int levelNumber, bool completed, bool showCompletedMark)
	{
		this.levelNumber.text = levelNumber.ToString();
		this.completedRoot.SetActive(completed);
		this.uncompletedRoot.SetActive(!completed);
		this.checkmark.SetActive(showCompletedMark);
		this.failmark.SetActive(false);
	}

	public IEnumerator AnimateMark(bool completed, bool instantly = false)
	{
		this.completedRoot.SetActive(true);
		this.uncompletedRoot.SetActive(false);
		GameObject mark = (!completed) ? this.failmark : this.checkmark;
		if (instantly)
		{
			mark.SetActive(true);
		}
		else
		{
			yield return this.AnimateMark(mark);
		}
		yield break;
	}

	private IEnumerator AnimateMark(GameObject mark)
	{
		Vector3 initialScale = Vector3.one * 8f;
		yield return FiberHelper.Wait(0.2f, (FiberHelper.WaitFlag)0);
		mark.SetActive(true);
		mark.transform.localScale = initialScale;
		yield return FiberAnimation.ScaleTransform(mark.transform, initialScale, Vector3.one, null, 0.2f);
		yield break;
	}

	public GameObject completedRoot;

	public GameObject uncompletedRoot;

	public UILabel levelNumber;

	public GameObject checkmark;

	public GameObject failmark;
}
