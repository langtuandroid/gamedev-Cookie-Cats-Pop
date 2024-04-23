using System;

public class FacebookInvitableFriend
{
	[JsonSerializable("name", null)]
	public string Name { get; set; }

	[JsonSerializable("id", null)]
	public string Id { get; set; }

	[JsonSerializable("picture", null)]
	public Picture Picture { get; set; }

	public override string ToString()
	{
		return string.Format("[FacebookInvitableFriend: name={0}, id={1}]", this.Name, this.Id);
	}
}
