using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExistingDBScript : MonoBehaviour
{
	private void Start()
	{
		DataService dataService = new DataService("existing.db");
		IEnumerable<Person> people = dataService.GetPersons();
		this.ToConsole(people);
		people = dataService.GetPersonsNamedRoberto();
		this.ToConsole("Searching for Roberto ...");
		this.ToConsole(people);
		dataService.CreatePerson();
		this.ToConsole("New person has been created");
		Person johnny = dataService.GetJohnny();
		this.ToConsole(johnny.ToString());
	}

	private void ToConsole(IEnumerable<Person> people)
	{
		foreach (Person person in people)
		{
			this.ToConsole(person.ToString());
		}
	}

	private void ToConsole(string msg)
	{
		Text debugText = this.DebugText;
		debugText.text = debugText.text + Environment.NewLine + msg;
	}

	public Text DebugText;
}
