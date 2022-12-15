using System;
using System.Collections.Generic;
using Engine;
using Engine.Serialization;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000173 RID: 371
	public abstract class SubsystemEditableItemBehavior<T> : SubsystemBlockBehavior where T : IEditableItemData, new()
	{
		// Token: 0x060007FA RID: 2042 RVA: 0x000342EA File Offset: 0x000324EA
		public SubsystemEditableItemBehavior(int contents)
		{
			this.m_contents = contents;
		}

		// Token: 0x060007FB RID: 2043 RVA: 0x00034310 File Offset: 0x00032510
		public T GetBlockData(Point3 point)
		{
			T result;
			this.m_blocksData.TryGetValue(point, out result);
			return result;
		}

		// Token: 0x060007FC RID: 2044 RVA: 0x0003432D File Offset: 0x0003252D
		public void SetBlockData(Point3 point, T t)
		{
			if (t != null)
			{
				this.m_blocksData[point] = t;
				return;
			}
			this.m_blocksData.Remove(point);
		}

		// Token: 0x060007FD RID: 2045 RVA: 0x00034354 File Offset: 0x00032554
		public T GetItemData(int id)
		{
			T result;
			this.m_itemsData.TryGetValue(id, out result);
			return result;
		}

		// Token: 0x060007FE RID: 2046 RVA: 0x00034374 File Offset: 0x00032574
		public int StoreItemDataAtUniqueId(T t)
		{
			int num = this.FindFreeItemId();
			this.m_itemsData[num] = t;
			return num;
		}

		// Token: 0x060007FF RID: 2047 RVA: 0x00034398 File Offset: 0x00032598
		public override void OnItemPlaced(int x, int y, int z, ref BlockPlacementData placementData, int itemValue)
		{
			int id = Terrain.ExtractData(itemValue);
			T itemData = this.GetItemData(id);
			if (itemData != null)
			{
				this.m_blocksData[new Point3(x, y, z)] = (T)((object)itemData.Copy());
			}
		}

		// Token: 0x06000800 RID: 2048 RVA: 0x000343E4 File Offset: 0x000325E4
		public override void OnItemHarvested(int x, int y, int z, int blockValue, ref BlockDropValue dropValue, ref int newBlockValue)
		{
			T blockData = this.GetBlockData(new Point3(x, y, z));
			if (blockData != null)
			{
				int num = this.FindFreeItemId();
				this.m_itemsData.Add(num, (T)((object)blockData.Copy()));
				dropValue.Value = Terrain.ReplaceData(dropValue.Value, num);
			}
		}

		// Token: 0x06000801 RID: 2049 RVA: 0x00034441 File Offset: 0x00032641
		public override void OnBlockRemoved(int value, int newValue, int x, int y, int z)
		{
			this.m_blocksData.Remove(new Point3(x, y, z));
		}

        // Token: 0x06000802 RID: 2050 RVA: 0x0003445C File Offset: 0x0003265C
        public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemItemsScanner = base.Project.FindSubsystem<SubsystemItemsScanner>(true);
			foreach (KeyValuePair<string, object> keyValuePair in valuesDictionary.GetValue<ValuesDictionary>("Blocks"))
			{
				Point3 key = HumanReadableConverter.ConvertFromString<Point3>(keyValuePair.Key);
				T value = Activator.CreateInstance<T>();
				value.LoadString((string)keyValuePair.Value);
				this.m_blocksData[key] = value;
			}
			foreach (KeyValuePair<string, object> keyValuePair2 in valuesDictionary.GetValue<ValuesDictionary>("Items"))
			{
				int key2 = HumanReadableConverter.ConvertFromString<int>(keyValuePair2.Key);
				T value2 = Activator.CreateInstance<T>();
				value2.LoadString((string)keyValuePair2.Value);
				this.m_itemsData[key2] = value2;
			}
			this.m_subsystemItemsScanner.ItemsScanned += this.GarbageCollectItems;
		}

        // Token: 0x06000803 RID: 2051 RVA: 0x00034588 File Offset: 0x00032788
        public override void Save(ValuesDictionary valuesDictionary)
		{
			base.Save(valuesDictionary);
			ValuesDictionary valuesDictionary2 = new ValuesDictionary();
			valuesDictionary.SetValue<ValuesDictionary>("Blocks", valuesDictionary2);
			foreach (KeyValuePair<Point3, T> keyValuePair in this.m_blocksData)
			{
				ValuesDictionary valuesDictionary3 = valuesDictionary2;
				string key = HumanReadableConverter.ConvertToString(keyValuePair.Key);
				T value = keyValuePair.Value;
				valuesDictionary3.SetValue<string>(key, value.SaveString());
			}
			ValuesDictionary valuesDictionary4 = new ValuesDictionary();
			valuesDictionary.SetValue<ValuesDictionary>("Items", valuesDictionary4);
			foreach (KeyValuePair<int, T> keyValuePair2 in this.m_itemsData)
			{
				ValuesDictionary valuesDictionary5 = valuesDictionary4;
				string key2 = HumanReadableConverter.ConvertToString(keyValuePair2.Key);
				T value = keyValuePair2.Value;
				valuesDictionary5.SetValue<string>(key2, value.SaveString());
			}
		}

		// Token: 0x06000804 RID: 2052 RVA: 0x00034698 File Offset: 0x00032898
		public int FindFreeItemId()
		{
			for (int i = 1; i < 1000; i++)
			{
				if (!this.m_itemsData.ContainsKey(i))
				{
					return i;
				}
			}
			return 0;
		}

		// Token: 0x06000805 RID: 2053 RVA: 0x000346C8 File Offset: 0x000328C8
		public void GarbageCollectItems(ReadOnlyList<ScannedItemData> allExistingItems)
		{
			HashSet<int> hashSet = new HashSet<int>();
			foreach (ScannedItemData scannedItemData in allExistingItems)
			{
				if (Terrain.ExtractContents(scannedItemData.Value) == this.m_contents)
				{
					hashSet.Add(Terrain.ExtractData(scannedItemData.Value));
				}
			}
			List<int> list = new List<int>();
			foreach (KeyValuePair<int, T> keyValuePair in this.m_itemsData)
			{
				if (!hashSet.Contains(keyValuePair.Key))
				{
					list.Add(keyValuePair.Key);
				}
			}
			foreach (int key in list)
			{
				this.m_itemsData.Remove(key);
			}
		}

		// Token: 0x0400042D RID: 1069
		public SubsystemItemsScanner m_subsystemItemsScanner;

		// Token: 0x0400042E RID: 1070
		public int m_contents;

		// Token: 0x0400042F RID: 1071
		public Dictionary<int, T> m_itemsData = new Dictionary<int, T>();

		// Token: 0x04000430 RID: 1072
		public Dictionary<Point3, T> m_blocksData = new Dictionary<Point3, T>();
	}
}
