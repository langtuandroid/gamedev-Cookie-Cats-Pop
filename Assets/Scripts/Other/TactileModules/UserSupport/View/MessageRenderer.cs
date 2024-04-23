using System;
using System.Diagnostics;
using TactileModules.UserSupport.Model;
using TactileModules.UserSupport.View.Utils;
using TactileModules.Validation;
using UnityEngine;

namespace TactileModules.UserSupport.View
{
	public class MessageRenderer : MonoBehaviour
	{
		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<Message> Selected;



		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<Toggle> Toggled;



		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<Message> AttachmentClicked;



		public bool UseAutoTranslatedField { get; set; }

		public virtual void Render(Message message, float labelWidth)
		{
			this.message = message;
			this.labelWidth = labelWidth;
			this.SetTranslateToggleClicked();
			this.SetProperties();
		}

		protected virtual void SetProperties()
		{
			this.attachmentsRendererComp = this.GetAttachmentRenderer();
			this.Layout();
			this.RenderAttachments();
			this.DisplayDate();
		}

		protected void Layout()
		{
			this.LayoutLabel();
			this.SetUIElementSize();
		}

		protected void LayoutLabel()
		{
			float x = this.labelWidth;
			this.messageLabel.Size = new Vector2(x, this.messageLabel.Size.y);
			this.SetLabelText();
			this.DisplayTranslatedIndicator();
			this.messageLabel.UpdateGeometry();
		}

		protected void SetLabelText()
		{
			if (this.UseAutoTranslatedField && this.message.Sender.Equals("support"))
			{
				this.messageLabel.text = this.message.GetTranslatedMessageBodyOrDefault();
			}
			else
			{
				this.messageLabel.text = this.message.Body;
			}
		}

		protected void SetUIElementSize()
		{
			Vector2 size = this.dateLabel.Size;
			Vector2 size2 = this.messageLabel.Size;
			Vector2 sizeAndDoLayout = default(Vector2);
			sizeAndDoLayout.y = size.y + size2.y + this.margins.y + this.GetTranslateButtonHeight();
			sizeAndDoLayout.x = size2.x + this.margins.x;
			UIElement component = base.GetComponent<UIElement>();
			component.SetSizeAndDoLayout(sizeAndDoLayout);
		}

		protected float GetTranslateButtonHeight()
		{
			if (this.translateButton == null)
			{
				return 0f;
			}
			if (!this.message.HasTranslatedMessageBody())
			{
				return 0f;
			}
			return this.translateButton.GetComponent<UIElement>().Size.y;
		}

		public MessageAttachmentRenderer GetAttachmentRenderer()
		{
			if (this.attachmentsRenderer == null)
			{
				return null;
			}
			return this.attachmentsRenderer.GetInstance<MessageAttachmentRenderer>();
		}

		protected virtual void RenderAttachments()
		{
			if (this.attachmentsRenderer == null)
			{
				return;
			}
			this.attachmentsRendererComp.AttachmentClicked += this.AttachmentsRendererCompOnClicked;
			this.attachmentsRendererComp.Render(this.message.Attachments);
		}

		private void AttachmentsRendererCompOnClicked()
		{
			this.AttachmentClicked(this.message);
		}

		protected virtual void DisplayDate()
		{
			DateTime d = DateTime.Parse(this.message.CreatedAt);
			this.dateLabel.text = PrettyDate.GetPrettyDate(d);
		}

		protected virtual void DisplayTranslatedIndicator()
		{
			if (this.translateButton == null)
			{
				return;
			}
			if (!this.message.HasTranslatedMessageBody())
			{
				this.translateButton.gameObject.SetActive(false);
				return;
			}
			bool isOn = this.UseAutoTranslatedField && this.message.HasTranslatedMessageBody();
			this.translateButton.GetInstance<Toggle>().IsOn = isOn;
		}

		protected void SetTranslateToggleClicked()
		{
			if (this.translateButton == null)
			{
				return;
			}
			this.translateButton.GetInstance<Toggle>().Toggled -= this.OnTranslateToggled;
			this.translateButton.GetInstance<Toggle>().Toggled += this.OnTranslateToggled;
		}

		private void OnTranslateToggled(Toggle toggle)
		{
			this.Toggled(toggle);
		}

		[SerializeField]
		protected UILabel messageLabel;

		[SerializeField]
		protected UILabel dateLabel;

		[Header("Only applies to messages to the user")]
		[OptionalSerializedField]
		[SerializeField]
		protected UIInstantiator translateButton;

		[Header("Only applies to messages to the user")]
		[OptionalSerializedField]
		[SerializeField]
		private UIInstantiator attachmentsRenderer;

		protected Message message;

		protected MessageAttachmentRenderer attachmentsRendererComp;

		protected float labelWidth;

		protected Vector2 margins = new Vector2(60f, 70f);
	}
}
