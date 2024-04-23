using System;
using System.Collections;
using System.Collections.Generic;
using TactileModules.PuzzleGame.PlayablePostcard.Model;
using UnityEngine;

namespace TactileModules.PuzzleGame.PlayablePostcard.UI
{
	public class PostcardItemContainer : MonoBehaviour
	{
		public Dictionary<string, UITextureQuad> GetItems(PostcardItemType itemType, List<PostcardItemTypeAndId> postcardData)
		{
			GameObject container = this.GetContainer(itemType);
			return this.GetItems(container, postcardData);
		}

		public GameObject GetContainer(PostcardItemType itemType)
		{
			switch (itemType)
			{
			case PostcardItemType.Background:
				return this.backgroundsContainer;
			case PostcardItemType.Character:
				return this.charactersContainer;
			case PostcardItemType.Costume:
				return this.costumesContainer;
			case PostcardItemType.Prop:
				return this.propsContainer;
			case PostcardItemType.Text:
				return this.textsContainer;
			case PostcardItemType.Frame:
				return this.framesContainer;
			default:
				return null;
			}
		}

		private Dictionary<string, UITextureQuad> GetItems(GameObject container, List<PostcardItemTypeAndId> postcardData)
		{
			ConditionalPostcardItems[] childrenItems = this.GetChildrenItems<ConditionalPostcardItems>(container.transform);
			Dictionary<string, UITextureQuad> dictionary = new Dictionary<string, UITextureQuad>();
			foreach (ConditionalPostcardItems conditionalPostcardItems in childrenItems)
			{
				if (conditionalPostcardItems.Match(postcardData))
				{
					UITextureQuad[] childrenItems2 = this.GetChildrenItems<UITextureQuad>(conditionalPostcardItems.transform);
					if (childrenItems2.Length > 0 && conditionalPostcardItems.ExcludeOthers)
					{
						Dictionary<string, UITextureQuad> dictionary2 = new Dictionary<string, UITextureQuad>();
						this.AddArrayToDictionary(dictionary2, childrenItems2);
						return dictionary2;
					}
					this.AddArrayToDictionary(dictionary, childrenItems2);
				}
			}
			this.AddArrayToDictionary(dictionary, this.GetChildrenItems<UITextureQuad>(container.transform));
			return dictionary;
		}

		private void AddArrayToDictionary(Dictionary<string, UITextureQuad> dictionary, UITextureQuad[] elements)
		{
			foreach (UITextureQuad uitextureQuad in elements)
			{
				dictionary.Add(uitextureQuad.name, uitextureQuad);
			}
		}

		public T[] GetChildrenItems<T>(Transform root)
		{
			List<T> list = new List<T>();
			IEnumerator enumerator = root.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					Transform transform = (Transform)obj;
					T component = transform.GetComponent<T>();
					if (component != null)
					{
						list.Add(component);
					}
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			return list.ToArray();
		}

		[SerializeField]
		private GameObject backgroundsContainer;

		[SerializeField]
		private GameObject charactersContainer;

		[SerializeField]
		private GameObject costumesContainer;

		[SerializeField]
		private GameObject propsContainer;

		[SerializeField]
		private GameObject textsContainer;

		[SerializeField]
		private GameObject framesContainer;
	}
}
