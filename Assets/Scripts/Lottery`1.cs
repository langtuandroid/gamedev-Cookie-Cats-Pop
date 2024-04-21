using System;
using System.Collections.Generic;
using UnityEngine;

public class Lottery<T>
{
	public Lottery()
	{
	}

	public Lottery(Lottery<T> source)
	{
		foreach (Lottery<T>.Ticket<T> ticket in source.tickets)
		{
			this.tickets.Add(new Lottery<T>.Ticket<T>(ticket.obj, ticket.weightedChance));
		}
	}

	public Lottery(ICollection<T> source)
	{
		foreach (T obj in source)
		{
			this.tickets.Add(new Lottery<T>.Ticket<T>(obj, 1f));
		}
	}

	public void Add(float weightedChance, T item)
	{
		this.tickets.Add(new Lottery<T>.Ticket<T>(item, weightedChance));
	}

	public void Remove(T item)
	{
		foreach (Lottery<T>.Ticket<T> ticket in this.tickets)
		{
			if (ticket.obj.Equals(item))
			{
				this.tickets.Remove(ticket);
				break;
			}
		}
	}

	public T PickRandomItem(bool remove = false)
	{
		float num = this.GetNextRandomFloat() * this.GetFullWeight();
		float num2 = 0f;
		for (int i = 0; i < this.tickets.Count; i++)
		{
			num2 += this.tickets[i].GetModifiedWeightedChance(this.ChanceModifier);
			if (num <= num2)
			{
				if (remove)
				{
					this.tickets.RemoveAt(i);
				}
				return this.tickets[i].obj;
			}
		}
		return default(T);
	}

	protected virtual float GetNextRandomFloat()
	{
		return UnityEngine.Random.value;
	}

	public void Clear()
	{
		this.tickets.Clear();
	}

	public int Count
	{
		get
		{
			return this.tickets.Count;
		}
	}

	public bool Contains(T item)
	{
		for (int i = 0; i < this.tickets.Count; i++)
		{
			if (this.tickets[i].obj.Equals(item))
			{
				return true;
			}
		}
		return false;
	}

	public void CopyFromOther(Lottery<T> other)
	{
		this.tickets.Clear();
		for (int i = 0; i < other.tickets.Count; i++)
		{
			this.tickets.Add(new Lottery<T>.Ticket<T>(other.tickets[i].obj, other.tickets[i].weightedChance));
		}
	}

	private float GetFullWeight()
	{
		float num = 0f;
		for (int i = 0; i < this.tickets.Count; i++)
		{
			num += this.tickets[i].GetModifiedWeightedChance(this.ChanceModifier);
		}
		return num;
	}

	private List<Lottery<T>.Ticket<T>> tickets = new List<Lottery<T>.Ticket<T>>();

	public Lottery<T>.ChanceModifierAction<T> ChanceModifier;

	private class Ticket<Q>
	{
		public Ticket(Q obj, float weightedChance)
		{
			this.obj = obj;
			this.weightedChance = weightedChance;
		}

		public float GetModifiedWeightedChance(Lottery<T>.ChanceModifierAction<Q> chanceModifier)
		{
			return (chanceModifier != null) ? chanceModifier(this.obj, this.weightedChance) : this.weightedChance;
		}

		internal Q obj;

		internal float weightedChance;
	}

	public delegate float ChanceModifierAction<Q>(Q obj, float weightedChance);
}
