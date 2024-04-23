using System;
using System.Collections.Generic;
using UnityEngine;

namespace TactileModules.NinjaUI.UI
{
	public class UIVirtualListPanel : UIListPanel
	{
		public void InitializeList(GameObject cellPrefab, int totalNumberOfCells, IVirtualListDataProvider virtualListDataProvider)
		{
			this.totalCells = totalNumberOfCells;
			this.dataProvider = virtualListDataProvider;
			this.cellSize = cellPrefab.GetElementSize();
			int num = Mathf.Clamp(this.numberOfCellsInPool, 0, totalNumberOfCells);
			base.BeginAdding();
			for (int i = 0; i < num; i++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(cellPrefab.gameObject);
				UIVirtualListPanel.Cell cell = new UIVirtualListPanel.Cell
				{
					index = i,
					gameObject = gameObject
				};
				this.AddToContent(gameObject.GetComponent<UIElement>());
				this.ActivateCell(cell);
				this.pool.Add(cell);
			}
			base.EndAdding();
			base.SetTotalContentSize(base.TotalContentSize.x, (float)totalNumberOfCells * cellPrefab.GetElementSize().y);
			base.SetScroll(Vector2.zero);
			foreach (UIVirtualListPanel.Cell cell2 in this.pool)
			{
				cell2.gameObject.transform.localPosition = new Vector3(0f, -5000f, 0f);
			}
		}

		private void OnDisable()
		{
			this.activeCells.Clear();
			this.pool.Clear();
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			UIVirtualListPanel.CellRange cellRange = this.GetCellRange();
			this.UpdatePool(cellRange);
			this.ActivateNewCells(cellRange);
		}

		private UIVirtualListPanel.CellRange GetCellRange()
		{
			int num = Mathf.Abs(Mathf.RoundToInt((base.ScrollRoot.localPosition.y - this.GetElementSize().y * 0.5f) / this.cellSize.y));
			int num2 = Mathf.Abs(Mathf.RoundToInt((base.ScrollRoot.localPosition.y + this.GetElementSize().y * 0.5f) / this.cellSize.y));
			num = Mathf.Clamp(num, 0, this.totalCells);
			num2 = Mathf.Clamp(num2, 0, this.totalCells);
			int num3 = this.totalCells - num - this.numberOfPreparedCells;
			int num4 = this.totalCells - num2 + this.numberOfPreparedCells;
			num3 = Mathf.Clamp(num3, 0, this.totalCells);
			num4 = Mathf.Clamp(num4, 0, this.totalCells);
			return new UIVirtualListPanel.CellRange
			{
				topIndex = num3,
				bottomIndex = num4
			};
		}

		private void UpdatePool(UIVirtualListPanel.CellRange range)
		{
			foreach (UIVirtualListPanel.Cell item in this.activeCells)
			{
				if (!item.gameObject.activeSelf || item.index < range.topIndex || item.index > range.bottomIndex)
				{
					this.pool.Add(item);
				}
			}
			foreach (UIVirtualListPanel.Cell item2 in this.pool)
			{
				this.activeCells.Remove(item2);
			}
		}

		private void ActivateNewCells(UIVirtualListPanel.CellRange range)
		{
			for (int i = range.topIndex; i < range.bottomIndex; i++)
			{
				if (this.pool.Count == 0)
				{
					break;
				}
				if (!this.ActiveCellsContainsIndex(i))
				{
					this.ActivateCellFromPool(i);
				}
			}
		}

		private bool ActiveCellsContainsIndex(int index)
		{
			foreach (UIVirtualListPanel.Cell cell in this.activeCells)
			{
				if (cell.index == index)
				{
					return true;
				}
			}
			return false;
		}

		private void ActivateCellFromPool(int index)
		{
			UIVirtualListPanel.Cell cell = this.pool[0];
			cell.index = index;
			this.ActivateCell(cell);
			this.activeCells.Add(cell);
			this.pool.RemoveAt(0);
		}

		private void ActivateCell(UIVirtualListPanel.Cell cell)
		{
			this.dataProvider.FillCellWithData(cell.index, cell.gameObject);
			this.SetCellPosition(cell.gameObject, cell.index);
			cell.gameObject.SetActive(true);
		}

		private void SetCellPosition(GameObject cell, int index)
		{
			int num = this.totalCells - index;
			float x = cell.GetElementSize().x;
			float y = cell.GetElementSize().y;
			float x2 = x * 0.5f;
			float y2 = y * (float)num - y * 0.5f;
			cell.transform.localPosition = new Vector3(x2, y2, 0f);
		}

		private readonly List<UIVirtualListPanel.Cell> activeCells = new List<UIVirtualListPanel.Cell>();

		private readonly List<UIVirtualListPanel.Cell> pool = new List<UIVirtualListPanel.Cell>();

		private int numberOfCellsInPool = 30;

		private int numberOfPreparedCells = 5;

		private int totalCells;

		private IVirtualListDataProvider dataProvider;

		private Vector2 cellSize;

		private struct CellRange
		{
			public int topIndex;

			public int bottomIndex;
		}

		private struct Cell
		{
			public int index;

			public GameObject gameObject;
		}
	}
}
