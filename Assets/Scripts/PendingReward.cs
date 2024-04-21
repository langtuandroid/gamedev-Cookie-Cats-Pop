using System;

public class PendingReward
{
	public PendingReward()
	{
	}

	public PendingReward(int questIndex, bool skipped = false)
	{
		this.QuestIndex = questIndex;
		this.Skipped = skipped;
	}

	[JsonSerializable("qi", null)]
	public int QuestIndex { get; set; }

	[JsonSerializable("s", null)]
	public bool Skipped { get; set; }

	public static bool operator !=(PendingReward emp1, PendingReward emp2)
	{
		return (!object.ReferenceEquals(emp1, null) || !object.ReferenceEquals(emp2, null)) && (object.ReferenceEquals(emp1, null) || object.ReferenceEquals(emp2, null) || emp1.QuestIndex != emp2.QuestIndex);
	}

	public static bool operator ==(PendingReward emp1, PendingReward emp2)
	{
		return (object.ReferenceEquals(emp1, null) && object.ReferenceEquals(emp2, null)) || (!object.ReferenceEquals(emp1, null) && !object.ReferenceEquals(emp2, null) && emp1.QuestIndex == emp2.QuestIndex);
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		PendingReward pendingReward = obj as PendingReward;
		return pendingReward != null && this.QuestIndex == pendingReward.QuestIndex;
	}

	public override int GetHashCode()
	{
		return this.QuestIndex;
	}

	public override string ToString()
	{
		return string.Concat(new object[]
		{
			"QuestIndex: ",
			this.QuestIndex,
			" Skipped: ",
			this.Skipped
		});
	}
}
