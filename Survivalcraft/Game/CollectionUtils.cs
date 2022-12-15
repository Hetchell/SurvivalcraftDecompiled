using System;
using System.Collections.Generic;

namespace Game
{
	// Token: 0x0200023F RID: 575
	public static class CollectionUtils
	{
		// Token: 0x060011A4 RID: 4516 RVA: 0x00088400 File Offset: 0x00086600
		public static T ElementAt<T, E>(E enumerator, int index) where E : IEnumerator<T>
		{
			int num = 0;
			while (enumerator.MoveNext())
			{
				num++;
				if (num > index)
				{
					return enumerator.Current;
				}
			}
			throw new IndexOutOfRangeException("ElementAt() index out of range.");
		}

		// Token: 0x060011A5 RID: 4517 RVA: 0x00088440 File Offset: 0x00086640
		public static void RandomShuffle<T>(this IList<T> list, Func<int, int> random)
		{
			for (int i = list.Count - 1; i > 0; i--)
			{
				int index = random(i + 1);
				T value = list[index];
				list[index] = list[i];
				list[i] = value;
			}
		}

		// Token: 0x060011A6 RID: 4518 RVA: 0x00088488 File Offset: 0x00086688
		public static int FirstIndex<T>(this IEnumerable<T> collection, T value)
		{
			int num = 0;
			using (IEnumerator<T> enumerator = collection.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (object.Equals(enumerator.Current, value))
					{
						return num;
					}
					num++;
				}
			}
			return -1;
		}

		// Token: 0x060011A7 RID: 4519 RVA: 0x000884E8 File Offset: 0x000866E8
		public static int FirstIndex<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
		{
			int num = 0;
			foreach (T arg in collection)
			{
				if (predicate(arg))
				{
					return num;
				}
				num++;
			}
			return -1;
		}

		// Token: 0x060011A8 RID: 4520 RVA: 0x00088540 File Offset: 0x00086740
		public static T SelectNth<T>(this IList<T> list, int n, IComparer<T> comparer)
		{
			if (list == null || list.Count <= n)
			{
				throw new ArgumentException();
			}
			int i = 0;
			int num = list.Count - 1;
			while (i < num)
			{
				int j = i;
				int num2 = num;
				T y = list[(j + num2) / 2];
				while (j < num2)
				{
					if (comparer.Compare(list[j], y) >= 0)
					{
						T value = list[num2];
						list[num2] = list[j];
						list[j] = value;
						num2--;
					}
					else
					{
						j++;
					}
				}
				if (comparer.Compare(list[j], y) > 0)
				{
					j--;
				}
				if (n <= j)
				{
					num = j;
				}
				else
				{
					i = j + 1;
				}
			}
			return list[n];
		}
	}
}
