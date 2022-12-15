using System;
using System.Collections.Generic;
using Engine;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001A9 RID: 425
	public class SubsystemSoilBlockBehavior : SubsystemPollableBlockBehavior, IUpdateable
	{
		// Token: 0x170000A8 RID: 168
		// (get) Token: 0x06000A56 RID: 2646 RVA: 0x0004CA43 File Offset: 0x0004AC43
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					168
				};
			}
		}

		// Token: 0x170000A9 RID: 169
		// (get) Token: 0x06000A57 RID: 2647 RVA: 0x0004CA53 File Offset: 0x0004AC53
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x06000A58 RID: 2648 RVA: 0x0004CA58 File Offset: 0x0004AC58
		public override void OnCollide(CellFace cellFace, float velocity, ComponentBody componentBody)
		{
			if (componentBody.Mass > 20f && !componentBody.IsSneaking)
			{
				Vector3 velocity2 = componentBody.Velocity;
				if (velocity2.Y < -3f || (velocity2.Y < 0f && this.m_random.Float(0f, 1f) < 1.5f * this.m_subsystemTime.GameTimeDelta && velocity2.LengthSquared() > 1f))
				{
					this.m_toDegrade[cellFace.Point] = true;
				}
			}
		}

		// Token: 0x06000A59 RID: 2649 RVA: 0x0004CAE4 File Offset: 0x0004ACE4
		public override void OnPoll(int value, int x, int y, int z, int pollPass)
		{
			bool hydration = SoilBlock.GetHydration(Terrain.ExtractData(value));
			if (this.DetermineHydration(x, y, z, 3))
			{
				if (!hydration)
				{
					this.m_toHydrate[new Point3(x, y, z)] = true;
					return;
				}
			}
			else if (hydration)
			{
				this.m_toHydrate[new Point3(x, y, z)] = false;
			}
		}

		// Token: 0x06000A5A RID: 2650 RVA: 0x0004CB3C File Offset: 0x0004AD3C
		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(x, y + 1, z);
			if (this.DegradesSoilIfOnTopOfIt(cellValue))
			{
				int cellValue2 = base.SubsystemTerrain.Terrain.GetCellValue(x, y, z);
				base.SubsystemTerrain.ChangeCell(x, y, z, Terrain.ReplaceContents(cellValue2, 2), true);
			}
		}

        // Token: 0x06000A5B RID: 2651 RVA: 0x0004CB92 File Offset: 0x0004AD92
        public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
		}

		// Token: 0x06000A5C RID: 2652 RVA: 0x0004CBB0 File Offset: 0x0004ADB0
		public void Update(float dt)
		{
			if (this.m_subsystemTime.PeriodicGameTimeEvent(2.5, 0.0))
			{
				foreach (Point3 point in this.m_toDegrade.Keys)
				{
					if (base.SubsystemTerrain.Terrain.GetCellContents(point.X, point.Y, point.Z) == 168)
					{
						int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(point.X, point.Y, point.Z);
						base.SubsystemTerrain.ChangeCell(point.X, point.Y, point.Z, Terrain.ReplaceContents(cellValue, 2), true);
					}
				}
				this.m_toDegrade.Clear();
			}
			if (this.m_subsystemTime.PeriodicGameTimeEvent(10.0, 0.0))
			{
				foreach (KeyValuePair<Point3, bool> keyValuePair in this.m_toHydrate)
				{
					Point3 key = keyValuePair.Key;
					bool value = keyValuePair.Value;
					int cellValue2 = base.SubsystemTerrain.Terrain.GetCellValue(key.X, key.Y, key.Z);
					if (Terrain.ExtractContents(cellValue2) == 168)
					{
						int data = SoilBlock.SetHydration(Terrain.ExtractData(cellValue2), value);
						int value2 = Terrain.ReplaceData(cellValue2, data);
						base.SubsystemTerrain.ChangeCell(key.X, key.Y, key.Z, value2, true);
					}
				}
				this.m_toHydrate.Clear();
			}
		}

		// Token: 0x06000A5D RID: 2653 RVA: 0x0004CD98 File Offset: 0x0004AF98
		public bool DegradesSoilIfOnTopOfIt(int value)
		{
			int num = Terrain.ExtractContents(value);
			Block block = BlocksManager.Blocks[num];
			return !block.IsFaceTransparent(base.SubsystemTerrain, 5, value) && block.IsCollidable;
		}

		// Token: 0x06000A5E RID: 2654 RVA: 0x0004CDCC File Offset: 0x0004AFCC
		public bool DetermineHydration(int x, int y, int z, int steps)
		{
			if (steps > 0 && y > 0 && y < 254)
			{
				if (this.DetermineHydrationHelper(x - 1, y, z, steps - 1))
				{
					return true;
				}
				if (this.DetermineHydrationHelper(x + 1, y, z, steps - 1))
				{
					return true;
				}
				if (this.DetermineHydrationHelper(x, y, z - 1, steps - 1))
				{
					return true;
				}
				if (this.DetermineHydrationHelper(x, y, z + 1, steps - 1))
				{
					return true;
				}
				if (steps >= 2)
				{
					if (this.DetermineHydrationHelper(x, y - 1, z, steps - 2))
					{
						return true;
					}
					if (this.DetermineHydrationHelper(x, y + 1, z, steps - 2))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06000A5F RID: 2655 RVA: 0x0004CE68 File Offset: 0x0004B068
		public bool DetermineHydrationHelper(int x, int y, int z, int steps)
		{
			int cellValueFast = base.SubsystemTerrain.Terrain.GetCellValueFast(x, y, z);
			int num = Terrain.ExtractContents(cellValueFast);
			int data = Terrain.ExtractData(cellValueFast);
			if (num != 18)
			{
				if (num == 168)
				{
					if (SoilBlock.GetHydration(data))
					{
						return this.DetermineHydration(x, y, z, steps);
					}
				}
				return num == 2 && this.DetermineHydration(x, y, z, steps);
			}
			return true;
		}

		// Token: 0x040005A5 RID: 1445
		public SubsystemTime m_subsystemTime;

		// Token: 0x040005A6 RID: 1446
		public Game.Random m_random = new Game.Random();

		// Token: 0x040005A7 RID: 1447
		public Dictionary<Point3, bool> m_toDegrade = new Dictionary<Point3, bool>();

		// Token: 0x040005A8 RID: 1448
		public Dictionary<Point3, bool> m_toHydrate = new Dictionary<Point3, bool>();
	}
}
