using System;
using UnityEngine;

namespace TactileModules.GameCore.MenuTutorial
{
	[Serializable]
	public class MenuTutorialStep
	{
		public string ViewGUID
		{
			get
			{
				return this.viewGUID;
			}
			set
			{
				this.viewGUID = value;
			}
		}

		public string ViewName
		{
			get
			{
				return this.viewName;
			}
			set
			{
				this.viewName = value;
			}
		}

		public string Message
		{
			get
			{
				return this.message;
			}
			set
			{
				this.message = value;
			}
		}

		public Vector2 MessagePosition
		{
			get
			{
				return this.messagePosition;
			}
			set
			{
				this.messagePosition = value;
			}
		}

		public Vector2 MessageSize
		{
			get
			{
				return this.messageSize;
			}
			set
			{
				this.messageSize = value;
			}
		}

		public UIAutoSizing MessageLayouting
		{
			get
			{
				return this.messageLayouting;
			}
			set
			{
				this.messageLayouting = value;
			}
		}

		public MenuTutorialStep.DimissTypes DimissType
		{
			get
			{
				return this.dimissType;
			}
			set
			{
				this.dimissType = value;
			}
		}

		public MenuTutorialStep.HighlightEntry Highlight
		{
			get
			{
				return this.highlight;
			}
		}

		[SerializeField]
		private string viewGUID;

		[SerializeField]
		private string viewName;

		[SerializeField]
		private string message;

		[SerializeField]
		private Vector2 messagePosition;

		[SerializeField]
		private Vector2 messageSize;

		[SerializeField]
		private UIAutoSizing messageLayouting;

		[SerializeField]
		private MenuTutorialStep.DimissTypes dimissType;

		[SerializeField]
		private MenuTutorialStep.HighlightEntry highlight = new MenuTutorialStep.HighlightEntry();

		public enum DimissTypes
		{
			Button,
			Click
		}

		[Flags]
		public enum HighlightUIAutoSizing
		{
			None = 0,
			FlexibleWidth = 1,
			FlexibleHeight = 2,
			LeftAnchor = 4,
			RightAnchor = 8,
			TopAnchor = 16,
			BottomAnchor = 32,
			AllCorners = 60,
			FlexibleHeightLeftAnchor = 6,
			FlexibleHeightRightAnchor = 10
		}

		[Serializable]
		public class HighlightEntry
		{
			public bool oval;

			public Vector3 position;

			public Vector2 size;

			public MenuTutorialStep.HighlightUIAutoSizing layouting;
		}
	}
}
