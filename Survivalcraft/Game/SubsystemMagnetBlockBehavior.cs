using System;
using System.Linq;
using Engine;
using Engine.Serialization;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x0200018D RID: 397
	public class SubsystemMagnetBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x17000077 RID: 119
		// (get) Token: 0x06000915 RID: 2325 RVA: 0x0003E66C File Offset: 0x0003C86C
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					167
				};
			}
		}

		// Token: 0x17000078 RID: 120
		// (get) Token: 0x06000916 RID: 2326 RVA: 0x0003E67C File Offset: 0x0003C87C
		public int MagnetsCount
		{
			get
			{
				return this.m_magnets.Count;
			}
		}

		// Token: 0x06000917 RID: 2327 RVA: 0x0003E68C File Offset: 0x0003C88C
		public Vector3 FindNearestCompassTarget(Vector3 compassPosition)
		{
			if (this.m_magnets.Count > 0)
			{
				float num = float.MaxValue;
				Vector3 v = Vector3.Zero;
				int num2 = 0;
				while (num2 < this.m_magnets.Count && num2 < 8)
				{
					Vector3 vector = this.m_magnets.Array[num2];
					float num3 = Vector3.DistanceSquared(compassPosition, vector);
					if (num3 < num)
					{
						num = num3;
						v = vector;
					}
					num2++;
				}
				return v + new Vector3(0.5f);
			}
			float num4 = float.MaxValue;
			Vector3 v2 = Vector3.Zero;
			foreach (PlayerData playerData in this.m_subsystemPlayers.PlayersData)
			{
				Vector3 spawnPosition = playerData.SpawnPosition;
				float num5 = Vector3.DistanceSquared(compassPosition, spawnPosition);
				if (num5 < num4)
				{
					num4 = num5;
					v2 = spawnPosition;
				}
			}
			return v2 + new Vector3(0.5f);
		}

        // Token: 0x06000918 RID: 2328 RVA: 0x0003E78C File Offset: 0x0003C98C
        public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemPlayers = base.Project.FindSubsystem<SubsystemPlayers>(true);
			string value = valuesDictionary.GetValue<string>("Magnets");
			this.m_magnets = new DynamicArray<Vector3>(HumanReadableConverter.ValuesListFromString<Vector3>(';', value));
		}

        // Token: 0x06000919 RID: 2329 RVA: 0x0003E7D4 File Offset: 0x0003C9D4
        public override void Save(ValuesDictionary valuesDictionary)
		{
			base.Save(valuesDictionary);
			string value = HumanReadableConverter.ValuesListToString<Vector3>(';', this.m_magnets.ToArray<Vector3>());
			valuesDictionary.SetValue<string>("Magnets", value);
		}

		// Token: 0x0600091A RID: 2330 RVA: 0x0003E807 File Offset: 0x0003CA07
		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
			this.m_magnets.Add(new Vector3((float)x, (float)y, (float)z));
		}

		// Token: 0x0600091B RID: 2331 RVA: 0x0003E821 File Offset: 0x0003CA21
		public override void OnBlockRemoved(int value, int newValue, int x, int y, int z)
		{
			this.m_magnets.Remove(new Vector3((float)x, (float)y, (float)z));
		}

		// Token: 0x0600091C RID: 2332 RVA: 0x0003E83C File Offset: 0x0003CA3C
		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			int cellContents = base.SubsystemTerrain.Terrain.GetCellContents(x, y - 1, z);
			if (BlocksManager.Blocks[cellContents].IsTransparent)
			{
				base.SubsystemTerrain.DestroyCell(0, x, y, z, 0, false, false);
			}
		}

		// Token: 0x040004BE RID: 1214
		public SubsystemPlayers m_subsystemPlayers;

		// Token: 0x040004BF RID: 1215
		public DynamicArray<Vector3> m_magnets = new DynamicArray<Vector3>();

		// Token: 0x040004C0 RID: 1216
		public const int MaxMagnets = 8;
	}
}
