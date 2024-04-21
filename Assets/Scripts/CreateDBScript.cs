using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateDBScript : MonoBehaviour
{
	private void Start()
	{
		this.StartSync();
	}

	private void StartSync()
	{
		DataService dataService = new DataService("tempDatabase.db");
		dataService.CreateDB();
		IEnumerable<Person> people = dataService.GetPersons();
		this.ToConsole(people);
		people = dataService.GetPersonsNamedRoberto();
		this.ToConsole("Searching for Roberto ...");
		this.ToConsole(people);
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
