using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using TactileModules.UserSupport.Model;
using TactileModules.UserSupport.ViewComponents;
using UnityEngine;

namespace TactileModules.UserSupport.View
{
	public class ConversationView : UIView, IConversationView
	{
		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action Closed;



		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<string> SubmitMessage;



		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<Message> AttachmentClicked;



		public void Init()
		{
			this.HideLoadingIndicator();
			this.SetupKeyboard();
		}

		public void SetUser(IUser user)
		{
			this.user = user;
		}

		public void RenderMessages(List<Message> messages)
		{
			this.messages = messages;
			this.messageRenderers = new List<MessageRenderer>();
			this.itemList.BeginAdding();
			this.AddConversationMessages();
			this.itemList.EndAdding();
			this.SetListScrollPosition();
		}

		protected void SetListScrollPosition()
		{
			if (this.scrollPosition == Vector2.zero)
			{
				this.itemList.SetScrollToItem(this.messageRenderers.Count);
			}
			else
			{
				this.itemList.SetScroll(this.scrollPosition);
			}
		}

		protected override void ScreenSizeChanged()
		{
			this.RenderMessages(this.messages);
		}

		private void SetupKeyboard()
		{
			this.keyboardHandler = base.gameObject.GetComponent<KeyboardHandler>();
			this.keyboardHandler.Done += this.KeyboardHandlerOnDone;
		}

		private void KeyboardHandlerOnDone(string message)
		{
			this.SubmitMessage(message);
		}

		public void Close(int i)
		{
			base.Close(i);
		}

		private void AddConversationMessages()
		{
			this.AddInitialUserSupportMessage();
			foreach (Message message in this.messages)
			{
				this.AddMessageToList(message);
			}
			if (this.IsLastMessageFromUser())
			{
				this.AddSentResponseSupportMessage();
			}
		}

		protected virtual void AddInitialUserSupportMessage()
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.messageRendererInitial);
			this.itemList.AddToContent(gameObject.GetComponent<UIElement>());
			GreetingMessageRenderer component = gameObject.GetComponent<GreetingMessageRenderer>();
			component.GreetUser(this.user.Name);
		}

		protected virtual void AddMessageToList(Message message)
		{
			GameObject gameObject = this.CreateItemRenderer(message);
			MessageRenderer component = gameObject.GetComponent<MessageRenderer>();
			component.UseAutoTranslatedField = this.user.PrefersTranslation;
			component.Toggled += this.ItemRendererOnToggled;
			component.Render(message, this.itemList.Size.x - this.margin);
			component.AttachmentClicked += this.ItemRendererOnAttachmentClicked;
			component.Selected += this.ItemRendererOnSelected;
			this.messageRenderers.Add(component);
			this.itemList.AddToContent(gameObject.GetComponent<UIElement>(), false);
		}

		private void ItemRendererOnAttachmentClicked(Message obj)
		{
			this.AttachmentClicked(obj);
		}

		private void ItemRendererOnToggled(Toggle obj)
		{
			this.scrollPosition = this.itemList.actualScrollOffset;
			this.user.PrefersTranslation = obj.IsOn;
			this.RenderMessages(this.messages);
		}

		private bool IsLastMessageFromUser()
		{
			return this.messages.Count > 0 && this.messages.Last<Message>().Sender != "support";
		}

		protected virtual void AddSentResponseSupportMessage()
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.messageRendererSentResponse);
			this.itemList.AddToContent(gameObject.GetComponent<UIElement>());
		}

		private void ItemRendererOnSelected(Message message)
		{
			this.keyboardHandler.Show(string.Empty);
		}

		protected virtual GameObject CreateItemRenderer(Message message)
		{
			GameObject original = (!(message.Sender == "support")) ? this.messageRendererUser : this.messageRendererSupport;
			return UnityEngine.Object.Instantiate<GameObject>(original);
		}

		private List<Attachment> GetUnclaimedAttachments()
		{
			List<Attachment> list = new List<Attachment>();
			foreach (Message message in this.messages)
			{
				list.AddRange(message.GetUnclaimedAttachments());
			}
			return list;
		}

		[UsedImplicitly]
		private void DismissClicked(UIEvent e)
		{
			this.Closed();
		}

		[UsedImplicitly]
		protected virtual void OnReply(UIEvent e)
		{
			this.keyboardHandler.Show(string.Empty);
		}

		public void ShowLoadingIndicator()
		{
			this.loadingIndicator.gameObject.SetActive(true);
		}

		public void HideLoadingIndicator()
		{
			if (this.loadingIndicator == null)
			{
				return;
			}
			this.loadingIndicator.gameObject.SetActive(false);
		}

		public void HideKeyboard()
		{
			this.keyboardHandler.Hide();
		}

		[SerializeField]
		protected UIVerticalListVariableHeightItems itemList;

		[SerializeField]
		protected GameObject messageRendererUser;

		[SerializeField]
		protected GameObject messageRendererSupport;

		[SerializeField]
		protected GameObject messageRendererInitial;

		[SerializeField]
		protected GameObject messageRendererSentResponse;

		[SerializeField]
		protected UIInstantiator loadingIndicator;

		protected KeyboardHandler keyboardHandler;

		protected float margin = 40f;

		protected List<Message> messages;

		protected List<MessageRenderer> messageRenderers = new List<MessageRenderer>();

		protected IUser user;

		private Vector2 scrollPosition = Vector2.zero;
	}
}
