using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetterList<T> : IEnumerable<T>, IEnumerable
{
	public IEnumerator<T> GetEnumerator()
	{
		if (this.buffer != null)
		{
			for (int i = 0; i < this.size; i++)
			{
				yield return this.buffer[i];
			}
		}
		yield break;
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		if (this.buffer != null)
		{
			for (int i = 0; i < this.size; i++)
			{
				yield return this.buffer[i];
			}
		}
		yield break;
	}

	public T this[int i]
	{
		get
		{
			return this.buffer[i];
		}
		set
		{
			this.buffer[i] = value;
		}
	}

	public void Push(T val)
	{
		this.Add(val);
	}

	public T Pop()
	{
		T last = this.Last;
		this.RemoveAt(this.size - 1);
		return last;
	}

	public T Peek()
	{
		return this.Last;
	}

	public T Last
	{
		get
		{
			return this.buffer[this.size - 1];
		}
		set
		{
			this.buffer[this.size - 1] = value;
		}
	}

	private void AllocateMore()
	{
		T[] array = (this.buffer == null) ? new T[32] : new T[Mathf.Max(this.buffer.Length << 1, 32)];
		if (this.buffer != null && this.size > 0)
		{
			this.buffer.CopyTo(array, 0);
		}
		this.buffer = array;
	}

	private void Trim()
	{
		if (this.size > 0)
		{
			if (this.size < this.buffer.Length)
			{
				T[] array = new T[this.size];
				for (int i = 0; i < this.size; i++)
				{
					array[i] = this.buffer[i];
				}
				this.buffer = array;
			}
		}
		else
		{
			this.buffer = null;
		}
	}

	public void Clear()
	{
		this.size = 0;
	}

	public void Release()
	{
		this.size = 0;
		this.buffer = null;
	}

	public void Add(T item)
	{
		if (this.buffer == null || this.size == this.buffer.Length)
		{
			this.AllocateMore();
		}
		this.buffer[this.size++] = item;
	}

	public void AddRange(IEnumerable<T> list)
	{
		foreach (T t in list)
		{
			if (this.buffer == null || this.size == this.buffer.Length)
			{
				this.AllocateMore();
			}
			this.buffer[this.size++] = t;
		}
	}

	public void InsertRange(IEnumerable<T> list, int index)
	{
		List<T> list2 = new List<T>(this);
		list2.InsertRange(0, list);
		this.Clear();
		this.AddRange(list2);
	}

	public void Remove(T item)
	{
		if (this.buffer != null)
		{
			EqualityComparer<T> @default = EqualityComparer<T>.Default;
			for (int i = 0; i < this.size; i++)
			{
				if (@default.Equals(this.buffer[i], item))
				{
					this.size--;
					this.buffer[i] = default(T);
					for (int j = i; j < this.size; j++)
					{
						this.buffer[j] = this.buffer[j + 1];
					}
					return;
				}
			}
		}
	}

	public void RemoveAt(int index)
	{
		if (this.buffer != null && index < this.size)
		{
			this.size--;
			this.buffer[index] = default(T);
			for (int i = index; i < this.size; i++)
			{
				this.buffer[i] = this.buffer[i + 1];
			}
		}
	}

	public bool Contains(T item)
	{
		return this.Contains(item, EqualityComparer<T>.Default);
	}

	public bool Contains(T item, IEqualityComparer<T> comp)
	{
		if (this.buffer != null)
		{
			for (int i = 0; i < this.size; i++)
			{
				if (comp.Equals(this.buffer[i], item))
				{
					return true;
				}
			}
		}
		return false;
	}

	public T[] ToArray()
	{
		this.Trim();
		return this.buffer;
	}

	public T[] buffer;

	public int size;
}
