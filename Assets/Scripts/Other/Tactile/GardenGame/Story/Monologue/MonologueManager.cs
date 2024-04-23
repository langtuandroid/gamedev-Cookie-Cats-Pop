using System;
using System.Collections.Generic;

namespace Tactile.GardenGame.Story.Monologue
{
	public class MonologueManager
	{
		public MonologueManager(MonologueDatabase database)
		{
			this.SetDatabase(database);
		}

		public Monologue GetTopMonologue()
		{
			this.EnsureMonologues();
			return (this.monologueQueue.Count <= 0) ? null : this.monologueQueue.Last<Monologue>();
		}

		public void Pop()
		{
			if (this.monologueQueue.Count == 0)
			{
				return;
			}
			this.monologueQueue.RemoveAt(this.monologueQueue.Count - 1);
		}

		public void Push(Monologue monologue)
		{
			this.monologueQueue.Add(monologue);
		}

		private void EnsureMonologues()
		{
			if (this.monologueQueue.Count > 0)
			{
				return;
			}
			this.FillQueueFromDatabase();
			this.ShuffleQueue();
		}

		private void FillQueueFromDatabase()
		{
			this.monologueQueue.AddRange(this.database.Monologues);
		}

		private void ShuffleQueue()
		{
			this.monologueQueue.Shuffle<Monologue>();
		}

		public void SetDatabase(MonologueDatabase database)
		{
			if (database == null)
			{
				return;
			}
			this.database = database;
			this.monologueQueue = new List<Monologue>();
		}

		private MonologueDatabase database;

		private List<Monologue> monologueQueue;
	}
}
