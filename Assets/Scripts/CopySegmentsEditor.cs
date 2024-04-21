using System;
using UnityEngine;

public class CopySegmentsEditor : ScriptableObject
{
	private const string mapSegmentName = "worldmap";

	public string sourcePath;

	public string destPath;

	public string smallImageSourcePath;

	public string smallImageDestPath;

	public CopySegmentsEditor.Theme[] themes;

	private string[] endings = new string[]
	{
		"L0",
		"L1",
		"R0",
		"R1"
	};

	[Serializable]
	public class Theme
	{
		public int startIndex;

		public int endIndex;
	}
}
