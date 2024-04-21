using System;
using System.Collections;

public class PurchaseAcknowledgementView : UIView
{
	protected override void ViewDidAppear()
	{
		FiberCtrl.Pool.Run(this.DelayClose(), false);
	}

	private IEnumerator DelayClose()
	{
		yield return FiberHelper.Wait(0.75f, (FiberHelper.WaitFlag)0);
		base.Close(null);
		yield break;
	}

	public void Close(UIEvent e)
	{
		base.Close(0);
	}
}
