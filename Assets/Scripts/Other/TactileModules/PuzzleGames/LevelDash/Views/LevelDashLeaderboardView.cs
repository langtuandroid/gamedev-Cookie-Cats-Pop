using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TactileModules.PuzzleGames.LevelDash.Views
{
	public class LevelDashLeaderboardView : UIView
	{
		public void Init(Func<string> getRemainingTimeFunc, List<Entry> prevStateEntries, Func<Entry, List<Entry>, GameObject, GameObject> instantiateCellItemFunc, Action onInfoButtonClickedCallback, int rankFrom, int rankTo)
		{
			this.getRemainingTimeFunc = getRemainingTimeFunc;
			this.instantiateCellItemFunc = instantiateCellItemFunc;
			this.onInfoButtonClickedCallback = onInfoButtonClickedCallback;
			this.rankFrom = rankFrom;
			this.rankTo = rankTo;
			this.UpdateList(prevStateEntries);
		}

		protected void Update()
		{
			this.timerLabel.text = this.getRemainingTimeFunc();
		}

		private void UpdateList(List<Entry> prevStateEntries)
		{
			this.itemList.DestroyAllContent();
			this.itemList.BeginAdding();
			for (int i = 0; i < prevStateEntries.Count; i++)
			{
				GameObject gameObject = this.instantiateCellItemFunc(prevStateEntries[i], prevStateEntries, this.elementPrefab);
				UIElement component = gameObject.GetComponent<UIElement>();
				this.itemList.AddToContent(component);
			}
			this.itemList.EndAdding();
			if (this.rankFrom != this.rankTo && this.rankFrom > -1 && this.rankTo > -1)
			{
				FiberCtrl.Pool.Run(this.AnimateMyCell(), false);
			}
			else if (this.rankTo > -1)
			{
				FiberCtrl.Pool.Run(this.AnimateFocusOnMyCell(), false);
			}
		}

		private IEnumerator AnimateMyCell()
		{
			UICamera.DisableInput();
			yield return this.itemList.AnimateCellToPosition(this.rankFrom, this.rankTo, 0.4f, 0.3f, this.cellScaleCurve, this.cellTranslationCurve);
			UICamera.EnableInput();
			yield break;
		}

		private IEnumerator AnimateFocusOnMyCell()
		{
			UICamera.DisableInput();
			yield return this.itemList.AnimateScrollToItem(this.rankTo, 0.4f, this.cellTranslationCurve);
			UICamera.EnableInput();
			yield break;
		}

		private void SortCellObjects()
		{
			List<Transform> list = new List<Transform>(this.itemList.ScrollRoot.GetComponentsInChildren<Transform>(true));
			list.Sort((Transform transform1, Transform transform2) => (int)(transform2.localPosition.y - transform1.localPosition.y));
			for (int i = 0; i < list.Count; i++)
			{
				list[i].SetSiblingIndex(i);
			}
		}

		private void OnInfoButtonClicked(UIEvent e)
		{
			if (this.onInfoButtonClickedCallback != null)
			{
				this.onInfoButtonClickedCallback();
			}
		}

		private void OnCloseClicked(UIEvent e)
		{
			base.Close(0);
		}

		[SerializeField]
		private UIListPanel itemList;

		[SerializeField]
		private UILabel timerLabel;

		[SerializeField]
		private GameObject elementPrefab;

		[Header("Cell Animation")]
		[SerializeField]
		private AnimationCurve cellScaleCurve;

		[SerializeField]
		private AnimationCurve cellTranslationCurve;

		private Func<string> getRemainingTimeFunc;

		private Func<Entry, List<Entry>, GameObject, GameObject> instantiateCellItemFunc;

		private Action onInfoButtonClickedCallback;

		private int rankFrom;

		private int rankTo;
	}
}
