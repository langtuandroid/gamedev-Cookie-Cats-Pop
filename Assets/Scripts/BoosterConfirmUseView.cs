using System;
using Tactile;
using UnityEngine;

public class BoosterConfirmUseView : UIView
{
	protected override void ViewLoad(object[] parameters)
	{
		BoosterMetaData metaData = InventoryManager.Instance.GetMetaData<BoosterMetaData>((InventoryItem)parameters[0]);
		this.text.text = string.Format(L.Get("Activate {0}"), metaData.Title);
	}

	private void ButtonClicked(UIEvent e)
	{
		base.Close(e.payload);
	}

	public Transform frame;

	public UILabel text;
}
