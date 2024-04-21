using System;

public class LevelEditorEventManager
{
	public static void Emit(LevelEditorEvent e)
	{
		if (LevelEditorEventManager.OnLevelEditorEvent != null)
		{
			LevelEditorEventManager.OnLevelEditorEvent(e);
		}
	}

	public static void Emit(LevelEditorEventType eventType)
	{
		LevelEditorEventManager.Emit(eventType, null, string.Empty);
	}

	public static void Emit(LevelEditorEventType eventType, object context, string value = "")
	{
		LevelEditorEventManager.reusableEvent.type = eventType;
		LevelEditorEventManager.reusableEvent.variableName = context.ToString();
		LevelEditorEventManager.reusableEvent.value = value;
		LevelEditorEventManager.Emit(LevelEditorEventManager.reusableEvent);
	}

	public static LevelEditorEventManager.LevelEditorEventHandler OnLevelEditorEvent;

	private static LevelEditorEvent reusableEvent = new LevelEditorEvent();

	public delegate void LevelEditorEventHandler(LevelEditorEvent e);
}
