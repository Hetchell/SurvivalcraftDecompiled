using System;
using System.Collections.Generic;
using Engine;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000183 RID: 387
	public class SubsystemGrassBlockBehavior : SubsystemPollableBlockBehavior, IUpdateable
	{
		// Token: 0x17000069 RID: 105
		// (get) Token: 0x060008DD RID: 2269 RVA: 0x0003D2B7 File Offset: 0x0003B4B7
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					8
				};
			}
		}

		// Token: 0x1700006A RID: 106
		// (get) Token: 0x060008DE RID: 2270 RVA: 0x0003D2C3 File Offset: 0x0003B4C3
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x060008DF RID: 2271 RVA: 0x0003D2C8 File Offset: 0x0003B4C8
		public override void OnPoll(int value, int x, int y, int z, int pollPass)
		{
			if (Terrain.ExtractData(value) != 0 || this.m_subsystemGameInfo.WorldSettings.EnvironmentBehaviorMode != EnvironmentBehaviorMode.Living)
			{
				return;
			}
			int num = Terrain.ExtractLight(base.SubsystemTerrain.Terrain.GetCellValue(x, y + 1, z));
			if (num == 0)
			{
				this.m_toUpdate[new Point3(x, y, z)] = Terrain.ReplaceContents(value, 8);
			}
			if (num < 13)
			{
				return;
			}
			for (int i = x - 1; i <= x + 1; i++)
			{
				for (int j = z - 1; j <= z + 1; j++)
				{
					for (int k = y - 2; k <= y + 1; k++)
					{
						int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(i, k, j);
						if (Terrain.ExtractContents(cellValue) == 2)
						{
							int cellValue2 = base.SubsystemTerrain.Terrain.GetCellValue(i, k + 1, j);
							if (!this.KillsGrassIfOnTopOfIt(cellValue2) && Terrain.ExtractLight(cellValue2) >= 13 && this.m_random.Float(0f, 1f) < 0.1f)
							{
								int num2 = Terrain.ReplaceContents(cellValue, 8);
								this.m_toUpdate[new Point3(i, k, j)] = num2;
								if (Terrain.ExtractContents(cellValue2) == 0)
								{
									int temperature = base.SubsystemTerrain.Terrain.GetTemperature(i, j);
									int humidity = base.SubsystemTerrain.Terrain.GetHumidity(i, j);
									int num3 = PlantsManager.GenerateRandomPlantValue(this.m_random, num2, temperature, humidity, k + 1);
									if (num3 != 0)
									{
										this.m_toUpdate[new Point3(i, k + 1, j)] = num3;
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x060008E0 RID: 2272 RVA: 0x0003D464 File Offset: 0x0003B664
		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(x, y + 1, z);
			if (Terrain.ExtractContents(cellValue) == 61)
			{
				int value = base.SubsystemTerrain.Terrain.GetCellValueFast(x, y, z);
				value = Terrain.ReplaceData(value, 1);
				base.SubsystemTerrain.ChangeCell(x, y, z, value, true);
			}
			else
			{
				int value2 = base.SubsystemTerrain.Terrain.GetCellValueFast(x, y, z);
				value2 = Terrain.ReplaceData(value2, 0);
				base.SubsystemTerrain.ChangeCell(x, y, z, value2, true);
			}
			if (this.KillsGrassIfOnTopOfIt(cellValue))
			{
				base.SubsystemTerrain.ChangeCell(x, y, z, Terrain.MakeBlockValue(2, 0, 0), true);
			}
		}

		// Token: 0x060008E1 RID: 2273 RVA: 0x0003D50B File Offset: 0x0003B70B
		public override void OnExplosion(int value, int x, int y, int z, float damage)
		{
			if (damage > BlocksManager.Blocks[8].ExplosionResilience * this.m_random.Float(0f, 1f))
			{
				base.SubsystemTerrain.ChangeCell(x, y, z, Terrain.MakeBlockValue(2, 0, 0), true);
			}
		}

        // Token: 0x060008E2 RID: 2274 RVA: 0x0003D54B File Offset: 0x0003B74B
        public override void Load(ValuesDictionary valuesDictionary)
		{
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			base.Load(valuesDictionary);
		}

		// Token: 0x060008E3 RID: 2275 RVA: 0x0003D578 File Offset: 0x0003B778
		public void Update(float dt)
		{
			if (this.m_subsystemTime.PeriodicGameTimeEvent(60.0, 0.0))
			{
				foreach (KeyValuePair<Point3, int> keyValuePair in this.m_toUpdate)
				{
					if (Terrain.ExtractContents(keyValuePair.Value) == 8)
					{
						if (base.SubsystemTerrain.Terrain.GetCellContents(keyValuePair.Key.X, keyValuePair.Key.Y, keyValuePair.Key.Z) != 2)
						{
							continue;
						}
					}
					else
					{
						int cellContents = base.SubsystemTerrain.Terrain.GetCellContents(keyValuePair.Key.X, keyValuePair.Key.Y - 1, keyValuePair.Key.Z);
						if ((cellContents != 8 && cellContents != 2) || base.SubsystemTerrain.Terrain.GetCellContents(keyValuePair.Key.X, keyValuePair.Key.Y, keyValuePair.Key.Z) != 0)
						{
							continue;
						}
					}
					base.SubsystemTerrain.ChangeCell(keyValuePair.Key.X, keyValuePair.Key.Y, keyValuePair.Key.Z, keyValuePair.Value, true);
				}
				this.m_toUpdate.Clear();
			}
		}

		// Token: 0x060008E4 RID: 2276 RVA: 0x0003D6FC File Offset: 0x0003B8FC
		public bool KillsGrassIfOnTopOfIt(int value)
		{
			int num = Terrain.ExtractContents(value);
			Block block = BlocksManager.Blocks[num];
			return block is FluidBlock || (!block.IsFaceTransparent(base.SubsystemTerrain, 5, value) && block.IsCollidable);
		}

		// Token: 0x040004AB RID: 1195
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x040004AC RID: 1196
		public SubsystemTime m_subsystemTime;

		// Token: 0x040004AD RID: 1197
		public Dictionary<Point3, int> m_toUpdate = new Dictionary<Point3, int>();

		// Token: 0x040004AE RID: 1198
		public Game.Random m_random = new Game.Random();
	}
}
