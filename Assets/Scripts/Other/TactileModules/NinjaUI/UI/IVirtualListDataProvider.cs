using System;
using UnityEngine;

namespace TactileModules.NinjaUI.UI
{
	public interface IVirtualListDataProvider
	{
		void FillCellWithData(int index, GameObject cell);
	}
}
