using System;
using System.Collections.Generic;
using TactileModules.Foundation;
using UnityEngine;

namespace TactileModules.UserSupport.View
{
	public class MessagesBadgeBehaviour : MonoBehaviour
	{
		private void Start()
		{
			this.CollectDependencies();
			this.InitializeInternal();
		}

		public void Initialize(IConversations conversations)
		{
			this.conversations = conversations;
			this.InitializeInternal();
		}

		protected void InitializeInternal()
		{
			this.Hide();
			this.SetEventHandlers();
			this.DisplayBadge(this.conversations.GetNumberOfUnreadMessagesAvailableOnServer());
		}

		protected void Hide()
		{
			this.badge.gameObject.SetActive(false);
		}

		protected void CollectDependencies()
		{
			try
			{
				UserSupportSystem userSupportSystem = ManagerRepository.Get<UserSupportSystem>();
				this.conversations = userSupportSystem.Conversations;
			}
			catch (KeyNotFoundException ex)
			{
			}
		}

		protected void SetEventHandlers()
		{
			this.conversations.NewMessagesAvailable += this.ConversationsNewMessagesAvailable;
			this.conversations.Loaded += this.ConversationsOnLoaded;
		}

		private void ConversationsOnLoaded()
		{
			this.DisplayBadge(this.conversations.GetUnreadMessages().Count);
		}

		private void ConversationsNewMessagesAvailable(int numMessages)
		{
			this.DisplayBadge(numMessages);
		}

		private void DisplayBadge(int numMessages)
		{
			if (numMessages > 0)
			{
				this.badge.gameObject.SetActive(true);
				this.label.text = numMessages + string.Empty;
			}
			else
			{
				this.badge.gameObject.SetActive(false);
			}
		}

		private void OnDestroy()
		{
			if (this.conversations != null)
			{
				this.conversations.Loaded -= this.ConversationsOnLoaded;
				this.conversations.NewMessagesAvailable -= this.ConversationsNewMessagesAvailable;
			}
		}

		[SerializeField]
		protected UISprite badge;

		[SerializeField]
		protected UILabel label;

		protected IConversations conversations = new NullConversations();
	}
}
