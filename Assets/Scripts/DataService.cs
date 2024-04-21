using System;
using System.Collections.Generic;
using System.IO;
using SQLite4Unity3d;
using UnityEngine;

public class DataService
{
	public DataService(string DatabaseName)
	{
		string text = string.Format("{0}/{1}", Application.persistentDataPath, DatabaseName);
		if (!File.Exists(text))
		{
			WWW www = new WWW("jar:file://" + Application.dataPath + "!/assets/" + DatabaseName);
			while (!www.isDone)
			{
			}
			File.WriteAllBytes(text, www.bytes);
		}
		string databasePath = text;
		this._connection = new SQLiteConnection(databasePath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create, false);
	}

	public void CreateDB()
	{
		this._connection.DropTable<Person>();
		this._connection.CreateTable<Person>(CreateFlags.None);
		this._connection.InsertAll(new Person[]
		{
			new Person
			{
				Id = 1,
				Name = "Tom",
				Surname = "Perez",
				Age = 56
			},
			new Person
			{
				Id = 2,
				Name = "Fred",
				Surname = "Arthurson",
				Age = 16
			},
			new Person
			{
				Id = 3,
				Name = "John",
				Surname = "Doe",
				Age = 25
			},
			new Person
			{
				Id = 4,
				Name = "Roberto",
				Surname = "Huertas",
				Age = 37
			}
		});
	}

	public IEnumerable<Person> GetPersons()
	{
		return this._connection.Table<Person>();
	}

	public IEnumerable<Person> GetPersonsNamedRoberto()
	{
		return from x in this._connection.Table<Person>()
		where x.Name == "Roberto"
		select x;
	}

	public Person GetJohnny()
	{
		return (from x in this._connection.Table<Person>()
		where x.Name == "Johnny"
		select x).FirstOrDefault();
	}

	public Person CreatePerson()
	{
		Person person = new Person
		{
			Name = "Johnny",
			Surname = "Mnemonic",
			Age = 21
		};
		this._connection.Insert(person);
		return person;
	}

	private SQLiteConnection _connection;
}
