using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace SimpleJson
{
	// Token: 0x02000009 RID: 9
	[GeneratedCode("simple-json", "1.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class JsonObject : IDictionary<string, object>, ICollection<KeyValuePair<string, object>>, IEnumerable<KeyValuePair<string, object>>, IEnumerable
	{
		// Token: 0x17000002 RID: 2
		public object this[int index]
		{
			get
			{
				return JsonObject.GetAtIndex(this._members, index);
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000028 RID: 40 RVA: 0x00003767 File Offset: 0x00001967
		public ICollection<string> Keys
		{
			get
			{
				return this._members.Keys;
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000029 RID: 41 RVA: 0x00003774 File Offset: 0x00001974
		public ICollection<object> Values
		{
			get
			{
				return this._members.Values;
			}
		}

		// Token: 0x17000005 RID: 5
		public object this[string key]
		{
			get
			{
				return this._members[key];
			}
			set
			{
				this._members[key] = value;
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600002C RID: 44 RVA: 0x0000379E File Offset: 0x0000199E
		public int Count
		{
			get
			{
				return this._members.Count;
			}
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x0600002D RID: 45 RVA: 0x000037AB File Offset: 0x000019AB
		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		// Token: 0x0600002E RID: 46 RVA: 0x000037AE File Offset: 0x000019AE
		public JsonObject()
		{
			this._members = new Dictionary<string, object>();
		}

		// Token: 0x0600002F RID: 47 RVA: 0x000037C1 File Offset: 0x000019C1
		public JsonObject(IEqualityComparer<string> comparer)
		{
			this._members = new Dictionary<string, object>(comparer);
		}

		// Token: 0x06000030 RID: 48 RVA: 0x000037D8 File Offset: 0x000019D8
		internal static object GetAtIndex(IDictionary<string, object> obj, int index)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			if (index >= obj.Count)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			int num = 0;
			foreach (KeyValuePair<string, object> keyValuePair in obj)
			{
				if (num++ == index)
				{
					return keyValuePair.Value;
				}
			}
			return null;
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00003854 File Offset: 0x00001A54
		public void Add(string key, object value)
		{
			this._members.Add(key, value);
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00003863 File Offset: 0x00001A63
		public bool ContainsKey(string key)
		{
			return this._members.ContainsKey(key);
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00003871 File Offset: 0x00001A71
		public bool Remove(string key)
		{
			return this._members.Remove(key);
		}

		// Token: 0x06000034 RID: 52 RVA: 0x0000387F File Offset: 0x00001A7F
		public bool TryGetValue(string key, out object value)
		{
			return this._members.TryGetValue(key, out value);
		}

		// Token: 0x06000035 RID: 53 RVA: 0x0000388E File Offset: 0x00001A8E
		public void Add(KeyValuePair<string, object> item)
		{
			this._members.Add(item.Key, item.Value);
		}

		// Token: 0x06000036 RID: 54 RVA: 0x000038A9 File Offset: 0x00001AA9
		public void Clear()
		{
			this._members.Clear();
		}

		// Token: 0x06000037 RID: 55 RVA: 0x000038B6 File Offset: 0x00001AB6
		public bool Contains(KeyValuePair<string, object> item)
		{
			return this._members.ContainsKey(item.Key) && this._members[item.Key] == item.Value;
		}

		// Token: 0x06000038 RID: 56 RVA: 0x000038EC File Offset: 0x00001AEC
		public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			int num = this.Count;
			foreach (KeyValuePair<string, object> keyValuePair in this)
			{
				array[arrayIndex++] = keyValuePair;
				if (--num <= 0)
				{
					break;
				}
			}
		}

		// Token: 0x06000039 RID: 57 RVA: 0x0000395C File Offset: 0x00001B5C
		public bool Remove(KeyValuePair<string, object> item)
		{
			return this._members.Remove(item.Key);
		}

		// Token: 0x0600003A RID: 58 RVA: 0x00003970 File Offset: 0x00001B70
		public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
		{
			return this._members.GetEnumerator();
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00003982 File Offset: 0x00001B82
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this._members.GetEnumerator();
		}

		// Token: 0x0600003C RID: 60 RVA: 0x00003994 File Offset: 0x00001B94
		public override string ToString()
		{
			return SimpleJson.SerializeObject(this);
		}

		// Token: 0x04000032 RID: 50
		public readonly Dictionary<string, object> _members;
	}
}
