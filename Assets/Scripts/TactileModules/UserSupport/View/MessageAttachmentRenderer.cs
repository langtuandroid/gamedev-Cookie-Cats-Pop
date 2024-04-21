using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TactileModules.UserSupport.Model;
using UnityEngine;

namespace TactileModules.UserSupport.View
{
	public class MessageAttachmentRenderer : MonoBehaviour
	{
		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action AttachmentClicked;



		public virtual void Render(List<Attachment> attachments)
		{
			this.attachments = attachments;
			this.HideGraphics();
			if (attachments.Count == 0)
			{
				return;
			}
			this.gift.gameObject.SetActive(true);
			if (attachments.Any((Attachment a) => a.Claimed))
			{
				this.checkmark.gameObject.SetActive(true);
				this.DisableButton();
			}
			else
			{
				this.EnableButton();
			}
		}

		private void EnableButton()
		{
			this.button = base.GetComponent<UIButton>();
			if (this.button != null)
			{
				this.button.Clicked += this.OnClicked;
			}
			base.gameObject.GetComponent<Collider>().enabled = true;
		}

		private void OnClicked(UIButton obj)
		{
			this.AttachmentClicked();
		}

		protected virtual void HideGraphics()
		{
			this.gift.gameObject.SetActive(false);
			this.checkmark.gameObject.SetActive(false);
			this.DisableButton();
		}

		public void MarkAsClaimed()
		{
			if (this.attachments.Count == 0)
			{
				return;
			}
			this.checkmark.gameObject.SetActive(true);
		}

		private void DisableButton()
		{
			base.gameObject.GetComponent<Collider>().enabled = false;
		}

		[SerializeField]
		protected UISprite checkmark;

		[SerializeField]
		protected UISprite gift;

		private List<Attachment> attachments;

		private UIButton button;
	}
}
