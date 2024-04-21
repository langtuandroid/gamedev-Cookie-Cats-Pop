using System;

public class QuestStateInfo
{
	public QuestStateInfo()
	{
	}

	public QuestStateInfo(string dateString, bool skipped = false, bool isCompleted = true)
	{
		this.DateString = dateString;
		this.Skipped = skipped;
		this.IsCompleted = isCompleted;
	}

	[JsonSerializable("cd", null)]
	public string DateString { get; set; }

	[JsonSerializable("s", null)]
	public bool Skipped { get; set; }

	[JsonSerializable("ic", null)]
	public bool IsCompleted { get; set; }

	public DateTime Date
	{
		get
		{
			return DateHelper.GetDateTimeFromString(this.DateString, "MM/dd/yyyy HH:mm");
		}
		set
		{
			this.DateString = value.ToShortDateString() + " " + value.ToShortTimeString();
		}
	}
}
