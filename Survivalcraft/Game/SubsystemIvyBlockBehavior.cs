using System;
using System.Collections.Generic;
using Engine;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x0200018A RID: 394
	public class SubsystemIvyBlockBehavior : SubsystemPollableBlockBehavior, IUpdateable
	{
		// Token: 0x17000072 RID: 114
		// (get) Token: 0x06000902 RID: 2306 RVA: 0x0003E057 File Offset: 0x0003C257
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x17000073 RID: 115
		// (get) Token: 0x06000903 RID: 2307 RVA: 0x0003E05A File Offset: 0x0003C25A
		public override int[] HandledBlocks
		{
			get
			{
				return new int[0];
			}
		}

		// Token: 0x06000904 RID: 2308 RVA: 0x0003E064 File Offset: 0x0003C264
		public void Update(float dt)
		{
			if (this.m_subsystemTime.PeriodicGameTimeEvent(60.0, 0.0))
			{
				foreach (KeyValuePair<Point3, int> keyValuePair in this.m_toUpdate)
				{
					if (base.SubsystemTerrain.Terrain.GetCellContents(keyValuePair.Key.X, keyValuePair.Key.Y, keyValuePair.Key.Z) == 0)
					{
						base.SubsystemTerrain.ChangeCell(keyValuePair.Key.X, keyValuePair.Key.Y, keyValuePair.Key.Z, keyValuePair.Value, true);
					}
				}
				this.m_toUpdate.Clear();
			}
		}

		// Token: 0x06000905 RID: 2309 RVA: 0x0003E14C File Offset: 0x0003C34C
		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			int face = IvyBlock.GetFace(Terrain.ExtractData(base.SubsystemTerrain.Terrain.GetCellValue(x, y, z)));
			bool flag = false;
			int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(x, y + 1, z);
			if (Terrain.ExtractContents(cellValue) == 197 && IvyBlock.GetFace(Terrain.ExtractData(cellValue)) == face)
			{
				flag = true;
			}
			if (!flag)
			{
				Point3 point = CellFace.FaceToPoint3(face);
				int cellValue2 = base.SubsystemTerrain.Terrain.GetCellValue(x + point.X, y + point.Y, z + point.Z);
				if (!BlocksManager.Blocks[Terrain.ExtractContents(cellValue2)].IsCollidable)
				{
					base.SubsystemTerrain.DestroyCell(0, x, y, z, 0, true, false);
				}
			}
		}

		// Token: 0x06000906 RID: 2310 RVA: 0x0003E208 File Offset: 0x0003C408
		public override void OnPoll(int value, int x, int y, int z, int pollPass)
		{
			if (this.m_random.Float(0f, 1f) < 0.5f && !IvyBlock.IsGrowthStopCell(x, y, z) && Terrain.ExtractContents(base.SubsystemTerrain.Terrain.GetCellValue(x, y - 1, z)) == 0)
			{
				this.m_toUpdate[new Point3(x, y - 1, z)] = value;
			}
		}

        // Token: 0x06000907 RID: 2311 RVA: 0x0003E270 File Offset: 0x0003C470
        public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
		}

		// Token: 0x040004B7 RID: 1207
		public SubsystemTime m_subsystemTime;

		// Token: 0x040004B8 RID: 1208
		public Game.Random m_random = new Game.Random();

		// Token: 0x040004B9 RID: 1209
		public Dictionary<Point3, int> m_toUpdate = new Dictionary<Point3, int>();
	}
}
