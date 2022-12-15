using System;
using System.Collections;
using System.Collections.Generic;

namespace Game
{
	// Token: 0x020002F7 RID: 759
	public class SortedMultiCollection<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
	{
		// Token: 0x1700035D RID: 861
		// (get) Token: 0x06001588 RID: 5512 RVA: 0x000A472C File Offset: 0x000A292C
		public int Count
		{
			get
			{
				return this.m_count;
			}
		}

		// Token: 0x1700035E RID: 862
		// (get) Token: 0x06001589 RID: 5513 RVA: 0x000A4734 File Offset: 0x000A2934
		// (set) Token: 0x0600158A RID: 5514 RVA: 0x000A4740 File Offset: 0x000A2940
		public int Capacity
		{
			get
			{
				return this.m_array.Length;
			}
			set
			{
				value = Math.Max(Math.Max(4, this.m_count), value);
				if (value != this.m_array.Length)
				{
					KeyValuePair<TKey, TValue>[] array = new KeyValuePair<TKey, TValue>[value];
					Array.Copy(this.m_array, array, this.m_count);
					this.m_array = array;
				}
			}
		}

		// Token: 0x1700035F RID: 863
		public KeyValuePair<TKey, TValue> this[int i]
		{
			get
			{
				if (i < this.m_count)
				{
					return this.m_array[i];
				}
				throw new ArgumentOutOfRangeException();
			}
		}

		// Token: 0x0600158C RID: 5516 RVA: 0x000A47A9 File Offset: 0x000A29A9
		public SortedMultiCollection()
		{
			this.m_array = new KeyValuePair<TKey, TValue>[4];
			this.m_comparer = Comparer<TKey>.Default;
		}

		// Token: 0x0600158D RID: 5517 RVA: 0x000A47C8 File Offset: 0x000A29C8
		public SortedMultiCollection(IComparer<TKey> comparer)
		{
			this.m_array = new KeyValuePair<TKey, TValue>[4];
			this.m_comparer = comparer;
		}

		// Token: 0x0600158E RID: 5518 RVA: 0x000A47E3 File Offset: 0x000A29E3
		public SortedMultiCollection(int capacity) : this(capacity, null)
		{
			capacity = Math.Max(capacity, 4);
			this.m_array = new KeyValuePair<TKey, TValue>[capacity];
			this.m_comparer = Comparer<TKey>.Default;
		}

		// Token: 0x0600158F RID: 5519 RVA: 0x000A480D File Offset: 0x000A2A0D
		public SortedMultiCollection(int capacity, IComparer<TKey> comparer)
		{
			capacity = Math.Max(capacity, 4);
			this.m_array = new KeyValuePair<TKey, TValue>[capacity];
			this.m_comparer = comparer;
		}

		// Token: 0x06001590 RID: 5520 RVA: 0x000A4834 File Offset: 0x000A2A34
		public void Add(TKey key, TValue value)
		{
			int num = this.Find(key);
			if (num < 0)
			{
				num = ~num;
			}
			this.EnsureCapacity(this.m_count + 1);
			Array.Copy(this.m_array, num, this.m_array, num + 1, this.m_count - num);
			this.m_array[num] = new KeyValuePair<TKey, TValue>(key, value);
			this.m_count++;
			this.m_version++;
		}

		// Token: 0x06001591 RID: 5521 RVA: 0x000A48AC File Offset: 0x000A2AAC
		public void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> items)
		{
			foreach (KeyValuePair<TKey, TValue> keyValuePair in items)
			{
				this.Add(keyValuePair.Key, keyValuePair.Value);
			}
		}

		// Token: 0x06001592 RID: 5522 RVA: 0x000A4904 File Offset: 0x000A2B04
		public bool Remove(TKey key)
		{
			int num = this.Find(key);
			if (num >= 0)
			{
				Array.Copy(this.m_array, num + 1, this.m_array, num, this.m_count - num - 1);
				this.m_array[this.m_count - 1] = default(KeyValuePair<TKey, TValue>);
				this.m_count--;
				this.m_version++;
				return true;
			}
			return false;
		}

		// Token: 0x06001593 RID: 5523 RVA: 0x000A4974 File Offset: 0x000A2B74
		public void Clear()
		{
			for (int i = 0; i < this.m_count; i++)
			{
				this.m_array[i] = default(KeyValuePair<TKey, TValue>);
			}
			this.m_count = 0;
			this.m_version++;
		}

		// Token: 0x06001594 RID: 5524 RVA: 0x000A49BC File Offset: 0x000A2BBC
		public bool TryGetValue(TKey key, out TValue value)
		{
			int num = this.Find(key);
			if (num >= 0)
			{
				value = this.m_array[num].Value;
				return true;
			}
			value = default(TValue);
			return false;
		}

