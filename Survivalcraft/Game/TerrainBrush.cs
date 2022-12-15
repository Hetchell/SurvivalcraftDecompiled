using System;
using System.Collections.Generic;
using Engine;

namespace Game
{
	// Token: 0x0200030B RID: 779
	public class TerrainBrush
	{
		// Token: 0x17000368 RID: 872
		// (get) Token: 0x06001618 RID: 5656 RVA: 0x000A6C50 File Offset: 0x000A4E50
		public TerrainBrush.Cell[] Cells
		{
			get
			{
				return this.m_cells;
			}
		}

		// Token: 0x06001619 RID: 5657 RVA: 0x000A6C58 File Offset: 0x000A4E58
		public static int Key(int x, int y, int z)
		{
			return y + 128 + (x + 128 << 8) + (z + 128 << 16);
		}

		// Token: 0x0600161A RID: 5658 RVA: 0x000A6C78 File Offset: 0x000A4E78
		public void Compile()
		{
			this.m_cells = new TerrainBrush.Cell[this.m_cellsDictionary.Values.Count];
			int num = 0;
			foreach (TerrainBrush.Cell cell in this.m_cellsDictionary.Values)
			{
				this.m_cells[num++] = cell;
			}
			Array.Sort<TerrainBrush.Cell>(this.m_cells);
			this.m_cellsDictionary = null;
		}

		// Token: 0x0600161B RID: 5659 RVA: 0x000A6D0C File Offset: 0x000A4F0C
		public int CountNonDiagonalNeighbors(int x, int y, int z, TerrainBrush.Counter counter)
		{
			return counter.Count(this, new Point3(x - 1, y, z)) + counter.Count(this, new Point3(x + 1, y, z)) + counter.Count(this, new Point3(x, y - 1, z)) + counter.Count(this, new Point3(x, y + 1, z)) + counter.Count(this, new Point3(x, y, z - 1)) + counter.Count(this, new Point3(x, y, z + 1));
		}

		// Token: 0x0600161C RID: 5660 RVA: 0x000A6D8C File Offset: 0x000A4F8C
		public int CountBox(int x, int y, int z, int sizeX, int sizeY, int sizeZ, TerrainBrush.Counter counter)
		{
			int num = 0;
			for (int i = x; i < x + sizeX; i++)
			{
				for (int j = y; j < y + sizeY; j++)
				{
					for (int k = z; k < z + sizeZ; k++)
					{
						num += counter.Count(this, new Point3(i, j, k));
					}
				}
			}
			return num;
		}

		// Token: 0x0600161D RID: 5661 RVA: 0x000A6DDC File Offset: 0x000A4FDC
		public void Replace(int oldValue, int newValue)
		{
			Dictionary<int, TerrainBrush.Cell> dictionary = new Dictionary<int, TerrainBrush.Cell>();
			foreach (KeyValuePair<int, TerrainBrush.Cell> keyValuePair in this.m_cellsDictionary)
			{
				TerrainBrush.Cell value = keyValuePair.Value;
				if (value.Value == oldValue)
				{
					value.Value = newValue;
				}
				dictionary[keyValuePair.Key] = value;
			}
			this.m_cellsDictionary = dictionary;
			this.m_cells = null;
		}

		// Token: 0x0600161E RID: 5662 RVA: 0x000A6E64 File Offset: 0x000A5064
		public void CalculateBounds(out Point3 min, out Point3 max)
		{
			min = Point3.Zero;
			max = Point3.Zero;
			bool flag = true;
			foreach (TerrainBrush.Cell cell in this.m_cellsDictionary.Values)
			{
				if (flag)
				{
					flag = false;
					min.X = (max.X = (int)cell.X);
					min.Y = (max.Y = (int)cell.Y);
					min.Z = (max.Z = (int)cell.Z);
				}
				else
				{
					min.X = MathUtils.Min(min.X, (int)cell.X);
					min.Y = MathUtils.Min(min.Y, (int)cell.Y);
					min.Z = MathUtils.Min(min.Z, (int)cell.Z);
					max.X = MathUtils.Max(max.X, (int)cell.X);
					max.Y = MathUtils.Max(max.Y, (int)cell.Y);
					max.Z = MathUtils.Max(max.Z, (int)cell.Z);
				}
			}
		}

		// Token: 0x0600161F RID: 5663 RVA: 0x000A6FA8 File Offset: 0x000A51A8
		public int? GetValue(Point3 p)
		{
			return this.GetValue(p.X, p.Y, p.Z);
		}

