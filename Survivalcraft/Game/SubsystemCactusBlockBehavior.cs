using System;
using System.Collections.Generic;
using Engine;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000168 RID: 360
	public class SubsystemCactusBlockBehavior : SubsystemPollableBlockBehavior, IUpdateable
	{
		// Token: 0x1700003B RID: 59
		// (get) Token: 0x0600071B RID: 1819 RVA: 0x0002D641 File Offset: 0x0002B841
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					127
				};
			}
		}

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x0600071C RID: 1820 RVA: 0x0002D64E File Offset: 0x0002B84E
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x0600071D RID: 1821 RVA: 0x0002D654 File Offset: 0x0002B854
		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			int cellContents = base.SubsystemTerrain.Terrain.GetCellContents(x, y - 1, z);
			if (cellContents != 7 && cellContents != 127)
			{
				base.SubsystemTerrain.DestroyCell(0, x, y, z, 0, false, false);
			}
		}

		// Token: 0x0600071E RID: 1822 RVA: 0x0002D694 File Offset: 0x0002B894
		public override void OnPoll(int value, int x, int y, int z, int pollPass)
		{
			if (this.m_subsystemGameInfo.WorldSettings.EnvironmentBehaviorMode != EnvironmentBehaviorMode.Living)
			{
				return;
			}
			int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(x, y + 1, z);
			if (Terrain.ExtractContents(cellValue) == 0 && Terrain.ExtractLight(cellValue) >= 12)
			{
				int cellContents = base.SubsystemTerrain.Terrain.GetCellContents(x, y - 1, z);
				int cellContents2 = base.SubsystemTerrain.Terrain.GetCellContents(x, y - 2, z);
				if ((cellContents != 127 || cellContents2 != 127) && this.m_random.Float(0f, 1f) < 0.25f)
				{
					this.m_toUpdate[new Point3(x, y + 1, z)] = Terrain.MakeBlockValue(127, 0, 0);
				}
			}
		}

		// Token: 0x0600071F RID: 1823 RVA: 0x0002D74E File Offset: 0x0002B94E
		public override void OnCollide(CellFace cellFace, float velocity, ComponentBody componentBody)
		{
			ComponentCreature componentCreature = componentBody.Entity.FindComponent<ComponentCreature>();
			if (componentCreature == null)
			{
				return;
			}
			componentCreature.ComponentHealth.Injure(0.01f * MathUtils.Abs(velocity), null, false, "Spiked by cactus");
		}

        // Token: 0x06000720 RID: 1824 RVA: 0x0002D77D File Offset: 0x0002B97D
        public override void Load(ValuesDictionary valuesDictionary)
		{
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			base.Load(valuesDictionary);
		}

		// Token: 0x06000721 RID: 1825 RVA: 0x0002D7AC File Offset: 0x0002B9AC
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

		// Token: 0x040003F4 RID: 1012
		public SubsystemTime m_subsystemTime;

		// Token: 0x040003F5 RID: 1013
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x040003F6 RID: 1014
		public Dictionary<Point3, int> m_toUpdate = new Dictionary<Point3, int>();

		// Token: 0x040003F7 RID: 1015
		public Game.Random m_random = new Game.Random();
	}
}