		// Token: 0x06001595 RID: 5525 RVA: 0x000A49F6 File Offset: 0x000A2BF6
		public bool ContainsKey(TKey key)
		{
			return this.Find(key) >= 0;
		}

		// Token: 0x06001596 RID: 5526 RVA: 0x000A4A05 File Offset: 0x000A2C05
		public SortedMultiCollection<TKey, TValue>.Enumerator GetEnumerator()
		{
			return new SortedMultiCollection<TKey, TValue>.Enumerator(this);
		}

		// Token: 0x06001597 RID: 5527 RVA: 0x000A4A0D File Offset: 0x000A2C0D
		IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
		{
			return new SortedMultiCollection<TKey, TValue>.Enumerator(this);
		}

		// Token: 0x06001598 RID: 5528 RVA: 0x000A4A1A File Offset: 0x000A2C1A
		IEnumerator IEnumerable.GetEnumerator()
		{
			return new SortedMultiCollection<TKey, TValue>.Enumerator(this);
		}

		// Token: 0x06001599 RID: 5529 RVA: 0x000A4A27 File Offset: 0x000A2C27
		public void EnsureCapacity(int capacity)
		{
			if (capacity > this.Capacity)
			{
				this.Capacity = Math.Max(capacity, 2 * this.Capacity);
			}
		}

		// Token: 0x0600159A RID: 5530 RVA: 0x000A4A48 File Offset: 0x000A2C48
		public int Find(TKey key)
		{
			if (this.m_count > 0)
			{
				int i = 0;
				int num = this.m_count - 1;
				while (i <= num)
				{
					int num2 = i + num >> 1;
					int num3 = this.m_comparer.Compare(this.m_array[num2].Key, key);
					if (num3 == 0)
					{
						return num2;
					}
					if (num3 < 0)
					{
						i = num2 + 1;
					}
					else
					{
						num = num2 - 1;
					}
				}
				return ~i;
			}
			return -1;
		}

		// Token: 0x04000F4E RID: 3918
		public const int MinCapacity = 4;

		// Token: 0x04000F4F RID: 3919
		public KeyValuePair<TKey, TValue>[] m_array;

		// Token: 0x04000F50 RID: 3920
		public int m_count;

		// Token: 0x04000F51 RID: 3921
		public int m_version;

		// Token: 0x04000F52 RID: 3922
		public IComparer<TKey> m_comparer;

		// Token: 0x020004E3 RID: 1251
		public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IDisposable, IEnumerator
		{
			// Token: 0x1700055E RID: 1374
			// (get) Token: 0x06002056 RID: 8278 RVA: 0x000E4108 File Offset: 0x000E2308
			public KeyValuePair<TKey, TValue> Current
			{
				get
				{
					return this.m_current;
				}
			}

			// Token: 0x1700055F RID: 1375
			// (get) Token: 0x06002057 RID: 8279 RVA: 0x000E4110 File Offset: 0x000E2310
			object IEnumerator.Current
			{
				get
				{
					return this.m_current;
				}
			}

			// Token: 0x06002058 RID: 8280 RVA: 0x000E411D File Offset: 0x000E231D
			internal Enumerator(SortedMultiCollection<TKey, TValue> collection)
			{
				this.m_collection = collection;
				this.m_current = default(KeyValuePair<TKey, TValue>);
				this.m_index = 0;
				this.m_version = collection.m_version;
			}

			// Token: 0x06002059 RID: 8281 RVA: 0x000E4145 File Offset: 0x000E2345
			public void Dispose()
			{
			}

			// Token: 0x0600205A RID: 8282 RVA: 0x000E4148 File Offset: 0x000E2348
			public bool MoveNext()
			{
				if (this.m_collection.m_version != this.m_version)
				{
					throw new InvalidOperationException("SortedMultiCollection was modified, enumeration cannot continue.");
				}
				if (this.m_index < this.m_collection.m_count)
				{
					this.m_current = this.m_collection.m_array[this.m_index];
					this.m_index++;
					return true;
				}
				this.m_current = default(KeyValuePair<TKey, TValue>);
				return false;
			}

			// Token: 0x0600205B RID: 8283 RVA: 0x000E41BF File Offset: 0x000E23BF
			public void Reset()
			{
				if (this.m_collection.m_version != this.m_version)
				{
					throw new InvalidOperationException("SortedMultiCollection was modified, enumeration cannot continue.");
				}
				this.m_index = 0;
				this.m_current = default(KeyValuePair<TKey, TValue>);
			}

			// Token: 0x040017E6 RID: 6118
			public SortedMultiCollection<TKey, TValue> m_collection;

			// Token: 0x040017E7 RID: 6119
			public KeyValuePair<TKey, TValue> m_current;

			// Token: 0x040017E8 RID: 6120
			public int m_index;

			// Token: 0x040017E9 RID: 6121
			public int m_version;
		}
	}
}