		// Token: 0x06001620 RID: 5664 RVA: 0x000A6FC4 File Offset: 0x000A51C4
		public int? GetValue(int x, int y, int z)
		{
			int key = TerrainBrush.Key(x, y, z);
			TerrainBrush.Cell cell;
			if (this.m_cellsDictionary.TryGetValue(key, out cell))
			{
				return new int?(cell.Value);
			}
			return null;
		}

		// Token: 0x06001621 RID: 5665 RVA: 0x000A7000 File Offset: 0x000A5200
		public void AddCell(int x, int y, int z, TerrainBrush.Brush brush)
		{
			int? num = brush.Paint(this, new Point3(x, y, z));
			if (num != null)
			{
				int key = TerrainBrush.Key(x, y, z);
				this.m_cellsDictionary[key] = new TerrainBrush.Cell
				{
					X = (sbyte)x,
					Y = (sbyte)y,
					Z = (sbyte)z,
					Value = num.Value
				};
				this.m_cells = null;
			}
		}

		// Token: 0x06001622 RID: 5666 RVA: 0x000A7078 File Offset: 0x000A5278
		public void AddBox(int x, int y, int z, int sizeX, int sizeY, int sizeZ, TerrainBrush.Brush brush)
		{
			for (int i = x; i < x + sizeX; i++)
			{
				for (int j = y; j < y + sizeY; j++)
				{
					for (int k = z; k < z + sizeZ; k++)
					{
						this.AddCell(i, j, k, brush);
					}
				}
			}
		}

		// Token: 0x06001623 RID: 5667 RVA: 0x000A70C0 File Offset: 0x000A52C0
		public void AddRay(int x1, int y1, int z1, int x2, int y2, int z2, int sizeX, int sizeY, int sizeZ, TerrainBrush.Brush brush)
		{
			Vector3 vector = new Vector3((float)x1, (float)y1, (float)z1) + new Vector3(0.5f);
			Vector3 vector2 = new Vector3((float)x2, (float)y2, (float)z2) + new Vector3(0.5f);
			Vector3 v = 0.33f * Vector3.Normalize(vector2 - vector);
			int num = (int)MathUtils.Round(3f * Vector3.Distance(vector, vector2));
			Vector3 vector3 = vector;
			for (int i = 0; i < num; i++)
			{
				int x3 = Terrain.ToCell(vector3.X);
				int y3 = Terrain.ToCell(vector3.Y);
				int z3 = Terrain.ToCell(vector3.Z);
				this.AddBox(x3, y3, z3, sizeX, sizeY, sizeZ, brush);
				vector3 += v;
			}
		}

		// Token: 0x06001624 RID: 5668 RVA: 0x000A718C File Offset: 0x000A538C
		public void PaintFastSelective(TerrainChunk chunk, int x, int y, int z, int onlyInValue)
		{
			x -= chunk.Origin.X;
			z -= chunk.Origin.Y;
			foreach (TerrainBrush.Cell cell in this.Cells)
			{
				int num = (int)cell.X + x;
				int num2 = (int)cell.Y + y;
				int num3 = (int)cell.Z + z;
				if (num >= 0 && num < 16 && num2 >= 0 && num2 < 256 && num3 >= 0 && num3 < 16)
				{
					int index = TerrainChunk.CalculateCellIndex(num, num2, num3);
					int cellValueFast = chunk.GetCellValueFast(index);
					if (onlyInValue == cellValueFast)
					{
						chunk.SetCellValueFast(index, cell.Value);
					}
				}
			}
		}

		// Token: 0x06001625 RID: 5669 RVA: 0x000A7240 File Offset: 0x000A5440
		public void PaintFastSelective(Terrain terrain, int x, int y, int z, int minX, int maxX, int minY, int maxY, int minZ, int maxZ, int onlyInValue)
		{
			foreach (TerrainBrush.Cell cell in this.Cells)
			{
				int num = (int)cell.X + x;
				int num2 = (int)cell.Y + y;
				int num3 = (int)cell.Z + z;
				if (num >= minX && num < maxX && num2 >= minY && num2 < maxY && num3 >= minZ && num3 < maxZ)
				{
					int cellValueFast = terrain.GetCellValueFast(num, num2, num3);
					if (onlyInValue == cellValueFast)
					{
						terrain.SetCellValueFast(num, num2, num3, cell.Value);
					}
				}
			}
		}

