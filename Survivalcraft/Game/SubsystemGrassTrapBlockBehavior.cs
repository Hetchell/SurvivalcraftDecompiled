using System;
using System.Collections.Generic;
using Engine;

namespace Game
{
	// Token: 0x02000184 RID: 388
	public class SubsystemGrassTrapBlockBehavior : SubsystemBlockBehavior, IUpdateable
	{
		// Token: 0x1700006B RID: 107
		// (get) Token: 0x060008E6 RID: 2278 RVA: 0x0003D758 File Offset: 0x0003B958
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					87
				};
			}
		}

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x060008E7 RID: 2279 RVA: 0x0003D765 File Offset: 0x0003B965
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x060008E8 RID: 2280 RVA: 0x0003D768 File Offset: 0x0003B968
		public override void OnCollide(CellFace cellFace, float velocity, ComponentBody componentBody)
		{
			if (cellFace.Face == 4 && componentBody.Mass > 20f)
			{
				Point3 key = new Point3(cellFace.X, cellFace.Y, cellFace.Z);
				SubsystemGrassTrapBlockBehavior.TrapValue trapValue;
				if (!this.m_trapValues.TryGetValue(key, out trapValue))
				{
					trapValue = new SubsystemGrassTrapBlockBehavior.TrapValue();
					this.m_trapValues.Add(key, trapValue);
				}
				trapValue.Damage += 0f - velocity;
			}
		}

		// Token: 0x060008E9 RID: 2281 RVA: 0x0003D7DC File Offset: 0x0003B9DC
		public void Update(float dt)
		{
			foreach (KeyValuePair<Point3, SubsystemGrassTrapBlockBehavior.TrapValue> keyValuePair in this.m_trapValues)
			{
				if (keyValuePair.Value.Damage > 1f)
				{
					for (int i = -1; i <= 1; i++)
					{
						for (int j = -1; j <= 1; j++)
						{
							if (MathUtils.Abs(i) + MathUtils.Abs(j) <= 1 && base.SubsystemTerrain.Terrain.GetCellContents(keyValuePair.Key.X + i, keyValuePair.Key.Y, keyValuePair.Key.Z + j) == 87)
							{
								base.SubsystemTerrain.DestroyCell(0, keyValuePair.Key.X + i, keyValuePair.Key.Y, keyValuePair.Key.Z + j, 0, false, false);
							}
						}
					}
					keyValuePair.Value.Damage = 0f;
				}
				else
				{
					keyValuePair.Value.Damage -= 0.5f * dt;
				}
				if (keyValuePair.Value.Damage <= 0f)
				{
					this.m_toRemove.Add(keyValuePair.Key);
				}
			}
			foreach (Point3 key in this.m_toRemove)
			{
				this.m_trapValues.Remove(key);
			}
			this.m_toRemove.Clear();
		}

		// Token: 0x040004AF RID: 1199
		public Dictionary<Point3, SubsystemGrassTrapBlockBehavior.TrapValue> m_trapValues = new Dictionary<Point3, SubsystemGrassTrapBlockBehavior.TrapValue>();

		// Token: 0x040004B0 RID: 1200
		public List<Point3> m_toRemove = new List<Point3>();

		// Token: 0x0200042B RID: 1067
		public class TrapValue
		{
			// Token: 0x0400159F RID: 5535
			public float Damage;
		}
	}
}
