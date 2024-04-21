using System;

public class AnswerResultData
{
	[JsonSerializable("question", null)]
	public string Question { get; set; }

	[JsonSerializable("answer", null)]
	public string Answer { get; set; }
}