		// Token: 0x06001626 RID: 5670 RVA: 0x000A72D0 File Offset: 0x000A54D0
		public void PaintFastAvoidWater(TerrainChunk chunk, int x, int y, int z)
		{
			Terrain terrain = chunk.Terrain;
			x -= chunk.Origin.X;
			z -= chunk.Origin.Y;
			foreach (TerrainBrush.Cell cell in this.Cells)
			{
				int num = (int)cell.X + x;
				int num2 = (int)cell.Y + y;
				int num3 = (int)cell.Z + z;
				if (num >= 0 && num < 16 && num2 >= 0 && num2 < 255 && num3 >= 0 && num3 < 16)
				{
					int num4 = num + chunk.Origin.X;
					int y2 = num2;
					int num5 = num3 + chunk.Origin.Y;
					if (chunk.GetCellContentsFast(num, num2, num3) != 18 && terrain.GetCellContents(num4 - 1, y2, num5) != 18 && terrain.GetCellContents(num4 + 1, y2, num5) != 18 && terrain.GetCellContents(num4, y2, num5 - 1) != 18 && terrain.GetCellContents(num4, y2, num5 + 1) != 18 && chunk.GetCellContentsFast(num, num2 + 1, num3) != 18)
					{
						chunk.SetCellValueFast(num, num2, num3, cell.Value);
					}
				}
			}
		}

		// Token: 0x06001627 RID: 5671 RVA: 0x000A741C File Offset: 0x000A561C
		public void PaintFastAvoidWater(Terrain terrain, int x, int y, int z, int minX, int maxX, int minY, int maxY, int minZ, int maxZ)
		{
			foreach (TerrainBrush.Cell cell in this.Cells)
			{
				int num = (int)cell.X + x;
				int num2 = (int)cell.Y + y;
				int num3 = (int)cell.Z + z;
				if (num >= minX && num < maxX && num2 >= minY && num2 < maxY && num3 >= minZ && num3 < maxZ && terrain.GetCellContentsFast(num, num2, num3) != 18 && terrain.GetCellContents(num - 1, num2, num3) != 18 && terrain.GetCellContents(num + 1, num2, num3) != 18 && terrain.GetCellContents(num, num2, num3 - 1) != 18 && terrain.GetCellContents(num, num2, num3 + 1) != 18 && terrain.GetCellContentsFast(num, num2 + 1, num3) != 18)
				{
					terrain.SetCellValueFast(num, num2, num3, cell.Value);
				}
			}
		}

		// Token: 0x06001628 RID: 5672 RVA: 0x000A7510 File Offset: 0x000A5710
		public void PaintFast(TerrainChunk chunk, int x, int y, int z)
		{
			x -= chunk.Origin.X;
			z -= chunk.Origin.Y;
			foreach (TerrainBrush.Cell cell in this.Cells)
			{
				int num = (int)cell.X + x;
				int num2 = (int)cell.Y + y;
				int num3 = (int)cell.Z + z;
				if (num >= 0 && num < 16 && num2 >= 0 && num2 < 256 && num3 >= 0 && num3 < 16)
				{
					chunk.SetCellValueFast(num, num2, num3, cell.Value);
				}
			}
		}

		// Token: 0x06001629 RID: 5673 RVA: 0x000A75AC File Offset: 0x000A57AC
		public void PaintFast(Terrain terrain, int x, int y, int z, int minX, int maxX, int minY, int maxY, int minZ, int maxZ)
		{
			foreach (TerrainBrush.Cell cell in this.Cells)
			{
				int num = (int)cell.X + x;
				int num2 = (int)cell.Y + y;
				int num3 = (int)cell.Z + z;
				if (num >= minX && num < maxX && num2 >= minY && num2 < maxY && num3 >= minZ && num3 < maxZ)
				{
					terrain.SetCellValueFast(num, num2, num3, cell.Value);
				}
			}
		}

		// Token: 0x0600162A RID: 5674 RVA: 0x000A7628 File Offset: 0x000A5828
		public void Paint(SubsystemTerrain terrain, int x, int y, int z)
		{
			foreach (TerrainBrush.Cell cell in this.Cells)
			{
				int x2 = (int)cell.X + x;
				int y2 = (int)cell.Y + y;
				int z2 = (int)cell.Z + z;
				terrain.ChangeCell(x2, y2, z2, cell.Value, true);
			}
		}

