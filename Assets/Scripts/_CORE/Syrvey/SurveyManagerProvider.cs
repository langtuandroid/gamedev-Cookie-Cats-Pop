using System;
using System.Collections;

public class SurveyManagerProvider : SurveyManager.ISurveyProvider
{
	public IEnumerator ShowRewards(SurveyRewardData reward)
	{
		if (reward == null || reward.Items.Count <= 0)
		{
			yield break;
		}
		UIViewManager.UIViewState vs = UIViewManager.Instance.ShowView<SurveyRewardView>(new object[]
		{
			reward
		});
		yield return vs.WaitForClose();
		yield break;
	}
	
}
