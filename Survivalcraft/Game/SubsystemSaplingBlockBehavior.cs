using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Engine;
using Engine.Serialization;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001A4 RID: 420
	public class SubsystemSaplingBlockBehavior : SubsystemBlockBehavior, IUpdateable
	{
		// Token: 0x17000096 RID: 150
		// (get) Token: 0x060009FE RID: 2558 RVA: 0x00048A3D File Offset: 0x00046C3D
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					119
				};
			}
		}

		// Token: 0x17000097 RID: 151
		// (get) Token: 0x060009FF RID: 2559 RVA: 0x00048A4A File Offset: 0x00046C4A
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x06000A00 RID: 2560 RVA: 0x00048A50 File Offset: 0x00046C50
		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			int cellContents = base.SubsystemTerrain.Terrain.GetCellContents(x, y - 1, z);
			if (BlocksManager.Blocks[cellContents].IsTransparent)
			{
				base.SubsystemTerrain.DestroyCell(0, x, y, z, 0, false, false);
			}
		}

		// Token: 0x06000A01 RID: 2561 RVA: 0x00048A94 File Offset: 0x00046C94
		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
			float num = (this.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Creative) ? this.m_random.Float(8f, 12f) : this.m_random.Float(480f, 600f);
			this.AddSapling(new SubsystemSaplingBlockBehavior.SaplingData
			{
				Point = new Point3(x, y, z),
				Type = (TreeType)Terrain.ExtractData(value),
				MatureTime = this.m_subsystemGameInfo.TotalElapsedGameTime + (double)num
			});
		}

		// Token: 0x06000A02 RID: 2562 RVA: 0x00048B1B File Offset: 0x00046D1B
		public override void OnBlockRemoved(int value, int newValue, int x, int y, int z)
		{
			this.RemoveSapling(new Point3(x, y, z));
		}

        // Token: 0x06000A03 RID: 2563 RVA: 0x00048B30 File Offset: 0x00046D30
        public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_enumerator = this.m_saplings.Values.GetEnumerator();
			foreach (object obj in valuesDictionary.GetValue<ValuesDictionary>("Saplings").Values)
			{
				string data = (string)obj;
				this.AddSapling(this.LoadSaplingData(data));
			}
		}

        // Token: 0x06000A04 RID: 2564 RVA: 0x00048BC4 File Offset: 0x00046DC4
        public override void Save(ValuesDictionary valuesDictionary)
		{
			ValuesDictionary valuesDictionary2 = new ValuesDictionary();
			valuesDictionary.SetValue<ValuesDictionary>("Saplings", valuesDictionary2);
			int num = 0;
			foreach (SubsystemSaplingBlockBehavior.SaplingData saplingData in this.m_saplings.Values)
			{
				valuesDictionary2.SetValue<string>(num++.ToString(CultureInfo.InvariantCulture), this.SaveSaplingData(saplingData));
			}
		}

		// Token: 0x06000A05 RID: 2565 RVA: 0x00048C4C File Offset: 0x00046E4C
		public void Update(float dt)
		{
			for (int i = 0; i < 10; i++)
			{
				if (!this.m_enumerator.MoveNext())
				{
					this.m_enumerator = this.m_saplings.Values.GetEnumerator();
					return;
				}
				this.MatureSapling(this.m_enumerator.Current);
			}
		}

		// Token: 0x06000A06 RID: 2566 RVA: 0x00048C9C File Offset: 0x00046E9C
		public SubsystemSaplingBlockBehavior.SaplingData LoadSaplingData(string data)
		{
			string[] array = data.Split(new string[]
			{
				";"
			}, StringSplitOptions.None);
			if (array.Length != 3)
			{
				throw new InvalidOperationException("Invalid sapling data string.");
			}
			return new SubsystemSaplingBlockBehavior.SaplingData
			{
				Point = HumanReadableConverter.ConvertFromString<Point3>(array[0]),
				Type = HumanReadableConverter.ConvertFromString<TreeType>(array[1]),
				MatureTime = HumanReadableConverter.ConvertFromString<double>(array[2])
			};
		}

		// Token: 0x06000A07 RID: 2567 RVA: 0x00048D00 File Offset: 0x00046F00
		public string SaveSaplingData(SubsystemSaplingBlockBehavior.SaplingData saplingData)
		{
			this.m_stringBuilder.Length = 0;
			this.m_stringBuilder.Append(HumanReadableConverter.ConvertToString(saplingData.Point));
			this.m_stringBuilder.Append(';');
			this.m_stringBuilder.Append(HumanReadableConverter.ConvertToString(saplingData.Type));
			this.m_stringBuilder.Append(';');
			this.m_stringBuilder.Append(HumanReadableConverter.ConvertToString(saplingData.MatureTime));
			return this.m_stringBuilder.ToString();
		}

		// Token: 0x06000A08 RID: 2568 RVA: 0x00048D94 File Offset: 0x00046F94
		public void MatureSapling(SubsystemSaplingBlockBehavior.SaplingData saplingData)
		{
			if (this.m_subsystemGameInfo.TotalElapsedGameTime < saplingData.MatureTime)
			{
				return;
			}
			int x = saplingData.Point.X;
			int y = saplingData.Point.Y;
			int z = saplingData.Point.Z;
			TerrainChunk chunkAtCell = base.SubsystemTerrain.Terrain.GetChunkAtCell(x - 6, z - 6);
			TerrainChunk chunkAtCell2 = base.SubsystemTerrain.Terrain.GetChunkAtCell(x - 6, z + 6);
			TerrainChunk chunkAtCell3 = base.SubsystemTerrain.Terrain.GetChunkAtCell(x + 6, z - 6);
			TerrainChunk chunkAtCell4 = base.SubsystemTerrain.Terrain.GetChunkAtCell(x + 6, z + 6);
			if (chunkAtCell != null && chunkAtCell.State == TerrainChunkState.Valid && chunkAtCell2 != null && chunkAtCell2.State == TerrainChunkState.Valid && chunkAtCell3 != null && chunkAtCell3.State == TerrainChunkState.Valid && chunkAtCell4 != null && chunkAtCell4.State == TerrainChunkState.Valid)
			{
				int cellContents = base.SubsystemTerrain.Terrain.GetCellContents(x, y - 1, z);
				if (cellContents != 2 && cellContents != 8)
				{
					base.SubsystemTerrain.ChangeCell(x, y, z, Terrain.MakeBlockValue(28, 0, 0), true);
					return;
				}
				if (base.SubsystemTerrain.Terrain.GetCellLight(x, y + 1, z) >= 9)
				{
					bool flag = false;
					for (int i = x - 1; i <= x + 1; i++)
					{
						for (int j = z - 1; j <= z + 1; j++)
						{
							int cellContents2 = base.SubsystemTerrain.Terrain.GetCellContents(i, y - 1, j);
							if (BlocksManager.Blocks[cellContents2] is WaterBlock)
							{
								flag = true;
								break;
							}
						}
					}
					float probability;
					if (this.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Creative)
					{
						probability = 1f;
					}
					else
					{
						int num = base.SubsystemTerrain.Terrain.GetTemperature(x, z) + SubsystemWeather.GetTemperatureAdjustmentAtHeight(y);
						int num2 = base.SubsystemTerrain.Terrain.GetHumidity(x, z);
						if (flag)
						{
							num = (num + 10) / 2;
							num2 = MathUtils.Max(num2, 12);
						}
						probability = 2f * PlantsManager.CalculateTreeProbability(saplingData.Type, num, num2, y);
					}
					if (!this.m_random.Bool(probability))
					{
						base.SubsystemTerrain.ChangeCell(x, y, z, Terrain.MakeBlockValue(28, 0, 0), true);
						return;
					}
					base.SubsystemTerrain.ChangeCell(x, y, z, Terrain.MakeBlockValue(0, 0, 0), true);
					if (!this.GrowTree(x, y, z, saplingData.Type))
					{
						base.SubsystemTerrain.ChangeCell(x, y, z, Terrain.MakeBlockValue(28, 0, 0), true);
						return;
					}
				}
				else if (this.m_subsystemGameInfo.TotalElapsedGameTime > saplingData.MatureTime + 1200.0)
				{
					base.SubsystemTerrain.ChangeCell(x, y, z, Terrain.MakeBlockValue(28, 0, 0), true);
					return;
				}
			}
			else
			{
				saplingData.MatureTime = this.m_subsystemGameInfo.TotalElapsedGameTime;
			}
		}

		// Token: 0x06000A09 RID: 2569 RVA: 0x00049070 File Offset: 0x00047270
		public bool GrowTree(int x, int y, int z, TreeType treeType)
		{
			ReadOnlyList<TerrainBrush> treeBrushes = PlantsManager.GetTreeBrushes(treeType);
			for (int i = 0; i < 20; i++)
			{
				TerrainBrush terrainBrush = treeBrushes[this.m_random.Int(0, treeBrushes.Count - 1)];
				bool flag = true;
				foreach (TerrainBrush.Cell cell in terrainBrush.Cells)
				{
					if (cell.Y >= 0 && (cell.X != 0 || cell.Y != 0 || cell.Z != 0))
					{
						int cellContents = base.SubsystemTerrain.Terrain.GetCellContents((int)cell.X + x, (int)cell.Y + y, (int)cell.Z + z);
						if (cellContents != 0 && !(BlocksManager.Blocks[cellContents] is LeavesBlock))
						{
							flag = false;
							break;
						}
					}
				}
				if (flag)
				{
					terrainBrush.Paint(base.SubsystemTerrain, x, y, z);
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000A0A RID: 2570 RVA: 0x0004915E File Offset: 0x0004735E
		public void AddSapling(SubsystemSaplingBlockBehavior.SaplingData saplingData)
		{
			this.m_saplings[saplingData.Point] = saplingData;
			this.m_enumerator = this.m_saplings.Values.GetEnumerator();
		}

		// Token: 0x06000A0B RID: 2571 RVA: 0x00049188 File Offset: 0x00047388
		public void RemoveSapling(Point3 point)
		{
			this.m_saplings.Remove(point);
			this.m_enumerator = this.m_saplings.Values.GetEnumerator();
		}

		// Token: 0x04000559 RID: 1369
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x0400055A RID: 1370
		public Dictionary<Point3, SubsystemSaplingBlockBehavior.SaplingData> m_saplings = new Dictionary<Point3, SubsystemSaplingBlockBehavior.SaplingData>();

		// Token: 0x0400055B RID: 1371
		public Dictionary<Point3, SubsystemSaplingBlockBehavior.SaplingData>.ValueCollection.Enumerator m_enumerator;

		// Token: 0x0400055C RID: 1372
		public Game.Random m_random = new Game.Random();

		// Token: 0x0400055D RID: 1373
		public StringBuilder m_stringBuilder = new StringBuilder();

		// Token: 0x0200043A RID: 1082
		public class SaplingData
		{
			// Token: 0x040015EA RID: 5610
			public Point3 Point;

			// Token: 0x040015EB RID: 5611
			public TreeType Type;

			// Token: 0x040015EC RID: 5612
			public double MatureTime;
		}
	}
}
