using System;

public class FacebookUser
{
	[JsonSerializable("name", null)]
	public string Name { get; set; }

	[JsonSerializable("first_name", null)]
	public string FirstName { get; set; }

	[JsonSerializable("last_name", null)]
	public string LastName { get; set; }

	[JsonSerializable("username", null)]
	public string Username { get; set; }

	[JsonSerializable("gender", null)]
	public string Gender { get; set; }

	[JsonSerializable("id", null)]
	public string Id { get; set; }

	[JsonSerializable("installed", null)]
	public bool Installed { get; set; }

	[JsonSerializable("email", null)]
	public string Email { get; set; }

	public override string ToString()
	{
		return string.Format("[FacebookUser: name={0}, first_name={1}, last_name={2}, username={3}, gender={4}, id={5}, installed={6}]", new object[]
		{
			this.Name,
			this.FirstName,
			this.LastName,
			this.Username,
			this.Gender,
			this.Id,
			this.Installed
		});
	}
}
