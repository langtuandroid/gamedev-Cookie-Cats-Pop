using System;
using System.Collections;
using UnityEngine;

public class PlatformAlertViewUnityGUI : MonoBehaviour
{
	public int ClosingResult { get; private set; }

	public IEnumerator Show(string title, string message, string button0Text)
	{
		this.title = title;
		this.message = message;
		this.button0Text = button0Text;
		this.ClosingResult = -1;
		while (this.ClosingResult < 0)
		{
			yield return null;
		}
		yield break;
	}

	public IEnumerator Show(string title, string message, string button0Text, string button1Text)
	{
		this.title = title;
		this.message = message;
		this.button0Text = button0Text;
		this.button1Text = button1Text;
		this.ClosingResult = -1;
		while (this.ClosingResult < 0)
		{
			yield return null;
		}
		yield break;
	}

	public void OnGUI()
	{
		float y = (float)(Screen.height / 2) - this.windowHeight / 2f;
		float x = (float)(Screen.width / 2) - 200f;
		GUI.ModalWindow(this.GetHashCode(), new Rect(x, y, 400f, this.windowHeight), new GUI.WindowFunction(this.OnGUIDialog), this.title);
	}

	private void OnGUIDialog(int windowId)
	{
		GUILayout.Space(40f);
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.FlexibleSpace();
		GUILayout.Label(this.message, new GUILayoutOption[0]);
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.Space(40f);
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.FlexibleSpace();
		GUIContent content = new GUIContent(this.button0Text);
		Rect rect = GUILayoutUtility.GetRect(content, GUI.skin.button);
		if (GUI.Button(rect, content))
		{
			this.ClosingResult = 0;
		}
		if (!string.IsNullOrEmpty(this.button1Text))
		{
			GUIContent content2 = new GUIContent(this.button1Text);
			Rect rect2 = GUILayoutUtility.GetRect(content2, GUI.skin.button);
			if (GUI.Button(rect2, content2))
			{
				this.ClosingResult = 1;
			}
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		if (Event.current.type == EventType.Repaint)
		{
			this.windowHeight = rect.y + rect.height + (float)GUI.skin.window.padding.bottom;
		}
	}

	private const float windowWidth = 400f;

	private float windowHeight = 200f;

	private string title;

	private string message;

	private string button0Text;

	private string button1Text;
}