		// Token: 0x04000F9A RID: 3994
		public Dictionary<int, TerrainBrush.Cell> m_cellsDictionary = new Dictionary<int, TerrainBrush.Cell>();

		// Token: 0x04000F9B RID: 3995
		public TerrainBrush.Cell[] m_cells;

		// Token: 0x020004E8 RID: 1256
		public struct Cell : IComparable<TerrainBrush.Cell>
		{
			// Token: 0x06002062 RID: 8290 RVA: 0x000E42FB File Offset: 0x000E24FB
			public int CompareTo(TerrainBrush.Cell other)
			{
				return TerrainBrush.Key((int)this.X, (int)this.Y, (int)this.Z) - TerrainBrush.Key((int)other.X, (int)other.Y, (int)other.Z);
			}

			// Token: 0x040017F9 RID: 6137
			public sbyte X;

			// Token: 0x040017FA RID: 6138
			public sbyte Y;

			// Token: 0x040017FB RID: 6139
			public sbyte Z;

			// Token: 0x040017FC RID: 6140
			public int Value;
		}

		// Token: 0x020004E9 RID: 1257
		public class Brush
		{
			// Token: 0x06002064 RID: 8292 RVA: 0x000E4334 File Offset: 0x000E2534
			public static implicit operator TerrainBrush.Brush(int value)
			{
				return new TerrainBrush.Brush
				{
					m_value = value
				};
			}

			// Token: 0x06002065 RID: 8293 RVA: 0x000E4342 File Offset: 0x000E2542
			public static implicit operator TerrainBrush.Brush(Func<int?, int?> handler)
			{
				return new TerrainBrush.Brush
				{
					m_handler1 = handler
				};
			}

			// Token: 0x06002066 RID: 8294 RVA: 0x000E4350 File Offset: 0x000E2550
			public static implicit operator TerrainBrush.Brush(Func<Point3, int?> handler)
			{
				return new TerrainBrush.Brush
				{
					m_handler2 = handler
				};
			}

			// Token: 0x06002067 RID: 8295 RVA: 0x000E4360 File Offset: 0x000E2560
			public int? Paint(TerrainBrush terrainBrush, Point3 p)
			{
				if (this.m_handler1 != null)
				{
					return this.m_handler1(terrainBrush.GetValue(p.X, p.Y, p.Z));
				}
				if (this.m_handler2 != null)
				{
					return this.m_handler2(p);
				}
				return new int?(this.m_value);
			}

			// Token: 0x040017FD RID: 6141
			public int m_value;

			// Token: 0x040017FE RID: 6142
			public Func<int?, int?> m_handler1;

			// Token: 0x040017FF RID: 6143
			public Func<Point3, int?> m_handler2;
		}

		// Token: 0x020004EA RID: 1258
		public class Counter
		{
			// Token: 0x06002069 RID: 8297 RVA: 0x000E43C1 File Offset: 0x000E25C1
			public static implicit operator TerrainBrush.Counter(int value)
			{
				return new TerrainBrush.Counter
				{
					m_value = value
				};
			}

			// Token: 0x0600206A RID: 8298 RVA: 0x000E43CF File Offset: 0x000E25CF
			public static implicit operator TerrainBrush.Counter(Func<int?, int> handler)
			{
				return new TerrainBrush.Counter
				{
					m_handler1 = handler
				};
			}

			// Token: 0x0600206B RID: 8299 RVA: 0x000E43DD File Offset: 0x000E25DD
			public static implicit operator TerrainBrush.Counter(Func<Point3, int> handler)
			{
				return new TerrainBrush.Counter
				{
					m_handler2 = handler
				};
			}

			// Token: 0x0600206C RID: 8300 RVA: 0x000E43EC File Offset: 0x000E25EC
			public int Count(TerrainBrush terrainBrush, Point3 p)
			{
				if (this.m_handler1 != null)
				{
					return this.m_handler1(terrainBrush.GetValue(p));
				}
				if (this.m_handler2 != null)
				{
					return this.m_handler2(p);
				}
				int? value = terrainBrush.GetValue(p);
				int value2 = this.m_value;
				if (!(value.GetValueOrDefault() == value2 & value != null))
				{
					return 0;
				}
				return 1;
			}

			// Token: 0x04001800 RID: 6144
			public int m_value;

			// Token: 0x04001801 RID: 6145
			public Func<int?, int> m_handler1;

			// Token: 0x04001802 RID: 6146
			public Func<Point3, int> m_handler2;
		}
	}
}
